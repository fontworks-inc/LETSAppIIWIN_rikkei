using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using Core.Entities;
using Infrastructure.File;
using Infrastructure.Memory;
using Newtonsoft.Json;
using NLog;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Infrastructure.API
{
    /// <summary>
    /// リポジトリの基底クラス
    /// </summary>
    public abstract class APIRepositoryBase
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="apiConfiguration">設定情報</param>
        protected APIRepositoryBase(APIConfiguration apiConfiguration)
        {
            this.BasePath = apiConfiguration.BasePath;
            this.NotifyBasePath = apiConfiguration.NotifyBasePath;

            //TODO:削除する（証明書エラー回避用）
            ServicePointManager.ServerCertificateValidationCallback +=
               (sender, certificate, chain, sslPolicyErrors) => true;
        }

        /// <summary>
        /// API呼び出し先のURL
        /// </summary>
        protected string BasePath { get; set; } = null;

        protected string NotifyBasePath { get; set; } = null;

        /// <summary>
        /// 定期確認間隔
        /// </summary>
        protected int FixedTermConfirmationInterval { get; set; } = 1800;

        /// <summary>
        /// リトライ回数
        /// </summary>
        protected int CommunicationRetryCount { get; set; } = 10;

        /// <summary>
        /// APIパラメータ受け渡し用
        /// </summary>
        protected Dictionary<APIParam, object> ApiParam { get; set; } = new Dictionary<APIParam, object>();

        /// <summary>
        /// APIレスポンス(OpenAPIToolsのXXResponse)
        /// </summary>
        protected object ApiResponse { get; set; } = null;

        /// <summary>
        /// フォント配信サーバAPIアクセス
        /// </summary>
        /// <param name="action">リトライするアクション</param>
        /// <return>actionの戻り値</return>
        /// <remarks>2.5.6.8.フォント配信サーバAPIアクセスのルールにしたがってactionのAPI呼び出しを行う。キャッシュの保管は呼び出し側で行うこと。</remarks>
        ///
        protected void Invoke(Action action)
        {
            VolatileSetting vSetting = new VolatileSettingMemoryRepository().GetVolatileSetting();
            DateTime lastAccess = vSetting.LastAccessAt ?? default(DateTime);
            //int retry_count = this.CommunicationRetryCount;
            int retry_count = 2;

            int i = 0;

            // [メモリ：通信状態]がオフライン]
            if (!vSetting.IsConnected)
            {
                // [メモリ：前回アクセス日時]から[共通設定：定期確認間隔]以上経過していない
                if (DateTime.Now < lastAccess.AddSeconds(this.FixedTermConfirmationInterval))
                {
                    // API通信しない。
                    throw new ApiException();
                }
            }

            // UserAgentのセット
            this.SetUserAgent();

            while (true)
            {
                try
                {
                    //if (!this.ApiParam.ContainsKey(APIParam.AccessToken) || this.ApiParam[APIParam.AccessToken].ToString() == string.Empty)
                    //{
                    //    if (this.ApiParam.ContainsKey(APIParam.DeviceId) && this.ApiParam[APIParam.DeviceId].ToString() != string.Empty
                    //        && this.ApiParam.ContainsKey(APIParam.RefreshToken) && this.ApiParam[APIParam.RefreshToken].ToString() != string.Empty)
                    //    {
                    //        // アクセストークンの更新
                    //        if (!this.RefreshAccessToken())
                    //        {
                    //            // 更新に失敗したのでループを止めてエラーコードを返す。
                    //            break;
                    //        }
                    //    }
                    //}

                    // 実APIの呼び出し
                    action();

                    // アクセストークン有効期限切れエラーかチェック
                    if (this.IsAccessTokenExpired(this.ApiResponse))
                    {
                        // アクセストークンの更新
                        if (!this.RefreshAccessToken())
                        {
                            // 更新に失敗したのでループを止めてエラーコードを返す。
                            break;
                        }

                        // 改めてAPIを実行する(次のループで実行される)
                    }
                    else
                    {
                        // なんかしらの応答が取得できたので終了
                        break;
                    }
                }
                catch (ApiException ex)
                {
                    // 通信エラーが発生した(ErrorCode:503など)
                    // [共通設定：通信リトライ回数]まで繰り返す
                    Logger.Debug("APIRepositoryBase:" + ex.Message, string.Empty);
                    if (i++ >= retry_count)
                    {
                        if (vSetting.IsConnected)
                        {
                            // オンライン⇒オフライン
                            vSetting.IsConnected = false;

                            // TODO:メッセージを通知する「オフラインモードへ移行しました」
                        }

                        // フォント配信サーバへ通信を行った日時を更新する
                        vSetting.LastAccessAt = DateTime.Now;
                        throw;
                    }
                }
                catch (Exception)
                {
                    // 予期しない例外
                    throw;
                }
            }

            // 通信が成功している
            if (!vSetting.IsConnected)
            {
                // オフライン⇒オンライン
                vSetting.IsConnected = true;

                // TODO:メッセージを通知する「オンラインモードへ移行しました」
            }

            // フォント配信サーバへ通信を行った日時を更新する
            vSetting.LastAccessAt = DateTime.Now;
        }

        /// <summary>
        /// UserAgentの設定。
        /// 初回の場合は生成した値でメモリの値を更新する。
        /// アプリバージョンの取得が必要だが、
        /// applicationServiceからではなく、個別に取得する。
        /// </summary>
        private void SetUserAgent()
        {
            VolatileSetting vSetting = new VolatileSettingMemoryRepository().GetVolatileSetting();

            this.ApiParam[APIParam.RefreshToken] = vSetting.RefreshToken;

            if (!string.IsNullOrEmpty(vSetting.UserAgent))
            {
                this.ApiParam[APIParam.UserAgent] = vSetting.UserAgent;
                return;
            }

            // アプリバージョンの取得
            var output = string.Empty;
            var appver = string.Empty;
            var apppath = vSetting.ClientApplicationPath;

            if (System.IO.File.Exists(apppath))
            {
                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(apppath);
                appver = versionInfo.ProductVersion;
            }

            System.Diagnostics.Process pro = new System.Diagnostics.Process();

            pro.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
            pro.StartInfo.Arguments = @"/c ver";
            pro.StartInfo.CreateNoWindow = true;
            pro.StartInfo.UseShellExecute = false;
            pro.StartInfo.RedirectStandardOutput = true;

            pro.Start();
            output = pro.StandardOutput.ReadToEnd();

            MatchCollection matches = Regex.Matches(output, @"\d+\.\d+\.\d+\.\d+");
            foreach (Match match in matches)
            {
                output = string.Concat("LETS/", appver, " (Win ", match.Value, ")");
            }

            vSetting.UserAgent = output;
            this.ApiParam[APIParam.UserAgent] = vSetting.UserAgent;
        }

        /// <summary>
        /// アクセストークン更新APIを呼び出しアクセストークンを更新する
        /// </summary>
        /// <returns>更新に成功:true、失敗:false</returns>
        private bool RefreshAccessToken()
        {
            VolatileSetting vSetting = new VolatileSettingMemoryRepository().GetVolatileSetting();
            Configuration config = new Configuration();
            config.BasePath = this.BasePath;
            config.UserAgent = (string)this.ApiParam[APIParam.UserAgent];
            if(!this.ApiParam.ContainsKey(APIParam.RefreshToken) || (string)this.ApiParam[APIParam.RefreshToken] == string.Empty)
            {
                this.ApiParam[APIParam.RefreshToken] = vSetting.RefreshToken;
            }
            LoginApi apiInstance = new LoginApi(config);
            AccessTokenResponse tokenResponse;
            try
            {
                if (this.ApiParam.ContainsKey(APIParam.AccessToken))
                {
                    this.ApiParam.Remove(APIParam.AccessToken);
                }
                tokenResponse = apiInstance.Token(
                    (string)this.ApiParam[APIParam.DeviceId],
                    (string)this.ApiParam[APIParam.UserAgent],
                    new InlineObject3((string)this.ApiParam[APIParam.RefreshToken]));
                if (tokenResponse.Code == (int)ResponseCode.Succeeded)
                {
                    vSetting.AccessToken = tokenResponse.Data.AccessToken;
                    this.ApiParam[APIParam.AccessToken] = vSetting.AccessToken;
                    Logger.Debug("RefreshAccessToken:Code=" + tokenResponse.Code, string.Empty);
                    Logger.Debug("RefreshAccessToken:DeviceId=" + this.ApiParam[APIParam.DeviceId]);
                    Logger.Debug("RefreshAccessToken:RefreshToken=" + this.ApiParam[APIParam.RefreshToken]);
                    Logger.Debug("RefreshAccessToken:AccessToken=" + this.ApiParam[APIParam.AccessToken]);
                }
                else if (tokenResponse.Code == (int)ResponseCode.AuthenticationFailed)
                {
                    // 認証エラー
                    //  デバイスIDが無効になっていると思われる
                    Logger.Debug("RefreshAccessToken:Code=" + tokenResponse.Code, string.Empty);
                    Logger.Debug("RefreshAccessToken:DeviceId=" + this.ApiParam[APIParam.DeviceId]);
                    Logger.Debug("RefreshAccessToken:RefreshToken=" + this.ApiParam[APIParam.RefreshToken]);
                    return false;
                }
                else
                {
                    Logger.Debug("RefreshAccessToken:Code=" + tokenResponse.Code, string.Empty);
                    Logger.Debug("RefreshAccessToken:DeviceId=" + this.ApiParam[APIParam.DeviceId]);
                    Logger.Debug("RefreshAccessToken:RefreshToken=" + this.ApiParam[APIParam.RefreshToken]);
                    return false;
                }
            }
            catch (ApiException e) when (e.ErrorCode == 400)
            {
                // リフレッシュトークン有効期限切れエラー
                // 通信自体は成功しているためエラーコードを返す。
                tokenResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(e.ErrorContent.ToString());
                return false;
            }
            catch (Exception e)
            {
                // throw;
                Logger.Debug("RefreshAccessToken:Exception=" + e.Message, string.Empty);
                return false;
            }

            return true;
        }

        /// <summary>
        /// アクセストークン有効期限切れエラーか。
        /// </summary>
        /// <param name="obj">APIResponse</param>
        /// <returns>有効期限切れエラーならtrue</returns>
        private bool IsAccessTokenExpired(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            {
                string objnm = obj.GetType().Name; 
                Logger.Info(string.Format("IsAccessTokenExpired:objtype=" + objnm, String.Empty));
            }

            int code = (int)ResponseCode.Succeeded;

            if (typeof(AccessTokenRefreshTokenResponse) == obj.GetType())
            {
                code = ((AccessTokenRefreshTokenResponse)obj).Code;
            }
            else if (typeof(AccessTokenResponse) == obj.GetType())
            {
                code = ((AccessTokenResponse)obj).Code;
            }
            else if (typeof(DeviceIdResponse) == obj.GetType())
            {
                code = ((DeviceIdResponse)obj).Code;
            }
            else if (typeof(DevicesResponse) == obj.GetType())
            {
                code = ((DevicesResponse)obj).Code;
            }
            else if (typeof(UrlResponse) == obj.GetType())
            {
                code = ((UrlResponse)obj).Code;
            }
            else if (typeof(Model200) == obj.GetType())
            {
                code = ((Model200)obj).Code;
            }
            else if (typeof(CustomerResponse) == obj.GetType())
            {
                code = ((CustomerResponse)obj).Code;
            }

            {
                Logger.Info(string.Format("IsAccessTokenExpired:code=" + code.ToString(), String.Empty));
            }

            return code == (int)ResponseCode.AccessTokenExpired;
            //return code == (int)ResponseCode.AccessTokenExpired
            //    || code == (int)ResponseCode.AuthenticationFailed;
            //return code == (int)ResponseCode.AccessTokenExpired
            //    || code == (int)ResponseCode.AuthenticationFailed
            //    || code == (int)ResponseCode.InvalidArgument;
        }
    }
}

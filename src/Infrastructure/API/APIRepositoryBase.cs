using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using Core.Entities;
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
            this.APIConfiguration = apiConfiguration;
            this.BasePath = apiConfiguration.BasePath;
            this.NotifyBasePath = apiConfiguration.NotifyBasePath;
            this.FixedTermConfirmationInterval = apiConfiguration.FixedTermConfirmationInterval;
            this.CommunicationRetryCount = apiConfiguration.CommunicationRetryCount;
        }

        /// <summary>
        /// API設定
        /// </summary>
        protected APIConfiguration APIConfiguration { get; set; } = null;

        /// <summary>
        /// API呼び出し先のURL
        /// </summary>
        protected string BasePath { get; set; } = null;

        /// <summary>
        /// 通知サーバーのURL
        /// </summary>
        protected string NotifyBasePath { get; set; } = null;

        /// <summary>
        /// 定期確認間隔
        /// </summary>
        protected int FixedTermConfirmationInterval { get; set; } = 1800;

        /// <summary>
        /// リトライ回数
        /// </summary>
        protected int CommunicationRetryCount { get; set; } = 2;

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
            int retry_count = this.CommunicationRetryCount;

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

            int retryCount = 5;
            while (retryCount > 0)
            {
                try
                {
                    // 実APIの呼び出し
                    action();
                    retryCount--;

                    try
                    {
                        Logger.Debug("Invoke:ApiParam");
                        foreach (KeyValuePair<Core.Entities.APIParam, object> kvp in this.ApiParam)
                        {
                            Logger.Debug($"Invoke:ApiParam:{kvp.Key}={kvp.Value}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.StackTrace);
                    }

                    if (this.ApiParam.ContainsKey(APIParam.OfflineDeviceId) && !string.IsNullOrEmpty((string)this.ApiParam[APIParam.OfflineDeviceId]))
                    {
                        Logger.Debug($"Invoke:オフラインデバイスIDが設定されているときはアクセストークン期限切れをチェックしない");
                        break;
                    }
                    else if (!this.ApiParam.ContainsKey(APIParam.DeviceId))
                    {
                        Logger.Debug($"Invoke:デバイスIDが設定されていないときはアクセストークン期限切れをチェックしない");
                        break;
                    }
                    else
                    {
                        // アクセストークン有効期限切れエラーかチェック
                        if (this.IsAccessTokenExpired(this.ApiResponse, vSetting.AccessToken))
                        {
                            // アクセストークンの更新
                            int ret = this.RefreshAccessToken();
                            if (ret != 0)
                            {
                                // 更新に失敗したのでループを止めてエラーコードを返す。
                                if (ret == -1)
                                {
                                    if (action.Method.Name == "CallGetDevicesApi")
                                    {
                                        if (this.APIConfiguration.ForceLogout != null)
                                        {
                                            this.APIConfiguration.ForceLogout();
                                        }
                                    }
                                }

                                break;
                            }

                            // 改めてAPIを実行する(次のループで実行される)
                        }
                        else if (this.IsRefreshTokenExpired(this.ApiResponse))
                        {
                            if (this.APIConfiguration.ForceLogout != null)
                            {
                                this.APIConfiguration.ForceLogout();
                            }

                            break;
                        }
                        else
                        {
                            // なんかしらの応答が取得できたので終了
                            break;
                        }
                    }
                }
                catch (ApiException ex)
                {
                    // 通信エラーが発生した(ErrorCode:503など)
                    // [共通設定：通信リトライ回数]まで繰り返す
                    Logger.Debug("APIRepositoryBase:" + ex.Message + "\n" + ex.StackTrace);
                    if (i++ >= retry_count)
                    {
                        if (vSetting.IsConnected)
                        {
                            // オンライン⇒オフライン
                            vSetting.IsConnected = false;

                            Logger.Info($"オフラインモードへ移行しました");
                        }

                        // フォント配信サーバへ通信を行った日時を更新する
                        vSetting.LastAccessAt = DateTime.Now;
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    // 予期しない例外
                    Logger.Error(ex.StackTrace);
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

            string useragent = this.APIConfiguration.GetUserAgent(vSetting.ClientApplicationPath);

            vSetting.UserAgent = useragent;
            this.ApiParam[APIParam.UserAgent] = vSetting.UserAgent;
        }

        /// <summary>
        /// アクセストークン更新APIを呼び出しアクセストークンを更新する
        /// </summary>
        /// <returns>更新に成功:true、失敗:false</returns>
        private int RefreshAccessToken()
        {
            VolatileSetting vSetting = new VolatileSettingMemoryRepository().GetVolatileSetting();
            Configuration config = new Configuration();
            config.BasePath = this.BasePath;
            config.UserAgent = (string)this.ApiParam[APIParam.UserAgent];
            config.WebProxy = this.APIConfiguration.GetWebProxy(this.BasePath);
            if (!this.ApiParam.ContainsKey(APIParam.RefreshToken) || (string)this.ApiParam[APIParam.RefreshToken] == string.Empty)
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
                    Logger.Debug("RefreshAccessToken:Code=" + tokenResponse.Code);
                    Logger.Debug("RefreshAccessToken:DeviceId=" + this.ApiParam[APIParam.DeviceId]);
                    Logger.Debug("RefreshAccessToken:RefreshToken=" + this.ApiParam[APIParam.RefreshToken]);
                    Logger.Debug("RefreshAccessToken:AccessToken=" + this.ApiParam[APIParam.AccessToken]);
                }
                else if (tokenResponse.Code == (int)ResponseCode.AuthenticationFailed)
                {
                    // 認証エラー
                    //  デバイスIDが無効になっていると思われる
                    Logger.Debug("RefreshAccessToken:Code=" + tokenResponse.Code);
                    Logger.Debug("RefreshAccessToken:DeviceId=" + this.ApiParam[APIParam.DeviceId]);
                    Logger.Debug("RefreshAccessToken:RefreshToken=" + this.ApiParam[APIParam.RefreshToken]);
                    return -1;
                }
                else
                {
                    Logger.Debug("RefreshAccessToken:Code=" + tokenResponse.Code);
                    Logger.Debug("RefreshAccessToken:DeviceId=" + this.ApiParam[APIParam.DeviceId]);
                    Logger.Debug("RefreshAccessToken:RefreshToken=" + this.ApiParam[APIParam.RefreshToken]);
                    return -2;
                }
            }
            catch (ApiException e) when (e.ErrorCode == 400)
            {
                // リフレッシュトークン有効期限切れエラー
                // 通信自体は成功しているためエラーコードを返す。
                tokenResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(e.ErrorContent.ToString());
                return -3;
            }
            catch (Exception e)
            {
                // throw;
                Logger.Error("RefreshAccessToken:Exception=" + e.Message + "\n" + e.StackTrace);
                return -4;
            }

            return 0;
        }

        /// <summary>
        /// アクセストークン有効期限切れエラーか。
        /// </summary>
        /// <param name="obj">APIResponse</param>
        /// <returns>有効期限切れエラーならtrue</returns>
        private bool IsAccessTokenExpired(object obj, string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                if (this.ApiParam.ContainsKey(APIParam.OfflineDeviceId) && !string.IsNullOrEmpty((string)this.ApiParam[APIParam.OfflineDeviceId]))
                {
                    Logger.Debug($"IsAccessTokenExpired:オフラインデバイスIDが設定されているときはアクセストークン期限切れをチェックしない");
                    return false;
                }

                if (!this.ApiParam.ContainsKey(APIParam.DeviceId))
                {
                    Logger.Debug($"IsAccessTokenExpired:デバイスIDが設定されていないときはアクセストークン期限切れをチェックしない");
                    return false;
                }

                if (this.ApiParam.ContainsKey(APIParam.DeviceId) && (string)this.ApiParam[APIParam.DeviceId] != string.Empty)
                {
                    return true;
                }
            }

            if (obj == null)
            {
                return false;
            }

            int code = (int)ResponseCode.Succeeded;

            try
            {
                string objnm = obj.GetType().Name;
                Logger.Debug("IsAccessTokenExpired:objtype=" + objnm);
                if (objnm == "FileStream")
                {
                    return false;
                }

                if (objnm == "MemoryStream")
                {
                    return true;
                }

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
                else if (typeof(ClientVersionResponse) == obj.GetType())
                {
                    code = ((ClientVersionResponse)obj).Code;
                }
                else if (typeof(InlineResponse200) == obj.GetType())
                {
                    code = ((InlineResponse200)obj).Code;
                }
                else if (typeof(UpdateLicenseResponse) == obj.GetType())
                {
                    code = ((UpdateLicenseResponse)obj).Code;
                }
                else
                {
                    try
                    {
                        ObjectResponse objectResponse = new ObjectResponse(obj.ToString());
                        code = objectResponse.Code;
                    }
                    catch (Exception ex)
                    {
                        Logger.Debug("IsAccessTokenExpired:" + ex.StackTrace);
                    }
                }

                {
                    Logger.Debug("IsAccessTokenExpired:code=" + code.ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.Debug("IsAccessTokenExpired:" + ex.Message + "\n" + ex.StackTrace);
            }

            return code == (int)ResponseCode.AccessTokenExpired || code == (int)ResponseCode.AuthenticationFailed;
        }

        /// <summary>
        /// リフレッシュトークン有効期限切れエラーか。
        /// </summary>
        /// <param name="obj">APIResponse</param>
        /// <returns>有効期限切れエラーならtrue</returns>
        private bool IsRefreshTokenExpired(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            int code = (int)ResponseCode.Succeeded;

            try
            {
                {
                    string objnm = obj.GetType().Name;
                    Logger.Debug("IsAccessTokenExpired:objtype=" + objnm);
                }

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
                    Logger.Debug("IsRefreshTokenExpired:code=" + code.ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.Debug("IsRefreshTokenExpired:" + ex.Message + "\n" + ex.StackTrace);
            }

            return code == (int)ResponseCode.RefreshTokenExpired;
        }
    }
}

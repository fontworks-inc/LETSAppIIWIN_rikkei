using System;
using System.Collections.Generic;
using System.Text;
using Core.Entities;
using Core.Interfaces;
using Newtonsoft.Json;
using NLog;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Infrastructure.API
{
    /// <summary>
    ///  フォントセキュリティ情報を格納するリポジトリ
    /// </summary>
    public class FontSecurityAPIRepository : APIRepositoryBase, IFontSecurityRepository
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="apiConfiguration">設定情報</param>
        public FontSecurityAPIRepository(APIConfiguration apiConfiguration)
            : base(apiConfiguration)
        {
        }

        /// <summary>
        /// ユーザIDを取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>ユーザID</returns>
        public UserId GetUserId(string deviceId, string accessToken)
        {
            // APIの引数の値をセット(個別処理)
            this.ApiParam[APIParam.DeviceId] = deviceId;
            this.ApiParam[APIParam.AccessToken] = accessToken;

            UserId response = null;

            // API通信を行う(リトライ込み)を行う（共通処理）
            try
            {
                this.Invoke(this.CallUserIdAPI);

                // 戻り値のセット（個別処理）
                var ret = new UserIdResponse(this.ApiResponse);
                if (ret.Code == (int)ResponseCode.Succeeded)
                {
                    response = new UserId(ret.Data.UserId);
                }
                else
                {
                    throw new ApiException(ret.Code, ret.Message);
                }
            }
            catch (ApiException)
            {
                // 通信に失敗or通信しなかった
                throw;
            }

            return response;
        }

        /// <summary>
        /// 他端末のフォントがコピーされた時にFW運用者に通知する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <param name="fontId">フォントID</param>
        /// <param name="originalUserId">当該フォントファイルをインストールした端末のユーザID</param>
        /// <param name="originalDeviceId">当該フォントファイルをインストールした端末のデバイスID</param>
        /// <param name="detected">検知日時</param>
        public void PostFontFileCopyDetection(string deviceId, string accessToken, string fontId, string originalUserId, string originalDeviceId, string detected)
        {
            // APIの引数の値をセット(個別処理)
            this.ApiParam[APIParam.DeviceId] = deviceId;
            this.ApiParam[APIParam.AccessToken] = accessToken;
            this.ApiParam[APIParam.FontId] = fontId;
            this.ApiParam[APIParam.OriginalUserId] = originalUserId;
            this.ApiParam[APIParam.OriginalDeviceId] = originalDeviceId;
            this.ApiParam[APIParam.Detected] = detected;

            // API通信を行う(リトライ込み)を行う（共通処理）
            try
            {
                this.Invoke(this.CallNotifyFontFileCopyDetection);

                // 戻り値のセット（個別処理）
                var ret = JsonConvert.DeserializeObject<ObjectResponse>(this.ApiResponse.ToString());
                if (ret.Code != (int)ResponseCode.Succeeded)
                {
                    // 正常終了以外の値が返ってきた
                    throw new ApiException(ret.Code, ret.Message);
                }
            }
            catch (ApiException)
            {
                // 通信に失敗or通信しなかった
                throw;
            }

            return;
        }

        /// <summary>
        /// 異常なフォントなしをFW運用者に通知する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        public void NotifyVerifiedFonts(string deviceId, string accessToken)
        {
            // APIの引数の値をセット(個別処理)
            this.ApiParam[APIParam.DeviceId] = deviceId;
            this.ApiParam[APIParam.AccessToken] = accessToken;

            // API通信を行う(リトライ込み)を行う（共通処理）
            try
            {
                this.Invoke(this.CallNotifyVerifiedFonts);

                // 戻り値のセット（個別処理）
                var ret = JsonConvert.DeserializeObject<ObjectResponse>(this.ApiResponse.ToString());
                if (ret.Code != (int)ResponseCode.Succeeded)
                {
                    // 正常終了以外の値が返ってきた
                    throw new ApiException(ret.Code, ret.Message);
                }
            }
            catch (ApiException)
            {
                // 通信に失敗or通信しなかった
                throw;
            }

            return;
        }

        /// <summary>
        /// ユーザID取得API呼び出し
        /// </summary>
        private void CallUserIdAPI()
        {
            Configuration config = new Configuration();
            config.BasePath = this.BasePath;
            config.UserAgent = (string)this.ApiParam[APIParam.UserAgent];
            config.AccessToken = (string)this.ApiParam[APIParam.AccessToken];
            FontSecurityApi apiInstance = new FontSecurityApi(config);
            this.ApiResponse = apiInstance.GetUserId((string)this.ApiParam[APIParam.DeviceId], config.UserAgent);
        }

        /// <summary>
        /// 他端末のフォントがコピーされた通知の呼び出し
        /// </summary>
        private void CallNotifyFontFileCopyDetection()
        {
            Configuration config = new Configuration();
            config.BasePath = this.BasePath;
            config.UserAgent = (string)this.ApiParam[APIParam.UserAgent];
            config.AccessToken = (string)this.ApiParam[APIParam.AccessToken];
            FontSecurityApi apiInstance = new FontSecurityApi(config);
            var body = new InlineObject5(
                (string)this.ApiParam[APIParam.FontId],
                (string)this.ApiParam[APIParam.OriginalUserId],
                (string)this.ApiParam[APIParam.OriginalDeviceId],
                (string)this.ApiParam[APIParam.Detected]);
            this.ApiResponse = apiInstance.NotifyFontFileCopyDetection((string)this.ApiParam[APIParam.DeviceId], config.UserAgent, body);
        }

        /// <summary>
        /// 異常なフォントなしの通知の呼び出し
        /// </summary>
        private void CallNotifyVerifiedFonts()
        {
            Configuration config = new Configuration();
            config.BasePath = this.BasePath;
            config.UserAgent = (string)this.ApiParam[APIParam.UserAgent];
            config.AccessToken = (string)this.ApiParam[APIParam.AccessToken];
            FontSecurityApi apiInstance = new FontSecurityApi(config);
            this.ApiResponse = apiInstance.NotifyVerifiedFonts((string)this.ApiParam[APIParam.DeviceId], config.UserAgent);
        }
    }
}

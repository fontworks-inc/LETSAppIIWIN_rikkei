using System;
using System.Collections.Generic;
using System.Net;
using Core.Entities;
using Core.Interfaces;
using NLog;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Infrastructure.API
{
    /// <summary>
    /// クライアントアプリの起動Ver情報を格納するAPIリポジトリ
    /// </summary>
    public class ClientApplicationVersionAPIRepository : APIRepositoryBase, IClientApplicationVersionRepository
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="apiConfiguration">設定情報</param>
        public ClientApplicationVersionAPIRepository(APIConfiguration apiConfiguration)
            : base(apiConfiguration)
        {
        }

        /// <summary>
        /// クライアントアプリケーションの更新情報を取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>更新情報</returns>
        public ClientApplicationUpdateInfomation GetClientApplicationUpdateInfomation(string deviceId, string accessToken)
        {
            ClientApplicationUpdateInfomation response = null;

            // APIの引数の値をセット(個別処理)
            this.ApiParam[APIParam.DeviceId] = deviceId;
            this.ApiParam[APIParam.AccessToken] = accessToken;

            // API通信を行う(リトライ込み)を行う（共通処理）
            try
            {
                this.Invoke(this.CallClientApplicationUpdateInfomationAPI);

                // 戻り値のセット（個別処理）
                var ret = new ClientAppUpdateInfoResponse(this.ApiResponse);
                if (ret.Code == (int)ResponseCode.Succeeded)
                {
                    if (ret.Data != null)
                    {
                        response = new ClientApplicationUpdateInfomation();
                        response.ApplicationUpdateType = ret.Data.IsForcedUpdate;
                        ClientApplicationVersion clientApplicationVersion = new ClientApplicationVersion(ret.Data.AppId, ret.Data.Version, ret.Data.Url);
                        response.ClientApplicationVersion = clientApplicationVersion;
                    }
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
        /// クライアントアプリケーションの起動バージョン情報を取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>ユーザ別フォント情報</returns>
        /// <remarks>APIリポジトリではこのメソッドは実装しない</remarks>
        public ClientApplicationVersion GetClientApplicationVersion()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// クライアントアプリケーションの起動バージョン情報を保存する
        /// </summary>
        /// <param name="clientApplicationVersion">ユーザ別フォント情報</param>
        /// <remarks>APIリポジトリではこのメソッドは実装しない</remarks>
        public void SaveClientApplicationVersion(ClientApplicationVersion clientApplicationVersion)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// クライアントアプリケーションの起動バージョン情報を取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>クライアントアプリケーションの起動バージョン</returns>
        public ClientApplicationVersion GetClientApplicationVersion(string deviceId, string accessToken)
        {
            ClientApplicationVersion response = null;

            if (string.IsNullOrEmpty(deviceId))
            {
                throw new ApiException(1001, "invalid argument");
            }

            // APIの引数の値をセット(個別処理)
            this.ApiParam[APIParam.DeviceId] = deviceId;
            this.ApiParam[APIParam.AccessToken] = accessToken;

            // API通信を行う(リトライ込み)を行う（共通処理）
            try
            {
                this.Invoke(this.CallClientApplicationVersionAPI);

                // 戻り値のセット（個別処理）
                var ret = new ClientVersionResponse(this.ApiResponse);
                if (ret.Code == (int)ResponseCode.Succeeded)
                {
                    response = new ClientApplicationVersion();

                    response.AppId = ret.Data.AppId;
                    response.Version = ret.Data.Version;
                    response.Url = ret.Data.Url;
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
        /// クライアントアプリケーションの更新情報を取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>更新情報</returns>
        public ClientApplicationUpdateInfomation GetClientApplicationDeviceUpdateInfomation(string offlineDeviceId, string indefiniteAccessToken)
        {
            Logger.Debug($"GetClientApplicationDeviceUpdateInfomation:Enter offlineDeviceId={offlineDeviceId}, indefiniteAccessToken={indefiniteAccessToken}");

            ClientApplicationUpdateInfomation response = null;

            // APIの引数の値をセット(個別処理)
            this.ApiParam[APIParam.OfflineDeviceId] = offlineDeviceId;
            this.ApiParam[APIParam.IndefiniteAccessToken] = indefiniteAccessToken;
            this.ApiParam[APIParam.HostName] = Dns.GetHostName();
            this.ApiParam[APIParam.OSUserName] = Environment.UserName;

            // API通信を行う(リトライ込み)を行う（共通処理）
            try
            {
                this.Invoke(this.CallClientApplicationDeviceUpdateInfomationAPI);

                // 戻り値のセット（個別処理）
                var ret = new ClientAppUpdateInfoResponse(this.ApiResponse);
                if (ret.Code == (int)ResponseCode.Succeeded)
                {
                    if (ret.Data != null)
                    {
                        response = new ClientApplicationUpdateInfomation();
                        response.ApplicationUpdateType = ret.Data.IsForcedUpdate;
                        ClientApplicationVersion clientApplicationVersion = new ClientApplicationVersion(ret.Data.AppId, ret.Data.Version, ret.Data.Url);
                        response.ClientApplicationVersion = clientApplicationVersion;
                    }
                }
                else
                {
                    ApiException apiException = new ApiException(ret.Code, ret.Message);
                    Logger.Error($"GetClientApplicationDeviceUpdateInfomation:ret.Code={ret.Code}, ret.Message={ret.Message}{Environment.NewLine}{apiException.StackTrace}");
                    Logger.Debug("GetClientApplicationDeviceUpdateInfomation:ApiParam");
                    foreach (KeyValuePair<Core.Entities.APIParam, object> kvp in this.ApiParam)
                    {
                        Logger.Debug($"GetClientApplicationDeviceUpdateInfomation:ApiParam:{kvp.Key}={kvp.Value}");
                    }

                    throw apiException;
                }
            }
            catch (ApiException e)
            {
                // 通信に失敗or通信しなかった
                Logger.Error(e.StackTrace);
                throw;
            }

            return response;
        }

        /// <summary>
        /// クライアントアプリの更新情報取得の呼び出し
        /// </summary>
        private void CallClientApplicationUpdateInfomationAPI()
        {
            Configuration config = new Configuration();
            config.BasePath = this.BasePath;
            config.UserAgent = (string)this.ApiParam[APIParam.UserAgent];
            config.WebProxy = this.APIConfiguration.GetWebProxy(this.BasePath);
            config.AccessToken = (string)this.ApiParam[APIParam.AccessToken];
            ClientAppApi apiInstance = new ClientAppApi(config);
            this.ApiResponse = apiInstance.GetClientAppUpdateInfo((string)this.ApiParam[APIParam.DeviceId], config.UserAgent);
        }

        /// <summary>
        /// クライアントアプリケーションの起動バージョン情報取得の呼び出し
        /// </summary>
        private void CallClientApplicationVersionAPI()
        {
            Configuration config = new Configuration();
            config.BasePath = this.BasePath;
            config.UserAgent = (string)this.ApiParam[APIParam.UserAgent];
            config.WebProxy = this.APIConfiguration.GetWebProxy(this.BasePath);
            config.AccessToken = (string)this.ApiParam[APIParam.AccessToken];
            ClientAppApi apiInstance = new ClientAppApi(config);
            this.ApiResponse = apiInstance.GetClientAppVersion((string)this.ApiParam[APIParam.DeviceId], config.UserAgent);
        }

        /// <summary>
        /// クライアントアプリの更新情報取得の呼び出し
        /// </summary>
        private void CallClientApplicationDeviceUpdateInfomationAPI()
        {
            Configuration config = new Configuration();
            config.BasePath = this.BasePath;
            config.UserAgent = (string)this.ApiParam[APIParam.UserAgent];
            config.WebProxy = this.APIConfiguration.GetWebProxy(this.BasePath);
            InlineObjectDeviceUpdateInfo inlineObjectDeviceUpdateInfo = new InlineObjectDeviceUpdateInfo();
            inlineObjectDeviceUpdateInfo.OfflineDeviceId = (string)this.ApiParam[APIParam.OfflineDeviceId];
            inlineObjectDeviceUpdateInfo.IndefiniteAccessToken = (string)this.ApiParam[APIParam.IndefiniteAccessToken];
            inlineObjectDeviceUpdateInfo.Hostname = (string)this.ApiParam[APIParam.HostName];
            inlineObjectDeviceUpdateInfo.OsUserName = (string)this.ApiParam[APIParam.OSUserName];
            ClientAppApi apiInstance = new ClientAppApi(config);
            this.ApiResponse = apiInstance.GetClientAppDeviceUpdateInfo(config.UserAgent, inlineObjectDeviceUpdateInfo);
        }

    }
}

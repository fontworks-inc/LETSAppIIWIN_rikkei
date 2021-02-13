using System;
using System.Collections.Generic;
using Core.Entities;
using Core.Interfaces;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Infrastructure.API
{
    /// <summary>
    /// 端末情報を格納するAPIリポジトリ
    /// </summary>
    public class DevicesAPIRepository : APIRepositoryBase, IDevicesRepository
    {
        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="apiConfiguration">設定情報</param>
        public DevicesAPIRepository(APIConfiguration apiConfiguration)
            : base(apiConfiguration)
        {
        }

        /// <summary>
        /// 全端末情報を取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>ユーザに紐づく全端末情報(削除済みデータを除く)</returns>
        /// <remarks>FUNCTION_08_01_04(端末情報取得API)</remarks>
        public IList<Device> GetAllDevices(string deviceId, string accessToken)
        {
            List<Device> response = new List<Device>();

            // APIの引数の値をセット(個別処理)
            this.ApiParam[APIParam.DeviceId] = deviceId;
            this.ApiParam[APIParam.AccessToken] = accessToken;

            // API通信を行う(リトライ込み)を行う（共通処理）
            try
            {
                this.Invoke(this.CallGetDevicesApi);

                // 戻り値のセット（個別処理）
                var ret = (DevicesResponse)this.ApiResponse;
                if (ret.Code == (int)ResponseCode.Succeeded)
                {
                    Action<DevicesData> action = item =>
                                  {
                                      var device = new Device();
                                      device.DeviceId = item.DeviceId;
                                      device.HostName = item.Hostname;
                                      device.OSType = item.OsType;
                                      device.OSUserName = item.OsUserName;
                                      device.OSVersion = item.OsVersion;
                                      device.UserDeviceId = item.UserDeviceId;
                                      response.Add(device);
                                  };
                    ret.Data.ForEach(action);
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
        /// 端末を利用解除する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <param name="targetDeviceId">端末解除対象デバイスID</param>
        /// <remarks>FUNCTION_08_01_05(端末解除API)</remarks>
        public void DeactivateDevice(string deviceId, string accessToken, string targetDeviceId)
        {
            // APIの引数の値をセット(個別処理)
            this.ApiParam[APIParam.AccessToken] = accessToken;
            this.ApiParam[APIParam.TargetDeviceId] = targetDeviceId;

            // API通信を行う(リトライ込み)を行う（共通処理）
            try
            {
                this.Invoke(this.CallChangeDeviceOutOfUseApi);

                // 戻り値なし
            }
            catch (ApiException)
            {
                // 通信に失敗or通信しなかった
                throw;
            }
        }

        /// <summary>
        /// 端末を利用可能にする
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>認証情報</returns>
        /// <remarks>FUNCTION_08_01_06(端末使用API)</remarks>
        public AuthenticationInformation ActivateDevice(string deviceId, string accessToken)
        {
            AuthenticationInformation response = new AuthenticationInformation();

            // APIの引数の値をセット(個別処理)
            this.ApiParam[APIParam.DeviceId] = deviceId;
            this.ApiParam[APIParam.AccessToken] = accessToken;

            // API通信を行う(リトライ込み)を行う（共通処理）
            try
            {
                this.Invoke(this.CallChangeDeviceInUseApi);

                // 戻り値のセット（個別処理）
                var ret = (AccessTokenRefreshTokenResponse)this.ApiResponse;
                if (ret.Code == (int)ResponseCode.Succeeded
                    || ret.Code == (int)ResponseCode.MaximumNumberOfDevicesInUse)
                {
                    // 暫定的に端末数オーバーもOKとする(本来はないはず)
                    response.AccessToken = ret.Data.AccessToken;
                    response.RefreshToken = ret.Data.RefreshToken;
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
        /// デバイスIDを発行する
        /// </summary>
        /// <param name="user">ユーザ情報</param>
        /// <returns>デバイスID</returns>
        /// <remarks>FUNCTION_08_01_12(デバイスID発行API)</remarks>
        public string GetDeviceId(User user, string deviceKey)
        {
            string response = null;

            // APIの引数の値をセット(個別処理)
            this.ApiParam[APIParam.MailAddress] = user.MailAddress;
            this.ApiParam[APIParam.Password] = user.Password;
            this.ApiParam[APIParam.HostName] = user.HostName;
            this.ApiParam[APIParam.OSUserName] = user.OSUserName;
            this.ApiParam[APIParam.DeviceKey] = deviceKey;

            // API通信を行う(リトライ込み)を行う（共通処理）
            try
            {
                this.Invoke(this.CallPostDeviceIdApi);

                // 戻り値のセット（個別処理）
                var ret = (DeviceIdResponse)this.ApiResponse;
                if (ret.Code == (int)ResponseCode.Succeeded)
                {
                    response = ret.Data.DeviceId;
                }
                else
                {
                    throw new SystemException(ret.Code.ToString());
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
        /// 端末情報を取得する
        /// </summary>
        private void CallGetDevicesApi()
        {
            Configuration config = new Configuration();
            config.BasePath = this.BasePath;
            config.UserAgent = (string)this.ApiParam[APIParam.UserAgent];
            config.AccessToken = (string)this.ApiParam[APIParam.AccessToken];
            LoginApi apiInstance = new LoginApi(config);

            this.ApiResponse = apiInstance.GetDevices((string)this.ApiParam[APIParam.DeviceId], config.UserAgent);
        }

        /// <summary>
        /// 端末解除の呼び出し
        /// </summary>
        private void CallChangeDeviceOutOfUseApi()
        {
            Configuration config = new Configuration();
            config.BasePath = this.BasePath;
            config.UserAgent = (string)this.ApiParam[APIParam.UserAgent];
            config.AccessToken = (string)this.ApiParam[APIParam.AccessToken];
            LoginApi apiInstance = new LoginApi(config);
            var body = new InlineObject2((string)this.ApiParam[APIParam.TargetDeviceId]);
            this.ApiResponse = apiInstance.ChangeDeviceOutOfUse((string)this.ApiParam[APIParam.DeviceId], config.UserAgent, body);
        }

        /// <summary>
        /// 端末使用の呼び出し
        /// </summary>
        private void CallChangeDeviceInUseApi()
        {
            Configuration config = new Configuration();
            config.BasePath = this.BasePath;
            config.UserAgent = (string)this.ApiParam[APIParam.UserAgent];
            config.AccessToken = (string)this.ApiParam[APIParam.AccessToken];
            LoginApi apiInstance = new LoginApi(config);
            var body = new object();
            this.ApiResponse = apiInstance.ChangeDeviceInUse((string)this.ApiParam[APIParam.DeviceId], config.UserAgent, body);
        }

        /// <summary>
        /// デバイスID発行の呼び出し
        /// </summary>
        private void CallPostDeviceIdApi()
        {
            Configuration config = new Configuration();
            config.BasePath = this.BasePath;
            config.UserAgent = (string)this.ApiParam[APIParam.UserAgent];
            if (this.ApiParam.ContainsKey(APIParam.AccessToken))
            {
                config.AccessToken = (string)this.ApiParam[APIParam.AccessToken];
            }

            LoginApi apiInstance = new LoginApi(config);
            var body = new InlineObject4(
                (string)this.ApiParam[APIParam.MailAddress],
                (string)this.ApiParam[APIParam.Password],
                (string)this.ApiParam[APIParam.HostName],
                (string)this.ApiParam[APIParam.OSUserName],
                (string)this.ApiParam[APIParam.DeviceKey]);
            this.ApiResponse = apiInstance.PostDeviceId(config.UserAgent, body);
        }
    }
}

using System;
using Core.Entities;
using Core.Interfaces;
using NLog;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Infrastructure.API
{
    /// <summary>
    /// 認証情報を格納するリポジトリ
    /// </summary>
    public class AuthenticationInformationAPIRepository : APIRepositoryBase, IAuthenticationInformationRepository
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="apiConfiguration">設定情報</param>
        public AuthenticationInformationAPIRepository(APIConfiguration apiConfiguration)
            : base(apiConfiguration)
        {
        }

        /// <summary>
        /// ログインする
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="mailAddress">メールアドレス</param>
        /// <param name="password">パスワード</param>
        /// <returns>認証情報</returns>
        /// <remarks>FUNCTION_08_01_01(ログインAPI)</remarks>
        public AuthenticationInformationResponse Login(string deviceId, string mailAddress, string password)
        {
            AuthenticationInformationResponse response = new AuthenticationInformationResponse();

            Logger.Info(string.Format(
                "AuthenticationInformationAPIRepository:Login Enter", string.Empty));

            // APIの引数の値をセット(個別処理)
            Logger.Debug("AuthenticationInformationAPIRepository:Login APIの引数の値をセット(個別処理)");

            this.ApiParam[APIParam.DeviceId] = deviceId;
            this.ApiParam[APIParam.MailAddress] = mailAddress;
            this.ApiParam[APIParam.Password] = password;

            // API通信を行う(リトライ込み)を行う（共通処理）
            try
            {
                Logger.Info(string.Format(
                    "AuthenticationInformationAPIRepository:Login API通信を行う(リトライ込み)を行う（共通処理）", string.Empty));
                this.Invoke(this.CallLoginApi);

                // 戻り値のセット（個別処理）
                Logger.Info(string.Format("AuthenticationInformationAPIRepository:Login 戻り値のセット（個別処理）", string.Empty));
                var ret = (AccessTokenRefreshTokenResponse)this.ApiResponse;
                response.Code = ret.Code;
                response.Message = ret.Message;
                if (response.Code == (int)ResponseCode.Succeeded || response.Code == (int)ResponseCode.MaximumNumberOfDevicesInUse)
                {
                    response.Data = new AuthenticationInformation(ret.Data.AccessToken, ret.Data.RefreshToken);
                }
            }
            catch (ApiException)
            {
                // 通信に失敗or通信しなかった
                Logger.Debug("AuthenticationInformationAPIRepository:Login 通信に失敗or通信しなかった");
                throw;
            }

            Logger.Debug("AuthenticationInformationAPIRepository:Login return");
            return response;
        }

        /// <summary>
        /// ログアウトする
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <remarks>FUNCTION_08_01_07(ログアウトAPI)</remarks>
        public void Logout(string deviceId, string accessToken)
        {
            // APIの引数の値をセット(個別処理)
            this.ApiParam[APIParam.DeviceId] = deviceId;
            this.ApiParam[APIParam.AccessToken] = accessToken;

            // API通信を行う(リトライ込み)を行う（共通処理）
            try
            {
                this.Invoke(this.CallLogoutApi);

                var ret = (Model200)this.ApiResponse;
                if (ret.Code != (int)ResponseCode.Succeeded)
                {
                    throw new ApiException(ret.Code, ret.Message);
                }

                // 戻り値なし
            }
            catch (ApiException)
            {
                // 通信に失敗or通信しなかった
                throw;
            }
        }

        /// <summary>
        /// 二要素認証をする
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="twoFactCode">認証コード</param>
        /// <returns>認証情報</returns>
        /// <remarks>FUNCTION_08_01_03(2要素認証API)</remarks>
        public AuthenticationInformationResponse TwoFactAuthentication(string deviceId, string twoFactCode)
        {
            AuthenticationInformationResponse response = new AuthenticationInformationResponse();

            // APIの引数の値をセット(個別処理)
            this.ApiParam[APIParam.DeviceId] = deviceId;
            this.ApiParam[APIParam.TwoFactCode] = twoFactCode;

            // API通信を行う(リトライ込み)を行う（共通処理）
            try
            {
                this.Invoke(this.CallAuth2factApi);

                // 戻り値のセット（個別処理）
                var ret = (AccessTokenRefreshTokenResponse)this.ApiResponse;
                response.Code = ret.Code;
                response.Message = ret.Message;
                if (response.Code == (int)ResponseCode.Succeeded || response.Code == (int)ResponseCode.MaximumNumberOfDevicesInUse)
                {
                    response.Data = new AuthenticationInformation(ret.Data.AccessToken, ret.Data.RefreshToken);
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
        /// オンライン利用／オフライン利用を判定する
        /// </summary>
        /// <param name="mailAddress">メールアドレス</param>
        /// <param name="password">パスワード</param>
        /// <returns>認証情報</returns>
        /// <remarks>FUNCTION_08_01_13(アカウント認証API)</remarks>
        public AuthenticationInformationResponse AuthenticateAccount(string mailAddress, string password)
        {
            AuthenticationInformationResponse response = new AuthenticationInformationResponse();

            Logger.Info(string.Format(
                "AuthenticationInformationAPIRepository:AuthenticateAccount Enter", string.Empty));

            // APIの引数の値をセット(個別処理)
            Logger.Debug("AuthenticationInformationAPIRepository:AuthenticateAccount APIの引数の値をセット(個別処理)");

            this.ApiParam[APIParam.MailAddress] = mailAddress;
            this.ApiParam[APIParam.Password] = password;

            // API通信を行う(リトライ込み)を行う（共通処理）
            try
            {
                Logger.Info(string.Format(
                    "AuthenticationInformationAPIRepository:AuthenticateAccount API通信を行う(リトライ込み)を行う（共通処理）", string.Empty));
                this.Invoke(this.CallAuthenticateAccountApi);

                // 戻り値のセット（個別処理）
                Logger.Info(string.Format("AuthenticationInformationAPIRepository:AuthenticateAccount 戻り値のセット（個別処理）", string.Empty));
                var ret = (AccessTokenRefreshTokenResponse)this.ApiResponse;
                response.Code = ret.Code;
                response.Message = ret.Message;
                if (response.Code == (int)ResponseCode.Succeeded || response.Code == (int)ResponseCode.MaximumNumberOfDevicesInUse)
                {
                    response.Data = new AuthenticationInformation(ret.Data.AccessToken, ret.Data.RefreshToken);
                }
            }
            catch (ApiException)
            {
                // 通信に失敗or通信しなかった
                Logger.Debug("AuthenticationInformationAPIRepository:AuthenticateAccount 通信に失敗or通信しなかった");
                throw;
            }

            Logger.Debug("AuthenticationInformationAPIRepository:AuthenticateAccount return");
            return response;
        }

        /// <summary>
        /// ログインの呼び出し
        /// </summary>
        private void CallLoginApi()
        {
            Logger.Debug("AuthenticationInformationAPIRepository:CallLoginApi Enter");
            Configuration config = new Configuration();
            config.BasePath = this.BasePath;
            config.UserAgent = (string)this.ApiParam[APIParam.UserAgent];
            config.WebProxy = this.APIConfiguration.GetWebProxy(this.BasePath);
            LoginApi apiInstance = new LoginApi(config);
            var body = new InlineObject(
                (string)this.ApiParam[APIParam.MailAddress],
                (string)this.ApiParam[APIParam.Password]);
            Logger.Debug("AuthenticationInformationAPIRepository:CallLoginApi apiInstance.Login:Before");
            Logger.Debug("AuthenticationInformationAPIRepository:CallLoginApi ApiParam[APIParam.DeviceId]=" + this.ApiParam[APIParam.DeviceId]);
            this.ApiResponse = apiInstance.Login((string)this.ApiParam[APIParam.DeviceId], config.UserAgent, body);
            Logger.Debug("AuthenticationInformationAPIRepository:CallLoginApi apiInstance.Login:After");
        }

        /// <summary>
        /// ログアウトの呼び出し
        /// </summary>
        private void CallLogoutApi()
        {
            Configuration config = new Configuration();
            config.BasePath = this.BasePath;
            config.UserAgent = (string)this.ApiParam[APIParam.UserAgent];
            config.WebProxy = this.APIConfiguration.GetWebProxy(this.BasePath);
            config.AccessToken = (string)this.ApiParam[APIParam.AccessToken];
            LoginApi apiInstance = new LoginApi(config);
            var body = new object();
            this.ApiResponse = apiInstance.Logout((string)this.ApiParam[APIParam.DeviceId], config.UserAgent, body);
        }

        /// <summary>
        /// ２要素認証の呼び出し
        /// </summary>
        private void CallAuth2factApi()
        {
            Configuration config = new Configuration();
            config.BasePath = this.BasePath;
            config.UserAgent = (string)this.ApiParam[APIParam.UserAgent];
            config.WebProxy = this.APIConfiguration.GetWebProxy(this.BasePath);
            LoginApi apiInstance = new LoginApi(config);
            var body = new InlineObject1((string)this.ApiParam[APIParam.TwoFactCode]);
            this.ApiResponse = apiInstance.Auth2fact((string)this.ApiParam[APIParam.DeviceId], config.UserAgent, body);
        }

        /// <summary>
        /// ログインの呼び出し
        /// </summary>
        private void CallAuthenticateAccountApi()
        {
            Logger.Debug("AuthenticationInformationAPIRepository:CallAuthenticateAccountApi Enter");
            Configuration config = new Configuration();
            config.BasePath = this.BasePath;
            config.UserAgent = (string)this.ApiParam[APIParam.UserAgent];
            config.WebProxy = this.APIConfiguration.GetWebProxy(this.BasePath);
            LoginApi apiInstance = new LoginApi(config);
            var body = new InlineObject(
                (string)this.ApiParam[APIParam.MailAddress],
                (string)this.ApiParam[APIParam.Password]);
            Logger.Debug("AuthenticationInformationAPIRepository:CallAuthenticateAccountApi apiInstance.AuthenticateAccount:Before");
            this.ApiResponse = apiInstance.AuthenticateAccount(config.UserAgent, body);
            Logger.Debug("AuthenticationInformationAPIRepository:CallAuthenticateAccountApi apiInstance.AuthenticateAccount:After");
        }

    }
}

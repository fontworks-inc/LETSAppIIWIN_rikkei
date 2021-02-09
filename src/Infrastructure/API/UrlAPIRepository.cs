using System;
using Core.Entities;
using Core.Interfaces;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Infrastructure.API
{
    /// <summary>
    ///  URLアドレスを格納するリポジトリのインターフェイス
    /// </summary>
    public class UrlAPIRepository : APIRepositoryBase, IUrlRepository
    {
        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="apiConfiguration">設定情報</param>
        public UrlAPIRepository(APIConfiguration apiConfiguration)
            : base(apiConfiguration)
        {
        }

        /// <summary>
        /// パスワード再設定ページのURLを取得する
        /// </summary>
        /// <returns>パスワード再設定ページのURL</returns>
        /// <remarks>FUNCTION_08_01_10(パスワードを忘れた方_再設定画面URLの取得API)</remarks>
        public Core.Entities.Url GetResetPasswordPageUrl()
        {
            Core.Entities.Url response = null;

            // API通信を行う(リトライ込み)を行う（共通処理）
            try
            {
                this.Invoke(this.CallGetPasswordResetUrl);

                // 戻り値のセット（個別処理）
                var ret = (UrlResponse)this.ApiResponse;
                if (ret.Code == (int)ResponseCode.Succeeded)
                {
                    response = new Core.Entities.Url(url: ret.Data.Url);
                }
            }
            catch (ApiException)
            {
                // 通信に失敗or通信しなかった
                // nullを返す
            }

            return response;
        }

        /// <summary>
        /// 会員登録ページのURLを取得する
        /// </summary>
        /// <returns>会員登録ページのURL</returns>
        /// <remarks>FUNCTION_08_01_11(会員登録画面URLの取得API)</remarks>
        public Core.Entities.Url GetUserRegistrationPageUrl()
        {
            Core.Entities.Url response = null;

            // API通信を行う(リトライ込み)を行う（共通処理）
            try
            {
                this.Invoke(this.CallGetMemberRegistrationUrl);

                // 戻り値のセット（個別処理）
                var ret = (UrlResponse)this.ApiResponse;
                if (ret.Code == (int)ResponseCode.Succeeded)
                {
                    response = new Core.Entities.Url(url: ret.Data.Url);
                }
            }
            catch (ApiException)
            {
                // 通信に失敗or通信しなかった
                // nullを返す
            }

            return response;
        }

        /// <summary>
        /// ホーム画面URLを取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>ホーム画面URL</returns>
        /// <remarks>FUNCTION_08_03_01(ホーム画面URLの取得API)</remarks>
        public Core.Entities.Url GetUserPageUrl(string deviceId, string accessToken)
        {
            this.ApiParam[APIParam.DeviceId] = deviceId;
            this.ApiParam[APIParam.AccessToken] = accessToken;

            Core.Entities.Url response = null;

            // API通信を行う(リトライ込み)を行う（共通処理）
            try
            {
                this.Invoke(this.CallGetHomeUrl);

                // 戻り値のセット（個別処理）
                var ret = (UrlResponse)this.ApiResponse;
                if (ret.Code == (int)ResponseCode.Succeeded)
                {
                    response = new Core.Entities.Url(url: ret.Data.Url);
                }
                else
                {
                   throw new ApiException(ret.Code, ret.Message);
                }
            }
            catch (ApiException)
            {
                // 通信に失敗or通信しなかった
                // nullを返す
                throw;
            }

            return response;
        }

        /// <summary>
        /// お知らせ画面URLを取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>お知らせ画面URL</returns>
        /// <remarks>FUNCTION_08_04_01(お知らせ画面URLの取得API)</remarks>
        public Core.Entities.Url GetAnnouncePageUrl(string deviceId, string accessToken)
        {
            this.ApiParam[APIParam.DeviceId] = deviceId;
            this.ApiParam[APIParam.AccessToken] = accessToken;

            Core.Entities.Url response = null;

            // API通信を行う(リトライ込み)を行う（共通処理）
            try
            {
                this.Invoke(this.CallGetNoticeUrl);

                // 戻り値のセット（個別処理）
                var ret = (UrlResponse)this.ApiResponse;
                if (ret.Code == (int)ResponseCode.Succeeded)
                {
                    response = new Core.Entities.Url(url: ret.Data.Url);
                }
            }
            catch (ApiException)
            {
                // 通信に失敗or通信しなかった
                // nullを返す
            }

            return response;
        }

        /// <summary>
        /// フォント一覧画面URLを取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>フォント一覧画面URL</returns>
        /// <remarks>FUNCTION_08_06_01(フォント一覧画面URLの取得API)</remarks>
        public Core.Entities.Url GetFontListPageUrl(string deviceId, string accessToken)
        {
            this.ApiParam[APIParam.DeviceId] = deviceId;
            this.ApiParam[APIParam.AccessToken] = accessToken;

            Core.Entities.Url response = null;

            // API通信を行う(リトライ込み)を行う（共通処理）
            try
            {
                this.Invoke(this.CallGetFontListUrl);

                // 戻り値のセット（個別処理）
                var ret = (UrlResponse)this.ApiResponse;
                if (ret.Code == (int)ResponseCode.Succeeded)
                {
                    response = new Core.Entities.Url(url: ret.Data.Url);
                }
                else
                {
                    throw new ApiException(ret.Code, ret.Message);
                }
            }
            catch (ApiException)
            {
                // 通信に失敗or通信しなかった
                // nullを返す
                throw;
            }

            return response;
        }

        /// <summary>
        /// パスワード再設定ページのURLの取得呼び出し
        /// </summary>
        private void CallGetPasswordResetUrl()
        {
            Configuration config = new Configuration();
            config.BasePath = this.BasePath;
            config.UserAgent = (string)this.ApiParam[APIParam.UserAgent];
            LoginApi apiInstance = new LoginApi(config);
            this.ApiResponse = apiInstance.GetPasswordResetUrl(config.UserAgent);
        }

        /// <summary>
        /// 会員登録画面URLの取得呼び出し
        /// </summary>
        private void CallGetMemberRegistrationUrl()
        {
            Configuration config = new Configuration();
            config.BasePath = this.BasePath;
            config.UserAgent = (string)this.ApiParam[APIParam.UserAgent];
            LoginApi apiInstance = new LoginApi(config);
            this.ApiResponse = apiInstance.GetMemberRegistrationUrl(config.UserAgent);
        }

        /// <summary>
        /// ホーム画面URLの取得呼び出し
        /// </summary>
        private void CallGetHomeUrl()
        {
            Configuration config = new Configuration();
            config.BasePath = this.BasePath;
            config.UserAgent = (string)this.ApiParam[APIParam.UserAgent];
            config.AccessToken = (string)this.ApiParam[APIParam.AccessToken];
            ContractApi apiInstance = new ContractApi(config);
            this.ApiResponse = apiInstance.GetHomeUrl((string)this.ApiParam[APIParam.DeviceId], config.UserAgent);
        }

        /// <summary>
        /// お知らせ画面URLの取得呼び出し
        /// </summary>
        private void CallGetNoticeUrl()
        {
            Configuration config = new Configuration();
            config.BasePath = this.BasePath;
            config.UserAgent = (string)this.ApiParam[APIParam.UserAgent];
            config.AccessToken = (string)this.ApiParam[APIParam.AccessToken];
            NoticeApi apiInstance = new NoticeApi(config);
            this.ApiResponse = apiInstance.GetNoticeUrl((string)this.ApiParam[APIParam.DeviceId], config.UserAgent);
        }

        /// <summary>
        /// フォント一覧画面URLの取得呼び出し
        /// </summary>
        private void CallGetFontListUrl()
        {
            Configuration config = new Configuration();
            config.BasePath = this.BasePath;
            config.UserAgent = (string)this.ApiParam[APIParam.UserAgent];
            config.AccessToken = (string)this.ApiParam[APIParam.AccessToken];
            FontApi apiInstance = new FontApi(config);
            this.ApiResponse = apiInstance.GetFontListUrl((string)this.ApiParam[APIParam.DeviceId], config.UserAgent);
        }
    }
}

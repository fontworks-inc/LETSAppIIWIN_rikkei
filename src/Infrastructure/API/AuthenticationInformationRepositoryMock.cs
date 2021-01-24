using System;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.API
{
    /// <summary>
    /// 認証情報を格納するリポジトリのモック
    /// </summary>
    public class AuthenticationInformationRepositoryMock : APIRepositoryBase, IAuthenticationInformationRepository
    {
        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="apiConfiguration">設定情報</param>
        /// <returns>認証情報</returns>
        public AuthenticationInformationRepositoryMock(APIConfiguration apiConfiguration)
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
            //// 正常
            // return this.GetTestDataSucceeded();

            // ２要素認証要求
            return this.GetTestDataTwoFAIsRequired();

            //// 認証エラー
            // return this.GetTestDataAuthenticationFailed();

            //// ２要素認証コード有効期限切れエラー
            // return this.GetTestDataTwoFACodeHasExpired();

            //// 同時使用デバイス数の上限エラー
            // return this.GetTestDataMaximumNumberOfDevicesInUse();

            //// その他のエラー
            // return this.GetTestDataInvalidResponseCodeException();

            //// 配信サーバアクセスエラー
            // throw new Exception("server access error.");
        }

        /// <summary>
        /// ログアウトする
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <remarks>FUNCTION_08_01_07(ログアウトAPI)</remarks>
        public void Logout(string deviceId, string accessToken)
        {
            // 正常系の場合は何も返さない
            // 異常系の場合はとりあえずException
            // throw new Exception("server access error.");
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
            //// 正常
            // return this.GetTestDataSucceeded();

            //// ２要素認証要求
            // return this.GetTestDataTwoFAIsRequired();

            //// 認証エラー
            // return this.GetTestDataAuthenticationFailed();

            //// ２要素認証コード有効期限切れエラー
            // return this.GetTestDataTwoFACodeHasExpired();

            // 同時使用デバイス数の上限エラー
            return this.GetTestDataMaximumNumberOfDevicesInUse();

            //// その他のエラー
            // return this.GetTestDataInvalidResponseCodeException();

            //// 配信サーバアクセスエラー
            // throw new Exception("server access error.");
        }

        /// <summary>
        /// テストデータ(正常)を取得
        /// </summary>
        private AuthenticationInformationResponse GetTestDataSucceeded()
        {
            return new AuthenticationInformationResponse()
            {
                Code = (int)ResponseCode.Succeeded,
                Message = "succeeded",
                Data = new AuthenticationInformation()
                {
                    AccessToken = "access_token_01",
                    RefreshToken = "refresh_token_01",
                },
            };
        }

        /// <summary>
        /// テストデータ(２要素認証要求)を取得
        /// </summary>
        private AuthenticationInformationResponse GetTestDataTwoFAIsRequired()
        {
            return new AuthenticationInformationResponse()
            {
                Code = (int)ResponseCode.TwoFAIsRequired,
                Message = "2FA is required",
                Data = new AuthenticationInformation()
                {
                    AccessToken = "access_token_02",
                    RefreshToken = "refresh_token_02",
                },
            };
        }

        /// <summary>
        /// テストデータ(認証エラー)を取得
        /// </summary>
        private AuthenticationInformationResponse GetTestDataAuthenticationFailed()
        {
            return new AuthenticationInformationResponse()
            {
                Code = (int)ResponseCode.AuthenticationFailed,
                Message = "authentication failed",
                Data = new AuthenticationInformation()
                {
                    AccessToken = "access_token_03",
                    RefreshToken = "refresh_token_03",
                },
            };
        }

        /// <summary>
        /// テストデータ(２要素認証コード有効期限切れエラー)を取得
        /// </summary>
        private AuthenticationInformationResponse GetTestDataTwoFACodeHasExpired()
        {
            return new AuthenticationInformationResponse()
            {
                Code = (int)ResponseCode.TwoFACodeHasExpired,
                Message = "the 2FA code has expired",
                Data = new AuthenticationInformation()
                {
                    AccessToken = "access_token_04",
                    RefreshToken = "refresh_token_04",
                },
            };
        }

        /// <summary>
        /// テストデータ(同時使用デバイス数の上限エラー)を取得
        /// </summary>
        private AuthenticationInformationResponse GetTestDataMaximumNumberOfDevicesInUse()
        {
            return new AuthenticationInformationResponse()
            {
                Code = (int)ResponseCode.MaximumNumberOfDevicesInUse,
                Message = "your account has exceeded the maximum number of devices in use",
                Data = new AuthenticationInformation()
                {
                    AccessToken = "access_token_05",
                    RefreshToken = "refresh_token_05",
                },
            };
        }

        /// <summary>
        /// テストデータ(その他のエラー)を取得
        /// </summary>
        private AuthenticationInformationResponse GetTestDataInvalidResponseCodeException()
        {
            return new AuthenticationInformationResponse()
            {
                Code = 9999,
                Message = "error message",
                Data = new AuthenticationInformation()
                {
                    AccessToken = "access_token_99",
                    RefreshToken = "refresh_token_99",
                },
            };
        }
    }
}

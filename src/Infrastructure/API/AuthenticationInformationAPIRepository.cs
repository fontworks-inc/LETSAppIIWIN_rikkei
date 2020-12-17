using System;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.API
{
    /// <summary>
    /// 認証情報を格納するリポジトリ
    /// </summary>
    public class AuthenticationInformationAPIRepository : APIRepositoryBase, IAuthenticationInformationRepository
    {
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// ログアウトする
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <remarks>FUNCTION_08_01_07(ログアウトAPI)</remarks>
        public void Logout(string deviceId, string accessToken)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}

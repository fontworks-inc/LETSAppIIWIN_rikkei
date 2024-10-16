using Core.Entities;

namespace Core.Interfaces
{
    /// <summary>
    /// 認証情報を格納するリポジトリのインターフェイス
    /// </summary>
    public interface IAuthenticationInformationRepository
    {
        /// <summary>
        /// ログインする
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="mailAddress">メールアドレス</param>
        /// <param name="password">パスワード</param>
        /// <returns>認証情報</returns>
        AuthenticationInformationResponse Login(string deviceId, string mailAddress, string password);

        /// <summary>
        /// ログアウトする
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        void Logout(string deviceId, string accessToken);

        /// <summary>
        /// 二要素認証をする
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="twoFactCode">認証コード</param>
        /// <returns>認証情報</returns>
        AuthenticationInformationResponse TwoFactAuthentication(string deviceId, string twoFactCode);

        /// <summary>
        /// オンライン利用／オフライン利用を判定する
        /// </summary>
        /// <param name="mailAddress">メールアドレス</param>
        /// <param name="password">パスワード</param>
        /// <returns>認証情報</returns>
        AuthenticationInformationResponse AuthenticateAccount(string mailAddress, string password);

        /// <summary>
        /// リフレッシュトークン再取得を行う
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>認証情報</returns>
        RefreshTokenResponse RefreshToken(string deviceId, string accessToken);
    }
}

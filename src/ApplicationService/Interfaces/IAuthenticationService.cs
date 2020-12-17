using Core.Entities;

namespace ApplicationService.Interfaces
{
    /// <summary>
    /// 認証サービスのインターフェイス
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// ログインする
        /// </summary>
        /// <param name="mailAddress">メールアドレス</param>
        /// <param name="password">パスワード</param>
        /// <returns>認証情報</returns>
        AuthenticationInformationResponse Login(string mailAddress, string password);

        /// <summary>
        /// ログイン情報を保存する（ログイン完了処理）
        /// </summary>
        /// <param name="authenticationInformation">認証情報</param>
        /// <remarks>ログイン完了処理でのデータ保存部分</remarks>
        void SaveLoginInfo(AuthenticationInformation authenticationInformation);

        /// <summary>
        /// ログアウトする
        /// </summary>
        /// <returns>ログアウト処理の成功時にtrue、それ以外はfalse</returns>
        bool Logout();
    }
}

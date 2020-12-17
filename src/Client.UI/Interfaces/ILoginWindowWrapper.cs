using System.Windows.Navigation;
using Client.UI.Views;
using Core.Entities;

namespace Client.UI.Interfaces
{
    /// <summary>
    /// LoginWindowWrapper のインタフェース
    /// </summary>
    public interface ILoginWindowWrapper
    {
        /// <summary>
        /// (メイン)ログイン画面
        /// </summary>
        LoginWindow Window { get; set; }

        /// <summary>
        /// NavigationService を取得
        /// </summary>
        NavigationService NavigationService { get; }

        /// <summary>
        /// (メイン)ログイン画面を閉じる
        /// </summary>
        void Close();

        /// <summary>
        /// 認証情報を設定する
        /// </summary>
        /// <param name="authenticationInformation">認証情報</param>
        void SetAuthenticationInformation(
            AuthenticationInformation authenticationInformation);

        /// <summary>
        /// 認証情報を取得する
        /// </summary>
        /// <returns>認証情報</returns>
        AuthenticationInformation GetAuthenticationInformation();
    }
}

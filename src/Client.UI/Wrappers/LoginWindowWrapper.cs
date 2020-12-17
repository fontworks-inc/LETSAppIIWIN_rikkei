using System.Windows;
using System.Windows.Navigation;
using Client.UI.Interfaces;
using Client.UI.Views;
using Core.Entities;

namespace Client.UI.Wrappers
{
    /// <summary>
    /// LoginWindowWrapper クラス
    /// </summary>
    public class LoginWindowWrapper : ILoginWindowWrapper
    {
        /// <summary>
        /// 認証情報
        /// </summary>
        private AuthenticationInformation authenticationInformation;

        /// <summary>
        /// (メイン)ログイン画面
        /// </summary>
        public LoginWindow Window { get; set; }

        /// <summary>
        /// NavigationService を取得
        /// </summary>
        public NavigationService NavigationService
        {
            get { return this.Window.MainFrame.NavigationService; }
        }

        /// <summary>
        /// (メイン)ログイン画面を閉じる
        /// </summary>
        public void Close()
        {
            this.Window.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// 認証情報を設定する
        /// </summary>
        /// <param name="authenticationInformation">認証情報</param>
        public void SetAuthenticationInformation(
            AuthenticationInformation authenticationInformation)
        {
            this.authenticationInformation = authenticationInformation;
        }

        /// <summary>
        /// 認証情報を取得する
        /// </summary>
        /// <returns>認証情報</returns>
        public AuthenticationInformation GetAuthenticationInformation()
        {
            return this.authenticationInformation;
        }
    }
}

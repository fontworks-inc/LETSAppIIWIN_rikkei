using System.Windows;
using System.Windows.Media;
using Client.UI.Interfaces;
using Core.Interfaces;
using Prism.Commands;
using Prism.Mvvm;

namespace Client.UI.ViewModels
{
    /// <summary>
    /// ログイン完了画面ビューモデル
    /// </summary>
    /// <remarks>画面ID：APP_06_01</remarks>
    public class LoginCompletedViewModel : BindableBase
    {
        /// <summary>
        /// (メイン)ログイン画面
        /// </summary>
        private ILoginWindowWrapper loginWindow;

        /// <summary>
        /// リソース読込み
        /// </summary>
        private IResourceWrapper resouceWrapper;

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="loginWindowWrapper">LoginWindowのラッパー</param>
        /// <param name="resouceWrapper">Resourceのラッパー</param>
        public LoginCompletedViewModel(
            ILoginWindowWrapper loginWindowWrapper,
            IResourceWrapper resouceWrapper)
        {
            this.loginWindow = loginWindowWrapper;
            this.resouceWrapper = resouceWrapper;

            this.FontsListButtonClick = new DelegateCommand(this.OnFontsListButtonClick);
        }

        /// <summary>
        /// フォント一覧へボタンクリックコマンド
        /// </summary>
        public DelegateCommand FontsListButtonClick { get; }

        /// <summary>
        /// ロゴ画像ソース
        /// </summary>
        public ImageSource ImageLogo
        {
            get
            {
                return this.resouceWrapper.GetImageSource("IMG_LOGO");
            }
        }

        /// <summary>
        /// 処理タイトル
        /// </summary>
        public string ProcessTitle
        {
            get
            {
                return this.resouceWrapper.GetString("APP_06_01_PROC_TITLE");
            }
        }

        /// <summary>
        /// 処理説明
        /// </summary>
        public string ProcessDescription
        {
            get
            {
                return this.resouceWrapper.GetString("APP_06_01_PROC_DESCRIPTION");
            }
        }

        /// <summary>
        /// フォント一覧へボタン名
        /// </summary>
        public string FontsListButtonTitle
        {
            get
            {
                return this.resouceWrapper.GetString("APP_06_01_BTN_FONTLIST");
            }
        }

        /// <summary>
        /// フォント一覧ボタンクリック時の処理
        /// </summary>
        private void OnFontsListButtonClick()
        {
            MessageBox.Show("フォント一覧ページを表示");

            this.loginWindow.Close();
        }
    }
}

using System.Windows.Media;
using Client.UI.Interfaces;
using Core.Interfaces;
using Prism.Mvvm;

namespace Client.UI.ViewModels
{
    /// <summary>
    /// 強制ログアウトダイアログビューモデル
    /// </summary>
    /// <remarks>画面ID：APP_08_01</remarks>
    public class ForceLogoutNotificationViewModel : BindableBase
    {
        /// <summary>
        /// リソース読込み
        /// </summary>
        private readonly IResourceWrapper resouceWrapper;

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="resouceWrapper">Resourceのラッパー</param>
        public ForceLogoutNotificationViewModel(IResourceWrapper resouceWrapper)
        {
            this.resouceWrapper = resouceWrapper;
        }

        /// <summary>
        /// 画面タイトル
        /// </summary>
        public string Title
        {
            get
            {
                return this.resouceWrapper.GetString("APP_08_01_WIN_TITLE");
            }
        }

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
                return this.resouceWrapper.GetString("APP_08_01_PROC_TITLE");
            }
        }

        /// <summary>
        /// 処理説明
        /// </summary>
        public string ProcessDescription
        {
            get
            {
                return this.resouceWrapper.GetString("APP_08_01_PROC_DESCRIPTION");
            }
        }

        /// <summary>
        /// ログアウトボタン名
        /// </summary>
        public string LogoutButtonTitle
        {
            get
            {
                return this.resouceWrapper.GetString("APP_08_01_BTN_LOGOUT");
            }
        }
    }
}

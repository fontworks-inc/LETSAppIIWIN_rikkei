using System;
using System.Windows.Media;
using Core.Interfaces;
using Prism.Mvvm;

namespace Client.UI.ViewModels
{
    /// <summary>
    /// ログアウト確認ダイアログビューモデル
    /// </summary>
    /// <remarks>画面ID：APP_9_01</remarks>
    public class LogoutConfirmationViewModel : BindableBase
    {
        /// <summary>
        /// リソース読込み
        /// </summary>
        private readonly IResourceWrapper resouceWrapper;

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="resouceWrapper">Resourceのラッパー</param>
        public LogoutConfirmationViewModel(IResourceWrapper resouceWrapper)
        {
            this.resouceWrapper = resouceWrapper;
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
                return this.resouceWrapper.GetString("APP_09_01_PROC_TITLE");
            }
        }

        /// <summary>
        /// 処理説明
        /// </summary>
        public string ProcessDescription
        {
            get
            {
                return this.resouceWrapper.GetString("APP_09_01_PROC_DESCRIPTION");
            }
        }

        /// <summary>
        /// ログアウトボタン名
        /// </summary>
        public string LogoutButtonTitle
        {
            get
            {
                return this.resouceWrapper.GetString("APP_09_01_BTN_LOGOUT");
            }
        }

        /// <summary>
        /// キャンセルボタン名
        /// </summary>
        public string CancelButtonTitle
        {
            get
            {
                return this.resouceWrapper.GetString("APP_09_01_BTN_CANCEL");
            }
        }

        /// <summary>
        /// 画面タイトル
        /// </summary>
        public string Title => throw new NotImplementedException();
    }
}

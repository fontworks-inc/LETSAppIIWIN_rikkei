using Core.Interfaces;
using Prism.Mvvm;

namespace Client.UI.ViewModels
{
    /// <summary>
    /// ログイン画面ビューモデル
    /// </summary>
    /// <remarks>画面ID：APP_04_01, APP_04_01_err</remarks>
    public class LoginWindowViewModel : BindableBase
    {
        /// <summary>
        /// リソース読込み
        /// </summary>
        private IResourceWrapper resouceWrapper;

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="resouceWrapper">Resourceのラッパー</param>
        public LoginWindowViewModel(IResourceWrapper resouceWrapper)
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
                return this.resouceWrapper.GetString("APP_04_WIN_TITLE");
            }
        }
    }
}

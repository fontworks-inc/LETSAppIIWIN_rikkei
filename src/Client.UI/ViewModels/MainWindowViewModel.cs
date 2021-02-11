using Prism.Mvvm;

namespace Client.UI.ViewModels
{
    /// <summary>
    /// メイン画面ビューモデル
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {
        /// <summary>
        /// 画面タイトル
        /// </summary>
        private string title = "Prism Application";

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        public MainWindowViewModel()
        {
        }

        /// <summary>
        /// 画面タイトル
        /// </summary>
        public string Title
        {
            get { return this.title; }
            set { this.SetProperty(ref this.title, value); }
        }

        /// <summary>
        /// 表示用の値
        /// </summary>
        public string TestValue
        {
            get;
            set;
        }
    }
}

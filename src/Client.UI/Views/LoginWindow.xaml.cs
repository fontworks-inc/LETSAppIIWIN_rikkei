using System.ComponentModel;
using System.Windows;
using Client.UI.Interfaces;
using Prism.Ioc;
using Prism.Unity;

namespace Client.UI.Views
{
    /// <summary>
    /// ログイン画面
    /// </summary>
    /// <remarks>画面ID：APP_04_01, APP_04_01_err</remarks>
    public partial class LoginWindow : Window
    {
        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        public LoginWindow()
        {
            this.InitializeComponent();

            this.Closing += (s, e) =>
            {
                this.OnClosing(e);
            };

            this.WindowState = WindowState.Normal;
            this.ShowInTaskbar = true;

            // コンテナに登録したオブジェクトに設定
            IContainerProvider container = (System.Windows.Application.Current as PrismApplication).Container;
            var loginWindowWrapper = container.Resolve<ILoginWindowWrapper>();
            loginWindowWrapper.Window = this;
        }

        /// <summary>
        /// 画面表示処理
        /// </summary>
        public new void ShowDialog()
        {
            this.WindowState = WindowState.Normal;
            this.ShowInTaskbar = true;

            this.MainFrame.NavigationService.Navigate(new Login());
            base.ShowDialog();
        }

        /// <summary>
        /// 画面が閉じるときの処理
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            // アプリケーションを終了しない
            this.Hide();
            e.Cancel = true;
        }
    }
}
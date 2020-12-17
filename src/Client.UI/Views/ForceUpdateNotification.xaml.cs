using System.ComponentModel;
using System.Windows;

namespace Client.UI.Views
{
    /// <summary>
    /// 強制アップデートの通知ダイアログ
    /// </summary>
    /// <remarks>画面ID：APP_7_01</remarks>
    public partial class ForceUpdateNotification : Window
    {
        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        public ForceUpdateNotification()
        {
            this.InitializeComponent();

            this.Closing += (s, e) =>
            {
                this.OnClosing(e);
            };

            this.OKButton.Click += (s, e) =>
            {
                this.Close();
            };

            this.WindowState = WindowState.Normal;
            this.ShowInTaskbar = true;
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

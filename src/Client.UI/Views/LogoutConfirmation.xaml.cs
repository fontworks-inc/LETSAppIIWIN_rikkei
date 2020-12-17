using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Client.UI.Views
{
    /// <summary>
    /// ログアウト確認ダイアログ
    /// </summary>
    public partial class LogoutConfirmation : Window
    {
        /// <summary>
        /// ログアウトするかどうか
        /// </summary>
        private bool executeLogout = false;

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        public LogoutConfirmation()
        {
            this.InitializeComponent();

            this.Closing += (s, e) =>
            {
                this.OnClosing(e);
            };
            this.LogoutButton.Click += (s, e) =>
            {
                this.executeLogout = true;
                this.Close();
            };
            this.CancelButton.Click += (s, e) =>
            {
                this.executeLogout = false;
                this.Close();
            };

            this.ShowInTaskbar = false;
        }

        /// <summary>
        /// ダイアログ表示処理
        /// </summary>
        /// <returns>結果</returns>
        public new bool? ShowDialog()
        {
            base.ShowDialog();
            return this.executeLogout;
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

        /// <summary>
        /// Window上でマウスの左ボタンが押されたときの処理
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">EventArgs</param>
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}

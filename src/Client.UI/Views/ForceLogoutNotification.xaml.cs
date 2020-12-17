using System;
using System.ComponentModel;
using System.Windows;
using Client.UI.Views.Helper;

namespace Client.UI.Views
{
    /// <summary>
    /// 強制ログアウトダイアログを表すクラス
    /// </summary>
    /// <remarks>画面ID：APP_08_01</remarks>
    public partial class ForceLogoutNotification : Window
    {
        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        public ForceLogoutNotification()
        {
            this.InitializeComponent();

            this.Closing += (s, e) =>
            {
                this.OnClosing(e);
            };

            this.LogoutButton.Click += (s, e) =>
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

        /// <summary>
        /// ウィンドウ初期化時の処理
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            WindowHelper.RemoveIcon(this);
            WindowHelper.RemoveFrameButton(this);
        }
    }
}

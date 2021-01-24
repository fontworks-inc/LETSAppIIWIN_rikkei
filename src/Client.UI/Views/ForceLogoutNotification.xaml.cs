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
        /// <remarks>本画面の中でログアウト処理は実施せず、画面を閉じたときにログアウト処理が実施される</remarks>
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

            // アイコン非表示
            WindowHelper.RemoveIcon(this);

            // 閉じるボタンの無効化（閉じるメニューを削除することで実現している）
            WindowHelper.RemoveCloseMenu(this);

            // タイトルバーのボタン削除 上記の閉じるボタンの無効化で対応するが、下記の場合はボタン全削除に対応
            // WindowHelper.RemoveFrameButton(this);
        }
    }
}

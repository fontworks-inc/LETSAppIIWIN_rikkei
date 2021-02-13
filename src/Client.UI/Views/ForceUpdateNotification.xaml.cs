using System;
using System.ComponentModel;
using System.Windows;
using Client.UI.Views.Helper;

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

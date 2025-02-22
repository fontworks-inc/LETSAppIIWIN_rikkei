﻿using System;
using System.ComponentModel;
using System.Windows;
using Client.UI.Interfaces;
using Client.UI.Views.Helper;
using Core.Interfaces;
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
        /// 画面の幅
        /// </summary>
        public static readonly int WindowWidth = 815;

        /// <summary>
        /// 画面の高さ
        /// </summary>
        public static readonly int WindowHeight = 510;

        /// <summary>
        /// (メイン)ログイン画面
        /// </summary>
        private readonly ILoginWindowWrapper loginWindowWrapper;

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

            this.Activated += (s, e) =>
            {
                this.WActivated(e);
            };

            this.WindowState = WindowState.Normal;
            this.ShowInTaskbar = true;

            // コンテナに登録したオブジェクトに設定
            IContainerProvider container = (System.Windows.Application.Current as PrismApplication).Container;
            this.loginWindowWrapper = container.Resolve<ILoginWindowWrapper>();
            this.loginWindowWrapper.Window = this;
        }

        /// <summary>
        /// 画面表示処理
        /// </summary>
        public new void ShowDialog()
        {
            this.WindowState = WindowState.Normal;
            this.ShowInTaskbar = true;

            // 最前面フラグを立てておく
            this.Topmost = true;

            // 画面サイズを初期化
            this.Width = LoginWindow.WindowWidth;
            this.Height = LoginWindow.WindowHeight;

            // 認証情報を初期化
            this.loginWindowWrapper.SetAuthenticationInformation(null);

            // デバイスモード判定を行う
            if (string.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("LETS_DEVICE_MODE")))
            {
                this.MainFrame.NavigationService.Navigate(new Login());
            }
            else
            {
                // デバイスモード
                this.MainFrame.NavigationService.Navigate(new DeviceModeApp());
            }

            base.ShowDialog();
        }

        /// <summary>
        /// 画面がアクティブになった時の処理
        /// </summary>
        protected override void OnActivated(EventArgs e)
        {
            // 最前面フラグを落とす
            this.Topmost = false;
            base.OnActivated(e);
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

            // 認証情報をクリア
            this.loginWindowWrapper.SetAuthenticationInformation(null);

            this.Topmost = false;
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
        }

        private void WActivated(EventArgs e)
        {
            if (!string.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("LETS_DEVICE_MODE")))
            {
                WindowHelper.SetWindowTitle(this, "LETSオフライン専用アプリ");
            }
        }
    }
}
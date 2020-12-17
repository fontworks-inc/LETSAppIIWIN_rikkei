using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Core.Interfaces;

namespace Client.UI.Components
{
    /// <summary>
    /// アプリケーションアイコンコンポーネントクラス
    /// </summary>
    public class ApplicationIconComponent : Component
    {
        /// <summary>
        /// タスクトレイアイコン
        /// </summary>
        private NotifyIcon tasktrayIcon;

        /// <summary>
        /// ローディングアイコン
        /// </summary>
        private List<Icon> loadingIcons;

        /// <summary>
        /// 現在表示中のローディングアイコンのインデックス
        /// </summary>
        private int currentLoadingIconIndex = 0;

        /// <summary>
        /// ローディングタイマー
        /// </summary>
        private Timer loadingTimer;

        /// <summary>
        /// ログインしているかどうか
        /// </summary>
        private bool isLogin = true;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="manager">ComponentManager</param>
        public ApplicationIconComponent(ComponentManager manager)
        {
            this.Manager = manager;
            this.Manager.Container.Add(this);
            this.Resource = this.Manager.GetResource();

            this.InitializeComponent();

            this.tasktrayIcon.Click += (s, e) =>
            {
                var mouseEvent = e as MouseEventArgs;

                if (!this.isLogin)
                {
                    this.Login();
                }
                else
                {
                    if (mouseEvent.Button == MouseButtons.Left)
                    {
                        MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                        mi.Invoke(this.tasktrayIcon, null);
                    }
                }
            };
        }

        /// <summary>
        /// ComponentManager
        /// </summary>
        protected ComponentManager Manager
        {
            get;
            private set;
        }

        /// <summary>
        /// IResourceWrapper
        /// </summary>
        protected IResourceWrapper Resource
        {
            get;
            private set;
        }

        /// <summary>
        /// ログイン画面を表示
        /// </summary>
        public void Login()
        {
            this.Manager.ShowLoginWindow();

            MessageBox.Show("ログイン完了");
            this.SetLoginMode();
        }

        /// <summary>
        /// 通常アイコンを表示
        /// </summary>
        public void SetNormalMode()
        {
            this.loadingTimer.Enabled = false;
            if (this.isLogin)
            {
                this.SetLoginMode();
            }
            else
            {
                this.SetLogoutMode();
            }
        }

        /// <summary>
        /// 通知アイコンを表示
        /// </summary>
        public void SetNoticeMode()
        {
            this.loadingTimer.Enabled = false;
            this.tasktrayIcon.Icon = this.Resource.GetIcon("ICON_NOTIFIED");
        }

        /// <summary>
        /// ローディングアイコンを表示
        /// </summary>
        public void SetLoadingMode()
        {
            this.currentLoadingIconIndex = 0;
            this.loadingTimer.Enabled = true;
        }

        /// <summary>
        /// 完了アイコンを表示
        /// </summary>
        public void SetCompleteMode()
        {
            this.loadingTimer.Enabled = false;
        }

        /// <summary>
        /// 選択中アイコンを表示
        /// </summary>
        public void SetSelectedMode()
        {
            // ローディングアイコン表示中は選択中アイコンは表示しない
            if (this.loadingTimer.Enabled)
            {
                return;
            }

            this.tasktrayIcon.Icon = this.Resource.GetIcon("ICON_SELECTED");
        }

        /// <summary>
        /// ログイン状態にする
        /// </summary>
        public void SetLoginMode()
        {
            this.loadingTimer.Enabled = false;

            this.isLogin = true;
            this.SetQuickMenu(this.Manager.QuickMenu);
            this.tasktrayIcon.Icon = this.Resource.GetIcon("ICON_LOGIN");
        }

        /// <summary>
        /// ログアウト状態にする
        /// </summary>
        public void SetLogoutMode()
        {
            this.loadingTimer.Enabled = false;

            this.isLogin = false;
            this.tasktrayIcon.ContextMenuStrip = null;
            this.tasktrayIcon.Icon = this.Resource.GetIcon("ICON_LOGOUT");
        }

        /// <summary>
        /// クイックメニューをセットする
        /// </summary>
        /// <param name="quickMenu">コンテキストメニュー</param>
        public void SetQuickMenu(QuickMenuComponent quickMenu)
        {
            this.tasktrayIcon.ContextMenuStrip = quickMenu.ContextMenu;
        }

        /// <summary>
        /// アプリケーションアイコン初期化処理
        /// </summary>
        protected void InitializeComponent()
        {
            this.tasktrayIcon = new NotifyIcon(this.Manager.Container);
            this.tasktrayIcon.Visible = true;
            this.tasktrayIcon.Text = "LETS"; // ツールチップに表示する文言は要確認（アプリ名、バージョン？）
            this.Manager.Container.Add(this.tasktrayIcon);

            // ローディングアイコンの登録
            this.loadingIcons = new List<Icon>();
            this.loadingIcons.Add(this.Resource.GetIcon("ICON_LOADING_0"));
            this.loadingIcons.Add(this.Resource.GetIcon("ICON_LOADING_1"));
            this.loadingIcons.Add(this.Resource.GetIcon("ICON_LOADING_2"));
            this.loadingIcons.Add(this.Resource.GetIcon("ICON_LOADING_3"));
            this.currentLoadingIconIndex = 0;

            this.loadingTimer = new Timer();
            this.loadingTimer.Enabled = false;
            this.loadingTimer.Tick += (s, e) =>
            {
                this.ShowLoadingIcons();
            };
        }

        /// <summary>
        /// ローディングアイコンを表示
        /// </summary>
        private void ShowLoadingIcons()
        {
            this.tasktrayIcon.Icon = this.loadingIcons[this.currentLoadingIconIndex];

            this.currentLoadingIconIndex++;
            if (this.currentLoadingIconIndex >= this.loadingIcons.Count)
            {
                this.currentLoadingIconIndex = 0;
            }
        }
    }
}

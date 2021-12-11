using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Core.Entities;
using Core.Interfaces;
using NLog;
using Prism.Ioc;
using Prism.Unity;

namespace Client.UI.Components
{
    /// <summary>
    /// アプリケーションアイコンコンポーネントクラス
    /// </summary>
    public class ApplicationIconComponent : Component
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>
        /// メモリで保持する情報を格納するリポジトリ
        /// </summary>
        private readonly IVolatileSettingRepository volatileSettingRepository;

        /// <summary>
        /// ユーザ別ステータス情報を格納するリポジトリ
        /// </summary>
        private readonly IUserStatusRepository userStatusRepository;

        /// <summary>
        /// タスクトレイアイコン
        /// </summary>
        private NotifyIcon tasktrayIcon;

        /// <summary>
        /// タスクトレイアイコンの有効無効状態
        /// </summary>
        private bool enabled = true;

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
        /// コンストラクタ
        /// </summary>
        /// <param name="manager">ComponentManager</param>
        public ApplicationIconComponent(ComponentManager manager)
        {
            this.Manager = manager;
            this.Manager.Container.Add(this);
            this.Resource = this.Manager.GetResource();

            IContainerProvider containerProvider = (System.Windows.Application.Current as PrismApplication).Container;
            this.volatileSettingRepository = containerProvider.Resolve<IVolatileSettingRepository>();
            this.userStatusRepository = containerProvider.Resolve<IUserStatusRepository>();

            this.InitializeComponent();

            this.tasktrayIcon.Click += (s, e) =>
            {
                var mouseEvent = e as MouseEventArgs;

                if (!this.userStatusRepository.GetStatus().IsLoggingIn)
                {
                    if (this.IsDeviceMode())
                    {
                        System.Environment.SetEnvironmentVariable("LETS_DEVICE_MODE", "TRUE");
                    }

                    // (ログアウト時)ログイン画面を起動する
                    this.Manager.ShowLoginWindow();
                }
                else
                {
                    // [メモリ：ダウンロード完了]をFALSEに設定する
                    this.volatileSettingRepository.GetVolatileSetting().CompletedDownload = false;

                    // [メモリ：アップデート完了]をFALSEに設定する
                    this.volatileSettingRepository.GetVolatileSetting().IsUpdated = false;

                    // (ログイン時)クイックメニューを起動する
                    Logger.Info(this.Manager.GetResource().GetString("LOG_INFO_ApplicationIconComponent_tasktrayIcon_Click"));
                    if (mouseEvent.Button == MouseButtons.Left)
                    {
                        // 左クリック時もクイックメニュー(タスクトレイアイコンに設定されたコンテキストメニュー)を表示
                        MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                        mi.Invoke(this.tasktrayIcon, null);
                    }
                }
            };
        }

        /// <summary>
        /// タスクトレイアイコンの有効無効状態
        /// </summary>
        public bool Enabled
        {
            get
            {
                return this.enabled;
            }

            set
            {
                this.enabled = value;
                if (this.enabled && this.userStatusRepository.GetStatus().IsLoggingIn)
                {
                    // タスクトレイアイコンが有効かつログイン中の場合
                    // タスクトレイアイコンにクイックメニューをセットする
                    this.SetQuickMenu();
                }
                else
                {
                    // タスクトレイアイコンからクイックメニューを外す
                    this.RemoveQuickMenu();
                }
            }
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
        /// アイコン表示ルールに従いアイコンを設定する
        /// </summary>
        /// <param name="selected">選択中かどうか（デフォルト：選択中でない）</param>
        public void SetIcon(bool selected = false)
        {
            // ローディングアイコンのタイマーをOFF
            this.loadingTimer.Enabled = false;

            // メモリで保持する情報を取得
            VolatileSetting volatileSetting = this.volatileSettingRepository.GetVolatileSetting();

            if (this.userStatusRepository.GetStatus().IsDeviceMode)
            {
                // 通常アイコン表示（デバイスモード）
                this.SetStartupMode();
                return;
            }

            // 選択アイコン表示（クイックメニューの表示状態変更時処理内で切替え）
            if (selected)
            {
                this.SetSelectedMode();
                return;
            }

            // ユーザー別に保持する情報を取得
            UserStatus userStatus = this.userStatusRepository.GetStatus();

            // ログアウト中アイコン表示（ログイン状態：ログアウト中）
            if (!userStatus.IsLoggingIn)
            {
                this.SetLogoutMode();
                return;
            }

            // 完了アイコン表示（ダウンロード完了時、または、アップデート完了時）
            if (volatileSetting.CompletedDownload || volatileSetting.IsUpdated)
            {
                this.SetCompleteMode();
                return;
            }

            // ローディングアイコン表示（ダウンロード中、または、アップデート中）
            if (volatileSetting.IsDownloading || volatileSetting.IsUpdating)
            {
                this.SetLoadingMode();
                return;
            }

            // 通知アイコン表示（通知あり）
            if (volatileSetting.IsNoticed || volatileSetting.IsUpdated)
            {
                this.SetNoticeMode();
                return;
            }

            // ログイン中アイコン表示（ログイン状態：ログイン中）
            if (userStatus.IsLoggingIn)
            {
                this.SetLoginMode();
                return;
            }

            // 通常アイコン表示（起動中）
            this.SetStartupMode();
        }

        /// <summary>
        /// タスクトレイアイコンにクイックメニューをセットする
        /// </summary>
        public void SetQuickMenu()
        {
            // (ログイン中の場合)タスクトレイアイコンが設定されていなかったら
            // タスクトレイアイコンにクイックメニューをセットする
            if (this.tasktrayIcon.ContextMenuStrip == null)
            {
                this.tasktrayIcon.ContextMenuStrip = this.Manager.QuickMenu.ContextMenu;
            }
        }

        /// <summary>
        /// タスクトレイアイコンからクイックメニューを外す
        /// </summary>
        public void RemoveQuickMenu()
        {
            this.tasktrayIcon.ContextMenuStrip = null;
        }

        /// <summary>
        /// アプリケーションアイコン初期化処理
        /// </summary>
        protected void InitializeComponent()
        {
            this.tasktrayIcon = new NotifyIcon(this.Manager.Container);
            this.tasktrayIcon.Visible = true;
            this.tasktrayIcon.Text = this.Resource.GetString("APP_TOOLTIP");
            this.Manager.Container.Add(this.tasktrayIcon);

            // ローディングアイコンの登録
            this.loadingIcons = new List<Icon>();
            this.loadingIcons.Add(this.Resource.GetIcon("ICON_LOADING_0"));
            this.loadingIcons.Add(this.Resource.GetIcon("ICON_LOADING_1"));
            this.loadingIcons.Add(this.Resource.GetIcon("ICON_LOADING_2"));
            this.loadingIcons.Add(this.Resource.GetIcon("ICON_LOADING_3"));
            this.currentLoadingIconIndex = 0;

            // ローディングアイコンのタイマーを設定
            this.loadingTimer = new Timer();
            this.loadingTimer.Enabled = false;
            this.loadingTimer.Tick += (s, e) =>
            {
                this.ShowLoadingIcons();
            };

            // 通常アイコンを設定
            this.SetStartupMode();
        }

        /// <summary>
        /// 通常アイコンを表示（起動中）
        /// </summary>
        private void SetStartupMode()
        {
            this.tasktrayIcon.Icon = this.Resource.GetIcon("ICON_APP");
        }

        /// <summary>
        /// ログイン中アイコンを表示
        /// </summary>
        private void SetLoginMode()
        {
            this.tasktrayIcon.Icon = this.Resource.GetIcon("ICON_LOGIN");
        }

        /// <summary>
        /// ログアウト中アイコンを表示
        /// </summary>
        private void SetLogoutMode()
        {
            this.tasktrayIcon.Icon = this.Resource.GetIcon("ICON_LOGOUT");
        }

        /// <summary>
        /// 選択中アイコンを表示
        /// </summary>
        private void SetSelectedMode()
        {
            this.tasktrayIcon.Icon = this.Resource.GetIcon("ICON_SELECTED");
        }

        /// <summary>
        /// 通知アイコンを表示
        /// </summary>
        private void SetNoticeMode()
        {
            this.tasktrayIcon.Icon = this.Resource.GetIcon("ICON_NOTIFIED");
        }

        /// <summary>
        /// 完了アイコンを表示
        /// </summary>
        private void SetCompleteMode()
        {
            this.tasktrayIcon.Icon = this.Resource.GetIcon("ICON_COMPLETED");
        }

        /// <summary>
        /// ローディングアイコンを表示
        /// </summary>
        private void SetLoadingMode()
        {
            // ローディングアイコンのタイマーをON
            this.currentLoadingIconIndex = 0;
            this.loadingTimer.Enabled = true;
        }

        /// <summary>
        /// ローディングアイコン表示処理
        /// </summary>
        /// <remarks>ローディングアイコンのタイマーがONのとき呼び出される</remarks>
        private void ShowLoadingIcons()
        {
            this.tasktrayIcon.Icon = this.loadingIcons[this.currentLoadingIconIndex];

            this.currentLoadingIconIndex++;
            if (this.currentLoadingIconIndex >= this.loadingIcons.Count)
            {
                this.currentLoadingIconIndex = 0;
            }
        }

        /// <summary>
        /// デバイスモード判定
        /// </summary>
        private bool IsDeviceMode()
        {
            if (!string.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("LETS_DEVICE_MODE")))
            {
                return true;
            }

            // デバイスモード判定を行う
            IContainerProvider container = (System.Windows.Application.Current as PrismApplication).Container;
            var userStatusRepository = container.Resolve<IUserStatusRepository>();

            bool isDeviceMode = false;

            if (userStatusRepository.GetStatus().IsDeviceMode)
            {
                isDeviceMode = true;
            }
            else
            {
                var deviceModeSettingRepository = container.Resolve<IDeviceModeSettingRepository>();
                if (!string.IsNullOrEmpty(deviceModeSettingRepository.GetDeviceModeSetting().OfflineDeviceID))
                {
                    isDeviceMode = true;
                }
            }

            return isDeviceMode;
        }
    }
}

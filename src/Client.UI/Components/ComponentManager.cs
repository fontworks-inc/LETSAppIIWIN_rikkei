using System;
using System.ComponentModel;
using System.Windows.Forms;
using ApplicationService.Authentication;
using Client.UI.Components.MenuItem;
using Client.UI.Interfaces;
using Client.UI.Views;
using Core.Interfaces;
using OS.Interfaces;
using Prism.Ioc;
using Prism.Unity;

namespace Client.UI.Components
{
    /// <summary>
    /// アプリケーションのコンポーネントを管理するクラス
    /// </summary>
    public partial class ComponentManager : Form
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ComponentManager()
            : this(new Container())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="container">IContainer</param>
        public ComponentManager(IContainer container)
        {
            this.components = container;
            this.components.Add(this);

            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.Opacity = 0;

            this.InitializeComponent();
        }

        /// <summary>
        /// アプリケーションアイコン
        /// </summary>
        public ApplicationIconComponent ApplicationIcon
        {
            get;
            private set;
        }

        /// <summary>
        /// クイックメニュー
        /// </summary>
        public QuickMenuComponent QuickMenu
        {
            get;
            private set;
        }

        /// <summary>
        /// クイックメニュー－ログイン中(状態表示)
        /// </summary>
        public MenuItemLoginStatus MenuLoginStatus
        {
            get;
            private set;
        }

        /// <summary>
        /// クイックメニュー－アップデート中(状態表示)
        /// </summary>
        public MenuItemUpdateStatus MenuUpdateStatus
        {
            get;
            private set;
        }

        /// <summary>
        /// クイックメニュー－ダウンロード中(状態表示)
        /// </summary>
        public MenuItemDownloadStatus MenuDownloadStatus
        {
            get;
            private set;
        }

        /// <summary>
        /// クイックメニュー－お知らせ
        /// </summary>
        public MenuItemAnnouncePage MenuAnnouncePage
        {
            get;
            private set;
        }

        /// <summary>
        /// クイックメニュー－アカウント
        /// </summary>
        public MenuItemAccountPage MenuAccountPage
        {
            get;
            private set;
        }

        /// <summary>
        /// クイックメニュー－フォント
        /// </summary>
        public MenuItemFontListPage MenuFontListPage
        {
            get;
            private set;
        }

        /// <summary>
        /// クイックメニュー－ログアウト
        /// </summary>
        public MenuItemLogout MenuLogout
        {
            get;
            private set;
        }

        /// <summary>
        /// クイックメニュー－アップデート
        /// </summary>
        public MenuItemUpdate MenuUpdate
        {
            get;
            private set;
        }

        /// <summary>
        /// アプリケーションのコンポーネントをすべて破棄する
        /// </summary>
        public new void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// IResourceWrapper を返す
        /// </summary>
        /// <returns>IResourceWrapper</returns>
        public IResourceWrapper GetResource()
        {
            IContainerProvider container = (System.Windows.Application.Current as PrismApplication).Container;
            var resouceWrapper = container.Resolve<IResourceWrapper>();
            return resouceWrapper;
        }

        /// <summary>
        /// ログイン画面を起動する
        /// </summary>
        public void ShowLoginWindow()
        {
            // コンテナに登録したオブジェクトに設定
            IContainerProvider container = (System.Windows.Application.Current as PrismApplication).Container;
            var loginWindowWrapper = container.Resolve<ILoginWindowWrapper>();

            // ログイン画面を取得
            var loginWindow = loginWindowWrapper.Window;
            if (loginWindow == null)
            {
                loginWindow = new LoginWindow();
            }

            if (loginWindow.Visibility != System.Windows.Visibility.Visible)
            {
                this.TopMost = true;
                loginWindow.ShowDialog();
            }
        }

        /// <summary>
        /// アプリケーションを終了する
        /// </summary>
        /// <param name="deactivateFonts">フォントをディアクティベートするかどうか</param>
        public void Exit(bool deactivateFonts)
        {
            if (deactivateFonts)
            {
                // ログアウト処理を実行する
                this.Logout();
            }

            // アプリケーションを終了
            System.Windows.Application.Current.Shutdown();
        }

        /// <summary>
        /// 強制アップデート
        /// </summary>
        public void ForcedUpdate()
        {
            var forcedUpdateDialog = new ForceUpdateNotification();
            forcedUpdateDialog.ShowDialog();

            this.StartUpdate();
        }

        /// <summary>
        /// アップデート開始
        /// </summary>
        public void StartUpdate()
        {
            this.MenuLoginStatus.Hide();
            this.MenuUpdate.Hide();

            this.MenuUpdateStatus.Show();

            this.ApplicationIcon.SetLoadingMode();
        }

        /// <summary>
        /// アップデート完了
        /// </summary>
        public void UpdateCompleted()
        {
            this.ApplicationIcon.SetCompleteMode();
        }

        /// <summary>
        /// 通知あり
        /// </summary>
        public void Notice()
        {
            this.ApplicationIcon.SetNoticeMode();
        }

        /// <summary>
        /// ダウンロード開始
        /// </summary>
        public void StartDownload()
        {
            this.MenuLoginStatus.Hide();
            this.MenuUpdateStatus.Hide();

            this.MenuDownloadStatus.Show();

            this.ApplicationIcon.SetLoadingMode();
        }

        /// <summary>
        /// ダウンロード完了
        /// </summary>
        public void DownloadCompleted()
        {
            this.ApplicationIcon.SetCompleteMode();
        }

        /// <summary>
        /// ログアウト
        /// </summary>
        public void Logout()
        {
            // コンテナに登録したオブジェクトに設定
            IContainerProvider container = (System.Windows.Application.Current as PrismApplication).Container;

            // 設定ファイルを読み込む
            var volatileSettingRepository = container.Resolve<IVolatileSettingRepository>();
            var userStatusRepository = container.Resolve<IUserStatusRepository>();
            var receiveNotificationRepository = container.Resolve<IReceiveNotificationRepository>();
            var authenticationInformationRepository = container.Resolve<IAuthenticationInformationRepository>();
            var userFontsSettingRepository = container.Resolve<IUserFontsSettingRepository>();
            var fontActivationService = container.Resolve<IFontActivationService>();

            // 認証のサービス
            var service = new AuthenticationService(
                this.GetResource(),
                authenticationInformationRepository,
                userStatusRepository,
                receiveNotificationRepository,
                volatileSettingRepository,
                userFontsSettingRepository,
                fontActivationService);

            // ログアウト処理実行
            if (service.Logout())
            {
                // クイックメニューをログアウト状態に設定
                this.ApplicationIcon.SetLogoutMode();
            }
        }

        /// <summary>
        /// 強制ログアウト
        /// </summary>
        public void ForcedLogout()
        {
            var forcedLogoutDialog = new ForceLogoutNotification();
            forcedLogoutDialog.ShowDialog();

            this.Logout();
        }

        /// <summary>
        /// ウィンドウメッセージを処理する
        /// </summary>
        /// <param name="m">ウィンドウメッセージ</param>
        protected override void WndProc(ref Message m)
        {
            // ユーザー定義メッセージを処理する
            try
            {
                WindowMessageType messageType = (WindowMessageType)m.Msg;
                switch (messageType)
                {
                    case WindowMessageType.General:
                        LParamType paramType = (LParamType)m.LParam;
                        switch (paramType)
                        {
                            case LParamType.LoadLoginWindow:
                                // ログインメッセージ
                                this.ShowLoginWindow();
                                break;

                            case LParamType.Shutdown:
                                // 終了メッセージ(ディアクティベートなし)
                                this.Exit(false);
                                break;

                            case LParamType.DeactivateFontsAndShutdown:
                                // 終了メッセージ(ディアクティベートあり)
                                this.Exit(true);
                                break;

                            default:
                                // 親のウィンドウメッセージ処理を返す　※ここを通ることはあり得ない
                                base.WndProc(ref m);
                                break;
                        }

                        break;

                    case WindowMessageType.ProgressOfUpdate:
                        // プログラムアップデート進捗メッセージ
                        int progress = (int)m.LParam;
                        this.MenuUpdateStatus.SetProgressStatus(progress);
                        break;

                    default:
                        // 親のウィンドウメッセージ処理を返す　※ここを通ることはあり得ない
                        base.WndProc(ref m);
                        break;
                }
            }
            catch (Exception)
            {
                // 親のウィンドウメッセージ処理を返す
                base.WndProc(ref m);
            }
        }

        /// <summary>
        /// コンポーネント初期化処理
        /// </summary>
        private void InitializeComponent()
        {
            // アプリケーションタイトル　※EXE名(バージョン番号も含まれる)を利用？【要確認】
            // this.Text = Assembly.GetExecutingAssembly().GetName().Name;
            this.Text = "LETS";
            this.SuspendLayout();

            // クイックメニューの生成
            this.ApplicationIcon = new ApplicationIconComponent(this);
            this.QuickMenu = new QuickMenuComponent(this);
            this.QuickMenu.SuspendLayout();

            IContainerProvider container = (System.Windows.Application.Current as PrismApplication).Container;
            var resouceWrapper = container.Resolve<IResourceWrapper>();

            // 設定ファイルを読み込む
            var volatileSettingRepository = container.Resolve<IVolatileSettingRepository>();
            var userStatusRepository = container.Resolve<IUserStatusRepository>();
            var urlRepository = container.Resolve<IUrlRepository>();

            this.MenuLoginStatus = new MenuItemLoginStatus(this);
            this.MenuUpdateStatus = new MenuItemUpdateStatus(this);
            this.MenuDownloadStatus = new MenuItemDownloadStatus(this);
            this.MenuAnnouncePage = new MenuItemAnnouncePage(this, volatileSettingRepository, userStatusRepository, urlRepository);
            this.MenuAccountPage = new MenuItemAccountPage(this);
            this.MenuFontListPage = new MenuItemFontListPage(this);
            this.MenuLogout = new MenuItemLogout(this);
            this.MenuUpdate = new MenuItemUpdate(this);

            // クイックメニューにメニュー追加
            this.MenuLoginStatus.SetMenu(this.QuickMenu);
            this.MenuUpdateStatus.SetMenu(this.QuickMenu);
            this.MenuDownloadStatus.SetMenu(this.QuickMenu);
            this.MenuAnnouncePage.SetMenu(this.QuickMenu);
            this.MenuAccountPage.SetMenu(this.QuickMenu);
            this.MenuFontListPage.SetMenu(this.QuickMenu);
            this.MenuLogout.SetMenu(this.QuickMenu);
            this.MenuUpdate.SetMenu(this.QuickMenu);

            // アプリケーションアイコンにクイックメニューを追加
            this.ApplicationIcon.SetQuickMenu(this.QuickMenu);

            this.QuickMenu.ResumeLayout();
            this.QuickMenu.PerformLayout();

            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}

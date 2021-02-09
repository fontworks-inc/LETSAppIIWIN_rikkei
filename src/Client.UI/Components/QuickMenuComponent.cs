using System.ComponentModel;
using System.Windows.Forms;
using Client.UI.Components.MenuItem;
using Core.Interfaces;
using NLog;
using Prism.Ioc;
using Prism.Unity;

namespace Client.UI.Components
{
    /// <summary>
    /// クイックメニュークラス
    /// </summary>
    public class QuickMenuComponent : Component
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>
        /// 文言の取得を行うインスタンス
        /// </summary>
        private readonly IResourceWrapper resourceWrapper = null;

        /// <summary>
        /// メモリで保持する情報を格納するリポジトリ
        /// </summary>
        private readonly IVolatileSettingRepository volatileSettingRepository;

        /// <summary>
        /// ユーザ別ステータス情報を格納するリポジトリ
        /// </summary>
        private readonly IUserStatusRepository userStatusRepository;

        /// <summary>
        /// URLアドレスを格納するリポジトリ
        /// </summary>
        private readonly IUrlRepository urlRepository;

        /// <summary>
        /// お客様情報を扱うリポジトリ
        /// </summary>
        private readonly ICustomerRepository customerRepository;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="manager">ComponentManager</param>
        public QuickMenuComponent(ComponentManager manager)
        {
            this.Manager = manager;
            this.Manager.Container.Add(this);

            IContainerProvider containerProvider = (System.Windows.Application.Current as PrismApplication).Container;

            // 設定ファイルを読み込む
            this.resourceWrapper = containerProvider.Resolve<IResourceWrapper>();
            this.volatileSettingRepository = containerProvider.Resolve<IVolatileSettingRepository>();
            this.userStatusRepository = containerProvider.Resolve<IUserStatusRepository>();

            // コンテナに登録されている情報を取得
            this.urlRepository = containerProvider.Resolve<IUrlRepository>();
            this.customerRepository = containerProvider.Resolve<ICustomerRepository>();

            this.InitializeComponent();

            // クイックメニュー表示変更時イベント
            this.ContextMenu.VisibleChanged += (s, e) =>
            {
                // アイコン表示ルールに従いアイコンを設定
                this.Manager.SetIcon(this.ContextMenu.Visible);
            };

            // クイックメニューを閉じた時のイベント
            this.ContextMenu.Closed += (s, e) =>
            {
                this.ShowLoginStatus();
            };
        }

        /// <summary>
        /// ComponentManager
        /// </summary>
        public ComponentManager Manager
        {
            get;
            private set;
        }

        /// <summary>
        /// ContextMenuStrip
        /// </summary>
        public ContextMenuStrip ContextMenu
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
        /// 状態表示：ログイン中を表示する
        /// </summary>
        public void ShowLoginStatus()
        {
            Logger.Info(this.Manager.GetResource().GetString("LOG_INFO_QuickMenuComponent_ShowLoginStatus"));

            this.MenuUpdateStatus.Hide();
            this.MenuDownloadStatus.Hide();
            this.MenuLoginStatus.Show();

            this.MenuLoginStatus.SetLoginStatus();
        }

        /// <summary>
        /// 状態表示：ダウンロード中を表示する
        /// </summary>
        public void ShowDownloadStatus()
        {
            Logger.Info(this.Manager.GetResource().GetString("LOG_INFO_QuickMenuComponent_ShowDownloadStatus"));

            this.MenuLoginStatus.Hide();
            this.MenuUpdateStatus.Hide();

            this.MenuDownloadStatus.Show();
        }

        /// <summary>
        /// 状態表示：アップデート中を表示する
        /// </summary>
        public void ShowUpdateStatus()
        {
            Logger.Info(this.Manager.GetResource().GetString("LOG_INFO_QuickMenuComponent_ShowUpdateStatus"));

            this.MenuLoginStatus.Hide();
            this.MenuDownloadStatus.Hide();
            this.MenuUpdate.Hide();

            this.MenuUpdateStatus.Show();
        }

        /// <summary>
        /// SuspendLayoutメソッドを呼び出す
        /// </summary>
        public void SuspendLayout()
        {
            this.ContextMenu.SuspendLayout();
        }

        /// <summary>
        /// ResumeLayoutメソッドを呼び出す
        /// </summary>
        public void ResumeLayout()
        {
            this.ContextMenu.ResumeLayout(false);
        }

        /// <summary>
        /// PerformLayoutメソッドを呼び出す
        /// </summary>
        public void PerformLayout()
        {
            this.ContextMenu.PerformLayout();
        }

        /// <summary>
        /// クイックメニュー初期化処理
        /// </summary>
        protected void InitializeComponent()
        {
            this.ContextMenu = new ContextMenuStrip(this.Manager.Container);
            this.ContextMenu.AutoSize = true;

            // レイアウトを停止
            this.SuspendLayout();

            // クイックメニューにメニューアイテムを追加
            this.MenuLoginStatus = new MenuItemLoginStatus(this, this.userStatusRepository, this.customerRepository);
            this.MenuUpdateStatus = new MenuItemUpdateStatus(this);
            this.MenuDownloadStatus = new MenuItemDownloadStatus(this);
            this.MenuAnnouncePage = new MenuItemAnnouncePage(this, this.volatileSettingRepository, this.userStatusRepository, this.urlRepository);
            this.MenuAccountPage = new MenuItemAccountPage(this, this.resourceWrapper, this.volatileSettingRepository, this.userStatusRepository, this.urlRepository);
            this.MenuFontListPage = new MenuItemFontListPage(this, this.resourceWrapper, this.volatileSettingRepository, this.userStatusRepository, this.urlRepository);
            this.MenuLogout = new MenuItemLogout(this);
            this.MenuUpdate = new MenuItemUpdate(this, this.resourceWrapper);

            // クイックメニューアイテムの初期表示状態を設定
            this.MenuAnnouncePage.Show();
            this.MenuAccountPage.Show();
            this.MenuFontListPage.Show();
            this.MenuLogout.Show();
            this.MenuUpdate.Hide();

            // レイアウトを再開
            this.ResumeLayout();
            this.PerformLayout();
        }
    }
}

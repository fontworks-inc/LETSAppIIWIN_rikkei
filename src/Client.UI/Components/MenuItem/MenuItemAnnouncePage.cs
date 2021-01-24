using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Core.Interfaces;
using NLog;

namespace Client.UI.Components.MenuItem
{
    /// <summary>
    /// クイックメニュー－アカウント
    /// </summary>
    public class MenuItemAnnouncePage : MenuItemBase
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
        /// URLアドレスを格納するリポジトリ
        /// </summary>
        private readonly IUrlRepository urlRepository;

        /// <summary>お知らせ</summary>
        private ToolStripMenuItem announce;

        /// <summary>セパレータ</summary>
        private ToolStripSeparator separator;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="quickMenu">QuickMenuComponent</param>
        /// <param name="volatileSettingRepository">メモリで保持する情報を格納するリポジトリ</param>
        /// <param name="userStatusRepository">ユーザ別ステータス情報を格納するリポジトリ</param>
        /// <param name="urlRepository">URLアドレスを格納するリポジトリ</param>
        public MenuItemAnnouncePage(
            QuickMenuComponent quickMenu,
            IVolatileSettingRepository volatileSettingRepository,
            IUserStatusRepository userStatusRepository,
            IUrlRepository urlRepository)
             : base(quickMenu)
        {
            this.announce.Click += (s, e) =>
            {
                this.OnAnnounceMenuItemClick();
            };
            this.volatileSettingRepository = volatileSettingRepository;
            this.userStatusRepository = userStatusRepository;
            this.urlRepository = urlRepository;
        }

        /// <summary>お知らせの未読件数を表示する</summary>
        /// <param name="numberOfUnreadMessages">未読件数</param>
        /// <remarks>SetNumberOfUnreadMessages を代入</remarks>
        public delegate void NumberOfUnreadMessages(int numberOfUnreadMessages);

        /// <summary>
        /// クイックメニューアイテムのリストを返す
        /// </summary>
        public override List<ToolStripItem> Items
        {
            get
            {
                return new List<ToolStripItem>()
                {
                    this.separator,
                    this.announce,
                };
            }
        }

        /// <summary>
        /// お知らせメニューに未読件数を表示する
        /// </summary>
        /// <param name="numberOfUnreadMessages">未読件数</param>
        public void SetNumberOfUnreadMessages(int numberOfUnreadMessages)
        {
            string number = (numberOfUnreadMessages > 0) ?
                string.Format(this.Resource.GetString("MENU_ANNOUNCE_COUNTER"), numberOfUnreadMessages)
                : string.Empty;

            this.announce.Text = $"{this.Resource.GetString("MENU_ANNOUNCE")}{number}";
        }

        /// <summary>
        /// クイックメニュー初期化処理
        /// </summary>
        protected override void InitializeComponent()
        {
            this.separator = this.CreateSeparator();
            this.announce = this.Create("MENU_ANNOUNCE", this.Resource.GetString("MENU_ANNOUNCE"));

            this.SetMenu();
        }

        /// <summary>
        /// クイックメニューにアイテムを追加する
        /// </summary>
        protected override void SetMenu()
        {
            this.QuickMenu.ContextMenu.Items.Add(this.separator);
            this.QuickMenu.ContextMenu.Items.Add(this.announce);
        }

        /// <summary>
        /// お知らせメニュークリック時処理
        /// </summary>
        private void OnAnnounceMenuItemClick()
        {
            Logger.Info(this.QuickMenu.Manager.GetResource().GetString("LOG_INFO_MenuItemAnnouncePage_OnAnnounceMenuItemClick"));

            // メモリ保存の項目を取得
            var volatileSetting = this.volatileSettingRepository.GetVolatileSetting();

            // デバイスIDを取得
            var deviceId = this.userStatusRepository.GetStatus().DeviceId;

            // URLの取得
            Core.Entities.Url url = this.urlRepository.GetAnnouncePageUrl(deviceId, volatileSetting.AccessToken);

            // 通知なし volatileSettingにある設定値の変更
            volatileSetting.IsNoticed = false;

            // アイコン表示ルールに従いアイコンを設定
            this.QuickMenu.Manager.SetIcon();

            // ブラウザ起動：お知らせ画面を表示する
            var browser = new Client.UI.Entities.WebBrowser();
            browser.Navigate(url);
        }
    }
}

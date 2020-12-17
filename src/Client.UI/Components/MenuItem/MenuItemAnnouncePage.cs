using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Core.Interfaces;

namespace Client.UI.Components.MenuItem
{
    /// <summary>
    /// クイックメニュー－アカウント
    /// </summary>
    public class MenuItemAnnouncePage : MenuItemBase
    {
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
        /// <param name="manager">ComponentManager</param>
        /// <param name="volatileSettingRepository">メモリで保持する情報を格納するリポジトリ</param>
        /// <param name="userStatusRepository">ユーザ別ステータス情報を格納するリポジトリ</param>
        /// <param name="urlRepository">URLアドレスを格納するリポジトリ</param>
        public MenuItemAnnouncePage(
            ComponentManager manager,
            IVolatileSettingRepository volatileSettingRepository,
            IUserStatusRepository userStatusRepository,
            IUrlRepository urlRepository)
             : base(manager)
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
        /// クイックメニューにアイテムを追加する
        /// </summary>
        /// <param name="quickMenu">クイックメニュー</param>
        public override void SetMenu(QuickMenuComponent quickMenu)
        {
            quickMenu.ContextMenu.Items.Add(this.separator);
            quickMenu.ContextMenu.Items.Add(this.announce);
        }

        /// <summary>
        /// クイックメニュー初期化処理
        /// </summary>
        protected override void InitializeComponent()
        {
            this.separator = this.CreateSeparator();
            this.announce = this.Create("MENU_ANNOUNCE", this.Resource.GetString("MENU_ANNOUNCE"));
        }

        /// <summary>
        /// お知らせメニュークリック時処理
        /// </summary>
        private void OnAnnounceMenuItemClick()
        {
            // メモリ保存の項目を取得
            var volatileSetting = this.volatileSettingRepository.GetVolatileSetting();

            // デバイスIDを取得
            var deviceId = this.userStatusRepository.GetStatus().DeviceId;

            // URLの取得
            var url = this.urlRepository.GetAnnouncePageUrl(deviceId, volatileSetting.AccessToken).ToString();

            // 通知なし volatileSettingにある設定値の変更
            volatileSetting.IsNoticed = false;

            // アイコンの更新 「2.1.2 アイコン表示ルール」のメソッド呼び出し　TODO　メソッド実装後に記載
            // this.Manager.ApplicationIcon.SetIcon();

            // ブラウザ起動：お知らせ画面を表示する
            // NET Frameworkでは、Process.Start(url);で既定のブラウザが開いたが.NET CoreではNGになったよう
            // Windowsの場合　& をエスケープ（シェルがコマンドの切れ目と認識するのを防ぐ）
            url = url.Replace("&", "^&");
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }
    }
}

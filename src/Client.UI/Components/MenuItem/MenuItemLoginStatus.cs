using System.Collections.Generic;
using System.Windows.Forms;
using Core.Interfaces;

namespace Client.UI.Components.MenuItem
{
    /// <summary>
    /// クイックメニュー－ログイン中(状態表示)
    /// </summary>
    public class MenuItemLoginStatus : MenuItemBase
    {
        /// <summary>
        /// ユーザ別ステータス情報を格納するリポジトリ
        /// </summary>
        private readonly IUserStatusRepository userStatusRepository;

        /// <summary>
        /// お客様情報を扱うリポジトリ
        /// </summary>
        private readonly ICustomerRepository customerRepository;

        /// <summary>利用者氏名</summary>
        private ToolStripLabel userName;

        /// <summary>メールアドレス</summary>
        private ToolStripLabel mailAddress;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="quickMenu">QuickMenuComponent</param>
        /// <param name="userStatusRepository">ユーザ別ステータス情報を格納するリポジトリ</param>
        /// <param name="customerRepository">お客様情報を扱うリポジトリ</param>
        public MenuItemLoginStatus(
            QuickMenuComponent quickMenu,
            IUserStatusRepository userStatusRepository,
            ICustomerRepository customerRepository)
             : base(quickMenu)
        {
            this.userStatusRepository = userStatusRepository;
            this.customerRepository = customerRepository;
        }

        /// <summary>
        /// クイックメニューアイテムのリストを返す
        /// </summary>
        public override List<ToolStripItem> Items
        {
            get
            {
                return new List<ToolStripItem>()
                {
                    this.userName,
                    this.mailAddress,
                };
            }
        }

        /// <summary>
        /// ログイン中
        /// </summary>
        public void SetLoginStatus()
        {
            // お客様情報を取得
            var userStatus = this.userStatusRepository.GetStatus();
            var deviceId = userStatus.DeviceId;
            var customer = this.customerRepository.GetCustomer(deviceId);

            // ※将来的な言語対応を考慮すると、利用言語から表示氏名を生成する仕組みが必要
            // 現在は「苗字名前」とする。
            this.userName.Text = $"{customer.LastName}{customer.FirstName}";

            this.mailAddress.Text = customer.MailAddress;
        }

        /// <summary>
        /// クイックメニュー初期化処理
        /// </summary>
        protected override void InitializeComponent()
        {
            this.userName = this.CreateLabel(this.Resource.GetString("MENU_LOGIN_USERNAME"));
            this.mailAddress = this.CreateLabel(this.Resource.GetString("MENU_LOGIN_MAILADDRESS"));

            this.SetMenu();
        }

        /// <summary>
        /// クイックメニューにアイテムを追加する
        /// </summary>
        protected override void SetMenu()
        {
            this.QuickMenu.ContextMenu.Items.Add(this.userName);
            this.QuickMenu.ContextMenu.Items.Add(this.mailAddress);
        }
    }
}

using System.Collections.Generic;
using System.Windows.Forms;
using Core.Entities;

namespace Client.UI.Components.MenuItem
{
    /// <summary>
    /// クイックメニュー－ログイン中(状態表示)
    /// </summary>
    public class MenuItemLoginStatus : MenuItemBase
    {
        /// <summary>メールアドレス</summary>
        private ToolStripLabel mailAddress;

        /// <summary>利用者氏名</summary>
        private ToolStripLabel userName;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="manager">ComponentManager</param>
        public MenuItemLoginStatus(ComponentManager manager)
             : base(manager)
        {
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
                    this.mailAddress,
                    this.userName,
                };
            }
        }

        /// <summary>
        /// クイックメニューにアイテムを追加する
        /// </summary>
        /// <param name="quickMenu">クイックメニュー</param>
        public override void SetMenu(QuickMenuComponent quickMenu)
        {
            quickMenu.ContextMenu.Items.Add(this.userName);
            quickMenu.ContextMenu.Items.Add(this.mailAddress);
        }

        /// <summary>
        /// ログイン中
        /// </summary>
        /// <param name="customer">お客様情報</param>
        public void SetLoginStatus(Customer customer)
        {
            this.mailAddress.Text = customer.MailAddress;

            // ※将来的な言語対応を考慮すると、利用言語から表示氏名を生成する仕組みが必要
            // 現在は「苗字名前」とする。
            this.userName.Text = $"{customer.LastName}{customer.FirstName}";
        }

        /// <summary>
        /// クイックメニュー初期化処理
        /// </summary>
        protected override void InitializeComponent()
        {
            this.mailAddress = this.CreateLabel(this.Resource.GetString("MENU_LOGIN_MAILADDRESS"));
            this.userName = this.CreateLabel(this.Resource.GetString("MENU_LOGIN_USERNAME"));
        }
    }
}

using System.Collections.Generic;
using System.Windows.Forms;

namespace Client.UI.Components.MenuItem
{
    /// <summary>
    /// クイックメニュー－アカウント
    /// </summary>
    public class MenuItemAccountPage : MenuItemBase
    {
        /// <summary>アカウント</summary>
        private ToolStripMenuItem account;

        /// <summary>セパレータ</summary>
        private ToolStripSeparator separator;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="manager">ComponentManager</param>
        public MenuItemAccountPage(ComponentManager manager)
             : base(manager)
        {
            this.account.Click += (s, e) =>
            {
                this.OnAccountMenuItemClick();
            };
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
                    this.separator,
                    this.account,
                };
            }
        }

        /// <summary>
        /// クイックメニューにアイテムを追加する
        /// </summary>
        /// <param name="quickMenu">クイックメニュー</param>
        public override void SetMenu(QuickMenuComponent quickMenu)
        {
            quickMenu.ContextMenu.Items.Add(this.separator);
            quickMenu.ContextMenu.Items.Add(this.account);
        }

        /// <summary>
        /// クイックメニュー初期化処理
        /// </summary>
        protected override void InitializeComponent()
        {
            this.separator = this.CreateSeparator();
            this.account = this.Create("MENU_ACCOUNT", this.Resource.GetString("MENU_ACCOUNT"));
        }

        /// <summary>
        /// アカウントメニュークリック時処理
        /// </summary>
        private void OnAccountMenuItemClick()
        {
            // ユーザーのホーム画面を表示する
        }
    }
}

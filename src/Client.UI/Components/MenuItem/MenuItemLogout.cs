using System.Collections.Generic;
using System.Windows.Forms;
using Client.UI.Views;

namespace Client.UI.Components.MenuItem
{
    /// <summary>
    /// クイックメニュー－ログアウト
    /// </summary>
    public class MenuItemLogout : MenuItemBase
    {
        /// <summary>ログアウト</summary>
        private ToolStripMenuItem logout;

        /// <summary>セパレータ</summary>
        private ToolStripSeparator separator;

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="manager">ComponentManager</param>
        public MenuItemLogout(ComponentManager manager)
            : base(manager)
        {
            this.logout.Click += (s, e) =>
            {
                this.OnLogoutMenuItemClick();
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
                    this.logout,
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
            quickMenu.ContextMenu.Items.Add(this.logout);
        }

        /// <summary>
        /// クイックメニュー初期化処理
        /// </summary>
        protected override void InitializeComponent()
        {
            this.separator = this.CreateSeparator();
            this.logout = this.Create("MENU_LOGOUT", this.Resource.GetString("MENU_LOGOUT"));
        }

        /// <summary>
        /// ログアウトメニュークリック時処理
        /// </summary>
        private void OnLogoutMenuItemClick()
        {
            var confirm = new LogoutConfirmation();

            // ログアウト確認ダイアログ(APP_09_01)を表示
            if (confirm.ShowDialog() == true)
            {
                // ログアウト処理実行
                this.Manager.Logout();
            }
        }
    }
}

using System.Collections.Generic;
using System.Windows.Forms;
using Client.UI.Views;
using NLog;

namespace Client.UI.Components.MenuItem
{
    /// <summary>
    /// クイックメニュー－ログアウト
    /// </summary>
    public class MenuItemLogout : MenuItemBase
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>ログアウト</summary>
        private ToolStripMenuItem logout;

        /// <summary>セパレータ</summary>
        private ToolStripSeparator separator;

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="quickMenu">QuickMenuComponent</param>
        public MenuItemLogout(QuickMenuComponent quickMenu)
            : base(quickMenu)
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
        /// クイックメニュー初期化処理
        /// </summary>
        protected override void InitializeComponent()
        {
            this.separator = this.CreateSeparator();
            this.logout = this.Create("MENU_LOGOUT", this.Resource.GetString("MENU_LOGOUT"));

            this.SetMenu();
        }

        /// <summary>
        /// クイックメニューにアイテムを追加する
        /// </summary>
        protected override void SetMenu()
        {
            this.QuickMenu.ContextMenu.Items.Add(this.separator);
            this.QuickMenu.ContextMenu.Items.Add(this.logout);
        }

        /// <summary>
        /// ログアウトメニュークリック時処理
        /// </summary>
        private void OnLogoutMenuItemClick()
        {
            Logger.Info(this.QuickMenu.Manager.GetResource().GetString("LOG_INFO_MenuItemLogout_OnLogoutMenuItemClick"));

            var confirm = new LogoutConfirmation();

            // タスクトレイアイコンを操作不可とする
            this.QuickMenu.Manager.ApplicationIcon.Enabled = false;

            // ログアウト確認ダイアログ(APP_09_01)を表示
            if (confirm.ShowDialog() == true)
            {
                // ログアウト処理実行
                this.QuickMenu.Manager.Logout();
            }

            // タスクトレイアイコンを操作可能とする
            this.QuickMenu.Manager.ApplicationIcon.Enabled = true;
        }
    }
}

using System.Collections.Generic;
using System.Windows.Forms;
using NLog;

namespace Client.UI.Components.MenuItem
{
    /// <summary>
    /// クイックメニュー－アカウント
    /// </summary>
    public class MenuItemAccountPage : MenuItemBase
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>アカウント</summary>
        private ToolStripMenuItem account;

        /// <summary>セパレータ</summary>
        private ToolStripSeparator separator;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="quickMenu">QuickMenuComponent</param>
        public MenuItemAccountPage(QuickMenuComponent quickMenu)
             : base(quickMenu)
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
        /// クイックメニュー初期化処理
        /// </summary>
        protected override void InitializeComponent()
        {
            this.separator = this.CreateSeparator();
            this.account = this.Create("MENU_ACCOUNT", this.Resource.GetString("MENU_ACCOUNT"));

            this.SetMenu();
        }

        /// <summary>
        /// クイックメニューにアイテムを追加する
        /// </summary>
        protected override void SetMenu()
        {
            this.QuickMenu.ContextMenu.Items.Add(this.separator);
            this.QuickMenu.ContextMenu.Items.Add(this.account);
        }

        /// <summary>
        /// アカウントメニュークリック時処理
        /// </summary>
        private void OnAccountMenuItemClick()
        {
            Logger.Info(this.QuickMenu.Manager.GetResource().GetString("LOG_INFO_MenuItemAccountPage_OnAccountMenuItemClick"));

            // ユーザーのホーム画面を表示する
        }
    }
}

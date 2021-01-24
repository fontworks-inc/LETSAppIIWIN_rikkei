using System.Collections.Generic;
using System.Windows.Forms;
using NLog;

namespace Client.UI.Components.MenuItem
{
    /// <summary>
    /// クイックメニュー－アップデート
    /// </summary>
    public class MenuItemUpdate : MenuItemBase
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>アップデート</summary>
        private ToolStripMenuItem update;

        /// <summary>セパレータ</summary>
        private ToolStripSeparator separator;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="quickMenu">QuickMenuComponent</param>
        public MenuItemUpdate(QuickMenuComponent quickMenu)
             : base(quickMenu)
        {
            this.update.Click += (s, e) =>
            {
                this.OnUpdateMenuItemClick();
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
                    this.update,
                };
            }
        }

        /// <summary>
        /// クイックメニュー初期化処理
        /// </summary>
        protected override void InitializeComponent()
        {
            this.separator = this.CreateSeparator();
            this.update = this.Create("MENU_UPDATE", this.Resource.GetString("MENU_UPDATE"));

            this.SetMenu();
        }

        /// <summary>
        /// クイックメニューにアイテムを追加する
        /// </summary>
        protected override void SetMenu()
        {
            this.QuickMenu.ContextMenu.Items.Add(this.separator);
            this.QuickMenu.ContextMenu.Items.Add(this.update);
        }

        /// <summary>
        /// アップデートメニュークリック時処理
        /// </summary>
        private void OnUpdateMenuItemClick()
        {
            Logger.Info(this.QuickMenu.Manager.GetResource().GetString("LOG_INFO_MenuItemUpdate_OnUpdateMenuItemClick"));

            // アップデート開始
            this.QuickMenu.Manager.StartUpdate();
        }
    }
}

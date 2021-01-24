using System.Collections.Generic;
using System.Windows.Forms;
using NLog;

namespace Client.UI.Components.MenuItem
{
    /// <summary>
    /// クイックメニュー－フォント
    /// </summary>
    public class MenuItemFontListPage : MenuItemBase
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>フォント</summary>
        private ToolStripMenuItem font;

        /// <summary>セパレータ</summary>
        private ToolStripSeparator separator;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="quickMenu">QuickMenuComponent</param>
        public MenuItemFontListPage(QuickMenuComponent quickMenu)
             : base(quickMenu)
        {
            this.font.Click += (s, e) =>
            {
                this.OnFontMenuItemClick();
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
                    this.font,
                };
            }
        }

        /// <summary>
        /// クイックメニュー初期化処理
        /// </summary>
        protected override void InitializeComponent()
        {
            this.separator = this.CreateSeparator();
            this.font = this.Create("MENU_FONT", this.Resource.GetString("MENU_FONT"));

            this.SetMenu();
        }

        /// <summary>
        /// クイックメニューにアイテムを追加する
        /// </summary>
        protected override void SetMenu()
        {
            this.QuickMenu.ContextMenu.Items.Add(this.separator);
            this.QuickMenu.ContextMenu.Items.Add(this.font);
        }

        /// <summary>
        /// フォントメニュークリック時処理
        /// </summary>
        private void OnFontMenuItemClick()
        {
            Logger.Info(this.QuickMenu.Manager.GetResource().GetString("LOG_INFO_MenuItemFontListPage_OnFontMenuItemClick"));

            // フォント一覧ページを表示する
        }
    }
}

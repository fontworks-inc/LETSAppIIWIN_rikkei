using System.Collections.Generic;
using System.Windows.Forms;

namespace Client.UI.Components.MenuItem
{
    /// <summary>
    /// クイックメニュー－フォント
    /// </summary>
    public class MenuItemFontListPage : MenuItemBase
    {
        /// <summary>フォント</summary>
        private ToolStripMenuItem font;

        /// <summary>セパレータ</summary>
        private ToolStripSeparator separator;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="manager">ComponentManager</param>
        public MenuItemFontListPage(ComponentManager manager)
             : base(manager)
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
        /// クイックメニューにアイテムを追加する
        /// </summary>
        /// <param name="quickMenu">クイックメニュー</param>
        public override void SetMenu(QuickMenuComponent quickMenu)
        {
            quickMenu.ContextMenu.Items.Add(this.separator);
            quickMenu.ContextMenu.Items.Add(this.font);
        }

        /// <summary>
        /// クイックメニュー初期化処理
        /// </summary>
        protected override void InitializeComponent()
        {
            this.separator = this.CreateSeparator();
            this.font = this.Create("MENU_FONT", this.Resource.GetString("MENU_FONT"));
        }

        /// <summary>
        /// フォントメニュークリック時処理
        /// </summary>
        private void OnFontMenuItemClick()
        {
            // フォント一覧ページを表示する
        }
    }
}

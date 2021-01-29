using System.Collections.Generic;
using System.Windows.Forms;
using Core.Interfaces;

namespace Client.UI.Components.MenuItem
{
    /// <summary>
    /// クイックメニュー－アップデート
    /// </summary>
    public class MenuItemUpdate : MenuItemBase
    {
        /// <summary>
        /// 文言の取得を行うインスタンス
        /// </summary>
        private readonly IResourceWrapper resourceWrapper = null;

        /// <summary>アップデート</summary>
        private ToolStripMenuItem update;

        /// <summary>セパレータ</summary>
        private ToolStripSeparator separator;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="quickMenu">QuickMenuComponent</param>
        /// <param name="resourceWrapper"> 文言の取得を行うインスタンス</param>
        public MenuItemUpdate(QuickMenuComponent quickMenu, IResourceWrapper resourceWrapper)
             : base(quickMenu)
        {
            this.update.Click += (s, e) =>
            {
                this.OnUpdateMenuItemClick();
            };
            this.resourceWrapper = resourceWrapper;
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
            // ダイアログを表示する
            DialogResult result = MessageBox.Show(
                this.resourceWrapper.GetString("MENU_UPDATE_CONFIRM_TEXT"),
                this.resourceWrapper.GetString("MENU_UPDATE_CONFIRM_CAPTION"),
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question);

            if (result == DialogResult.OK)
            {
                // アップデート開始
                this.QuickMenu.Manager.StartUpdate();
            }
        }
    }
}

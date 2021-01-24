using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Core.Interfaces;

namespace Client.UI.Components.MenuItem
{
    /// <summary>
    /// クイックメニューアイテムの基底クラス
    /// </summary>
    public class MenuItemBase : Component
    {
        /// <summary>メニューアイテムのマージンサイズ</summary>
        private readonly int paddingSize = 3;

        /// <summary>状態表示メニューアイテムの表示領域の高さ</summary>
        private readonly int itemHeight = 20;

        /// <summary>状態表示メニューアイテムの表示領域の幅</summary>
        private readonly int itemWidth = 200;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="quickMenu">QuickMenuComponent</param>
        public MenuItemBase(QuickMenuComponent quickMenu)
        {
            this.QuickMenu = quickMenu;
            this.QuickMenu.Manager.Container.Add(this);
            this.Resource = this.QuickMenu.Manager.GetResource();

            this.InitializeComponent();
        }

        /// <summary>
        /// クイックメニューアイテムのリストを返す
        /// </summary>
        public virtual List<ToolStripItem> Items
        {
            get;
            set;
        }

        /// <summary>
        /// QuickMenuComponent
        /// </summary>
        protected QuickMenuComponent QuickMenu
        {
            get;
            private set;
        }

        /// <summary>
        /// IResourceWrapper
        /// </summary>
        protected IResourceWrapper Resource
        {
            get;
            private set;
        }

        /// <summary>
        /// クイックメニューアイテムを表示する
        /// </summary>
        public void Show()
        {
            foreach (var menuItem in this.Items)
            {
                menuItem.Visible = true;
            }
        }

        /// <summary>
        /// クイックメニューアイテムを非表示にする
        /// </summary>
        public void Hide()
        {
            foreach (var menuItem in this.Items)
            {
                menuItem.Visible = false;
            }
        }

        /// <summary>
        /// クイックメニューアイテム初期化処理
        /// </summary>
        protected virtual void InitializeComponent()
        {
        }

        /// <summary>
        /// クイックメニューにアイテムを追加する
        /// </summary>
        protected virtual void SetMenu()
        {
        }

        /// <summary>
        /// ToolStripMenuItemを生成
        /// </summary>
        /// <param name="name">名称(キー)</param>
        /// <param name="text">表示名称</param>
        /// <returns>生成されたToolStripMenuItem</returns>
        protected ToolStripMenuItem Create(string name, string text)
        {
            var menuItem = new ToolStripMenuItem();

            menuItem.Name = name;
            menuItem.Text = text;
            menuItem.Margin = new Padding(this.paddingSize);

            return menuItem;
        }

        /// <summary>
        /// ToolStripLabelを生成
        /// </summary>
        /// <param name="text">表示名称</param>
        /// <returns>生成されたToolStripLabel</returns>
        protected ToolStripLabel CreateLabel(string text)
        {
            return this.CreateLabel(text, this.itemWidth, this.itemHeight);
        }

        /// <summary>
        /// ToolStripLabelを生成
        /// </summary>
        /// <param name="text">表示名称</param>
        /// <param name="height">メニューの高さ</param>
        /// <returns>生成されたToolStripLabel</returns>
        protected ToolStripLabel CreateLabel(string text, int height)
        {
            return this.CreateLabel(text, this.itemWidth, height);
        }

        /// <summary>
        /// ToolStripLabelを生成
        /// </summary>
        /// <param name="text">表示名称</param>
        /// <param name="width">メニューの幅</param>
        /// <param name="height">メニューの高さ</param>
        /// <returns>生成されたToolStripLabel</returns>
        protected ToolStripLabel CreateLabel(string text, int width, int height)
        {
            var menuLabel = new ToolStripLabel(text);

            menuLabel.DisplayStyle = ToolStripItemDisplayStyle.Text;
            menuLabel.TextAlign = ContentAlignment.MiddleLeft;
            menuLabel.AutoSize = false;
            menuLabel.Size = new Size(width, height);

            return menuLabel;
        }

        /// <summary>
        /// ToolStripSeparatorを生成
        /// </summary>
        /// <returns>生成されたToolStripSeparator</returns>
        protected ToolStripSeparator CreateSeparator()
        {
            return new ToolStripSeparator();
        }
    }
}

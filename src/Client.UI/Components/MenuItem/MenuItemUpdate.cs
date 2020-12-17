using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Client.UI.Components.MenuItem
{
    /// <summary>
    /// クイックメニュー－アップデート
    /// </summary>
    public class MenuItemUpdate : MenuItemBase
    {
        /// <summary>アップデート</summary>
        private ToolStripMenuItem update;

        /// <summary>セパレータ</summary>
        private ToolStripSeparator separator;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="manager">ComponentManager</param>
        public MenuItemUpdate(ComponentManager manager)
             : base(manager)
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
        /// クイックメニューにアイテムを追加する
        /// </summary>
        /// <param name="quickMenu">クイックメニュー</param>
        public override void SetMenu(QuickMenuComponent quickMenu)
        {
            quickMenu.ContextMenu.Items.Add(this.separator);
            quickMenu.ContextMenu.Items.Add(this.update);
        }

        /// <summary>
        /// クイックメニュー初期化処理
        /// </summary>
        protected override void InitializeComponent()
        {
            this.separator = this.CreateSeparator();
            this.update = this.Create("MENU_UPDATE", this.Resource.GetString("MENU_UPDATE"));
        }

        /// <summary>
        /// アップデートメニュークリック時処理
        /// </summary>
        private void OnUpdateMenuItemClick()
        {
            var menuUpdateStatus = this.Manager.MenuUpdateStatus;
            MenuItemUpdateStatus.UpdateProgress progress = menuUpdateStatus.SetProgressStatus;
            MenuItemUpdateStatus.UpdateCompleted completed = menuUpdateStatus.SetCompleted;

            // アップデート開始
            this.Manager.StartUpdate();

            // アップデート処理を実行するサービスを呼出し
            // XXXXXService.Update(progress, completed);

            // アップデート処理内で、進捗率を更新する処理を呼出し
            // 例）50％
            progress(50);

            // アップデート処理内で、完了処理を呼出し
            // completed();

            // アップデート完了
            // this.Manager.UpdateCompleted();
        }
    }
}

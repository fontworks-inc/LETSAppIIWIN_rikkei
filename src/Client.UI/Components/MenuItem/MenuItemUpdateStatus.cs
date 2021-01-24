using System.Collections.Generic;
using System.Windows.Forms;

namespace Client.UI.Components.MenuItem
{
    /// <summary>
    /// クイックメニュー－アップデート中(状態表示)
    /// </summary>
    public class MenuItemUpdateStatus : MenuItemBase
    {
        /// <summary>状態表示メニューアイテムの表示領域の高さ</summary>
        private readonly int itemHeight = 40;

        /// <summary>アップデート中</summary>
        private ToolStripLabel updateStatus;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="quickMenu">QuickMenuComponent</param>
        public MenuItemUpdateStatus(QuickMenuComponent quickMenu)
             : base(quickMenu)
        {
        }

        /// <summary>アップデート進捗状況</summary>
        /// <param name="progressRate">進捗率</param>
        /// <remarks>SetUpdateProgressStatus を代入</remarks>
        public delegate void UpdateProgress(int progressRate);

        /// <summary>アップデート完了時処理</summary>
        /// <remarks>SetUpdateCompleted を代入</remarks>
        public delegate void UpdateCompleted();

        /// <summary>
        /// クイックメニューアイテムのリストを返す
        /// </summary>
        public override List<ToolStripItem> Items
        {
            get
            {
                return new List<ToolStripItem>()
                {
                    this.updateStatus,
                };
            }
        }

        /// <summary>
        /// アップデートの進捗状況を設定
        /// </summary>
        /// <param name="progressRate">進捗率</param>
        public void SetProgressStatus(int progressRate)
        {
            var rate = string.Format(this.Resource.GetString("MENU_PROGRESS_RATE"), progressRate);
            this.updateStatus.Text = $"{this.Resource.GetString("MENU_UPDATE_LOADING")}{rate}";
        }

        /// <summary>
        /// アップデートの完了を設定
        /// </summary>
        public void SetCompleted()
        {
            this.updateStatus.Text = $"{this.Resource.GetString("MENU_UPDATE_COMPLETED")}";
        }

        /// <summary>
        /// クイックメニュー初期化処理
        /// </summary>
        protected override void InitializeComponent()
        {
            this.updateStatus = this.CreateLabel(this.Resource.GetString("MENU_UPDATE_LOADING"), this.itemHeight);

            this.SetMenu();
        }

        /// <summary>
        /// クイックメニューにアイテムを追加する
        /// </summary>
        protected override void SetMenu()
        {
            this.QuickMenu.ContextMenu.Items.Add(this.updateStatus);
        }
    }
}

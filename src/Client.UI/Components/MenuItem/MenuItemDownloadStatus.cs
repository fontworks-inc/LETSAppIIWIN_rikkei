using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;

namespace Client.UI.Components.MenuItem
{
    /// <summary>
    /// クイックメニュー－ダウンロード中(状態表示)
    /// </summary>
    public class MenuItemDownloadStatus : MenuItemBase
    {
        /// <summary>ダウンロード中</summary>
        private ToolStripLabel downloadStatus;

        /// <summary>フォント名称</summary>
        private ToolStripLabel fontName;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="manager">ComponentManager</param>
        public MenuItemDownloadStatus(ComponentManager manager)
             : base(manager)
        {
        }

        /// <summary>ダウンロード進捗状況</summary>
        /// <param name="progressRate">進捗率</param>
        /// <param name="fontInfomation">フォント情報　※現状とりあえず string としているが、Entityを渡す想定</param>
        /// <remarks>SetDownloadProgressStatus を代入</remarks>
        public delegate void DownloadProgress(int progressRate, string fontInfomation);

        /// <summary>ダウンロード完了時処理</summary>
        /// <remarks>SetDownloadCompleted を代入</remarks>
        public delegate void DownloadCompleted();

        /// <summary>
        /// クイックメニューアイテムのリストを返す
        /// </summary>
        public override List<ToolStripItem> Items
        {
            get
            {
                return new List<ToolStripItem>()
                {
                    this.downloadStatus,
                    this.fontName,
                };
            }
        }

        /// <summary>
        /// クイックメニューにアイテムを追加する
        /// </summary>
        /// <param name="quickMenu">クイックメニュー</param>
        public override void SetMenu(QuickMenuComponent quickMenu)
        {
            quickMenu.ContextMenu.Items.Add(this.downloadStatus);
            quickMenu.ContextMenu.Items.Add(this.fontName);
        }

        /// <summary>
        /// ダウンロードの進捗状況を設定
        /// </summary>
        /// <param name="progressRate">進捗率</param>
        /// <param name="fontInfomation">フォント情報　※現状とりあえず string としているが、Entityを渡す想定</param>
        public void SetProgressStatus(int progressRate, string fontInfomation)
        {
            var rate = string.Format(this.Resource.GetString("MENU_PROGRESS_RATE"), progressRate);
            this.downloadStatus.Text = $"{this.Resource.GetString("MENU_DOWNLOAD_LOADING")}{rate}";

            // フォント名称表示部は文字数で切り、末尾に「...」を付与？
            this.fontName.Text = $"{fontInfomation}...";
        }

        /// <summary>
        /// ダウンロードの完了を設定
        /// </summary>
        public void SetCompleted()
        {
            this.Hide();
        }

        /// <summary>
        /// クイックメニュー初期化処理
        /// </summary>
        protected override void InitializeComponent()
        {
            this.downloadStatus = this.CreateLabel(this.Resource.GetString("MENU_DOWNLOAD_LOADING"));
            this.fontName = this.CreateLabel(this.Resource.GetString("MENU_DOWNLOAD_FONTNAME"));
        }
    }
}

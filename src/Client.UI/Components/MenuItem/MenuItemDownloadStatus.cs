using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Entities;

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
        /// <param name="quickMenu">QuickMenuComponent</param>
        public MenuItemDownloadStatus(QuickMenuComponent quickMenu)
             : base(quickMenu)
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
        /// ダウンロードの進捗状況を設定
        /// </summary>
        /// <param name="font">フォント</param>
        /// <param name="compFileSize">ダウンロード済みファイルサイズ</param>
        /// <param name="totalFileSize">合計ファイルサイズ</param>
        public void SetProgressStatus(InstallFont font, double compFileSize, double totalFileSize)
        {
            double progressRate = Math.Floor(compFileSize / totalFileSize * 100);
            var rate = string.Format(this.Resource.GetString("MENU_PROGRESS_RATE"), progressRate);
            this.downloadStatus.Text = $"{this.Resource.GetString("MENU_DOWNLOAD_LOADING")}{rate}";

            // フォント名称表示部は文字数で切り、末尾に「...」を付与？
            string text = string.Format(this.Resource.GetString("MENU_DOWNLOAD_FONTNAME"), font.DisplayFontName);
            this.fontName.Text = text;
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

            this.SetMenu();
        }

        /// <summary>
        /// クイックメニューにアイテムを追加する
        /// </summary>
        protected override void SetMenu()
        {
            this.QuickMenu.ContextMenu.Items.Add(this.downloadStatus);
            this.QuickMenu.ContextMenu.Items.Add(this.fontName);
        }
    }
}

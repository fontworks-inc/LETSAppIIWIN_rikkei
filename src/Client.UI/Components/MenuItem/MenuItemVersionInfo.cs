using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using ApplicationService.Interfaces;
using Client.UI.Wrappers;
using Core.Interfaces;
using NLog;

namespace Client.UI.Components.MenuItem
{
    /// <summary>
    /// クイックメニュー－LETSアプリバージョン情報
    /// </summary>
    public class MenuItemVersionInfo : MenuItemBase
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>
        /// 文言の取得を行うインスタンス
        /// </summary>
        private readonly IResourceWrapper resourceWrapper = null;

        /// <summary>
        /// プログラムからバージョンを取得するサービス
        /// </summary>
        private IApplicationVersionService applicationVersionService = null;

        /// <summary>LETSアプリバージョン情報</summary>
        private ToolStripMenuItem letsAplVersionInfo;

        ///// <summary>セパレータ</summary>
        //private ToolStripSeparator separator;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="quickMenu">QuickMenuComponent</param>
        /// <param name="resourceWrapper"> 文言の取得を行うインスタンス</param>
        /// <param name="applicationVersionService">フォントマネージャサービス</param>
        public MenuItemVersionInfo(
            QuickMenuComponent quickMenu,
            IResourceWrapper resourceWrapper,
            IApplicationVersionService applicationVersionService)
             : base(quickMenu)
        {
            this.letsAplVersionInfo.Click += (s, e) =>
            {
                this.OnLetsAplVersionInfoItemClick();
            };
            this.resourceWrapper = resourceWrapper;
            this.applicationVersionService = applicationVersionService;
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
                    this.letsAplVersionInfo,
                };
            }
        }

        /// <summary>
        /// クイックメニュー初期化処理
        /// </summary>
        protected override void InitializeComponent()
        {
            this.letsAplVersionInfo = this.Create("MENU_VERSION_INFO", this.Resource.GetString("MENU_VERSION_INFO"));

            this.SetMenu();
        }

        /// <summary>
        /// クイックメニューにアイテムを追加する
        /// </summary>
        protected override void SetMenu()
        {
            this.QuickMenu.ContextMenu.Items.Add(this.letsAplVersionInfo);
        }

        /// <summary>
        /// フォントメニュークリック時処理
        /// </summary>
        private void OnLetsAplVersionInfoItemClick()
        {
            try
            {
                if (this.applicationVersionService == null)
                {
                    return;
                }

                Logger.Debug($"クイックメニュー－バージョン情報");
                string appver = $"Ver {this.applicationVersionService.GetVerison()}";

                // ダイアログを表示する
                DialogResult result = MessageBox.Show(
                    appver,
                    this.resourceWrapper.GetString("MENU_VERSION_INFO_CAPTION"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                // エラーがあった場合は通知を表示
                Logger.Error(e, this.resourceWrapper.GetString("MENU_VERSION_ERROR_CAPTION"));
                ToastNotificationWrapper.Show(this.resourceWrapper.GetString("MENU_VERSION_ERROR_CAPTION"), e.Message);
            }
        }

    }
}

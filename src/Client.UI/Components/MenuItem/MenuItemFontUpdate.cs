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
    /// クイックメニュー－フォント同期
    /// </summary>
    public class MenuItemFontUpdate : MenuItemBase
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
        /// メモリで保持する情報を格納するリポジトリ
        /// </summary>
        private readonly IVolatileSettingRepository volatileSettingRepository;

        /// <summary>
        /// ユーザ別ステータス情報を格納するリポジトリ
        /// </summary>
        private readonly IUserStatusRepository userStatusRepository;

        /// <summary>
        /// URLアドレスを格納するリポジトリ
        /// </summary>
        private readonly IUrlRepository urlRepository;

        /// <summary>
        /// フォント管理サービス
        /// </summary>
        private IFontManagerService fontManagerService = null;

        /// <summary>フォント同期</summary>
        private ToolStripMenuItem fontUpdate;

        /// <summary>セパレータ</summary>
        private ToolStripSeparator separator;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="quickMenu">QuickMenuComponent</param>
        /// <param name="resourceWrapper"> 文言の取得を行うインスタンス</param>
        /// <param name="volatileSettingRepository">メモリで保持する情報を格納するリポジトリ</param>
        /// <param name="userStatusRepository">ユーザ別ステータス情報を格納するリポジトリ</param>
        /// <param name="urlRepository">URLアドレスを格納するリポジトリ</param>
        /// <param name="fontManagerService">フォントマネージャサービス</param>
        public MenuItemFontUpdate(
            QuickMenuComponent quickMenu,
            IResourceWrapper resourceWrapper,
            IVolatileSettingRepository volatileSettingRepository,
            IUserStatusRepository userStatusRepository,
            IUrlRepository urlRepository,
            IFontManagerService fontManagerService)
             : base(quickMenu)
        {
            this.fontUpdate.Click += (s, e) =>
            {
                this.OnFontMenuItemClick();
            };
            this.resourceWrapper = resourceWrapper;
            this.volatileSettingRepository = volatileSettingRepository;
            this.userStatusRepository = userStatusRepository;
            this.urlRepository = urlRepository;
            this.fontManagerService = fontManagerService;
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
                    this.fontUpdate,
                };
            }
        }

        /// <summary>
        /// クイックメニュー初期化処理
        /// </summary>
        protected override void InitializeComponent()
        {
            this.separator = this.CreateSeparator();
            this.fontUpdate = this.Create("MENU_FONT_UPDATE", this.Resource.GetString("MENU_FONT_UPDATE"));

            this.SetMenu();
        }

        /// <summary>
        /// クイックメニューにアイテムを追加する
        /// </summary>
        protected override void SetMenu()
        {
            //this.QuickMenu.ContextMenu.Items.Add(this.separator);
            this.QuickMenu.ContextMenu.Items.Add(this.fontUpdate);
        }

        /// <summary>
        /// フォントメニュークリック時処理
        /// </summary>
        private void OnFontMenuItemClick()
        {
            try
            {
                if (this.fontManagerService == null)
                {
                    return;
                }

                this.fontManagerService.CheckFontsList();
            }
            catch (Exception e)
            {
                // エラーがあった場合は通知を表示
                Logger.Error(e, this.resourceWrapper.GetString("MENU_FONT_ERROR_CAPTION"));
                ToastNotificationWrapper.Show(this.resourceWrapper.GetString("MENU_FONT_ERROR_CAPTION"), e.Message);
            }
        }

        /// <summary>
        /// APIからフォント一覧ページのURLを取得
        /// </summary>
        private string GetFontListPageUrl(string deviceId, string accessToken)
        {
            try
            {
                return this.urlRepository.GetFontListPageUrl(deviceId, accessToken).ToString();
            }
            catch (Exception e)
            {
                Logger.Error(e);
                throw new Exception(this.resourceWrapper.GetString("MENU_FONT_ERROR_TEXT"), e);
            }
        }
    }
}

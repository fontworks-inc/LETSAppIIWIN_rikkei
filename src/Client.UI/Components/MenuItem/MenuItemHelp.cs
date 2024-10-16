using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Client.UI.Wrappers;
using Core.Interfaces;
using NLog;

namespace Client.UI.Components.MenuItem
{
    /// <summary>
    /// クイックメニュー－ヘルプ
    /// </summary>
    public class MenuItemHelp : MenuItemBase
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

        /// <summary>ヘルプ</summary>
        private ToolStripMenuItem showHelp;

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
        public MenuItemHelp(
            QuickMenuComponent quickMenu,
            IResourceWrapper resourceWrapper,
            IVolatileSettingRepository volatileSettingRepository,
            IUserStatusRepository userStatusRepository,
            IUrlRepository urlRepository)
             : base(quickMenu)
        {
            this.showHelp.Click += (s, e) =>
            {
                this.OnHelpMenuItemClick();
            };
            this.resourceWrapper = resourceWrapper;
            this.volatileSettingRepository = volatileSettingRepository;
            this.userStatusRepository = userStatusRepository;
            this.urlRepository = urlRepository;
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
                    this.showHelp,
                };
            }
        }

        /// <summary>
        /// クイックメニュー初期化処理
        /// </summary>
        protected override void InitializeComponent()
        {
            this.separator = this.CreateSeparator();
            this.showHelp = this.Create("MENU_HELP", this.Resource.GetString("MENU_HELP"));

            this.SetMenu();
        }

        /// <summary>
        /// クイックメニューにアイテムを追加する
        /// </summary>
        protected override void SetMenu()
        {
            this.QuickMenu.ContextMenu.Items.Add(this.separator);
            this.QuickMenu.ContextMenu.Items.Add(this.showHelp);
        }

        /// <summary>
        /// ヘルプメニュークリック時処理
        /// </summary>
        private void OnHelpMenuItemClick()
        {
            try
            {
                Logger.Debug($"クイックメニュー－ヘルプ");

                // デバイスIDを取得
                string deviceId = this.userStatusRepository.GetStatus().DeviceId;

                // アクセストークンを取得
                string accessToken = this.volatileSettingRepository.GetVolatileSetting().AccessToken;

                // URLの取得
                string url = this.GetHelpUrl(deviceId, accessToken);

                // ブラウザ起動：フォント一覧ページを表示する
                // NET Frameworkでは、Process.Start(url);で既定のブラウザが開いたが.NET CoreではNGになったよう
                // Windowsの場合　& をエスケープ（シェルがコマンドの切れ目と認識するのを防ぐ））
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }
            catch (Exception e)
            {
                // エラーがあった場合は通知を表示
                Logger.Error(e, this.resourceWrapper.GetString("MENU_HELP_ERROR_CAPTION"));
                ToastNotificationWrapper.Show(e.Message, this.resourceWrapper.GetString("MENU_HELP_ERROR_CAPTION"));
            }
        }

        /// <summary>
        /// APIからフォント一覧ページのURLを取得
        /// </summary>
        private string GetHelpUrl(string deviceId, string accessToken)
        {
            try
            {
                return this.urlRepository.GetHelpUrl(deviceId, accessToken).ToString();
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
                throw new Exception(this.resourceWrapper.GetString("MENU_HELP_ERROR_TEXT"), e);
            }
        }
    }
}

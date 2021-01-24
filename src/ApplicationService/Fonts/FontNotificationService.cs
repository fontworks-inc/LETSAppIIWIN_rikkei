using ApplicationService.Interfaces;
using Core.Entities;
using Core.Interfaces;

namespace ApplicationService.Fonts
{
    /// <summary>
    /// フォントのアクティベートディアクティベート通知を格納するサービスのクラス
    /// </summary>
    public class FontNotificationService : IFontNotificationService
    {
        /// <summary>
        /// フォント管理に関する処理を行うサービス
        /// </summary>
        private readonly IFontManagerService fontManagerService;

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="fontManagerService">フォント管理に関する処理を行うサービス</param>
        public FontNotificationService(IFontManagerService fontManagerService)
        {
            this.fontManagerService = fontManagerService;
        }

        /// <summary>
        /// フォントアクティベート通知処理
        /// </summary>
        /// <param name="font">アクティベートフォント情報</param>
        public void Activate(ActivateFont font)
        {
            this.fontManagerService.Synchronize(font);
        }

        /// <summary>
        /// フォントディアクティベート通知処理
        /// </summary>
        /// <param name="fontId">ディアクティベートするフォントID</param>
        public void Deactivate(string fontId)
        {
            this.fontManagerService.DeactivateFont(fontId);
        }

        /// <summary>
        /// ディアクティブフォントの一括アンインストール
        /// </summary>
        public void AllUninstall()
        {
            this.fontManagerService.DeactivateSettingFonts();
        }
    }
}
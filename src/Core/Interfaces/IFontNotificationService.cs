using Core.Entities;

namespace Core.Interfaces
{
    /// <summary>
    /// フォントのアクティベートディアクティベート通知を格納するサービスのインターフェイス
    /// </summary>
    public interface IFontNotificationService
    {
        /// <summary>
        /// フォントアクティベート通知処理
        /// </summary>
        /// <param name="font">アクティベートフォント情報</param>
        void Activate(ActivateFont font);

        /// <summary>
        /// フォントディアクティベート通知処理
        /// </summary>
        /// <param name="fontId">ディアクティベート対象のフォントID</param>
        void Deactivate(string fontId);

        /// <summary>
        /// ディアクティブフォントの一括アンインストール
        /// </summary>
        void AllUninstall();
    }
}

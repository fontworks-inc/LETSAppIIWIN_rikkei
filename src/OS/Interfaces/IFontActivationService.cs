using Core.Entities;

namespace OS.Interfaces
{
    /// <summary>
    /// フォントアクティベートサービスを表すインターフェイス
    /// </summary>
    public interface IFontActivationService
    {
        /// <summary>
        /// フォントをインストールする
        /// </summary>
        /// <param name="font">対象フォント</param>
        bool Install(Font font);

        /// <summary>
        /// フォントチェンジメッセージを送信する
        /// </summary>
        void BroadcastFont();

        /// <summary>
        /// フォントをアンインストールする
        /// </summary>
        /// <param name="font">対象フォント</param>
        void Uninstall(Font font);

        /// <summary>
        /// フォントをアクティベートする
        /// </summary>
        /// <param name="font">対象フォント</param>
        /// <returns>アクティベートに成功したらtrueを返す</returns>
        bool Activate(Font font);

        /// <summary>
        /// フォントをディアクティベートする
        /// </summary>
        /// <param name="font">対象フォント</param>
        void Deactivate(Font font);
    }
}

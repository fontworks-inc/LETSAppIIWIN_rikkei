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
        void Install(Font font);

        /// <summary>
        /// フォントをアンインストールする
        /// </summary>
        /// <param name="font">対象フォント</param>
        void Uninstall(Font font);

        /// <summary>
        /// フォントをアクティベートする
        /// </summary>
        /// <param name="font">対象フォント</param>
        void Activate(Font font);

        /// <summary>
        /// フォントをディアクティベートする
        /// </summary>
        /// <param name="font">対象フォント</param>
        void Deactivate(Font font);
    }
}

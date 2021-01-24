using Core.Entities;
using OS.Interfaces;

namespace OS.Services
{
    /// <summary>
    /// フォントアクティベートサービスを表すクラスのモック
    /// </summary>
    public class FontActivationServiceMock : IFontActivationService
    {
        /// <summary>
        /// フォントをインストールする
        /// </summary>
        /// <param name="font">対象フォント</param>
        public bool Install(Font font)
        {
            return true;
        }

        public void BroadcastFont()
        {
        }

        /// <summary>
        /// フォントをアクティベートする
        /// </summary>
        /// <param name="font">対象フォント</param>
        public bool Activate(Font font)
        {
            return true;
        }

        /// <summary>
        /// フォントをディアクティベートする
        /// </summary>
        /// <param name="font">対象フォント</param>
        public void Deactivate(Font font)
        {
            font.IsActivated = false;
        }

        /// <summary>
        /// フォントをアンインストールする
        /// </summary>
        /// <param name="font">対象フォント</param>
        public void Uninstall(Font font)
        {
        }
    }
}

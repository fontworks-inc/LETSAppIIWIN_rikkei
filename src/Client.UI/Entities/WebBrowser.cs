using System.Diagnostics;
using Core.Entities;

namespace Client.UI.Entities
{
    /// <summary>
    /// Webブラウザを表すクラス
    /// </summary>
    public class WebBrowser
    {
        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        public WebBrowser()
        {
        }

        /// <summary>
        /// 指定されたURLを表示する。
        /// </summary>
        /// <param name="url">URL</param>
        public void Navigate(Url url)
        {
            // NET Frameworkでは、Process.Start(url);で既定のブラウザが開いたが.NET CoreではNGになったよう
            // Windowsの場合　& をエスケープ（シェルがコマンドの切れ目と認識するのを防ぐ）
            if (url == null)
            {
                return;
            }

            string urlString = url.ToString().Replace("&", "^&");
            Process.Start(new ProcessStartInfo("cmd", $"/c start {urlString}") { CreateNoWindow = true });
        }
    }
}

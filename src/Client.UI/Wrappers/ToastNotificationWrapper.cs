using NLog;
using System.IO;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Client.UI.Wrappers
{
    /// <summary>
    /// トースト通知処理をラップするクラス
    /// </summary>
    public static class ToastNotificationWrapper
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>
        /// アプリケーション名
        /// </summary>
        private static readonly string AppId = "LETS";

        /// <summary>
        /// アプリケーションアイコンファイル名
        /// </summary>
        private static readonly string AppIconName = "LETS.ico";

        /// <summary>
        /// トースト通知を表示する
        /// </summary>
        /// <param name="mainMessage">メッセージ概要部</param>
        /// <param name="description">メッセージ説明部</param>
        public static void Show(string mainMessage, string description)
        {
            Logger.Debug($"ToastNotificationWrapper#Show:Enter [{mainMessage}][{description}]");

            // テンプレート(ToastImageAndText02)を使用してトースト通知のXMLを生成
            var xml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText02);
            var stringElements = xml.GetElementsByTagName("text");
            stringElements[0].AppendChild(xml.CreateTextNode(mainMessage));
            stringElements[1].AppendChild(xml.CreateTextNode(description));

            // アプリケーションアイコンを表示
            // アイコンファイルは実行ファイルと同じディレクトリにコピーする(コンテンツ－常にコピーする)
            var images = xml.GetElementsByTagName("image");
            var imagePath = "file:///" + Path.GetFullPath(AppIconName);
            ((Windows.Data.Xml.Dom.XmlElement)images[0]).SetAttribute("src", imagePath);

            // 表示間隔を設定("short":7秒、"long":25秒)　※デフォルトはshort
            IXmlNode toastNode = xml.SelectSingleNode("/toast");
            ((Windows.Data.Xml.Dom.XmlElement)toastNode).SetAttribute("duration", "short");

            // トースト通知を表示する
            var toast = new ToastNotification(xml);
            ToastNotificationManager.CreateToastNotifier(AppId).Show(toast);

            Logger.Debug("ToastNotificationWrapper#Show:Exit");
        }
    }
}

using System;
using Client.UI.Wrappers;
using NLog;

namespace Client.UI
{
    /// <summary>
    /// グローバル例外を通知するクラス
    /// </summary>
    public static class ExceptionNotifier
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>
        /// 例外を通知する
        /// </summary>
        /// <param name="exception">例外</param>
        public static void Notify(Exception exception)
        {
            // 例外発生時の処理のため、コンテナを経由せず直接インスタンスを生成する
            var resourceWrapper = new ResourceWrapper();

            // ログ出力
            string logMessage = resourceWrapper.GetString("LOG_FATAL_UnhandledException");
            Logger.Fatal(exception, logMessage);

            // トースト通知を表示
            string mainMessage = resourceWrapper.GetString("APP_FATAL_UnhandledException_MainMassage");
            string description = resourceWrapper.GetString("APP_FATAL_UnhandledException_Description");
            ToastNotificationWrapper.Show(mainMessage, description);
        }
    }
}

using System;
using System.IO;
using System.Windows;
using Core.Entities;
using NLog;

namespace Client.UI
{
    /// <summary>
    /// アプリケーションに関する設定を行う
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>
        /// エントリポイント
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Logger.Debug("Main:return:Enter");

            // 常駐アプリケーションを起動
            Shell shell = new Shell();
            MultiplePreventionInfo multipleInfo = Shell.MultiplePrevention;
            multipleInfo.MutexInfo = new System.Threading.Mutex(false, multipleInfo.MutexName);
            try
            {
                try
                {
                    multipleInfo.HasHandle = multipleInfo.MutexInfo.WaitOne(0, false);
                }
                catch (System.Threading.AbandonedMutexException)
                {
                    multipleInfo.HasHandle = true;
                }

                if (!multipleInfo.HasHandle)
                {
                    Logger.Debug("Main:return:!multipleInfo.HasHandle");
                    return;
                }

                shell.Run();
            }
            finally
            {
                if (multipleInfo.HasHandle)
                {
                    multipleInfo.MutexInfo.ReleaseMutex();
                }

                multipleInfo.MutexInfo.Close();

                Logger.Debug("Main:finally");
            }
        }
    }
}

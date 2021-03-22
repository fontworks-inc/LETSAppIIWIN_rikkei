using System;
using System.Windows;
using Core.Entities;

namespace Client.UI
{
    /// <summary>
    /// アプリケーションに関する設定を行う
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// エントリポイント
        /// </summary>
        [STAThread]
        public static void Main()
        {
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
            }
        }
    }
}

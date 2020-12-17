using System;
using System.Windows;

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
            // 二重起動チェック

            // 常駐アプリケーションを起動
            Shell shell = new Shell();
            shell.Run();
        }
    }
}

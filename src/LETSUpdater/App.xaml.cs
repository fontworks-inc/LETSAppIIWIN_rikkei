using System;
using System.Windows;
using System.IO;
using System.Diagnostics;

namespace Updater
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        public static void Main(string[] args)
        {
            //  表示WAIT処理
            App app = new App();
            if(args.Length > 0)
            {
                try
                {
                    //  プログラムアップデートロジックを実行して終了
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                Environment.Exit(0);
            }

            //  引数なしの場合はTutorial画面を表示する
            ////  最初にLETSアプリを起動する
            //try
            //{
            //    string appPath = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\');
            //    string targetpg = File.ReadAllText(appPath + @"\targetpg.txt");
            //    Process p = new Process();
            //    p.StartInfo.FileName = targetpg;
            //    p.StartInfo.WorkingDirectory = Path.GetDirectoryName(targetpg);
            //    p.Start();
            //    File.WriteAllText(appPath + @"\targetwk.txt", Path.GetDirectoryName(targetpg));
            //}
            //catch(Exception e)
            //{
            //    File.WriteAllText(@"C:\work\LETSUpdater.log", e.Message);
            //}

            ////  msiexecプロセスが終了する(減る)のを待って、画面表示
            //System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName("msiexec");
            //int initCnt = processes.Length;
            //int nowCnt = initCnt;
            //do
            //{
            //    processes = System.Diagnostics.Process.GetProcessesByName("msiexec");
            //    nowCnt = processes.Length;
            //} while (processes != null && processes.Length > 0 && nowCnt >= initCnt);

            app.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
            app.InitializeComponent();
            app.Run();
        }
    }
}

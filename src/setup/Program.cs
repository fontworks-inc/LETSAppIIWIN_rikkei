using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace setup
{
    class Program
    {
        static void Main(string[] args)
        {
            //var os = Environment.OSVersion;
            //if (os.Version.Major < 10)
            //{
            //    System.Windows.Forms.MessageBox.Show("Windows10 未満の OS では LETS をご利用できません。");
            //    return;
            //}

            // ホームドライブの取得
            string homedrive = Environment.GetEnvironmentVariable("HOMEDRIVE");

            // ショートカットにバッチがあるか確認
            // LETSアプリの起動(ショートカットを実行する)
            string uninstshortcut = $@"{homedrive}\ProgramData\Microsoft\Windows\Start Menu\Programs\StartUp\uninstallfonts.bat";
            if (File.Exists(uninstshortcut))
            {
                System.Windows.Forms.MessageBox.Show("アンインストール処理が完了していません。");
                return;
            }

            // 実行ファイルパスの取得(圧縮フォルダ内でも取得できる)
            System.Reflection.Assembly executionAsm = System.Reflection.Assembly.GetExecutingAssembly();
            string actualPath = System.IO.Path.GetDirectoryName(executionAsm.Location);

            // インストーラ開始日時を取得する
            DateTime inststt = DateTime.Now;

            // インストーラの起動
            string installer = actualPath + @"\LETS-Installer.msi";
            Process p1 = Process.Start(installer);
            p1.WaitForExit();

            // LETSアプリの起動(ショートカットを実行する)
            string shortcut = $@"{homedrive}\ProgramData\Microsoft\Windows\Start Menu\Programs\StartUp\LETS デスクトップアプリ.lnk";

            // ショートカットの作成日時を確認する
            DateTime dtCreate = File.GetCreationTime(shortcut);

            // ショートカット作成日時がインストーラ起動日時より後ならば、起動を行う
            if (inststt.CompareTo(dtCreate) <= 0)
            {
                Process p2 = Process.Start(shortcut);

                // チュートリアル画面の起動
                Thread.Sleep(5000); // アプリの起動待ち
                string updator = $@"{homedrive}\ProgramData\Fontworks\LETS\LETSUpdater.exe";
                Process p3 = Process.Start(updator);
            }
        }
    }
}

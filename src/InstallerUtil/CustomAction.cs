using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace InstallerUtil
{
    [System.ComponentModel.RunInstaller(true)]

    public class CustomAction : System.Configuration.Install.Installer
    {
        public CustomAction() : base()
        {
            BeforeUninstall += CustomAction_BeforeUninstall;
            AfterUninstall += CustomAction_AfterUninstall;
        }

        private void CustomAction_AfterUninstall(object sender, System.Configuration.Install.InstallEventArgs e)
        {
            // 親フォームを作成
            using (Form f = new Form())
            {
                f.TopMost = true; // 親フォームを常に最前面に表示する
                f.Activate();
                                  // 作成したフォームを親フォームとしてメッセージボックスに設定
                MessageBox.Show(f, "アンインストールを完了するためには、PCの再起動が必要です。\n編集中のファイルがある場合は保存したのち、再起動を実施してください。");
                f.TopMost = false;
            }
        }

        private void CustomAction_BeforeUninstall(object sender, System.Configuration.Install.InstallEventArgs e)
        {
            // LETSアプリを終了させる
            ExitLETS();

            // ホームドライブの取得
            string homedrive = Environment.GetEnvironmentVariable("HOMEDRIVE");

            // LETSフォルダ
            string letsfolder = $@"{homedrive}\ProgramData\Fontworks\LETS";

            // uninstallfontバッチファイル名
            string uninstallbat = "uninstallfonts.bat";
            string uninstallfonts = $@"{letsfolder}\{uninstallbat}";

            // ユーザ情報クリアバッチ名
            string clearinfobat = $@"{letsfolder}\clearuserdata.bat";

            try
            {
                // フォント削除バッチの登録
                if (File.Exists(uninstallfonts))
                {
                    // バッチファイルが存在したらショートカットへ登録
                    string shortcutfolder = $@"{homedrive}\ProgramData\Microsoft\Windows\Start Menu\Programs\StartUp";
                    string destfile = $@"{shortcutfolder}\{uninstallbat}";
                    if (File.Exists(destfile))
                    {
                        File.Delete(destfile);
                    }
                    File.Move($@"{letsfolder}\{uninstallbat}", destfile);
                }

                // ユーザ情報削除バッチの実行
                if (File.Exists(clearinfobat))
                {
                    // バッチファイルが存在したら実行
                    Process.Start(clearinfobat);
                }
            }
            catch(Exception ex)
            {
                File.WriteAllText($@"{letsfolder}\Uninstall.log", ex.Message);
            }

        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);
        }

        public override void Commit(System.Collections.IDictionary stateSaver)
        {
            base.Commit(stateSaver);
        }

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            base.Uninstall(savedState);

            // ホームドライブの取得
            string homedrive = Environment.GetEnvironmentVariable("HOMEDRIVE");

            // LETSフォルダ
            string letsfolder = $@"{homedrive}\ProgramData\Fontworks\LETS";

            try
            {
                // configフォルダの削除
                Delete($@"{letsfolder}\config");

                // LETS-Ver*フォルダを削除
                string[] verFolders = Directory.GetDirectories(letsfolder, "LETS-Ver*");
                foreach (string f in verFolders)
                {
                    Delete(f);
                }
            }
            catch(Exception ex)
            {
                File.WriteAllText($@"{letsfolder}\Uninst.log", ex.Message);
            }
        }

        /// <summary>
        /// 指定したディレクトリとその中身を全て削除する
        /// </summary>
        public static void Delete(string targetDirectoryPath)
        {
            if (!Directory.Exists(targetDirectoryPath))
            {
                return;
            }

            //ディレクトリ以外の全ファイルを削除
            string[] filePaths = Directory.GetFiles(targetDirectoryPath);
            foreach (string filePath in filePaths)
            {
                try
                {
                    File.SetAttributes(filePath, FileAttributes.Normal);
                    File.Delete(filePath);
                }
                catch(Exception)
                {
                    // 消せないファイルは無視する
                }
            }

            //ディレクトリの中のディレクトリも再帰的に削除
            string[] directoryPaths = Directory.GetDirectories(targetDirectoryPath);
            foreach (string directoryPath in directoryPaths)
            {
                Delete(directoryPath);
            }

            //中が空になったらディレクトリ自身も削除
            try
            {
                Directory.Delete(targetDirectoryPath, false);
            }
            catch(Exception)
            {
                // 消せないフォルダも無視する
            }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public extern static bool EnumWindows(EnumWindowsDelegate lpEnumFunc,
            IntPtr lparam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd,
            StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetClassName(IntPtr hWnd,
            StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        public void ExitLETS()
        {
            EnumWindows(new EnumWindowsDelegate(EnumWindowCallBack), IntPtr.Zero);
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, int fuFlags, int uTimeout, IntPtr lpdwResult);

        public delegate bool EnumWindowsDelegate(IntPtr hWnd, IntPtr lparam);
        private static bool EnumWindowCallBack(IntPtr hWnd, IntPtr lparam)
        {
            //ウィンドウのタイトルの長さを取得する
            int textLen = GetWindowTextLength(hWnd);
            if (0 < textLen)
            {
                //ウィンドウのタイトルを取得する
                StringBuilder tsb = new StringBuilder(textLen + 1);
                GetWindowText(hWnd, tsb, tsb.Capacity);

                // プロセスIDからプロセス名を取得する
                int pid;
                GetWindowThreadProcessId(hWnd, out pid);
                Process p = Process.GetProcessById(pid);
                string procname = p.ProcessName;

                //ウィンドウのタイトルが"LETS"ならば終了メッセージを送信する
                if (tsb.ToString() == "LETS" && procname == "LETS")
                {
                    SendMessageTimeout(hWnd, (uint)0x8001, IntPtr.Zero, new IntPtr(3), 0x2, 30*1000, IntPtr.Zero);
                }
            }

            //すべてのウィンドウを列挙する
            return true;
        }

    }
}

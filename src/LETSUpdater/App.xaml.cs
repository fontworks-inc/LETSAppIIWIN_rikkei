using System;
using System.IO;
using System.IO.Compression;
using System.Windows;

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
            if (args.Length > 0)
            {
                try
                {
                    //  プログラムアップデートロジックを実行して終了
                    if (args.Length < 2)
                    {
                        Environment.Exit(0);
                    }

                    string updateVersion = args[0];
                    string runVersion = args[1];

                    updateLETS(updateVersion, runVersion);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
                Environment.Exit(0);
            }

            app.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
            app.InitializeComponent();
            app.Run();
            WindowHelper.LoginLETS();
        }

        private static void updateLETS(string updateVersion, string runVersion)
        {
            // ホームドライブの取得
            string homedrive = Environment.GetEnvironmentVariable("HOMEDRIVE");

            // LETSプログラムフォルダの取得
            string letsFolder = $@"{homedrive}\ProgramData\Fontworks\LETS";
            string programFolder = $@"{letsFolder}\LETS-Ver{updateVersion}";
            string programExePath = $@"{letsFolder}\LETS-Ver{updateVersion}\LETS.exe";

            string updateZip = $@"{programFolder}\LETS-Ver{updateVersion}.zip";

            // ショートカットパスの取得
            string shortcut = $@"{homedrive}\ProgramData\Microsoft\Windows\Start Menu\Programs\StartUp\LETS デスクトップアプリ.lnk";

            // 1.管理者権限で実行されていることをチェックする
            // →管理者権限ではない場合、エラーメッセージを表示する
            //　「管理者権限でアップデートを行ってください」
            // 2.  [共通保存：更新プログラム情報.ダウンロード状態]が「ダウンロード完了」であることを確認する
            // 3.実行中のLETSプログラムを検索する
            // ・以下のプロセスを検索する
            //　・名前が”LETS”
            //　・クラス名に”LETS.exe”を含む
            // 4.実行中LETSプログラムへ進捗メッセージを送信する
            // ・進捗率：10 %
            WindowHelper.UpdateProgressLETS(10);

            //// 5.バージョン別プログラムフォルダパスを生成する
            //Directory.CreateDirectory(programFolder);

            // 6.バージョン別プログラムフォルダパスにダウンロードされているzipファイルを展開する
            UnzipProgram(updateZip, programFolder);

            // 7.実行中LETSプログラムへ進捗メッセージを送信する
            // ・進捗率：80 %
            WindowHelper.UpdateProgressLETS(80);

            // 8.展開したプログラムの実行ファイルをスタートアップに登録されているショートカットに設定する
            // 元のショートカットを削除する
            if (System.IO.File.Exists(shortcut))
            {
                System.IO.File.Delete(shortcut);
            }
            CreateShortcut(shortcut, programExePath, programFolder);

            // 9.実行中LETSプログラムへ進捗メッセージを送信する
            // ・進捗率：100 %
            WindowHelper.UpdateProgressLETS(100);

            // 10.実行中のLETSプログラムへ終了メッセージを送信する
            // ・終了メッセージ(ディアクティベートなし)を送信する
            // ・更新したバージョンのプログラムを起動する
            WindowHelper.ExitLETS();

            // 11.過去バージョンのプログラムを削除する
            // ・更新バージョンより２つ以上古いプログラムフォルダがある場合、削除する
            // LETS-Ver*フォルダを削除
            string[] verFolders = Directory.GetDirectories(letsFolder, "LETS-Ver*");
            if (verFolders.Length >= 3)
            {
                string runFolder = $@"{letsFolder}\LETS-Ver{runVersion}";
                Array.Sort(verFolders, new VerCompare());
                for (int i = 2; i < verFolders.Length; i++)
                {
                    // ・起動指定バージョンのフォルダは削除しない           
                    if (!verFolders[i].Equals(runFolder))
                    {
                        Delete(verFolders[i]);
                    }
                }
            }

        }

        public class VerCompare : System.Collections.IComparer
        {
            public int Compare(object x, object y)
            {
                if (x == null && y == null)
                {
                    return 0;
                }
                if (x == null)
                {
                    return 1;
                }
                if (y == null)
                {
                    return -1;
                }

                string homedrive = Environment.GetEnvironmentVariable("HOMEDRIVE");
                string letsFolder = $@"{homedrive}\ProgramData\Fontworks\LETS";
                string verFolderPrefix = $@"{letsFolder}\LETS-Ver";
                int lenPrefix = verFolderPrefix.Length;

                string xver = x.ToString();
                string yver = y.ToString();
                xver = xver.Substring(lenPrefix);
                yver = yver.Substring(lenPrefix);
                string[] xvers = xver.Split('.');
                string[] yvers = yver.Split('.');

                for (int i = 0; i < 3; i++)
                {
                    int xvernum = 0;
                    int yvernum = 0;
                    if (xvers.Length > i)
                    {
                        xvernum = int.Parse(xvers[i]);
                    }
                    if (yvers.Length > i)
                    {
                        yvernum = int.Parse(yvers[i]);
                    }

                    if (xvernum != yvernum)
                    {
                        return yvernum - xvernum;
                    }
                }

                return 0;
            }
        }

        /// <summary>
        /// 指定したディレクトリとその中身を全て削除する
        /// </summary>
        private static void Delete(string targetDirectoryPath)
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
                    System.IO.File.SetAttributes(filePath, FileAttributes.Normal);
                    System.IO.File.Delete(filePath);
                }
                catch (Exception)
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
            catch (Exception)
            {
                // 消せないフォルダも無視する
            }
        }

        private static void UnzipProgram(string ZipPath, string targetfolder)
        {
            //ディレクトリ以外の全ファイルを削除
            string[] filePaths = Directory.GetFiles(targetfolder);
            foreach (string filePath in filePaths)
            {
                try
                {
                    if (!filePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                    {
                        System.IO.File.SetAttributes(filePath, FileAttributes.Normal);
                        System.IO.File.Delete(filePath);
                    }
                }
                catch (Exception)
                {
                    // 消せないファイルは無視する
                }
            }

            // ディレクトリを削除する
            string[] directoryPaths = Directory.GetDirectories(targetfolder);
            foreach (string directoryPath in directoryPaths)
            {
                Delete(directoryPath);
            }

            // Zipファイルを展開する
            ZipFile.ExtractToDirectory(ZipPath, targetfolder);
        }

        private static void CreateShortcut(string shortcutPath, string targetPath, string wkfolder)
        {
            //string shortcutPath = System.IO.Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory), @"MyApp.lnk");
            // ショートカットのリンク先(起動するプログラムのパス)
            //string targetPath = Application.ExecutablePath;

            // WshShellを作成
            IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
            // ショートカットのパスを指定して、WshShortcutを作成
            string wkShortcut = $@"{wkfolder}\LETS デスクトップアプリ.lnk";
            //IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutPath);
            IWshRuntimeLibrary.IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(wkShortcut);
            // ①リンク先
            shortcut.TargetPath = targetPath;
            //// ②引数
            //shortcut.Arguments = "/a /b /c";
            // ③作業フォルダ
            shortcut.WorkingDirectory = wkfolder;
            // ④実行時の大きさ 1が通常、3が最大化、7が最小化
            shortcut.WindowStyle = 1;
            // ⑤コメント
            shortcut.Description = "LETS デスクトップアプリ";
            // ⑥アイコンのパス 自分のEXEファイルのインデックス0のアイコン
            shortcut.IconLocation = targetPath + ",0";

            // ショートカットを作成
            shortcut.Save();

            // ショートカットをスタートアップフォルダへ移動
            try
            {
                System.IO.File.Move(wkShortcut, shortcutPath);
            }
            catch (Exception)
            {
                //
            }

            // 後始末
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(shortcut);
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(shell);
        }
    }
}

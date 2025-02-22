﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

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
            if(IsRunFromSetup())
            {
                return;
            }

            using (UninstallConfirm f = new UninstallConfirm())
            {
                f.ShowDialog();
            }
        }

        private void CustomAction_BeforeUninstall(object sender, System.Configuration.Install.InstallEventArgs e)
        {
            // LETSアプリを終了させる
            ExitLETS();

            // ホームドライブの取得
            string winDir = System.Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            string homedrive = winDir.Substring(0, winDir.IndexOf("\\"));

            // LETSフォルダ
            //string letsfolder = $@"{homedrive}\ProgramData\Fontworks\LETS";
            string programdataFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string letsfolder = $@"{programdataFolder}\Fontworks\LETS";

            if (IsRunFromSetup())
            {
                return;
            }

            // ログアウト実行
            try
            {
                //string updator = $@"{homedrive}\ProgramData\Fontworks\LETS\LETSUpdater.exe";
                string updator = $@"{programdataFolder}\Fontworks\LETS\LETSUpdater.exe";
                Process upd = Process.Start(updator, "Logout");
                upd.WaitForExit();
            }
            catch(Exception)
            {
                // NOP
            }

            // uninstallfontバッチファイル名
            string uninstallbat = "uninstallfonts.bat";
            string uninstallfontsbat = $@"{letsfolder}\{uninstallbat}";
            //string shortcutfolder = $@"{homedrive}\ProgramData\Microsoft\Windows\Start Menu\Programs\StartUp";
            string shortcutfolder = $@"{programdataFolder}\Microsoft\Windows\Start Menu\Programs\StartUp";
            string destfile = $@"{shortcutfolder}\{uninstallbat}";

            try
            {
                string[] pids = this.GetUserregIDs();

                // ユーザ名の取得
                string[] usernames = this.GetUsers(letsfolder);
                if(usernames != null)
                {
                    List<string> pidlist = new List<string>(pids);
                    foreach (string username in usernames)
                    {
                        pidlist.Add(username);
                    }
                    pids = pidlist.ToArray();
                }

                if (pids != null)
                {
                    // フォント削除バッチの実行
                    string[] dellines = this.UninstallFontsAllUser(letsfolder, pids);
                    File.WriteAllText(uninstallfontsbat, "REM フォントファイル削除" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
                    File.AppendAllText(uninstallfontsbat, "@ECHO OFF" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));

                    if (dellines.Length > 0)
                    {
                        writeBatRunas(uninstallfontsbat);
                    }

                    foreach (string l in dellines)
                    {
                        File.AppendAllText(uninstallfontsbat, l + Environment.NewLine);
                    }
                    File.AppendAllText(uninstallfontsbat, @"Del /F ""%~dp0%~nx0""" + "\n");
                    if (File.Exists(destfile))
                    {
                        File.Delete(destfile);
                    }
                    File.Move($@"{uninstallfontsbat}", destfile);

                    // レジストリ削除バッチの実行
                    this.UnregistAllUser(letsfolder, pids);

                    // ユーザ情報削除バッチの実行
                    this.ClearAllUserInfo(letsfolder, pids);
                }
                else
                {
                    File.WriteAllText(uninstallfontsbat, "REM フォントファイル削除" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
                    File.AppendAllText(uninstallfontsbat, "@ECHO OFF" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));

                    //writeBatRunas(uninstallfontsbat);

                    File.AppendAllText(uninstallfontsbat, @"Del /F ""%~dp0%~nx0""" + "\n");
                    if (File.Exists(destfile))
                    {
                        File.Delete(destfile);
                    }
                    File.Move($@"{uninstallfontsbat}", destfile);
                }
            }
            catch (Exception)
            {
                // NOP
            }

            //デバイス情報ファイルの削除
            DeleteDevInfo($@"{letsfolder}\config");
        }

        private void    writeBatRunas(string fnm)
        {
            //File.AppendAllText(fnm, "cd /d %~dp0" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
            //File.AppendAllText(fnm, "whoami /priv | find \"SeDebugPrivilege\" > nul" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
            //File.AppendAllText(fnm, "if %errorlevel% neq 0 (" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
            //File.AppendAllText(fnm, "      @powershell start-process %~0 -verb runas" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
            //File.AppendAllText(fnm, "      exit" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
            //File.AppendAllText(fnm, "  )" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
        }

        private string[] UninstallFontsAllUser(string letsfolder, string[] pids)
        {
            List<string> alldellines = new List<string>();

            foreach (string pid in pids)
            {
                string uninstfontbat = Path.Combine(letsfolder, $"uninstallfonts_{pid}.bat");
                if (File.Exists(uninstfontbat))
                {
                    this.SetHidden(uninstfontbat, false);
                    string[] lines = File.ReadAllLines(uninstfontbat);
                    foreach (string l in lines)
                    {
                        if (l.StartsWith("DEL "))
                        {
                            alldellines.Add(l);
                        }
                    }
                    Process.Start(new ProcessStartInfo(uninstfontbat) { CreateNoWindow = true, UseShellExecute = false });
                }
            }
            {
                string uninstfontbat = Path.Combine(letsfolder, "uninstallfonts_device.bat");
                if (File.Exists(uninstfontbat))
                {
                    this.SetHidden(uninstfontbat, false);
                    string[] lines = File.ReadAllLines(uninstfontbat);
                    foreach (string l in lines)
                    {
                        if (l.StartsWith("DEL "))
                        {
                            alldellines.Add(l);
                        }
                    }
                    Process.Start(new ProcessStartInfo(uninstfontbat) { CreateNoWindow = true, UseShellExecute = false });
                }
            }

            return alldellines.ToArray();
        }

        private string[] GetUsers(string letsfolder)
        {
            string userlist = Path.Combine(letsfolder, "userlist.txt");
            string[] users = null;
            if (File.Exists(userlist))
            {
                this.SetHidden(userlist, false);
                users = File.ReadAllLines(userlist);
                File.Delete(userlist);
            }
            else
            {
                return users;
            }

            return users;
        }

        private void UnregistAllUser(string letsfolder, string[] pids)
        {
            foreach (string pid in pids)
            {
                string uninstregbat = Path.Combine(letsfolder, $"uninstreg_{pid}.bat");
                if (File.Exists(uninstregbat))
                {
                    this.SetHidden(uninstregbat, false);
                    Process.Start(new ProcessStartInfo(uninstregbat) { CreateNoWindow = true, UseShellExecute = false });
                }
            }
            {
                string uninstregbat = Path.Combine(letsfolder, "uninstreg_device.bat");
                if (File.Exists(uninstregbat))
                {
                    this.SetHidden(uninstregbat, false);
                    Process.Start(new ProcessStartInfo(uninstregbat) { CreateNoWindow = true, UseShellExecute = false });
                }
            }
        }
        private void ClearAllUserInfo(string letsfolder, string[] pids)
        {
            foreach (string pid in pids)
            {
                string clearuserbat = Path.Combine(letsfolder, $"clearuserdata_{pid}.bat");
                if (File.Exists(clearuserbat))
                {
                    this.SetHidden(clearuserbat, false);
                    Process.Start(new ProcessStartInfo(clearuserbat) { CreateNoWindow = true, UseShellExecute = false });
                }
            }
        }

        private string[] GetUserregIDs()
        {
            try
            {
                string regpath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList";
                var regkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(regpath);
                string[] subkeys = regkey.GetSubKeyNames();
                regkey.Close();

                return subkeys;
            }
            catch (Exception)
            {
                // NOP
            }

            return null;
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
            string winDir = System.Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            string homedrive = winDir.Substring(0, winDir.IndexOf("\\"));

            // LETSフォルダ
            //string letsfolder = $@"{homedrive}\ProgramData\Fontworks\LETS";
            string programdataFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string letsfolder = $@"{programdataFolder}\Fontworks\LETS";

            try
            {
                //// configフォルダの削除
                //Delete($@"{letsfolder}\config");

                ////デバイス情報ファイルの削除
                //DeleteDevInfo($@"{letsfolder}\config");

                // LETS-Ver*フォルダを削除
                string[] verFolders = Directory.GetDirectories(letsfolder, "LETS-Ver*");
                foreach (string f in verFolders)
                {
                    Delete(f);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    File.WriteAllText($@"{letsfolder}\Uninst.log", ex.Message);
                }
                catch(Exception)
                {
                    // NOP
                }
            }
        }

        private void DeleteDevInfo(string configFolder)
        {
            // 指定フォルダ下のデバイス情報ファイルを削除
            string[] filePaths = Directory.GetFiles(configFolder, "dev-*.*");
            foreach (string filePath in filePaths)
            {
                try
                {
                    File.SetAttributes(filePath, FileAttributes.Normal);
                    File.Delete(filePath);
                }
                catch (Exception)
                {
                    // 消せないファイルは無視する
                }
            }
        }

        private void SetHidden(string filepath, bool isHidden)
        {
            FileAttributes fa = System.IO.File.GetAttributes(filepath);
            if (isHidden)
            {
                fa = fa | FileAttributes.Hidden;
            }
            else
            {
                fa = fa & ~FileAttributes.Hidden;
            }

            System.IO.File.SetAttributes(filepath, fa);
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

        private static bool IsRunFromSetup()
        {
            {
                // アンインストール時のユーザ一時フォルダ
                string tempPath = Path.GetTempPath();
                string logpath = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), ".letsuninst.log");
                File.AppendAllText(logpath, $"tempPath={tempPath}" + "\n");
            }


            // ホームドライブの取得
            string winDir = System.Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            string homedrive = winDir.Substring(0, winDir.IndexOf("\\"));

            // Setupプログラムから実行していることを示す一時ファイル
            //string tmpfile = $@"{homedrive}\ProgramData\.runfromletssetup";
            string programdataFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string tmpfile = $@"{programdataFolder}\.runfromletssetup";

            if (File.Exists(tmpfile))
            {
                return true;
            }
            return false;
        }

        [DllImport("gdi32.dll", EntryPoint = "RemoveFontResourceW", SetLastError = true)]
        private static extern int RemoveFontResource([In][MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

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

        [DllImport("user32.dll")]
        private static extern int PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

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
                string procname = string.Empty;
                try
                {
                    int pid;
                    GetWindowThreadProcessId(hWnd, out pid);
                    Process p = Process.GetProcessById(pid);
                    procname = p.ProcessName;
                }
                catch (Exception)
                {
                    // NOP
                }

                //ウィンドウのタイトルが"LETS"ならば終了メッセージを送信する
                if (tsb.ToString() == "LETS" && (procname == "LETS" || string.IsNullOrEmpty(procname)))
                {
                    if (IsRunFromSetup())
                    {
                        // 終了メッセージ(ディアクティベートなし)
                        //SendMessageTimeout(hWnd, (uint)0x8001, IntPtr.Zero, new IntPtr(4), 0x2, 30 * 1000, IntPtr.Zero);
                        PostMessage(hWnd, (uint)0x8001, 0, 4);
                    }
                    else
                    {
                        // 終了メッセージ(ディアクティベートあり)
                        //SendMessageTimeout(hWnd, (uint)0x8001, IntPtr.Zero, new IntPtr(3), 0x2, 30 * 1000, IntPtr.Zero);
                        PostMessage(hWnd, (uint)0x8001, 0, 3);
                    }
                }
            }

            //すべてのウィンドウを列挙する
            return true;
        }

    }
}

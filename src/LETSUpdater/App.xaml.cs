using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
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
                    if (args.Length == 1)
                    {
                        // LETSログアウト処理を行って終了
                        DebugLog("LETSUpdater:Before logoutLETS");
                        LogoutLETS();
                        DebugLog("LETSUpdater:After logoutLETS");
                        Environment.Exit(0);
                    }

                    //========================================
                    // デバイスモードフォント操作対応
                    string param0 = args[0];
                    string param1 = args[1];
                    if (param0 == "FontInstall")
                    {
                        FontInstallFromList(param1);
                        Environment.Exit(0);
                    }

                    if (param0 == "FontUninstall")
                    {
                        FontUninstallFromList(param1);
                        Environment.Exit(0);
                    }
                    //========================================

                    updateLETS(param0, param1);
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
            bool isCpmpletelyOffline = false;
#if COMPLETELY_OFFLINE
            isCpmpletelyOffline = true; //完全オフラインインストールのとき。
#endif
            WindowHelper.LoginLETS(isCpmpletelyOffline);
        }

        private static void FontInstallFromList(string tempPath)
        {
            try
            {
                // インストールフォント一覧ファイルから情報を取得
                string installFontListFile = Path.Combine(tempPath, "InstallFontInfo.lst");
                string installResultListFile = Path.Combine(tempPath, "InstallFontInfoResult.lst");
                string[] installFonts = File.ReadAllLines(installFontListFile, Encoding.GetEncoding("shift_jis"));
                foreach (string installFont in installFonts)
                {
                    string[] installFontInfo = installFont.Split('\t');
                    string fontPath = installFontInfo[0];
                    string uniqName = installFontInfo[1];
                    string letsKind = installFontInfo[2];
                    List<string> resultList = FontInstall(fontPath, uniqName, letsKind);

                    string resultLine = string.Join("\t", resultList);
                    File.AppendAllText(installResultListFile, resultLine + Environment.NewLine, Encoding.GetEncoding("shift_jis"));
                }
            }
            catch (Exception ex)
            {
                DebugLog(ex.StackTrace);
            }
        }

        private static void FontUninstallFromList(string tempPath)
        {
            try
            {
                // インストールフォント一覧ファイルから情報を取得
                string uninstallFontListFile = Path.Combine(tempPath, "UninstallFontInfo.lst");
                string uninstallResultListFile = Path.Combine(tempPath, "UninstallFontInfoResult.lst");
                string[] uninstallFonts = File.ReadAllLines(uninstallFontListFile, Encoding.GetEncoding("shift_jis"));
                foreach (string uninstallFont in uninstallFonts)
                {
                    string[] uninstallFontInfo = uninstallFont.Split('\t');
                    string fontPath = uninstallFontInfo[0];
                    string uniqName = uninstallFontInfo[1];
                    string letsKind = uninstallFontInfo[2];
                    string opeKind = uninstallFontInfo[3];
                    List<string> resultList = FontUninstall(fontPath, uniqName, letsKind, opeKind);

                    string resultLine = string.Join("\t", resultList);
                    File.AppendAllText(uninstallResultListFile, resultLine + Environment.NewLine, Encoding.GetEncoding("shift_jis"));
                }
            }
            catch (Exception ex)
            {
                DebugLog(ex.StackTrace);
            }
        }

        private static List<string> FontUninstall(string filePath, string regkey, string letsKind, string opeKind)
        {
            List<string> resultInfo = new List<string>();
            resultInfo.Add(filePath);
            resultInfo.Add(regkey);
            resultInfo.Add(letsKind);

            if (!string.IsNullOrEmpty(regkey))
            {
                ReleaseRegistry(regkey);
            }

            var result = RemoveFontResource(filePath);

            if (opeKind == "Deactivate")
            {
                // Deactivateだけのときはここまで
                resultInfo.Add("OK");
                return resultInfo;
            }

            // フォントファイルの削除を試みる
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    resultInfo.Add("OK");  // ファイルが削除出来た場合
                }
            }
            catch (UnauthorizedAccessException)
            {
                resultInfo.Add("NG");  // ファイルが削除できなかった場合
            }
            catch (Exception ex)
            {
                DebugLog("FontUninstall:" + ex.StackTrace);
                resultInfo.Add("ERR");  // 想定外のエラーが起きた場合
                resultInfo.Add(ex.Message);
            }

            return resultInfo;
        }

        private static List<string> FontInstall(string fontPath, string uniqName, string letsKind)
        {
            List<string> resultInfo = new List<string>();
            resultInfo.Add(fontPath);
            resultInfo.Add(uniqName);
            try
            {
                // インストール先パスを取得
                string appPath = AppDomain.CurrentDomain.BaseDirectory;
                string homedrive = appPath.Substring(0, appPath.IndexOf("\\"));
                string sysFontFolder = $@"{homedrive}\Windows\Fonts";

                // フォントファイル名を取得
                string fontFileName = System.IO.Path.GetFileName(fontPath);

                // インストールファイルのフルパスを作成
                string targetFullPath = System.IO.Path.Combine(sysFontFolder, fontFileName);

                // 既にファイルがあるか確認
                if (File.Exists(targetFullPath))
                {
                    // ログを出力して、スキップ
                    DebugLog($"同名フォントが存在したため、インストールを中断しました。:{uniqName}");
                    resultInfo.Add("NG");
                    resultInfo.Add($"同名フォントが存在したため、インストールを中断しました。:{uniqName}");
                    return resultInfo;
                }

                try
                {
                    File.Copy(fontPath, targetFullPath);

                    string regKey = GetFontName(targetFullPath, uniqName);

                    AddRegistry(regKey, targetFullPath);

                    AddFontResource(targetFullPath);

                    resultInfo.Add("OK");
                    resultInfo.Add(targetFullPath);
                    resultInfo.Add(regKey);
                    resultInfo.Add(letsKind);
                }
                catch (Exception ex)
                {
                    DebugLog(ex.StackTrace);
                    resultInfo.Add("ERR");
                    resultInfo.Add(ex.Message);
                    return null;
                }

                return resultInfo;
            }
            catch (Exception ex)
            {
                DebugLog(ex.StackTrace);
                resultInfo.Add("ERR");
                resultInfo.Add(ex.Message);
                return resultInfo;
            }
        }

        /// <summary>
        /// フォント名取得
        /// </summary>
        private static string GetFontName(string fontPath, string fontname)
        {
            try
            {
                // 識別子確認のDLLを介し、情報を取得する
                string filepath = fontPath;
                if (File.Exists(filepath))
                {
                    string ext = Path.GetExtension(fontPath).ToLower();
                    if (ext.CompareTo(".ttc") == 0)
                    {
                        fontname += "(TTC)";
                    }

                    return fontname;
                }
            }
            catch (Exception ex)
            {
                DebugLog(ex.StackTrace);
            }

            return string.Empty;
        }

        /// <summary>
        /// Registryのフォントパス
        /// </summary>
        private static readonly string registryFontsPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts";

        /// <summary>
        /// レジストリに登録する
        /// </summary>
        /// <param name="key">キー：フォント名</param>
        /// <param name="value">値：フォントファイルパス</param>
        private static void AddRegistry(string key, string value)
        {
            var regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(registryFontsPath, true);

            try
            {
                // 同じフォントパスに対して別のレジストリがあれば削除する
                string[] keys = regkey.GetValueNames();
                foreach (string k in keys)
                {
                    string v = (string)regkey.GetValue(k);
                    if (!string.IsNullOrEmpty(v) && v.Equals(value))
                    {
                        if (!k.Equals(key))
                        {
                            regkey.DeleteValue(k);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLog(ex.StackTrace);
            }

            regkey.SetValue(key, value);
            regkey.Close();
        }

        /// <summary>
        /// レジストリから除外する
        /// </summary>
        /// <param name="key">キー：フォント名</param>
        private static void ReleaseRegistry(string key)
        {
            try
            {
                var regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(registryFontsPath, true);
                regkey.DeleteValue(key);
            }
            catch (Exception)
            {
                // 無視
            }
        }

        /// <summary>
        /// フォント追加
        /// </summary>
        /// <param name="lpFileName">フォント名称</param>
        /// <returns>成功時：追加されたフォントの数、失敗時：0</returns>
        [DllImport("gdi32.dll", EntryPoint = "AddFontResourceW", SetLastError = true)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static elements should appear before instance elements", Justification = "<保留中>")]
        private static extern int AddFontResource([In][MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

        /// <summary>
        /// フォント削除
        /// </summary>
        /// <param name="lpFileName">フォント名称</param>
        /// <returns>成功時：0以外、失敗時：0</returns>
        [DllImport("gdi32.dll", EntryPoint = "RemoveFontResourceW", SetLastError = true)]
        private static extern int RemoveFontResource([In][MarshalAs(UnmanagedType.LPWStr)] string lpFileName);

        private static void updateLETS(string updateVersion, string runVersion)
        {
            // ホームドライブの取得
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string homedrive = appPath.Substring(0, appPath.IndexOf("\\"));

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

        private static void DebugLog(string msg)
        {
#if DEBUGLOG
            try
            {
                string logpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Fontworks", "LETS", "config", "logout.log");
                File.AppendAllText(logpath, msg + "\n");
            }
            catch (Exception)
            {
                // NOP
            }

#endif
        }

        private static void LogoutLETS()
        {
            try
            {
                // アプリケーション設定フォルダパス
                string letsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Fontworks", "LETS");

                // ログアウト情報ファイルを取得する
                string[] logoutinfos = Directory.GetFiles(letsFolder, "logoutinfo_*.dat");
                List<StatusDat> statusDats = new List<StatusDat>();
                foreach (string logoutinfo in logoutinfos)
                {
                    DebugLog("logoutinfo=" + logoutinfo);
                    // 内容を復号化する
                    DecryptFile decryptFile = new DecryptFile();
                    string decryptText = string.Empty;
                    try
                    {
                        decryptText = decryptFile.ReadAll(logoutinfo);
                        DebugLog("decryptText=" + decryptText);
                        var statusDat = new StatusDat();
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(decryptText));
                        var ser = new DataContractJsonSerializer(statusDat.GetType());
                        statusDat = ser.ReadObject(ms) as StatusDat;
                        if (statusDat.IsLoggingIn)
                        {
                            statusDats.Add(statusDat);
                        }

                        File.Delete(logoutinfo);
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }

                if(statusDats.Count <= 0)
                {
                    return;
                }

                // サーバURLの取得
                string letsConfigFolder = Path.Combine(letsFolder, "config");

                string baseurl = "https://delivery-lets.fontworks.co.jp";
                string appsettingpath = Path.Combine(letsConfigFolder, "appsettings.json");
                DebugLog("appsettingpath=" + appsettingpath);
                if (File.Exists(appsettingpath))
                {
                    try
                    {
                        string appsettingtext = File.ReadAllText(appsettingpath);
                        var appSettig = new AppSetting();
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(appsettingtext));
                        var ser = new DataContractJsonSerializer(appSettig.GetType());
                        appSettig = ser.ReadObject(ms) as AppSetting;
                        baseurl = appSettig.FontDeliveryServerUri;
                    }
                    catch (Exception)
                    {
                        return;
                    }

                }
                DebugLog("baseurl=" + baseurl);

                // プロキシ認証情報の取得
                string proxyauthpath = Path.Combine(letsConfigFolder, "puroxyauth.json");
                ProxyAuthSetting proxyAuthSetting = null;
                if (File.Exists(proxyauthpath))
                {
                    try
                    {
                        string proxyauthtext = File.ReadAllText(proxyauthpath);
                        proxyAuthSetting = new ProxyAuthSetting();
                        var ms = new MemoryStream(Encoding.UTF8.GetBytes(proxyauthtext));
                        var ser = new DataContractJsonSerializer(proxyauthtext.GetType());
                        proxyAuthSetting = ser.ReadObject(ms) as ProxyAuthSetting;
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }
                IWebProxy proxyserver = GetWebProxy(baseurl, proxyAuthSetting);

                Logout(baseurl, statusDats, proxyAuthSetting);
            }
            catch (Exception)
            {
                // NOP
            }
        }

        static void Logout(string serverbase, List<StatusDat> statusDats, ProxyAuthSetting proxyAuthSetting)
        {
            try
            {
                HttpClientHandler ch = new HttpClientHandler();
                IWebProxy proxyserver = GetWebProxy(serverbase, proxyAuthSetting);
                if(proxyserver != null)
                {
                    ch.Proxy = proxyserver;
                }

                using (HttpClient client = new HttpClient(ch))
                {
                    foreach (StatusDat statusDat in statusDats)
                    {
                        var refreshbody = $@"{{""refreshToken"":""{statusDat.RefreshToken}""}}";

                        var request = new HttpRequestMessage(HttpMethod.Post, $"{serverbase}/api/v1/token");
                        request.Headers.Add("X-LETS-DEVICEID", $"{statusDat.DeviceId}");
                        var content = new StringContent(refreshbody, Encoding.UTF8, "application/json");
                        request.Content = content;
                        using (System.Net.Http.HttpResponseMessage response = client.SendAsync(request).Result)
                        {
                            string responseBody = response.Content.ReadAsStringAsync().Result;
                            DebugLog("responseBody=" + responseBody);
                            if (responseBody.Contains("succeeded"))
                            {
                                string res = responseBody.Substring(responseBody.IndexOf("accessToken"));
                                string accesstoken = res.Replace("accessToken", "").Replace(@"""", "").Replace("}", "").Replace(":", "");
                                DebugLog("accesstoken=" + accesstoken);

                                var logoutreq = new HttpRequestMessage(HttpMethod.Post, $"{serverbase}/api/v1/logout");
                                logoutreq.Headers.Add("X-LETS-DEVICEID", $"{statusDat.DeviceId}");
                                logoutreq.Headers.Add("Authorization", $"Bearer {accesstoken}");
                                var logoutbody = $@"{{}}";
                                var logoutcontent = new StringContent(logoutbody, Encoding.UTF8, "application/json");
                                logoutreq.Content = logoutcontent;
                                using (System.Net.Http.HttpResponseMessage logoutresponse = client.SendAsync(logoutreq).Result)
                                {
                                    string logoutresponseBody = logoutresponse.Content.ReadAsStringAsync().Result;
                                    DebugLog("logoutresponseBody=" + logoutresponseBody);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // NOP
            }
        }

        /// <summary>
        /// プロキシを取得する
        /// </summary>
        /// <param name="targeturl">接続先URI</param>
        /// <returns>プロキシ情報</returns>
        static private IWebProxy GetWebProxy(string targeturl, ProxyAuthSetting proxyAuthSetting)
        {
            IWebProxy webproxy = WebRequest.GetSystemWebProxy();
            if (!webproxy.IsBypassed(new Uri(targeturl)))
            {
                if (proxyAuthSetting != null)
                {
                    webproxy.Credentials = new NetworkCredential(proxyAuthSetting.ID, proxyAuthSetting.Password);
                }
                else
                {
                    webproxy.Credentials = CredentialCache.DefaultCredentials;
                }

                return webproxy;
            }

            return null;
        }


        static private void SetHidden(string filepath, bool isHidden)
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

                string appPath = AppDomain.CurrentDomain.BaseDirectory;
                string homedrive = appPath.Substring(0, appPath.IndexOf("\\"));
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
            System.IO.Compression.ZipFile.ExtractToDirectory(ZipPath, targetfolder);
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

    [DataContract]
    internal class StatusDat
    {
        [DataMember]
        public string DeviceKey { get; set; }
        [DataMember]
        public string DeviceId { get; set; }
        [DataMember]
        public bool IsLoggingIn { get; set; }
        [DataMember]
        public string RefreshToken { get; set; }
        [DataMember]
        public string LastEventId { get; set; }
    }

    [DataContract]
    internal class AppSetting
    {
        [DataMember]
        public string FontDeliveryServerUri { get; set; }
        [DataMember]
        public string NotificationServerUri { get; set; }
        [DataMember]
        public int CommunicationRetryCount { get; set; }
        [DataMember]
        public int FixedTermConfirmationInterval { get; set; }
        [DataMember]
        public int FontCalculationFactor { get; set; }
    }

    [DataContract]
    internal class ProxyAuthSetting
    {
        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public string Password { get; set; }
    }

}

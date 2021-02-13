using System;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text.Json;
using Core.Entities;
using Core.Interfaces;
using NLog;

namespace Infrastructure.File
{
    /// <summary>
    /// ユーザ別フォント情報を格納するファイルリポジトリ
    /// </summary>
    public class UserFontsSettingFileRepository : TextFileRepositoryBase, IUserFontsSettingRepository
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>
        /// ユーザレジストリID
        /// </summary>
        private static string userRegID = string.Empty;

        /// <summary>
        /// ユーザレジストリプロファイルパス
        /// </summary>
        private static string userProfileImagePath = string.Empty;

        private static int preFontCnt = 0;
        private static int preFontsCnt = 0;
        private static object saveLockObject = new object();

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        public UserFontsSettingFileRepository(string filePath)
            : base(filePath)
        {
            // システムエンコーディング設定
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        /// <summary>
        /// ユーザ別フォント情報を取得する
        /// </summary>
        /// <returns>ユーザ別フォント情報</returns>
        public UserFontsSetting GetUserFontsSetting()
        {
            if (System.IO.File.Exists(this.FilePath))
            {
                // ファイルが存在する場合、内容を返す
                try
                {
                    string jsonString = this.ReadAll();
                    return JsonSerializer.Deserialize<UserFontsSetting>(jsonString);
                }
                catch (Exception ex)
                {
                    // ファイルが壊れている？
                    Logger.Debug("GetUserFontsSetting:" + ex.StackTrace + "\n");
                    return new UserFontsSetting();
                }
            }
            else
            {
                // ファイルが存在しない場合、新規のオブジェクトを返す
                return new UserFontsSetting();
            }
        }

        /// <summary>
        /// ユーザ別フォント情報を保存する
        /// </summary>
        /// <param name="userFontsSetting">ユーザ別フォント情報</param>
        public void SaveUserFontsSetting(UserFontsSetting userFontsSetting)
        {
            lock (saveLockObject)
            {
                if (preFontsCnt > userFontsSetting.Fonts.Count)
                {
                    Logger.Debug($"SaveUserFontsSetting:FontDecrease!! {preFontsCnt} => {userFontsSetting.Fonts.Count}");
                }

                preFontsCnt = userFontsSetting.Fonts.Count;

                this.WriteAll(JsonSerializer.Serialize(userFontsSetting));
                try
                {
                    this.OutputUninstInfo(userFontsSetting);
                }
                catch (Exception ex)
                {
                    Logger.Debug("OutputUninstInfo:" + ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// アンインストール時に削除する情報を出力する
        /// </summary>
        /// <param name="userFontsSetting">ユーザ別フォント情報</param>
        private void OutputUninstInfo(UserFontsSetting userFontsSetting)
        {
            Logger.Debug("OutputLetsFontsList:Enter");

            if (preFontCnt == userFontsSetting.Fonts.Count)
            {
                return;
            }

            preFontCnt = userFontsSetting.Fonts.Count;

            // ユーザレジストリIDを取得する
            string userregid = this.GetUserRegID();

            // uninstall情報フォルダを作成
            // ホームドライブの取得
            string homedrive = Environment.GetEnvironmentVariable("HOMEDRIVE");

            // LETSフォルダ
            string letsfolder = $@"{homedrive}\ProgramData\Fontworks\LETS";

            // フォント一覧の取得
            var fonts = userFontsSetting.Fonts;

            // 「LETSフォントフラグ」が「true」のファイルからファイルパス抽出する
            var letsFontPaths = fonts.Where(font => font.IsLETS).Select(font => font.Path);
            var letsFontReg = fonts.Where(font => font.IsLETS).Select(font => font.RegistryKey);

            // LETSフォントファイル一覧を出力する
            string uninstfontsPath = Path.Combine(letsfolder, $"uninstallfonts_{userregid}.bat");
            string regfilePath = Path.Combine(letsfolder, $"uninstreg_{userregid}.bat");
            string clearUserDataPath = Path.Combine(letsfolder, $"clearuserdata_{userregid}.bat");
            string logoutPath = Path.Combine(letsfolder, $"logout.bat");

            // フォントファイル削除バッチ出力
            if (System.IO.File.Exists(uninstfontsPath))
            {
                this.SetHidden(uninstfontsPath, false);
            }

            System.IO.File.WriteAllText(uninstfontsPath, "REM フォントファイル削除" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
            foreach (string f in letsFontPaths)
            {
                if (!string.IsNullOrEmpty(f))
                {
                    System.IO.File.AppendAllText(uninstfontsPath, $@"DEL {f}" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
                }
            }

            System.IO.File.AppendAllText(uninstfontsPath, @"Del /F ""%~dp0%~nx0""" + "\n");
            this.SetFileAccessEveryone(uninstfontsPath);
            this.SetHidden(uninstfontsPath, true);

            // レジストリ削除バッチ出力
            if (System.IO.File.Exists(regfilePath))
            {
                this.SetHidden(regfilePath, false);
            }

            System.IO.File.WriteAllText(regfilePath, "REM レジストリ削除" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
            System.IO.File.AppendAllText(regfilePath, $@"reg load HKU\{userRegID} {userProfileImagePath}\NTUSER.DAT" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
            foreach (string r in letsFontReg)
            {
                if (!string.IsNullOrEmpty(r))
                {
                    System.IO.File.AppendAllText(regfilePath, $@"reg delete ""HKU\{userregid}\Software\Microsoft\Windows NT\CurrentVersion\Fonts"" /v ""{r}"" /f" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
                }
            }

            System.IO.File.AppendAllText(regfilePath, $@"reg unload HKU\{userRegID}" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
            System.IO.File.AppendAllText(regfilePath, @"Del /F ""%~dp0%~nx0""" + "\n");
            this.SetFileAccessEveryone(regfilePath);
            this.SetHidden(regfilePath, true);

            // ユーザデータ削除バッチを出力する
            string userDataDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Fontworks", "LETS");

            if (System.IO.File.Exists(clearUserDataPath))
            {
                this.SetHidden(clearUserDataPath, false);
            }

            System.IO.File.WriteAllText(clearUserDataPath, "REM ユーザー削除" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
            System.IO.File.AppendAllText(clearUserDataPath, $"rd /s /q {userDataDirectory}\n");
            System.IO.File.AppendAllText(clearUserDataPath, @"Del /F ""%~dp0%~nx0""" + "\n");
            this.SetFileAccessEveryone(clearUserDataPath);
            this.SetHidden(clearUserDataPath, true);

            Logger.Debug("OutputLetsFontsList:Exit");
        }

        /// <summary>
        /// ファイルの隠し属性を設定/解除する
        /// </summary>
        /// <param name="filepath">ファイルパス</param>
        /// <param name="isHidden">隠し属性フラグ</param>
        private void SetHidden(string filepath, bool isHidden)
        {
            try
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
            catch (Exception ex)
            {
                Logger.Debug("SetHidden:" + ex.StackTrace);
            }
        }

        private void SetFileAccessEveryone(string path)
        {
            try
            {
                FileSystemAccessRule rule = new FileSystemAccessRule(
                    new NTAccount("everyone"),
                    FileSystemRights.FullControl,
                    AccessControlType.Allow);

                var sec = new FileSecurity();
                sec.AddAccessRule(rule);
                System.IO.FileSystemAclExtensions.SetAccessControl(new FileInfo(path), sec);
            }
            catch (Exception ex)
            {
                Logger.Debug("SetFileAccessEveryone:" + ex.StackTrace);
            }
        }

        /// <summary>
        /// ユーザのレジストリIDを取得する
        /// </summary>
        private string GetUserRegID()
        {
            if (string.IsNullOrEmpty(userRegID))
            {
                try
                {
                    string username = Environment.UserName;
                    Logger.Debug($"GetUserRegID:username={username}");
                    string regpath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList";
                    var regkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(regpath);
                    string[] subkeys = regkey.GetSubKeyNames();
                    regkey.Close();
                    foreach (string k in subkeys)
                    {
                        Logger.Debug($"GetUserRegID:subkey={k}");
                        var subregkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey($@"{regpath}\{k}");
                        string profileImagePath = (string)subregkey.GetValue("ProfileImagePath");
                        subregkey.Close();
                        if (!string.IsNullOrEmpty(profileImagePath))
                        {
                            string[] paths = profileImagePath.Split('\\');
                            string uname = paths[paths.Length - 1];
                            Logger.Debug($"GetUserRegID:uname={uname}");
                            if (string.Compare(uname, username, true) == 0)
                            {
                                userRegID = k;
                                userProfileImagePath = profileImagePath;
                                Logger.Debug($"userProfileImagePath:{userProfileImagePath}");
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Debug("GetUserRegID:" + ex.StackTrace);
                }
            }

            return userRegID;
        }
    }
}

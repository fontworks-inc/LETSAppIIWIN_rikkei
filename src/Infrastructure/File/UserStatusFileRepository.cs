using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text.Json;
using Core.Entities;
using Core.Interfaces;
using NLog;

namespace Infrastructure.File
{
    /// <summary>
    /// ユーザ別ステータス情報を格納するファイルリポジトリ
    /// </summary>
    public class UserStatusFileRepository : EncryptFileBase, IUserStatusRepository
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        private static object saveLockObject = new object();

        /// <summary>
        /// ユーザレジストリID
        /// </summary>
        private static string userRegID = string.Empty;

        /// <summary>
        /// ユーザレジストリプロファイルパス
        /// </summary>
        private static string userProfileImagePath = string.Empty;

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        public UserStatusFileRepository(string filePath)
            : base(filePath)
        {
        }

        /// <summary>
        /// ユーザ別ステータス情報を取得する
        /// </summary>
        /// <returns>ユーザ別ステータス情報</returns>
        public UserStatus GetStatus()
        {
            if (System.IO.File.Exists(this.FilePath))
            {
                // ファイルが存在する場合、内容を返す
                try
                {
                    string jsonString = this.ReadAll();
                    return JsonSerializer.Deserialize<UserStatus>(jsonString);
                }
                catch (Exception ex)
                {
                    Logger.Error("UserStatus:" + ex.StackTrace);
                    return new UserStatus();
                }
            }
            else
            {
                // ファイルが存在しない場合、新規のオブジェクトを返す
                return new UserStatus();
            }
        }

        /// <summary>
        /// ユーザ別ステータス情報を保存する
        /// </summary>
        /// <param name="status">ユーザ別ステータス情報</param>
        public void SaveStatus(UserStatus status)
        {
            lock (saveLockObject)
            {
                this.WriteAll(JsonSerializer.Serialize(status));
                Logger.Info($"status.IsLoggingIn={status.IsLoggingIn}");
                if (status.IsLoggingIn)
                {
                    // アンインストール時ログアウトのための情報を出力する
                    this.OutputUninstallLogoutInfo();
                }
            }
        }

        /// <summary>
        /// アンインストール時ログアウトのための情報を出力する
        /// </summary>
        private void OutputUninstallLogoutInfo()
        {
            Logger.Info("CopyUserStatusInfo:Enter");

            try
            {
                // ユーザレジストリIDを取得する
                string userregid = this.GetUserRegID();

                string userDataDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Fontworks", "LETS");

                // ホームドライブの取得
                string appPath = AppDomain.CurrentDomain.BaseDirectory;
                string homedrive = appPath.Substring(0, appPath.IndexOf("\\"));

                // LETSフォルダ
                string letsfolder = $@"{homedrive}\ProgramData\Fontworks\LETS";

                // ログアウト情報を出力する
                Logger.Info("CopyUserStatusInfo:OutputUninstInfo:ログアウト情報を出力する");
                string userdatpath = Path.Combine(userDataDirectory, "status.dat");
                if (System.IO.File.Exists(userdatpath))
                {
                    string logoutinfopath = Path.Combine(letsfolder, $"logoutinfo_{userregid}.dat");
                    Logger.Info($"OutputUninstInfo:{logoutinfopath}");
                    System.IO.File.Copy(userdatpath, logoutinfopath, true);
                    Logger.Info($"CopyUserStatusInfo:Copy {userdatpath}→{logoutinfopath}");
                    this.SetFileAccessEveryone(logoutinfopath);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace);
            }

            Logger.Info("CopyUserStatusInfo:Exit");
        }

        /// <summary>
        /// ユーザのレジストリIDを取得する
        /// </summary>
        private string GetUserRegID()
        {
            string username = Environment.UserName;

            if (string.IsNullOrEmpty(userRegID))
            {
                try
                {
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

            if (string.IsNullOrEmpty(userRegID))
            {
                userRegID = username.Replace(' ', '_');
            }

            return userRegID;
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
    }
}

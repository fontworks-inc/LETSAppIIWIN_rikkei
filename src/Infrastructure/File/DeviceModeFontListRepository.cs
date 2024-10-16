using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using Core.Entities;
using Core.Interfaces;
using NLog;

namespace Infrastructure.File
{
    /// <summary>
    /// フォント情報一覧(デバイスモード時)を格納するリポジトリ
    /// </summary>
    public class DeviceModeFontListRepository : TextFileRepositoryBase, IDeviceModeFontListRepository
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        public DeviceModeFontListRepository(string filePath)
            : base(filePath)
        {
        }

        /// <summary>
        /// フォント情報一覧(デバイスモード時)を取得する
        /// </summary>
        /// <returns>フォント情報一覧(デバイスモード時)</returns>
        public DeviceModeFontList GetDeviceModeFontList()
        {
            if (System.IO.File.Exists(this.FilePath))
            {
                // ファイルが存在する場合、内容を返す
                string jsonString = this.ReadAll();
                return JsonSerializer.Deserialize<DeviceModeFontList>(jsonString);
            }
            else
            {
                // ファイルが存在しない場合、新規のオブジェクトを返す
                return new DeviceModeFontList();
            }
        }

        /// <summary>
        /// フォント情報一覧(デバイスモード時)を保存する
        /// </summary>
        /// <param name="deviceModeFontList">フォント情報一覧(デバイスモード時)</param>
        public void SaveDeviceModeFontList(DeviceModeFontList deviceModeFontList)
        {
            this.WriteAll(JsonSerializer.Serialize(deviceModeFontList));
            try
            {
                this.OutputUninstInfo(deviceModeFontList);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace);
            }
        }

        /// <summary>
        /// アンインストール時に削除する情報を出力する
        /// </summary>
        /// <param name="deviceModeFontList">デバイスモードフォント情報</param>
        private void OutputUninstInfo(DeviceModeFontList deviceModeFontList)
        {
            Logger.Debug("OutputLetsFontsList:Enter");

            // uninstall情報フォルダを作成
            // ホームドライブの取得
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string homedrive = appPath.Substring(0, appPath.IndexOf("\\"));

            // LETSフォルダ
            //string letsfolder = $@"{homedrive}\ProgramData\Fontworks\LETS";
            string programdataFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string letsfolder = $@"{programdataFolder}\Fontworks\LETS";

            // フォント一覧の取得
            var fonts = deviceModeFontList.Fonts;

            // LETSフォントファイル一覧を出力する
            string uninstfontsPath = Path.Combine(letsfolder, "uninstallfonts_device.bat");
            string regfilePath = Path.Combine(letsfolder, "uninstreg_device.bat");

            // フォントファイル削除バッチ出力
            if (System.IO.File.Exists(uninstfontsPath))
            {
                this.SetHidden(uninstfontsPath, false);
            }

            System.IO.File.WriteAllText(uninstfontsPath, "REM フォントファイル削除" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
            bool firstLine = true;
            foreach (DeviceModeFontInfo f in fonts)
            {
                if (string.IsNullOrEmpty(f.FontFilePath))
                {
                    continue;
                }

                if (firstLine)
                {
                    this.writeBatRunas(uninstfontsPath);
                    firstLine = false;
                }

                System.IO.File.AppendAllText(uninstfontsPath, $@"DEL ""{f.FontFilePath}""" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
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
            firstLine = true;
            foreach (DeviceModeFontInfo f in fonts)
            {
                if (string.IsNullOrEmpty(f.FontFilePath))
                {
                    continue;
                }

                if (string.IsNullOrEmpty(f.RegistryKey))
                {
                    continue;
                }

                if (firstLine)
                {
                    this.writeBatRunas(uninstfontsPath);
                    firstLine = false;
                }

                System.IO.File.AppendAllText(regfilePath, $@"reg delete ""SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts"" /v ""{f.RegistryKey}"" /f" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
                System.IO.File.AppendAllText(regfilePath, $@"reg delete ""HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts"" /v ""{f.RegistryKey}"" /f" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
            }

            System.IO.File.AppendAllText(regfilePath, @"Del /F ""%~dp0%~nx0""" + "\n");
            this.SetFileAccessEveryone(regfilePath);
            this.SetHidden(regfilePath, true);

            Logger.Debug("OutputLetsFontsList:Exit");
        }

        private void writeBatRunas(string fnm)
        {
            //System.IO.File.AppendAllText(fnm, "cd /d %~dp0" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
            //System.IO.File.AppendAllText(fnm, "whoami /priv | find \"SeDebugPrivilege\" > nul" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
            //System.IO.File.AppendAllText(fnm, "if %errorlevel% neq 0 (" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
            //System.IO.File.AppendAllText(fnm, "      @powershell start-process %~0 -verb runas" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
            //System.IO.File.AppendAllText(fnm, "      exit" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
            //System.IO.File.AppendAllText(fnm, "  )" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
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
    }
}

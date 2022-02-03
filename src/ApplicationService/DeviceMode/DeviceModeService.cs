using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using ApplicationService.Interfaces;
using Core.Entities;
using Core.Interfaces;
using NLog;

namespace ApplicationService.DeviceMode
{
    /// <summary>
    /// デバイスモードに関する処理を行うサービス
    /// </summary>
    public class DeviceModeService : IDeviceModeService
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceModeService"/> class.
        /// </summary>
        /// <param name="deviceModeSettingRepository">デバイスモード設定リポジトリ</param>
        /// <param name="deviceModeFontListRepository">デバイスモードフォントリストリポジトリ</param>
        /// <param name="deviceModeLicenseInfoRepository">デバイスモードライセンスファイルリポジトリ</param>
        /// <param name="deviceModeLicenseInfoAPIRepository">デバイスモードライセンスAPIリポジトリ</param>
        public DeviceModeService(
            IDeviceModeSettingRepository deviceModeSettingRepository,
            IDeviceModeFontListRepository deviceModeFontListRepository,
            IDeviceModeLicenseInfoRepository deviceModeLicenseInfoRepository,
            IDeviceModeLicenseInfoRepository deviceModeLicenseInfoAPIRepository,
            IStartProcessService startProcessService,
            IFontFileRepository fontFileRepository)
        {
            this.DeviceModeSettingRepository = deviceModeSettingRepository;
            this.DeviceModeFontListRepository = deviceModeFontListRepository;
            this.DeviceModeLicenseInfoRepository = deviceModeLicenseInfoRepository;
            this.DeviceModeLicenseInfoAPIRepository = deviceModeLicenseInfoAPIRepository;
            this.StartProcessService = startProcessService;
            this.FontFileRepository = fontFileRepository;
        }

        private IDeviceModeSettingRepository DeviceModeSettingRepository { get; }

        private IDeviceModeFontListRepository DeviceModeFontListRepository { get; }

        private IDeviceModeLicenseInfoRepository DeviceModeLicenseInfoRepository { get; }

        private IDeviceModeLicenseInfoRepository DeviceModeLicenseInfoAPIRepository { get; }

        private IStartProcessService StartProcessService { get; }

        private IFontFileRepository FontFileRepository { get; }

        /// <summary>
        /// 定期チェック(デバイスモード)
        /// </summary>
        /// <param name="isStartup">起動時フラグ</param>
        /// <returns>チェック結果メッセージを返す</returns>
        public IList<string> FixedTermCheck(bool isStartup)
        {
            IList<string> messageList = new List<string>();

            IDictionary<int, string> letsKindNameMap = this.LetsKindNameMap();  // 契約情報が消える可能性があるため、先に名前を取得しておく

            if (isStartup)
            {
                // ① 設定ファイル存在チェック
                if (!this.DeviceModeSettingRepository.Exists())
                {
                    throw new InvalidOperationException("設定ファイルがありません。LETSアプリを再インストールしてください。");
                }

                // ② 削除対象フォントの削除処理
                DeviceModeFontList deviceModeFontList = this.DeviceModeFontListRepository.GetDeviceModeFontList();
                List<string> uninstallFontList = new List<string>();
                foreach (DeviceModeFontInfo deviceModeFontInfo in deviceModeFontList.Fonts)
                {
                    if ((bool)deviceModeFontInfo.IsRemove)
                    {
                        string letsKindName = string.Empty;
                        try
                        {
                            letsKindName = letsKindNameMap[(int)deviceModeFontInfo.LetsKind];
                        }
                        catch (Exception)
                        {
                            // NOP
                        }

                        uninstallFontList.Add(string.Join("\t", deviceModeFontInfo.FontFilePath, deviceModeFontInfo.RegistryKey, letsKindName, "Uninstall"));
                    }
                }

                if (uninstallFontList.Count > 0)
                {
                    this.UninstallFonts(uninstallFontList, string.Empty);
                }
            }

            // ③ 定期確認処理(以後、24時間毎に実行)
            // ・「2.5.7.9ライセンス情報更新処理」をオンライン登録 = TRUEで呼び出す。
            try
            {
                DeviceModeSetting deviceModeSetting = this.DeviceModeSettingRepository.GetDeviceModeSetting();
                this.DeviceModeLicenseInfoRepository.GetDeviceModeLicenseInfo(true, deviceModeSetting.OfflineDeviceID, deviceModeSetting.IndefiniteAccessToken, deviceModeSetting.LicenceFileKeyPath, deviceModeSetting.LicenseDecryptionKey);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace);
            }

            {
                // 契約情報とフォントのインストール状態を合わせる
                // 結果が正常、エラーに関わらず以下を実行する。
                // ・[ライセンス：有効期限]が過ぎているフォントをディアクティベートする
                // ・[ライセンス：有効期限]が１ヶ月以上過ぎているフォントを削除する
                // ・契約情報にないフォントを削除する
                DateTime now = DateTime.Now;

                DeviceModeLicenseInfo deviceModeLicenseInfo = this.DeviceModeLicenseInfoRepository.GetDeviceModeLicenseInfo();

                // 契約切れ、契約なしの処理
                IList<int> overMonthLetsKind = new List<int>();
                IList<int> expiredLetsKind = new List<int>();
                IList<int> contractedLetsKind = new List<int>();
                foreach (DeviceModeLicense deviceModeLicense in deviceModeLicenseInfo.DeviceModeLicenceList)
                {
                    if (deviceModeLicense.ExpireDate.AddMonths(1).AddDays(1) < now)
                    {
                        // 契約切れ1か月以上過ぎている
                        overMonthLetsKind.Add((int)deviceModeLicense.LetsKind);
                    }
                    else if (deviceModeLicense.ExpireDate.AddDays(1) < now)
                    {
                        // 契約切れしている
                        expiredLetsKind.Add((int)deviceModeLicense.LetsKind);
                    }
                    else
                    {
                        // 契約が有効
                        contractedLetsKind.Add((int)deviceModeLicense.LetsKind);
                    }
                }

                DeviceModeFontList deviceModeFontList = this.DeviceModeFontListRepository.GetDeviceModeFontList();
                List<string> uninstallFontList = new List<string>();
                foreach (DeviceModeFontInfo deviceModeFontInfo in deviceModeFontList.Fonts)
                {
                    if (overMonthLetsKind.Contains((int)deviceModeFontInfo.LetsKind))
                    {
                        string letsKindName = string.Empty;
                        try
                        {
                            letsKindName = letsKindNameMap[(int)deviceModeFontInfo.LetsKind];
                        }
                        catch (Exception)
                        {
                            // NOP
                        }

                        uninstallFontList.Add(string.Join("\t", deviceModeFontInfo.FontFilePath, deviceModeFontInfo.RegistryKey, letsKindName, "Uninstall"));
                    }

                    if (expiredLetsKind.Contains((int)deviceModeFontInfo.LetsKind))
                    {
                        string letsKindName = string.Empty;
                        try
                        {
                            letsKindName = letsKindNameMap[(int)deviceModeFontInfo.LetsKind];
                        }
                        catch (Exception)
                        {
                            // NOP
                        }

                        uninstallFontList.Add(string.Join("\t", deviceModeFontInfo.FontFilePath, deviceModeFontInfo.RegistryKey, letsKindName, "Deactivate"));
                    }

                    if (!contractedLetsKind.Contains((int)deviceModeFontInfo.LetsKind))
                    {
                        string letsKindName = string.Empty;
                        try
                        {
                            letsKindName = letsKindNameMap[(int)deviceModeFontInfo.LetsKind];
                        }
                        catch (Exception)
                        {
                            // NOP
                        }

                        uninstallFontList.Add(string.Join("\t", deviceModeFontInfo.FontFilePath, deviceModeFontInfo.RegistryKey, letsKindName, "Uninstall"));
                    }
                }

                if (uninstallFontList.Count > 0)
                {
                    messageList = this.UninstallFonts(uninstallFontList, "ライセンスを再度契約後、インストールしてください。");
                }
            }

            return messageList;
        }

        /// <summary>
        /// フォントインストール(デバイスモード)
        /// </summary>
        /// <param name="tempPath">一時フォルダ</param>
        /// <returns>チェック結果メッセージを返す</returns>
        public IList<string> InstallFonts(string tempPath)
        {
            IList<string> messageList = new List<string>();
            List<string> fontNameList = new List<string>();

            IDictionary<int, DateTime> fontExpireMap = new Dictionary<int, DateTime>();
            DateTime now = DateTime.Now;

            // 展開されたフォルダ(/font/device, /font/windows/device)にあるフォントをインストールする
            string installFontListFile = System.IO.Path.Combine(tempPath, "InstallFontInfo.lst");
            int installfontcnt = 0;

            DeviceModeLicenseInfo deviceModeLicenseInfo = this.DeviceModeLicenseInfoRepository.GetDeviceModeLicenseInfo();
            IDictionary<int, string> letsKindNameMap = this.LetsKindNameMap();
            List<int> expiredLetsKind = new List<int>();
            foreach (DeviceModeLicense deviceModeLicense in deviceModeLicenseInfo.DeviceModeLicenceList)
            {
                if (deviceModeLicense.ExpireDate.AddDays(1) < now)
                {
                    expiredLetsKind.Add((int)deviceModeLicense.LetsKind);
                }
            }

            string commonFontPath = System.IO.Path.Combine(tempPath, @"font\device");
            if (Directory.Exists(commonFontPath))
            {
                string[] commonFiles = Directory.GetFiles(commonFontPath, "*");
                foreach (string fontPath in commonFiles)
                {
                    var fontIdInfo = this.FontFileRepository.GetFontInfo(fontPath);
                    if (!letsKindNameMap.ContainsKey(fontIdInfo.LetsKind))
                    {
                        // 「LETS種別」が[デバイスモードライセンス：LETS種別]に存在しない場合、次のファイルへ処理を移行する
                        continue;
                    }

                    if (expiredLetsKind.Contains(fontIdInfo.LetsKind))
                    {
                        // 「LETS種別」の[デバイスモードライセンス：有効期限]が過ぎているとき、次のファイルへ処理を移行する
                        continue;
                    }

                    List<string> installFontInfo = new List<string>();
                    installFontInfo.Add(fontPath);
                    installFontInfo.Add(fontIdInfo.NameInfo.UniqueName);
                    installFontInfo.Add(fontIdInfo.LetsKind.ToString());
                    string installFontLine = string.Join("\t", installFontInfo);
                    File.AppendAllText(installFontListFile, installFontLine + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
                    installfontcnt++;
                }
            }

            string winFontPath = System.IO.Path.Combine(tempPath, @"font\windows\device");
            if (Directory.Exists(winFontPath))
            {
                string[] winFiles = Directory.GetFiles(winFontPath, "*");
                foreach (string fontPath in winFiles)
                {
                    var fontIdInfo = this.FontFileRepository.GetFontInfo(fontPath);
                    if (!letsKindNameMap.ContainsKey(fontIdInfo.LetsKind))
                    {
                        // 「LETS種別」が[デバイスモードライセンス：LETS種別]に存在しない場合、次のファイルへ処理を移行する
                        continue;
                    }

                    if (expiredLetsKind.Contains(fontIdInfo.LetsKind))
                    {
                        // 「LETS種別」の[デバイスモードライセンス：有効期限]が過ぎているとき、次のファイルへ処理を移行する
                        continue;
                    }

                    List<string> installFontInfo = new List<string>();
                    installFontInfo.Add(fontPath);
                    installFontInfo.Add(fontIdInfo.NameInfo.UniqueName);
                    installFontInfo.Add(fontIdInfo.LetsKind.ToString());
                    string installFontLine = string.Join("\t", installFontInfo);
                    File.AppendAllText(installFontListFile, installFontLine + Environment.NewLine, Encoding.GetEncoding("shift_jis"));
                    installfontcnt++;
                }
            }

            if (installfontcnt > 0)
            {
                // LETSUpdaterを起動してフォントをインストールする
                this.RunFontOpeProgram("FontInstall", tempPath);

                // 結果ファイルを読み込む
                DeviceModeFontList deviceModeFontList = this.DeviceModeFontListRepository.GetDeviceModeFontList();
                string installResultListFile = System.IO.Path.Combine(tempPath, "InstallFontInfoResult.lst");
                if (File.Exists(installResultListFile))
                {
                    string[] resultLines = File.ReadAllLines(installResultListFile, Encoding.GetEncoding("shift_jis"));
                    foreach (string resultLine in resultLines)
                    {
                        string[] resultInfo = resultLine.Split('\t');
                        switch (resultInfo[2])
                        {
                            case "OK":
                                fontNameList.Add(resultInfo[1]);
                                DeviceModeFontInfo deviceModeFontInfo = new DeviceModeFontInfo(resultInfo[3], resultInfo[1], int.Parse(resultInfo[5]));
                                deviceModeFontList.Fonts.Add(deviceModeFontInfo);
                                break;

                            case "NG":
                            case "ERR":
                                string text = resultInfo[3];
                                messageList.Add(text);
                                break;

                            default:
                                break;
                        }
                    }
                }

                if (fontNameList.Count > 0)
                {
                    this.DeviceModeFontListRepository.SaveDeviceModeFontList(deviceModeFontList);

                    string[] nameList = fontNameList.ToArray();
                    string text = $"インストールが完了しました 書体数：{fontNameList.Count} {string.Join("、", nameList)}";
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    Encoding enc = Encoding.GetEncoding("Shift_JIS");
                    if (enc.GetByteCount(text) > 138)
                    {
                        text = text.Substring(0, enc.GetString(enc.GetBytes(text), 0, 136).Length) + "…";
                    }

                    messageList.Add(text);
                }
            }

            return messageList;
        }

        /// <summary>
        /// フォントアンインストール(デバイスモード)
        /// </summary>
        /// <param name="uninstallFontList">アンインストールフォントリスト</param>
        /// <param name="messageHead">メッセージ接頭辞</param>
        /// <returns>チェック結果メッセージを返す</returns>
        public IList<string> UninstallFonts(IList<string> uninstallFontList, string messageHead)
        {
            List<string> messageList = new List<string>();

            // 一時フォルダを作成する
            string tempPath = System.IO.Path.GetTempPath();
            tempPath = System.IO.Path.Combine(tempPath, Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempPath);
            try
            {
                // 削除対象リストファイルを作成する
                string uninstallFontListFile = System.IO.Path.Combine(tempPath, "UninstallFontInfo.lst");
                File.AppendAllLines(uninstallFontListFile, uninstallFontList, Encoding.GetEncoding("shift_jis"));

                // LETSUpdaterを起動してフォントをインストールする
                this.RunFontOpeProgram("FontUninstall", tempPath);

                // 結果ファイルを読み込む
                List<string> letsKindNameList = new List<string>();
                string uninstallResultListFile = System.IO.Path.Combine(tempPath, "UninstallFontInfoResult.lst");
                if (File.Exists(uninstallResultListFile))
                {
                    string[] resultLines = File.ReadAllLines(uninstallResultListFile, Encoding.GetEncoding("shift_jis"));
                    List<string> okList = new List<string>();
                    List<string> ngList = new List<string>();
                    foreach (string resultLine in resultLines)
                    {
                        string[] resultInfo = resultLine.Split('\t');
                        switch (resultInfo[3])
                        {
                            case "OK":
                                okList.Add(resultInfo[0]);
                                if (!letsKindNameList.Contains(resultInfo[2]))
                                {
                                    letsKindNameList.Add(resultInfo[2]);
                                }

                                break;

                            case "NG":
                                ngList.Add(resultInfo[0]);
                                if (!letsKindNameList.Contains(resultInfo[2]))
                                {
                                    letsKindNameList.Add(resultInfo[2]);
                                }

                                break;

                            case "ERR":
                                string text = resultInfo[4];
                                messageList.Add(text);
                                break;

                            default:
                                break;
                        }
                    }

                    List<int> removeList = new List<int>();
                    List<string> letsKindList = new List<string>();
                    int cnt = 0;
                    DeviceModeFontList deviceModeFontList = this.DeviceModeFontListRepository.GetDeviceModeFontList();
                    foreach (DeviceModeFontInfo deviceModeFontInfo in deviceModeFontList.Fonts)
                    {
                        if (okList.Contains(deviceModeFontInfo.FontFilePath))
                        {
                            removeList.Add(cnt);
                        }

                        if (ngList.Contains(deviceModeFontInfo.FontFilePath))
                        {
                            deviceModeFontInfo.IsRemove = true;
                        }

                        cnt++;
                    }

                    // ファイルを削除出来たフォントをリストから削除する
                    removeList.Reverse();
                    foreach (int idx in removeList)
                    {
                        deviceModeFontList.Fonts.RemoveAt(idx);
                    }

                    this.DeviceModeFontListRepository.SaveDeviceModeFontList(deviceModeFontList);

                    // 通知を表示する。
                    if (letsKindNameList.Count > 0)
                    {
                        string text = $"{messageHead} LETS種別：{string.Join("、", letsKindNameList)}";
                        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                        Encoding enc = Encoding.GetEncoding("Shift_JIS");
                        if (enc.GetByteCount(text) > 138)
                        {
                            text = text.Substring(0, enc.GetString(enc.GetBytes(text), 0, 136).Length) + "…";
                        }

                        messageList.Add(text);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.StackTrace);
            }
            finally
            {
                this.DeleteFolder(tempPath);
            }

            return messageList;
        }

        /// <summary>
        /// LETS種別名→LETS種別マップを作成する
        /// </summary>
        /// <returns>LETS種別名→LETS種別マップ</returns>
        public IDictionary<string, int> LetsNameKindMap()
        {
            // ライセンス情報から、LEST種別名-LETS種別マップを作成する
            DeviceModeLicenseInfo deviceModeLicenseInfo = this.DeviceModeLicenseInfoRepository.GetDeviceModeLicenseInfo();
            IDictionary<string, int> letsNameKindMap = new Dictionary<string, int>();
            foreach (DeviceModeLicense deviceModeLicense in deviceModeLicenseInfo.DeviceModeLicenceList)
            {
                if (!letsNameKindMap.ContainsKey(deviceModeLicense.LetsKindName))
                {
                    letsNameKindMap.Add(deviceModeLicense.LetsKindName, (int)deviceModeLicense.LetsKind);
                }
            }

            return letsNameKindMap;
        }

        /// <summary>
        /// LETS種別→LETS種別名マップを作成する
        /// </summary>
        /// <returns>LETS種別→LETS種別名マップ</returns>
        public IDictionary<int, string> LetsKindNameMap()
        {
            // ライセンス情報から、LEST種別名-LETS種別マップを作成する
            DeviceModeLicenseInfo deviceModeLicenseInfo = this.DeviceModeLicenseInfoRepository.GetDeviceModeLicenseInfo();
            IDictionary<int, string> letsKindNameMap = new Dictionary<int, string>();
            foreach (DeviceModeLicense deviceModeLicense in deviceModeLicenseInfo.DeviceModeLicenceList)
            {
                if (!letsKindNameMap.ContainsKey((int)deviceModeLicense.LetsKind))
                {
                    letsKindNameMap.Add((int)deviceModeLicense.LetsKind, deviceModeLicense.LetsKindName);
                }
            }

            return letsKindNameMap;
        }

        /// <summary>
        /// 指定したディレクトリとその中身を全て削除する
        /// </summary>
        /// <param name="targetDirectoryPath">削除対象フォルダパス</param>
        public void DeleteFolder(string targetDirectoryPath)
        {
            if (!Directory.Exists(targetDirectoryPath))
            {
                return;
            }

            // ディレクトリ以外の全ファイルを削除
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

            // ディレクトリの中のディレクトリも再帰的に削除
            string[] directoryPaths = Directory.GetDirectories(targetDirectoryPath);
            foreach (string directoryPath in directoryPaths)
            {
                this.DeleteFolder(directoryPath);
            }

            // 中が空になったらディレクトリ自身も削除
            try
            {
                Directory.Delete(targetDirectoryPath, false);
            }
            catch (Exception)
            {
                // 消せないフォルダも無視する
            }
        }

        public bool IsAdministratorsMember()
        {
            //ローカルコンピュータストアのPrincipalContextオブジェクトを作成する
            using (PrincipalContext pc = new PrincipalContext(ContextType.Machine))
            {
                //現在のユーザーのプリンシパルを取得する
                UserPrincipal up = UserPrincipal.Current;
                //ローカルAdministratorsグループを探す
                //"S-1-5-32-544"はローカルAdministratorsグループを示すSID
                GroupPrincipal gp = GroupPrincipal.FindByIdentity(pc, "S-1-5-32-544");
                //グループのメンバーであるか調べる
                return up.IsMemberOf(gp);
            }
        }


        /// <summary>
        /// フォント走査プログラムを実行
        /// </summary>
        private void RunFontOpeProgram(string opeKind, string tempPath)
        {
            // アプリ実行ユーザの所属グループに「管理者グループ（SID : S-1-5-32-544）」が含まれているかチェックする
            AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
            WindowsPrincipal principal = (WindowsPrincipal)Thread.CurrentPrincipal;
            List<Claim> userClaims = new List<Claim>(principal.UserClaims);
            bool isAdministrator = userClaims.Where(claim => claim.Value.Equals("S-1-5-32-544")).Any();
            if (!isAdministrator)
            {
                // 管理者グループではない場合、エラーを出力して処理を終了する
                throw new UnauthorizedAccessException("管理者権限で起動してください");
            }

            // アップデータのディレクトリパスを作成する
            string directoryPath = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName;

            // 第一引数にバージョン、第二引数に起動指定バージョン
            List<string> argList = new List<string>();
            argList.Add(opeKind);
            argList.Add(tempPath);

            // 実行権限を管理者権限とし、プログラムアップデータを実行する
            Process proc = this.StartProcessService.StartProcessAdministrator(directoryPath, "LETSUpdater.exe", argList.ToArray());
            proc.WaitForExit();
        }
    }
}

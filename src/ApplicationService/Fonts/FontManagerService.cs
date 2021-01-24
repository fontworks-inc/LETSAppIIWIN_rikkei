using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ApplicationService.Interfaces;
using Core.Entities;
using Core.Interfaces;
using NLog;
using OS.Interfaces;
using static ApplicationService.Interfaces.IFontManagerService;

namespace ApplicationService.Fonts
{
    /// <summary>
    /// フォント管理に関する処理を行うサービスクラス
    /// </summary>
    public class FontManagerService : IFontManagerService
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>
        /// 文言の取得を行うインスタンス
        /// </summary>
        private readonly IResourceWrapper resourceWrapper;

        /// <summary>
        /// 共通設定情報を格納するリポジトリ
        /// </summary>
        private readonly IApplicationSettingRepository applicationSettingRepository;

        /// <summary>
        /// メモリで保持する情報を格納するリポジトリ
        /// </summary>
        private readonly IVolatileSettingRepository volatileSettingRepository;

        /// <summary>
        /// ユーザ別フォント情報を格納するリポジトリ
        /// </summary>
        private readonly IUserFontsSettingRepository userFontsSettingRepository;

        /// <summary>
        /// ユーザ別ステータス情報を格納するリポジトリ
        /// </summary>
        private readonly IUserStatusRepository userStatusRepository = null;

        /// <summary>
        /// フォント情報リポジトリのインターフェイス
        /// </summary>
        private readonly IFontsRepository fontsRepository;

        /// <summary>
        /// フォントアクティベートサービス
        /// </summary>
        private readonly IFontActivationService fontActivationService;

        /// <summary>
        /// フォント内部情報を格納するリポジトリ
        /// </summary>
        private readonly IFontFileRepository fontInfoRepository = null;

        /// <summary>
        /// ダウンロード処理が実行中かどうか
        /// </summary>
        /// <remarks>実行中の場合true, そうでない場合はfalse</remarks>
        private bool isExecuting;

        /// <summary>
        /// エラーダイアログ表示用のイベント
        /// </summary>
        public ShowErrorDialogEvent ShowErrorDialogEvent { get; set; }

        /// <summary>
        /// ダウンロード開始時のイベント
        /// </summary>
        public FontStartDownloadEvent FontStartDownloadEvent { get; set; }

        /// <summary>
        /// ダウンロード完了時のイベント
        /// </summary>
        public FontDownloadCompletedEvent FontDownloadCompletedEvent { get; set; }

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="resourceWrapper">文言の取得を行うインスタンス</param>
        /// <param name="applicationSettingRepository">共通設定情報を格納するリポジトリ</param>
        /// <param name="volatileSettingRepository">メモリで保持する情報を格納するリポジトリ</param>
        /// <param name="userFontsSettingRepository">ユーザ別フォント情報を格納するリポジトリ</param>
        /// <param name="userStatusRepository">ユーザ別ステータス情報を格納するリポジトリ</param>
        /// <param name="fontsRepository">フォント情報リポジトリのインターフェイス</param>
        /// <param name="fontActivationService">フォントアクティベートサービス</param>
        /// <param name="fontInfoRepository">フォント内部情報を格納するリポジトリ</param>
        public FontManagerService(
            IResourceWrapper resourceWrapper,
            IApplicationSettingRepository applicationSettingRepository,
            IVolatileSettingRepository volatileSettingRepository,
            IUserFontsSettingRepository userFontsSettingRepository,
            IUserStatusRepository userStatusRepository,
            IFontsRepository fontsRepository,
            IFontActivationService fontActivationService,
            IFontFileRepository fontInfoRepository = null
            )
        {
            this.resourceWrapper = resourceWrapper;
            this.applicationSettingRepository = applicationSettingRepository;
            this.volatileSettingRepository = volatileSettingRepository;
            this.userFontsSettingRepository = userFontsSettingRepository;
            this.userStatusRepository = userStatusRepository;
            this.fontsRepository = fontsRepository;
            this.fontActivationService = fontActivationService;
            this.fontInfoRepository = fontInfoRepository;
        }

        private static readonly object LockHandler = new object();

        /// <summary>
        /// フォントの同期処理を実施する
        /// </summary>
        /// <param name="font">アクティベート対象フォント</param>
        /// <remarks>アクティベート通知からの同期処理</remarks>
        public void Synchronize(ActivateFont font)
        {
            lock (LockHandler)
            {
                InstallFont installFont = null;

                // 保持しているフォントからアクティベート対象フォントとIDが一致するフォントを取得する
                IList<Font> userFonts = this.userFontsSettingRepository.GetUserFontsSetting().Fonts;
                //Font userFont = userFonts.Where(userFont => userFont.Id.Equals(font.FontId)).FirstOrDefault();
                Font userFont = userFonts.Where(userFont => this.FormatFontID(userFont.Id).Equals(this.FormatFontID(font.FontId))).FirstOrDefault();
                if (userFont == null)
                {
                    // ID一致のフォントが無い場合、インストール対象フォントに加える
                    installFont = new InstallFont(string.Empty, true, font.FontId, font.DisplayFontName, string.Empty, font.FileSize, font.Version, false, true, font.IsFreemium, font.ContractIds);
                }
                else
                {
                    // ID一致のフォントがある場合、バージョンを比較する
                    //Version activateFontVersion = new System.Version(font.Version);
                    //Version userFontVersion = new System.Version(userFont.Version);
                    if (font.Version.CompareTo(userFont.Version) != 0)
                    {
                        // アクティベート対象フォントのバージョンが新しい場合、インストール対象フォントに加える
                        installFont = new InstallFont(string.Empty, true, font.FontId, font.DisplayFontName, string.Empty, font.FileSize, font.Version, false, true, font.IsFreemium, font.ContractIds);
                    }
                }

                // フォントをダウンロードする
                try
                {
                    Task.Run(() =>
                    {
                        if (this.isExecuting)
                        {
                            Logger.Info("ダウンロード中なのでスキップ");
                            if (!this.volatileSettingRepository.GetVolatileSetting().InstallTargetFonts.Contains(installFont))
                            {
                                this.volatileSettingRepository.GetVolatileSetting().InstallTargetFonts.Add(installFont);
                            }
                            return;
                        }

                        try
                        {
                            Logger.Info("ダウンロード開始");
                            this.isExecuting = true;
                            List<InstallFont> installFontList = new List<InstallFont>();
                            installFontList.Add(installFont);
                            this.DownloadFontFile(installFontList);
                        }
                        finally
                        {
                            Logger.Info("ダウンロード終了");
                            this.isExecuting = false;
                        }
                    });
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// フォントの同期処理を実施する
        /// </summary>
        /// <param name="startUp">起動時かどうか</param>
        /// <remarks>アクティベート通知以外からの同期処理</remarks>
        public void Synchronize(bool startUp)
        {
            lock (LockHandler)
            {
                // 起動時のみ下記処理を実施
                if (startUp)
                {
                    // 「削除対象フォント」=TRUEのフォント削除を行う
                    UserFontsSetting userFontsSetting = this.userFontsSettingRepository.GetUserFontsSetting();
                    foreach (Font font in userFontsSetting.Fonts.Where(font => font.IsRemove == true))
                    {
                        this.DeleteFontFile(font.Path);
                    }
                }

                // APIからインストール対象フォント情報を取得し、インストールが必要なフォントを抽出する
                IList<InstallFont> installFontInformations = null;
                try
                {
                    installFontInformations = this.fontsRepository.GetInstallFontInformations(this.userStatusRepository.GetStatus().DeviceId, VaildFontType.Both);
                }
                catch (Exception e)
                {
                    // API実行時にエラーメッセージを残す（通知はしない）
                    string message = this.resourceWrapper.GetString("LOG_ERR_FontManagerService_GetInstallFontInformations");
                    Logger.Error(e, message);
                }

                if (installFontInformations == null || installFontInformations.Count() == 0)
                {
                    // インストール対象フォントが存在しない
                    return;
                }

                List<InstallFont> targetFontList = new List<InstallFont>();
                try
                {
                    // APIから取得したフォント情報からインストール対象フォントを抽出する
                    this.CollectInstallTargetFontFromFontInfomations(startUp, installFontInformations);
                    targetFontList = new List<InstallFont>(this.volatileSettingRepository.GetVolatileSetting().InstallTargetFonts);
                    this.volatileSettingRepository.GetVolatileSetting().InstallTargetFonts.Clear();
                }
                catch (Exception ex)
                {
                    Logger.Debug("Synchronize:" + ex.Message + "\n" + ex.StackTrace);
                }

                // フォントをダウンロードする
                try
                {
                    Task.Run(() =>
                    {
                        if (this.isExecuting)
                        {
                            Logger.Info("ダウンロード中なのでスキップ");
                            return;
                        }

                        try
                        {
                            Logger.Info("ダウンロード開始");
                            this.isExecuting = true;
                            this.DownloadFontFile(targetFontList);
                        }
                        finally
                        {
                            Logger.Info("ダウンロード終了");
                            this.isExecuting = false;
                        }
                    });
                }
                catch (Exception ex)
                {
                    Logger.Debug("Synchronize:" + ex.Message + "\n" + ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// 契約切れフォントのディアクティベート
        /// </summary>
        /// <param name="contracts">契約情報の集合体</param>
        public void DeactivateExpiredFonts(IList<Contract> contracts)
        {
            // 有効な契約終了日の契約情報(「契約終了日」が過ぎていない契約情報)を取得する
            var now = DateTime.Now;
            var today = new DateTime(now.Year, now.Month, now.Day);
            IEnumerable<Contract> validContracts = contracts.Where(contract => today.CompareTo(contract.ContractEndDate) <= 0);

            // 有効な契約終了日の契約情報を持っていないLETSフォントの情報を取得し、ディアクティベートする
            UserFontsSetting userFontsSetting = this.userFontsSettingRepository.GetUserFontsSetting();
            userFontsSetting.Fonts
                .Where(font => font.IsLETS && !font.ContractIds
                    .Any(contractId => validContracts.Select(contract => contract.ContractId).Contains(contractId)))
                        .Select(font =>
                        {
                            this.fontActivationService.Deactivate(font);
                            Logger.Info(string.Format("DeactivateExpiredFonts:" + font.Path, string.Empty));
                            font.IsActivated = false;
                            return font;
                        }).ToList();

            // 【Phase2】最後の期限から１ヶ月以上経過しているとき、[フォント：フォント一覧.削除対象]をTRUEに設定する
            //this.fontActivationService.BroadcastFont();
        }

        /// <summary>
        /// フォント：フォント一覧の一括ディアクティベート
        /// </summary>
        /// <remarks>保存ファイル内のアクティベートフォントを一括でディアクティベートする</remarks>
        public void DeactivateSettingFonts()
        {
            // [フォント：フォント情報]でアクティベートされているLETSフォントをディアクティベート
            var setting = this.userFontsSettingRepository.GetUserFontsSetting();
            setting.Fonts = setting.Fonts
                .Select(font =>
                {
                    if (font.IsLETS && font.IsActivated == true)
                    {
                        this.fontActivationService.Deactivate(font);
                    }

                    return font;
                }).ToList();

            // 保存処理
            this.userFontsSettingRepository.SaveUserFontsSetting(setting);
        }

        /// <summary>
        /// フォントのディアクティベート処理
        /// </summary>
        /// <param name="fontId">フォントID</param>
        /// <remark>フォントディアクティベート通知での呼び出し用</remark>
        public void DeactivateFont(string fontId)
        {
            UserFontsSetting userFontsSetting = this.userFontsSettingRepository.GetUserFontsSetting();
            //Font font = userFontsSetting.Fonts.Where(font => font.Id.Equals(fontId)).FirstOrDefault();
            Font font = userFontsSetting.Fonts.Where(font => this.FormatFontID(font.Id).Equals(this.FormatFontID(fontId))).FirstOrDefault();
            this.fontActivationService.Deactivate(font);
            //this.fontActivationService.BroadcastFont();
        }

        /// <summary>
        /// フォント：フォント一覧の更新
        /// </summary>
        /// <param name="userFontsDir">ユーザーフォントのディレクトリ</param>
        public void UpdateFontsList(string userFontsDir)
        {
            if (this.fontInfoRepository == null)
            {
                throw new InvalidOperationException(this.resourceWrapper.GetString("LOG_ERR_FontManagerService_UpdateFontsList_InvalidOperationException"));
            }

            // フォント一覧の取得
            var fonts = this.userFontsSettingRepository.GetUserFontsSetting().Fonts;

            // フォルダ配下のファイルをすべて取得する
            try
            {
                IEnumerable<string> files = Directory.EnumerateFiles(userFontsDir, "*", SearchOption.TopDirectoryOnly);

                var updateFonts = new List<Font>();
                foreach (string filePath in files)
                {
                    try
                    {
                        //var savedFont = fonts.SingleOrDefault(f => f.Path == filePath);
                        var savedFont = fonts.FirstOrDefault(f => f.Path == filePath);
                        updateFonts.Add((savedFont != null) ? this.UpdateSavedFont(filePath, savedFont) : this.CreateFont(filePath));
                    }
                    catch (Exception ex)
                    {
                        // フォント情報取得時に例外が発生しましたが、本フォントは {0} は対象外として扱い処理を継続します
                        Logger.Error(ex, string.Format(this.resourceWrapper.GetString("LOG_ERR_FontManagerService_UpdateFontsList"), filePath));
                    }
                }

                // 保存
                this.userFontsSettingRepository.SaveUserFontsSetting(new UserFontsSetting() { Fonts = updateFonts });
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                return;
            }
        }

        /// <summary>
        /// 「LETSフォント」一覧を出力する
        /// </summary>
        public void OutputLetsFontsList()
        {
            Logger.Info(string.Format("OutputLetsFontsList:Enter", ""));
            // フォント一覧の取得
            var fonts = this.userFontsSettingRepository.GetUserFontsSetting().Fonts;

            // 「LETSフォントフラグ」が「true」のファイルからファイルパス抽出する
            var letsFontPaths = fonts.Where(font => font.IsLETS).Select(font => font.Path);

            // LETSフォントファイル一覧を出力する
            string dirPath = Directory.GetParent(
                this.volatileSettingRepository.GetVolatileSetting().ClientApplicationPath).Parent.FullName;
            string filePath = Path.Combine(dirPath, "uninstallfonts.bat");
            if (!File.Exists(filePath))
            {
                // ファイルが無ければ新規作成
                File.CreateText(filePath).Close();
            }

            File.WriteAllText(filePath, "REM LETSフォント削除\n");
            foreach (string l in letsFontPaths)
            {
                File.AppendAllText(filePath, $"DEL {l}\n");
            }
            File.AppendAllText(filePath, @"Del /F ""%~dp0%~nx0""" + "\n");

            // ユーザデータ削除バッチを出力する
            string clearUserDataPath = Path.Combine(dirPath, "clearuserdata.bat");
            string userDataDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Fontworks", "LETS");
            File.WriteAllText(clearUserDataPath, $"rd /s /q {userDataDirectory}\n");
            File.AppendAllText(clearUserDataPath, @"Del /F ""%~dp0%~nx0""" + "\n");
        }

        public void BroadcastFontChange()
        {
            this.fontActivationService.BroadcastFont();
        }

        /// <summary>
        /// フォントファイルを削除する
        /// </summary>
        /// <param name="filePath">削除対象ファイルのパス</param>
        /// <returns>正常に削除できれば0、失敗すれば-1</returns>
        private int DeleteFontFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                return 0;
            }
            catch (Exception e)
            {
                // ファイル削除失敗時のメッセージ
                string message = string.Format(this.resourceWrapper.GetString("LOG_ERR_FontManagerService_DeleteFontFile"), filePath);
                Logger.Error(e, message);
                return -1;
            }
        }

        /// <summary>
        /// インストール対象フォント情報をもとに、インストール対象フォント抽出し、必要であればアクティベート／ディアクティベートを実行する。
        /// </summary>
        /// <param name="startUp">起動時かどうか</param>
        /// <param name="installFontInformations">インストール対象フォント情報</param>
        private void CollectInstallTargetFontFromFontInfomations(bool startUp, IList<InstallFont> installFontInformations)
        {
            // 取得した各フォント情報と内部に保持するフォント情報を比較する
            UserFontsSetting settings = this.userFontsSettingRepository.GetUserFontsSetting();
            IList<Font> userFonts = settings.Fonts;
            foreach (InstallFont installFont in installFontInformations)
            {
                //Font userFont = userFonts.Where(fonts => fonts.Id.Equals(installFont.FontId)).FirstOrDefault();
                Font userFont = userFonts.Where(fonts => this.FormatFontID(fonts.Id) == this.FormatFontID(installFont.FontId)).FirstOrDefault();
                if (userFont != null)
                {
                    bool isFontChange = false;
                    // インストール対象フォントと保持しているフォントのIDが一致する場合
                    // 「アクティベート状態」と「バージョン」を確認し、いずれかが不一致の場合にそれぞれ処理を実施する
                    bool matchActivate = installFont.ActivateFlg == userFont.IsActivated.GetValueOrDefault();
                    bool matchVersion = installFont.Version.Equals(userFont.Version);
                    if (!matchActivate || !matchVersion)
                    {
                        /*
                         * [フォント利用可否]
                         *      インストール対象フォント | 保持しているフォント | 処理
                         *       false                   | -                    | ディアクティベート実行
                         *
                         * [アクティベート状態]
                         *      インストール対象フォント | 保持しているフォント | 処理
                         *       true                    | false                | アクティベート実行
                         *       false                   | true                 | ディアクティベート実行
                         */
                        if (!installFont.IsAvailableFont)
                        {
                            this.fontActivationService.Deactivate(userFont); // ディアクティベート実行
                            isFontChange = true;
                        }
                        else if (installFont.ActivateFlg && !userFont.IsActivated.GetValueOrDefault())
                        {
                            this.fontActivationService.Activate(userFont); // アクティベート実行

                            // 契約IDを更新する
                            userFont.ContractIds = installFont.ContractIds;
                            this.userFontsSettingRepository.SaveUserFontsSetting(settings);

                            isFontChange = true;
                        }
                        else if (!installFont.ActivateFlg && userFont.IsActivated.GetValueOrDefault())
                        {
                            this.fontActivationService.Deactivate(userFont); // ディアクティベート実行
                            isFontChange = true;
                        }

                        /*
                         *  [フォントバージョンアップ要否]
                         *      インストール対象フォント | 保持しているフォント | 処理
                         *       true                    | -                    | インストール対象フォントに加える
                         */
                        if (installFont.NeedFontVersionUpdate)
                        {
                            if (!this.volatileSettingRepository.GetVolatileSetting().InstallTargetFonts.Contains(installFont))
                            {
                                this.volatileSettingRepository.GetVolatileSetting().InstallTargetFonts.Add(installFont);
                            }
                        }
                    }
                    if (isFontChange)
                    {
                        this.fontActivationService.BroadcastFont();
                    }
                }
                else
                {
                    // インストール対象フォントと保持しているフォントのIDが一致しない場合
                    // 「アクティベート状態」がTrueであればインストール対象フォントに追加
                    if (installFont.ActivateFlg)
                    {
                        if (!this.volatileSettingRepository.GetVolatileSetting().InstallTargetFonts.Contains(installFont))
                        {
                            this.volatileSettingRepository.GetVolatileSetting().InstallTargetFonts.Add(installFont);
                        }
                    }
                }
            }
        }

        private string FormatFontID(string fontId)
        {
            string formatedId = "000000" + fontId;
            return formatedId.Substring(formatedId.Length - 6);
        }

        /// <summary>
        /// インストール対象フォントをダウンロードする
        /// </summary>
        private async void DownloadFontFile(IList<InstallFont> installFontList)
        {
            // インストール対象フォントが存在する場合のみ下記処理を実施する
            if (installFontList.Count != 0)
            {
                // フォントインストール容量確認処理（PCの容量確認）
                double totalFontSize = 0;
                foreach (InstallFont installFont in installFontList)
                {
                    totalFontSize += Math.Round(installFont.FileSize * this.applicationSettingRepository.GetSetting().FontCalculationFactor, 10, MidpointRounding.ToPositiveInfinity);
                }

                DriveInfo drive = new DriveInfo(Path.GetPathRoot(this.volatileSettingRepository.GetVolatileSetting().ClientApplicationPath));
                double freeSpace = drive.AvailableFreeSpace;

                if (totalFontSize > freeSpace)
                {
                    if (this.ShowErrorDialogEvent == null)
                    {
                        this.ShowErrorDialogEvent = (text, caption) =>
                            MessageBox.Show(
                                text,
                                caption,
                                MessageBoxButton.OK,
                                MessageBoxImage.Hand);
                    }

                    // 容量が不足しているときはエラーを表示
                    Logger.Error(this.resourceWrapper.GetString("LOG_ERR_FontManagerService_OutOfDiskSpace_Caption"));
                    Logger.Error(this.resourceWrapper.GetString("LOG_ERR_FontManagerService_OutOfDiskSpace_Text"));
                    this.ShowErrorDialogEvent(
                        this.resourceWrapper.GetString("LOG_ERR_FontManagerService_OutOfDiskSpace_Text"),
                        this.resourceWrapper.GetString("LOG_ERR_FontManagerService_OutOfDiskSpace_Caption"));
                    return;
                }

                // フォントダウンロード
                VolatileSetting memory = this.volatileSettingRepository.GetVolatileSetting();
                try
                {
                    double compFileSize = 0;
                    double totalFileSize = installFontList.Sum(font => font.FileSize);

                    memory.IsDownloading = true;

                    foreach (InstallFont installFont in installFontList)
                    {
                        this.FontStartDownloadEvent(installFont, compFileSize, totalFileSize);

                        var fileStream = this.fontsRepository.DownloadFonts(this.userStatusRepository.GetStatus().DeviceId, installFont.FontId);
                        installFont.Path = fileStream.Name;
                        fileStream.Close();

                        compFileSize += installFont.FileSize;

                        Logger.Info(string.Format("DownloadFontFile:Path=" + installFont.Path, ""));
                    }
                }
                finally
                {
                    memory.IsDownloading = false;
                    memory.CompletedDownload = true;
                }

                //  フォントインストール
                Logger.Info(string.Format("フォントインストール", ""));
                bool isInstall = false;
                foreach (InstallFont installFont in installFontList)
                {
                    Logger.Info(string.Format("フォントインストール:" + installFont.Path, ""));
                    Font f = new Font(installFont.FontId, installFont.FileName, true, installFont.ActivateFlg, installFont.DisplayFontName, installFont.Version, "", installFont.IsFreemium, installFont.ContractIds);
                    f.Path = installFont.Path;
                    if (this.fontActivationService.Install(f))
                    {
                        isInstall = true;
                        var notifyList = memory.NotificationFonts;
                        notifyList.Add(installFont);
                    }
                }
                if (isInstall)
                {
                    this.fontActivationService.BroadcastFont();

                    this.FontDownloadCompletedEvent(memory.NotificationFonts);

                    memory.NotificationFonts.Clear();
                }
            }
        }

        /// <summary>
        /// [フォント一覧]に存在するフォント情報の更新
        /// </summary>
        /// <param name="filePath">フォントファイルパス</param>
        /// <param name="savedFont">保存されていたフォント</param>
        /// <returns>更新後のフォント情報</returns>
        private Font UpdateSavedFont(string filePath, Font savedFont)
        {
            // [フォント一覧]に存在する場合
            if (savedFont.IsLETS)
            {
                // 識別子確認のDLLを介し、情報を取得する
                var idInfo = this.fontInfoRepository.GetFontInfo(filePath);

                // バージョンが異なる場合、フォント情報を更新し、一覧に追加する
                if (savedFont.Version != idInfo.NameInfo.Version)
                {
                    savedFont.Version = idInfo.NameInfo.Version;
                    savedFont.DisplayName = idInfo.NameInfo.UniqueName;
                }
            }

            // 更新用のフォント
            return savedFont;
        }

        /// <summary>
        /// [フォント一覧]に存在しないフォント情報の生成
        /// </summary>
        /// <param name="filePath">フォントファイルパス</param>
        /// <returns>フォント情報</returns>
        private Font CreateFont(string filePath)
        {
            // [フォント一覧]に存在しない場合、識別子確認のDLLを介し情報を取得する
            var idInfo = this.fontInfoRepository.GetFontInfo(filePath);

            // 基本的にLETSフォント以外がユーザーフォントに追加された際に情報を追加することになる
            var addFont = new Font(
                idInfo.NameInfo.Ids.FontId,
                filePath,
                !string.IsNullOrEmpty(idInfo.UserId) || !string.IsNullOrEmpty(idInfo.DeviceId),
                false,
                idInfo.NameInfo.UniqueName,
                idInfo.NameInfo.Version,
                string.Empty,
                false,
                new List<string>(),
                false);

            return addFont;
        }

    }
}
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
        /// 同期処理のロックを行うためのオブジェクト
        /// </summary>
        private static readonly object SynchronizeLockHandler = new object();

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
        /// ダウンロード実行中フラグ
        /// </summary>
        private bool isExecutingDownload = false;

        /// <summary>
        /// 起動時ダウンロード実行終了フラグ
        /// </summary>
        private bool isFirstDownloadCompleted = false;

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
            IFontFileRepository fontInfoRepository = null)
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
        /// ダウンロード失敗時のイベント
        /// </summary>
        /// <param name="font">失敗したフォント</param>
        public FontDownloadFailedEvent FontDownloadFailedEvent { get; set; }

        /// <summary>
        /// ダウンロード失敗時のイベント
        /// </summary>
        /// <param name="font">失敗したフォント</param>
        public FontDownloadCancelledEvent FontDownloadCancelledEvent { get; set; }

        /// <summary>
        /// フォントの同期処理を実施する
        /// </summary>
        /// <param name="font">アクティベート対象フォント</param>
        /// <remarks>アクティベート通知からの同期処理</remarks>
        public void Synchronize(ActivateFont font)
        {
            InstallFont installFont = null;

            // 保持しているフォントからアクティベート対象フォントとIDが一致するフォントを取得する
            IList<Font> userFonts = this.userFontsSettingRepository.GetUserFontsSetting().Fonts;
            Font userFont = userFonts.Where(userFont => this.FormatFontID(userFont.Id).Equals(this.FormatFontID(font.FontId))).FirstOrDefault();
            if (userFont == null)
            {
                // ID一致のフォントが無い場合、インストール対象フォントに加える
                installFont = new InstallFont(string.Empty, true, font.FontId, font.DisplayFontName, string.Empty, font.FileSize, font.Version, false, true, font.IsFreemium, font.ContractIds);
            }
            else
            {
                // ID一致のフォントがある場合、バージョンを比較する
                if (font.Version.CompareTo(userFont.Version) != 0)
                {
                    // アクティベート対象フォントのバージョンが新しい場合、インストール対象フォントに加える
                    installFont = new InstallFont(string.Empty, true, font.FontId, font.DisplayFontName, string.Empty, font.FileSize, font.Version, false, true, font.IsFreemium, font.ContractIds);
                    if (!this.FontExitsInUserFonts(installFont.FileName))
                    {
                        // ユーザーフォントフォルダにフォントファイルがない場合、インストール対象に加える
                        Logger.Warn($"[Ph.2] Synchronize(Activate):VersionDiff: install font={installFont.FileName}");
                        this.volatileSettingRepository.GetVolatileSetting().InstallTargetFonts.Add(installFont);

                        // 削除フラグを落とす
                        this.fontActivationService.RemoveTargetSettings(userFont, false);
                    }
                    else
                    {
                        // ユーザーフォントフォルダにフォントファイルがある場合、削除対象に加える
                        Logger.Warn($"[Ph.2] Synchronize(Activate):VersionDiff: remove font={installFont.FileName}");

                        // フォントレジストリ削除
                        this.fontActivationService.DelRegistory(userFont);

                        // 削除フラグを立てる
                        this.fontActivationService.RemoveTargetSettings(userFont);

                        // フォントダウンロード中断メッセージ
                        this.FontDownloadCancelledEvent(installFont);
                    }
                }

                // Activateのみ実行する
                this.fontActivationService.Activate(userFont);
                this.UpdateFontInfo(font);
            }

            if (!this.userStatusRepository.GetStatus().IsLoggingIn)
            {
                // ログアウトされていたら処理を行わない
                return;
            }

            if (installFont == null)
            {
                return;
            }

            // フォントをダウンロードする
            try
            {
                Task.Run(() =>
                {
                    Task.Delay(10);

                    if (this.isExecutingDownload)
                    {
                        Logger.Info("ダウンロード中なのでスキップ");

                        this.volatileSettingRepository.GetVolatileSetting().InstallTargetFonts.Add(installFont);
                        return;
                    }

                    try
                    {
                        Logger.Info("ダウンロード開始");
                        this.isExecutingDownload = true;
                        List<InstallFont> installFontList = new List<InstallFont>();
                        installFontList = new List<InstallFont>(this.volatileSettingRepository.GetVolatileSetting().InstallTargetFonts);
                        installFontList.Add(installFont);
                        while (installFontList.Count > 0)
                        {
                            this.DownloadFontFile(installFontList);
                            installFontList = new List<InstallFont>(this.volatileSettingRepository.GetVolatileSetting().InstallTargetFonts);
                        }
                    }
                    finally
                    {
                        Logger.Info("ダウンロード終了");
                        this.isExecutingDownload = false;
                    }
                });
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// フォントの同期処理を実施する
        /// </summary>
        /// <param name="startUp">起動時かどうか</param>
        /// <remarks>アクティベート通知以外からの同期処理</remarks>
        public void Synchronize(bool startUp)
        {
            // 起動時のみ下記処理を実施
            if (startUp)
            {
                this.isFirstDownloadCompleted = false;

                // 「削除対象フォント」=TRUEのフォント削除を行う
                bool delflg = false;
                UserFontsSetting userFontsSetting = this.userFontsSettingRepository.GetUserFontsSetting();
                foreach (Font font in userFontsSetting.Fonts.Where(font => font.IsRemove == true))
                {
                    if (this.fontActivationService.Delete(font))
                    {
                        delflg = true;
                    }
                }

                if (delflg)
                {
                    // ユーザー配下のフォントフォルダ
                    var wlocal = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    var wuserFontsDir = @$"{wlocal}\Microsoft\Windows\Fonts";
                    this.UpdateFontsList(wuserFontsDir);
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
                    if (this.isExecutingDownload)
                    {
                        Logger.Info("ダウンロード中なのでスキップ");
                        return;
                    }

                    try
                    {
                        Logger.Info("ダウンロード開始");
                        this.isExecutingDownload = true;
                        while (targetFontList.Count > 0)
                        {
                            this.DownloadFontFile(targetFontList);
                            targetFontList = new List<InstallFont>(this.volatileSettingRepository.GetVolatileSetting().InstallTargetFonts);
                        }
                    }
                    finally
                    {
                        Logger.Info("ダウンロード終了");
                        this.isExecutingDownload = false;
                        this.isFirstDownloadCompleted = true;
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Debug("Synchronize:" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        /// <inheritdoc/>
        public bool GetIsFirstDownloadCompleted()
        {
            return this.isFirstDownloadCompleted;
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

        /// <inheritdoc/>
        public void UninstallDeactivatedFonts()
        {
            // [フォント：フォント情報]でディアクティベートされているLETSフォントを削除
            var setting = this.userFontsSettingRepository.GetUserFontsSetting();
            setting.Fonts = setting.Fonts
                .Select(font =>
                {
                    if (font.IsLETS && font.IsActivated != true)
                    {
                        this.fontActivationService.Delete(font);
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
            Font font = userFontsSetting.Fonts.Where(font => this.FormatFontID(font.Id).Equals(this.FormatFontID(fontId))).FirstOrDefault();
            if (font != null)
            {
                this.fontActivationService.Deactivate(font);
            }
        }

        /// <inheritdoc/>
        public void CheckFontsList()
        {
            // ユーザー配下のフォントフォルダ
            var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var userFontsDir = @$"{local}\Microsoft\Windows\Fonts";

            if (!Directory.Exists(userFontsDir))
            {
                return;
            }

            try
            {
                int fileCount = Directory.GetFiles(userFontsDir, "*", SearchOption.TopDirectoryOnly).Length;

                // フォント一覧の取得
                var fonts = this.userFontsSettingRepository.GetUserFontsSetting().Fonts;

                if (fileCount != fonts.Count)
                {
                    Logger.Debug("CheckFontsList:Update");
                    this.UpdateFontsList(userFontsDir);
                    this.Synchronize(false);
                }
            }
            catch (Exception)
            {
                // NOP
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "<保留中>")]
        private static object lockUpdateFontList = new object();

        /// <summary>
        /// フォント：フォント一覧の更新
        /// </summary>
        /// <param name="userFontsDir">ユーザーフォントのディレクトリ</param>
        public void UpdateFontsList(string userFontsDir)
        {
            lock (lockUpdateFontList)
            {
                if (this.fontInfoRepository == null)
                {
                    throw new InvalidOperationException(this.resourceWrapper.GetString("LOG_ERR_FontManagerService_UpdateFontsList_InvalidOperationException"));
                }

                if (!Directory.Exists(userFontsDir))
                {
                    return;
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
        }

        /// <summary>
        /// フォントチェンジメッセージを送信する
        /// </summary>
        public void BroadcastFontChange()
        {
            this.fontActivationService.BroadcastFont();
        }

        /// <summary>
        /// サーバより削除されたフォント情報の一覧を取得する
        /// </summary>
        /// <returns>サーバより削除されたフォント情報の一覧</returns>
        public IList<InstallFont> GetDeletedFontInformations()
        {
            return this.fontsRepository.GetInstallFontInformations(this.userStatusRepository.GetStatus().DeviceId, VaildFontType.DeletedFonts);
        }

        /// <summary>
        /// フォントをアンインストールする
        /// </summary>
        /// <param name="font">対象フォント</param>
        public void Uninstall(Font font)
        {
            // FontActivationService のアンインストールを呼び出す
            this.fontActivationService.Uninstall(font);
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
            IList<string> validContracts = new List<string>();
            foreach (Contract cont in contracts)
            {
                if (today.CompareTo(cont.ContractEndDate) <= 0)
                {
                    validContracts.Add(cont.ContractId);
                }
            }

            // 有効な契約終了日の契約情報を持っていないLETSフォントの情報を取得し、ディアクティベートする
            IList<Font> invalidFontList = new List<Font>();
            UserFontsSetting userFontsSetting = this.userFontsSettingRepository.GetUserFontsSetting();
            foreach (Font font in userFontsSetting.Fonts)
            {
                if (!font.IsLETS)
                {
                    // LETSフォントじゃなければ対象外
                    continue;
                }

                if (font.IsFreemium)
                {
                    continue;
                }

                if (font.ContractIds.Count == 0)
                {
                    invalidFontList.Add(font);
                    this.fontActivationService.Deactivate(font);
                    Logger.Debug("DeactivateExpiredFonts:" + font.Path);
                    font.IsActivated = false;
                    continue;
                }

                bool isValid = false;
                foreach (string contid in font.ContractIds)
                {
                    if (validContracts.Contains(contid))
                    {
                        isValid = true;
                        break;
                    }
                }

                if (!isValid)
                {
                    invalidFontList.Add(font);
                    this.fontActivationService.Deactivate(font);
                    Logger.Debug("DeactivateExpiredFonts:" + font.Path);
                    font.IsActivated = false;
                    continue;
                }
            }

            // 最後の期限から１ヶ月以上経過しているとき、[フォント：フォント一覧.削除対象]をTRUEに設定する
            foreach (Font font in userFontsSetting.Fonts)
            {
                if (!font.IsLETS)
                {
                    // LETSフォントじゃなければ対象外
                    continue;
                }

                if (font.IsActivated != false)
                {
                    continue;
                }

                if (font.IsFreemium)
                {
                    continue;
                }

                if (font.ContractIds.Count == 0)
                {
                    continue;
                }

                bool isValid = false;
                foreach (string contid in font.ContractIds)
                {
                    foreach (Contract contract in contracts)
                    {
                        if (contract.ContractId == contid)
                        {
                            if (today.AddMonths(-1).CompareTo(contract.ContractEndDate) < 0)
                            {
                                isValid = true;
                                break;
                            }
                        }
                    }
                }

                if (!isValid)
                {
                    this.fontActivationService.Uninstall(font);
                }
            }
        }

        private void UpdateFontInfo(InstallFontBase installFont)
        {
            var setting = this.userFontsSettingRepository.GetUserFontsSetting();
            IList<Font> userFonts = setting.Fonts;
            Font userFont = userFonts.Where(userFont => this.FormatFontID(userFont.Id).Equals(this.FormatFontID(installFont.FontId))).FirstOrDefault();

            if (userFont == null)
            {
                return;
            }

            bool isUpdated = false;

            // 契約IDを更新する
            if (installFont.ContractIds.Count > 0)
            {
                userFont.ContractIds = installFont.ContractIds;
                isUpdated = true;
            }

            // フリーミアムフラグが変更されたときは更新する
            if (installFont.IsFreemium != userFont.IsFreemium)
            {
                userFont.IsFreemium = installFont.IsFreemium;
                isUpdated = true;
            }

            if (isUpdated)
            {
                this.userFontsSettingRepository.SaveUserFontsSetting(setting);
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
                Font userFont = userFonts.Where(fonts => this.FormatFontID(fonts.Id) == this.FormatFontID(installFont.FontId)).FirstOrDefault();
                if (userFont != null)
                {
                    // インストール対象フォントと保持しているフォントのIDが一致する場合
                    // 「アクティベート状態」と「バージョン」を確認し、いずれかが不一致の場合にそれぞれ処理を実施する
                    bool matchActivate = installFont.ActivateFlg == userFont.IsActivated.GetValueOrDefault();
                    bool matchVersion = installFont.Version.Equals(userFont.Version);
                    if (!matchActivate || !matchVersion)
                    {
                        // アンインストール実施済み
                        // bool didUninstall = false;

                        /*
                         *  [フリーミアムフォントフラグ]
                         *      インストール対象フォント | 契約しているID一覧 |  保持しているフォント | 処理
                         *       false                   |  空                |  true                 | フォントアンインストール処理
                         *
                         * [フォント利用可否]
                         *      インストール対象フォント | 保持しているフォント | 処理
                         *       false                   | -                    | ディアクティベート実行
                         *
                         * [アクティベート状態]
                         *      インストール対象フォント | 保持しているフォント | 処理
                         *       true                    | false                | アクティベート実行
                         *       false                   | true                 | ディアクティベート実行
                         *
                         */

                        if (!installFont.IsFreemium && installFont.ContractIds.Count == 0 && userFont.IsFreemium)
                        {
                            this.fontActivationService.Uninstall(userFont); // ディアクティベート実行 + 削除フラグ
                        }
                        else if (!installFont.IsAvailableFont)
                        {
                            this.fontActivationService.Deactivate(userFont); // ディアクティベート実行
                        }
                        else if (installFont.ActivateFlg && !userFont.IsActivated.GetValueOrDefault())
                        {
                            if (installFont.ContractIds.Count > 0 || installFont.IsFreemium)
                            {
                                this.fontActivationService.Activate(userFont); // アクティベート実行
                            }
                        }
                        else if (!installFont.ActivateFlg && userFont.IsActivated.GetValueOrDefault())
                        {
                            this.fontActivationService.Deactivate(userFont); // ディアクティベート実行
                        }
                        else if (installFont.ContractIds.Count == 0 && !installFont.IsFreemium && userFont.IsActivated.GetValueOrDefault())
                        {
                            this.fontActivationService.Deactivate(userFont); // ディアクティベート実行
                        }

                        /*
                         *  [フォントバージョンアップ要否]
                         *      インストール対象フォント | 保持しているフォント | 処理
                         *       true                    | -                    | インストール対象フォントに加える
                         *                               |                      | or 削除フラグ=TRUE & レジストリ削除
                         */
                        if (installFont.NeedFontVersionUpdate || !matchVersion)
                        {
                            if (!this.FontExitsInUserFonts(installFont.FileName))
                            {
                                // ユーザーフォントフォルダにフォントファイルがない場合、インストール対象に加える
                                Logger.Warn($"[Ph.2] CollectInstallTargetFontFromFontInfomations(NeedFontVersionUpdate): install font={installFont.FileName}");
                                this.volatileSettingRepository.GetVolatileSetting().InstallTargetFonts.Add(installFont);

                                // 削除フラグを落とす
                                this.fontActivationService.RemoveTargetSettings(userFont, false);
                            }
                            else
                            {
                                Logger.Warn($"[Ph.2] CollectInstallTargetFontFromFontInfomations(NeedFontVersionUpdate): remove font={installFont.FileName}");

                                // フォントレジストリ削除
                                this.fontActivationService.DelRegistory(userFont);

                                // 削除フラグを立てる
                                this.fontActivationService.RemoveTargetSettings(userFont);

                                // フォントダウンロード中断メッセージ
                                this.FontDownloadCancelledEvent(installFont);
                            }
                        }

                        // フォント情報を更新する
                        this.UpdateFontInfo(installFont);
                    }
                }
                else
                {
                    // インストール対象フォントと保持しているフォントのIDが一致しない場合
                    // 「アクティベート状態」がTrueかつ(有効な契約ありまたはフリーミアム)であればインストール対象フォントに追加
                    if (installFont.ActivateFlg && (installFont.ContractIds.Count > 0 || installFont.IsFreemium))
                    {
                        if (!this.FontExitsInUserFonts(installFont.FileName))
                        {
                            if (!this.volatileSettingRepository.GetVolatileSetting().InstallTargetFonts.Contains(installFont))
                            {
                                // ユーザーフォントフォルダにフォントファイルがない場合、インストール対象に加える
                                Logger.Warn($"[Ph.2] CollectInstallTargetFontFromFontInfomations:DiffID: install font={installFont.FileName}");
                                this.volatileSettingRepository.GetVolatileSetting().InstallTargetFonts.Add(installFont);
                            }
                        }
                        else
                        {
                            Logger.Warn($"[Ph.2] CollectInstallTargetFontFromFontInfomations:DiffID:already exist font={installFont.FileName}");

                            // フォントダウンロード中断メッセージ
                            this.FontDownloadCancelledEvent(installFont);
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

#pragma warning disable SA1201 // Elements should appear in the correct order
        private static DateTime notEnoughCapacityMessage = new DateTime(0);
#pragma warning restore SA1201 // Elements should appear in the correct order

        /// <summary>
        /// インストール対象フォントをダウンロードする
        /// </summary>
#pragma warning disable CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
        private async void DownloadFontFile(IList<InstallFont> installFontList)
#pragma warning restore CS1998 // 非同期メソッドは、'await' 演算子がないため、同期的に実行されます
        {
            lock (SynchronizeLockHandler)
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
                        if (notEnoughCapacityMessage.AddHours(24).CompareTo(DateTime.Now) < 0)
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
                            Logger.Error(this.resourceWrapper.GetString("FUNC_01_03_01_NOTIFIED_FailedToDownloadFonts"));
                            Logger.Error(this.resourceWrapper.GetString("FUNC_01_03_01_NOTIFIED_LackOfDiscSpace"));
                            this.ShowErrorDialogEvent(
                                this.resourceWrapper.GetString("FUNC_01_03_01_NOTIFIED_FailedToDownloadFonts"),
                                this.resourceWrapper.GetString("FUNC_01_03_01_NOTIFIED_LackOfDiscSpace"));

                            notEnoughCapacityMessage = DateTime.Now;
                        }

                        return;
                    }

                    // フォントダウンロード
                    bool isInstall = false;
                    VolatileSetting memory = this.volatileSettingRepository.GetVolatileSetting();
                    try
                    {
                        double compFileSize = 0;
                        double totalFileSize = installFontList.Sum(font => font.FileSize);

                        memory.IsDownloading = true;

                        foreach (InstallFont installFont in installFontList)
                        {
                            this.FontStartDownloadEvent(installFont, compFileSize, totalFileSize);

                            try
                            {
                                Logger.Debug("DownloadFonts:FontId=" + installFont.FontId);
                                var fileStream = this.fontsRepository.DownloadFonts(this.userStatusRepository.GetStatus().DeviceId, installFont.FontId);
                                installFont.Path = fileStream.Name;
                                fileStream.Close();

                                compFileSize += installFont.FileSize;

                                Logger.Debug("DownloadFontFile:Path=" + installFont.Path);

                                if (!this.userStatusRepository.GetStatus().IsLoggingIn)
                                {
                                    break;
                                }

                                if (installFont.Path != null)
                                {
                                    Font f = new Font(installFont.FontId, installFont.FileName, true, installFont.ActivateFlg, installFont.DisplayFontName, installFont.Version, string.Empty, installFont.IsFreemium, installFont.ContractIds);
                                    f.Path = installFont.Path;
                                    if (this.fontActivationService.Install(f))
                                    {
                                        isInstall = true;
                                        var notifyList = memory.NotificationFonts;
                                        notifyList.Add(installFont);
                                    }
                                }

                                memory.InstallTargetFonts.Remove(installFont);
                            }
                            catch (Exception ex)
                            {
                                Logger.Debug("DownloadFontFile:" + ex.StackTrace);

                                this.FontDownloadFailedEvent(installFont);

                                // フォントダウンロードの失敗は、通信エラーとして扱わない
                                this.volatileSettingRepository.GetVolatileSetting().IsConnected = true;
                            }
                        }
                    }
                    finally
                    {
                        memory.IsDownloading = false;
                        memory.CompletedDownload = true;
                    }

                    if (isInstall)
                    {
                        if (this.userStatusRepository.GetStatus().IsLoggingIn)
                        {
                            this.FontDownloadCompletedEvent(memory.NotificationFonts);
                        }

                        memory.NotificationFonts.Clear();
                    }
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

                // 保存されているレジストリキーが正しくない場合、再アクティベートを行いレジストリキーを更新する
                string savedRegKey = savedFont.RegistryKey;
                if (!string.IsNullOrEmpty(savedRegKey))
                {
                    if (savedRegKey.CompareTo(idInfo.NameInfo.UniqueName) != 0)
                    {
                        this.fontActivationService.Deactivate(savedFont);
                        this.fontActivationService.Activate(savedFont);
                        savedFont.RegistryKey = idInfo.NameInfo.UniqueName;
                    }
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
                !string.IsNullOrEmpty(idInfo.NameInfo.Ids.FontId),
                false,
                idInfo.NameInfo.UniqueName,
                idInfo.NameInfo.Version,
                string.Empty,
                false,
                new List<string>(),
                false);

            return addFont;
        }

        private bool FontExitsInUserFonts(string filename)
        {
            try
            {
                // ユーザー配下のフォントフォルダ
                var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var userFontsDir = @$"{local}\Microsoft\Windows\Fonts";

                if (!Directory.Exists(userFontsDir))
                {
                    return false;
                }

                string fontpath = Path.Combine(userFontsDir, filename);
                return File.Exists(fontpath);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace);
            }

            return false;
        }
    }
}
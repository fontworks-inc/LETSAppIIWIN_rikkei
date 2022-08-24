using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ApplicationService.Entities;
using ApplicationService.Exceptions;
using ApplicationService.Interfaces;
using Core.Entities;
using Core.Interfaces;
using NLog;

namespace ApplicationService.Startup
{
    /// <summary>
    /// 起動時処理に関するサービスクラス
    /// </summary>
    public class StartupService : IStartupService
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>
        /// 文言の取得を行うインスタンス
        /// </summary>
        private IResourceWrapper resourceWrapper = null;

        /// <summary>
        /// プログラムフォルダからバージョンを取得するサービス
        /// </summary>
        private IApplicationVersionService applicationVersionService;

        /// <summary>
        /// 指定のプロセスを実施するサービス
        /// </summary>
        private IStartProcessService startProcessService;

        /// <summary>
        /// フォント管理に関する処理を行うサービス
        /// </summary>
        private IFontManagerService fontManagerService = null;

        /// <summary>
        /// メモリで保持する情報を格納するリポジトリ
        /// </summary>
        private IVolatileSettingRepository volatileSettingRepository = null;

        /// <summary>
        /// 共通保存情報情報を格納するリポジトリ
        /// </summary>
        private IApplicationRuntimeRepository applicationRuntimeRepository;

        /// <summary>
        /// ユーザ別フォント情報を格納するリポジトリ
        /// </summary>
        private IUserFontsSettingRepository userFontsSettingRepository;

        /// <summary>
        /// 契約情報を格納するリポジトリ
        /// </summary>
        private IContractsAggregateRepository contractsAggregateRepository = null;

        /// <summary>
        /// 端末情報を格納するリポジトリ
        /// </summary>
        private IDevicesRepository devicesRepository = null;

        /// <summary>
        /// クライアントアプリの起動Ver情報のファイルリポジトリ
        /// </summary>
        private IClientApplicationVersionRepository clientApplicationVersionFileRepository;

        /// <summary>
        /// ユーザ別ステータス情報を格納するリポジトリ
        /// </summary>
        private IUserStatusRepository userStatusRepository = null;

        /// <summary>
        /// 未読お知らせ情報を格納するリポジトリ
        /// </summary>
        private IUnreadNoticeRepository unreadNoticeRepository = null;

        /// <summary>
        /// フォントセキュリティ情報を格納するリポジトリ
        /// </summary>
        private IFontSecurityRepository fontSecurityRepository = null;

        /// <summary>
        /// フォント内部情報を格納するリポジトリ
        /// </summary>
        private IFontFileRepository fontInfoRepository = null;

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="resourceWrapper">文言の取得を行うインスタンス</param>
        /// <param name="applicationVersionService">プログラムフォルダからバージョンを取得するサービス</param>
        /// <param name="startProcessService">指定のプロセスを実施するサービス</param>
        /// <param name="fontManagerService">フォント管理に関する処理を行うサービス</param>
        /// <param name="volatileSettingRepository">メモリで保持する情報を格納するリポジトリ</param>
        /// <param name="applicationRuntimeRepository">共通保存情報を格納するリポジトリ</param>
        /// <param name="userFontsSettingRepository">ユーザ別フォント情報を格納するリポジトリ</param>
        /// <param name="contractsAggregateRepository">契約情報を格納するリポジトリ</param>
        /// <param name="devicesRepository">端末情報を格納するリポジトリ</param>
        /// <param name="userStatusRepository">ユーザ別ステータス情報を格納するリポジトリ</param>
        /// <param name="clientApplicationVersionFileRepository">クライアントアプリの起動Ver情報のファイルリポジトリ</param>
        /// <param name="unreadNoticeRepository">未読お知らせ情報を格納するリポジトリ</param>
        /// <param name="fontSecurityRepository">フォントセキュリティ情報を格納するリポジトリ</param>
        /// <param name="fontInfoRepository">フォント内部情報を格納するリポジトリ</param>
        public StartupService(
            IResourceWrapper resourceWrapper,
            IApplicationVersionService applicationVersionService,
            IStartProcessService startProcessService,
            IFontManagerService fontManagerService,
            IVolatileSettingRepository volatileSettingRepository,
            IApplicationRuntimeRepository applicationRuntimeRepository,
            IUserFontsSettingRepository userFontsSettingRepository,
            IContractsAggregateRepository contractsAggregateRepository,
            IDevicesRepository devicesRepository,
            IUserStatusRepository userStatusRepository,
            IClientApplicationVersionRepository clientApplicationVersionFileRepository,
            IUnreadNoticeRepository unreadNoticeRepository,
            IFontSecurityRepository fontSecurityRepository,
            IFontFileRepository fontInfoRepository)
        {
            this.resourceWrapper = resourceWrapper;
            this.applicationVersionService = applicationVersionService;
            this.startProcessService = startProcessService;
            this.fontManagerService = fontManagerService;
            this.volatileSettingRepository = volatileSettingRepository;
            this.applicationRuntimeRepository = applicationRuntimeRepository;
            this.userFontsSettingRepository = userFontsSettingRepository;
            this.contractsAggregateRepository = contractsAggregateRepository;
            this.devicesRepository = devicesRepository;
            this.userStatusRepository = userStatusRepository;
            this.clientApplicationVersionFileRepository = clientApplicationVersionFileRepository;
            this.unreadNoticeRepository = unreadNoticeRepository;
            this.fontSecurityRepository = fontSecurityRepository;
            this.fontInfoRepository = fontInfoRepository;
        }

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="resourceWrapper">文言の取得を行うインスタンス</param>
        /// <param name="volatileSettingRepository">メモリで保持する情報を格納するリポジトリ</param>
        /// <param name="devicesRepository">端末情報を格納するリポジトリ</param>
        public StartupService(
            IResourceWrapper resourceWrapper,
            IVolatileSettingRepository volatileSettingRepository,
            IDevicesRepository devicesRepository)
        {
            this.resourceWrapper = resourceWrapper;
            this.volatileSettingRepository = volatileSettingRepository;
            this.devicesRepository = devicesRepository;
        }

        /// <summary>
        /// 起動時チェック処理
        /// </summary>
        /// <param name="shutdownClientApplicationRequiredEvent">クライアントアプリを終了するイベント</param>
        /// <param name="forceUpdateEvent">強制アップデートを実施するイベント</param>
        /// <param name="downloadCompletedEvent">ダウンロード完了時に実施するイベント</param>
        /// <param name="existsUpdateProgramEvent">更新プログラムがダウンロード済みのときに呼び出されるイベント</param>
        /// <param name="startDownloadEvent">更新プログラムのダウンロードを実施するイベント</param>
        /// <param name="notContainsDeviceEvent">自デバイスの情報が端末情報に含まれていない場合に呼び出されるイベント</param>
        /// <param name="existsUnreadNotificationEvent">未読お知らせが有るときに呼び出されるイベント</param>
        /// <param name="detectionFontCopyEvent">他端末からフォントがコピーされていたときに呼び出されるイベント</param>
        /// <param name="multipleInfo">多重起動チェック情報</param>
        /// <returns>チェック結果を返す</returns>
        public bool IsCheckedStartup(
            ShutdownClientApplicationRequiredEvent shutdownClientApplicationRequiredEvent,
            ForceUpdateEvent forceUpdateEvent,
            DownloadCompletedEvent downloadCompletedEvent,
            ExistsUpdateProgramEvent existsUpdateProgramEvent,
            StartDownloadEvent startDownloadEvent,
            NotContainsDeviceEvent notContainsDeviceEvent,
            ExistsUnreadNotificationEvent existsUnreadNotificationEvent,
            DetectionFontCopyEvent detectionFontCopyEvent,
            MultiplePreventionInfo multipleInfo)
        {
            try
            {
                Logger.Info(this.resourceWrapper.GetString("LOG_INFO_StartupService_IsCheckedStartup_Start"));

                // 開始時に起動時チェック処理を「未設定」に設定する
                this.volatileSettingRepository.GetVolatileSetting().IsCheckedStartup = false;

                if (this.applicationVersionService == null
                    || this.startProcessService == null
                    || this.fontManagerService == null
                    || this.applicationRuntimeRepository == null
                    || this.userFontsSettingRepository == null
                    || this.contractsAggregateRepository == null
                    || this.userStatusRepository == null
                    || this.clientApplicationVersionFileRepository == null
                    || this.unreadNoticeRepository == null
                    || this.fontSecurityRepository == null
                    || this.fontInfoRepository == null)
                {
                    throw new InvalidOperationException(this.resourceWrapper.GetString("LOG_ERR_StartupService_IsCheckedStartup_InvalidOperationException"));
                }

                // 強制アップデートチェックを実施する（実行結果は以降の起動時チェックに影響しない）
                this.ForceUpdateCheck(forceUpdateEvent, downloadCompletedEvent);

                // 起動指定バージョンチェックを実施し、起動する別バージョンのクライアントアプリがある場合は起動中のクライアントアプリを終了する
                if (!this.StartingVersionCheck(multipleInfo))
                {
                    shutdownClientApplicationRequiredEvent();
                    return false;
                }

                // ログイン状態確認処理を実行し、ログアウト中になる場合は以後の起動時チェックを行わない
                UserStatus userStatus = this.userStatusRepository.GetStatus();
                if (!userStatus.IsLoggingIn || !this.ConfirmLoginStatus(userStatus.DeviceId, notContainsDeviceEvent))
                {
                    Logger.Info(this.resourceWrapper.GetString("LOG_INFO_StartupService_IsCheckedStartup_LoggingOut"));
                    return false;
                }

                // 次バージョンチェックを実行し、配信サーバアクセスでエラーが発生したときは以後の起動時チェックを行わない
                if (!this.NextVersionCheck(existsUpdateProgramEvent, startDownloadEvent))
                {
                    return false;
                }

                // ライセンス更新チェック
                ContractsResult contractsResult = this.GetContractsAggregate(userStatus.DeviceId);
                if (!contractsResult.IsCashed && contractsResult.ContractsAggregate.NeedContractRenewal)
                {
                    // キャッシュを利用していない かつ 契約更新の必要があった場合、メモリ情報を通知ありとしイベントを実行
                    // 実際のアイコン表示変更は呼び出し元で行う
                    Logger.Info(this.resourceWrapper.GetString("LOG_INFO_StartupService_IsCheckedStartup_IsNoticed"));
                    this.volatileSettingRepository.GetVolatileSetting().IsNoticed = true;
                    detectionFontCopyEvent();
                }

                // お知らせ有無チェック
                if (!this.ExistsNotificationCheck(existsUnreadNotificationEvent))
                {
                    return false;
                }

                Task.Run(() => this.StartupFontCheck(contractsResult.ContractsAggregate.Contracts, detectionFontCopyEvent));

                // 起動時チェック状態の保存(チェックした日時と「処理済みである」という情報)
                VolatileSetting volatileSetting = this.volatileSettingRepository.GetVolatileSetting();
                volatileSetting.CheckedStartupAt = DateTime.Now;
                volatileSetting.IsCheckedStartup = true;

                Logger.Info(this.resourceWrapper.GetString("LOG_INFO_StartupService_IsCheckedStartup_Result_True"));

                return true;
            }
            catch (GetContractsAggregateException e)
            {
                Logger.Warn(e);
                return false;
            }
        }

        /// <summary>
        /// 強制アップデートチェック
        /// </summary>
        /// <param name="forceUpdateEvent">強制アップデートを実施するイベント</param>
        /// <param name="downloadCompletedEvent">ダウンロード完了時に実施するイベント</param>
        public void ForceUpdateCheck(ForceUpdateEvent forceUpdateEvent, DownloadCompletedEvent downloadCompletedEvent)
        {
            // 共通保存情報より「ダウンロード状態」、「強制/任意」を取得
            Installer installer = this.applicationRuntimeRepository.GetApplicationRuntime().NextVersionInstaller;
            if (installer == null)
            {
                // インストーラ情報がなければアップデート不要
                return;
            }

            DownloadStatus downloadStatus = installer.DownloadStatus;
            bool isForceUpdate = installer.ApplicationUpdateType;

            // ダウンロード状態が「ダウンロード完了」かつ、現在「アップデート中」ではない
            if (downloadStatus.Equals(DownloadStatus.Completed) && !this.volatileSettingRepository.GetVolatileSetting().IsUpdating)
            {
                if (isForceUpdate)
                {
                    // アップデートが「強制」であれば強制アップデート実行
                    //forceUpdateEvent();   //とりあえず無効化
                }
                else
                {
                    // メモリに「ダウンロード済み」と設定
                    VolatileSetting volatileSetting = this.volatileSettingRepository.GetVolatileSetting();
                    volatileSetting.CompletedDownload = true;

                    // アップデートが「任意」であればダウンロード完了時の処理を実施
                    downloadCompletedEvent();
                }
            }
        }

        /// <summary>
        /// 起動指定バージョンチェック
        /// </summary>
        /// <param name="multipleInfo">多重起動チェック情報</param>
        /// <returns>再起動が不要な場合true、必要な場合false</returns>
        public bool StartingVersionCheck(MultiplePreventionInfo multipleInfo)
        {
            // 起動Ver情報取得APIを呼び出し、「アプリバージョン」（起動指定バージョン）を取得する
            string startingVersion = string.Empty;
            try
            {
                ClientApplicationVersion applicationVersion = this.clientApplicationVersionFileRepository.GetClientApplicationVersion(
                    this.userStatusRepository.GetStatus().DeviceId,
                    this.volatileSettingRepository.GetVolatileSetting().AccessToken);
                startingVersion = applicationVersion != null ? applicationVersion.Version : null;
            }
            catch (Exception e)
            {
                Logger.Debug(e, this.resourceWrapper.GetString("LOG_ERR_StartingVersionCheck_clientApplicationVersionAPIRepository"));

                // APIから取れなければキャッシュからバージョンを取得する
                startingVersion = this.clientApplicationVersionFileRepository.GetClientApplicationVersion().Version;
            }

            if (string.IsNullOrEmpty(startingVersion))
            {
                // 起動Verが取得できなければ再起動無しで続行する
                return true;
            }

            // 自バージョン取得
            string selfVersion = this.applicationVersionService.GetVerison();

            // 起動指定バージョンが自バージョンと異なる場合
            if (!startingVersion.Equals(selfVersion))
            {
                // 起動指定バージョンのプログラムフォルダが存在する場合、そちらのクライアントアプリケーションで再起動する
                string startingVersionDirectoryPath = this.applicationVersionService.GetTargetVerisonDirectory(startingVersion);
                if (Directory.Exists(startingVersionDirectoryPath))
                {
                    string programPath = Path.Combine(startingVersionDirectoryPath, "LETS.exe");
                    if (File.Exists(programPath))
                    {
                        // Mutexを削除する
                        if (multipleInfo != null && multipleInfo.HasHandle)
                        {
                            multipleInfo.MutexInfo.ReleaseMutex();
                            multipleInfo.MutexInfo.Close();
                            multipleInfo.HasHandle = false;
                        }

                        this.startProcessService.StartProcessAdministrator(startingVersionDirectoryPath, "LETS.exe", null, false);

                        Logger.Info(this.resourceWrapper.GetString("LOG_Info_StartingVersionCheck_RebootClientApplication"));
                        return false;
                    }
                }
            }

            // 再起動不要
            return true;
        }

        /// <summary>
        /// 次バージョンチェック
        /// </summary>
        /// <param name="existsUpdateProgramEvent">更新プログラムがダウンロード済みのときに呼び出されるイベント</param>
        /// <param name="startDownloadEvent">更新プログラムのダウンロードを実施するイベント</param>
        /// <returns>起動時チェック処理を続行する場合はtrue、起動時チェック処理を終了する場合はfalse</returns>
        public bool NextVersionCheck(ExistsUpdateProgramEvent existsUpdateProgramEvent, StartDownloadEvent startDownloadEvent)
        {
            // 更新情報取得APIを呼び出し、クライアントアプリの更新情報を取得する
            ClientApplicationUpdateInfomation clientApplicationUpdateInfomation = null;
            try
            {
                clientApplicationUpdateInfomation = this.clientApplicationVersionFileRepository.GetClientApplicationUpdateInfomation(
                    this.userStatusRepository.GetStatus().DeviceId,
                    this.volatileSettingRepository.GetVolatileSetting().AccessToken);
            }
            catch (Exception e)
            {
                Logger.Warn(e, this.resourceWrapper.GetString("LOG_ERR_NextVersionCheck_clientApplicationVersionAPIRepository"));

                // APIでエラーがあった場合、起動時チェックを終了する
                return false;
            }

            // 更新情報がある場合
            if (clientApplicationUpdateInfomation != null)
            {
                if (this.CheckVersionInstalled(clientApplicationUpdateInfomation.ClientApplicationVersion.Version))
                {
                    // 次バージョンがインストール済みの時は処理を続行する
                    return true;
                }

                if (this.CheckDownloadCompleted(clientApplicationUpdateInfomation))
                {
                    // 次バージョンの更新プログラムがダウンロードされている場合の処理
                    existsUpdateProgramEvent();
                }
                else
                {
                    // 次バージョンの更新プログラムがダウンロードされていない場合の処理
                    Installer installer = new Installer()
                    {
                        Version = clientApplicationUpdateInfomation.ClientApplicationVersion.Version,
                        ApplicationUpdateType = clientApplicationUpdateInfomation.ApplicationUpdateType,
                        Url = clientApplicationUpdateInfomation.ClientApplicationVersion.Url,
                    };

                    ApplicationRuntime applicationRuntime = this.applicationRuntimeRepository.GetApplicationRuntime();
                    if (applicationRuntime == null)
                    {
                        applicationRuntime = new ApplicationRuntime();
                    }

                    applicationRuntime.NextVersionInstaller = installer;

                    this.applicationRuntimeRepository.SaveApplicationRuntime(applicationRuntime);

                    // 次バージョンの更新プログラムがダウンロードされていない場合の処理
                    startDownloadEvent();
                }
            }

            return true;
        }

        /// <summary>
        /// ログイン状態確認処理
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="notContainsDeviceEvent">自デバイスの情報が含まれていない場合に呼び出されるイベント</param>
        /// <returns>ログイン中の時にtrue, ログアウト時にfalseを返す</returns>
        /// <summary>
        public bool ConfirmLoginStatus(string deviceId, NotContainsDeviceEvent notContainsDeviceEvent)
        {
            try
            {
                IList<Device> devices = this.GetAllDevices(deviceId);
                if (devices.Count <= 0)
                {
                    Logger.Debug("ConfirmLoginStatus:デバイスリストが存在しない場合ログアウト");
                    notContainsDeviceEvent();
                    return false;
                }

                if (!devices.Any(device => device.DeviceId == deviceId))
                {
                    // 自デバイスの情報が含まれていない場合、イベントを実行する
                    Logger.Debug("ConfirmLoginStatus:自デバイスの情報が含まれていない場合、イベントを実行する");
                    notContainsDeviceEvent();
                    return false;
                }

                Logger.Debug("ConfirmLoginStatus:true");
                return true;
            }
            catch (GetAllDevicesException ex)
            {
                // 全端末情報を取得で例外が発生した場合は失敗扱い
                Logger.Debug("ConfirmLoginStatus:全端末情報を取得で例外が発生した場合は失敗扱い：" + ex.StackTrace);
                return false;
            }
            catch (Exception ex)
            {
                // 全端末情報を取得で例外が発生した場合は失敗扱い
                Logger.Debug("ConfirmLoginStatus:その他のエラー:" + ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// お知らせ有無チェック
        /// </summary>
        /// <param name="existsUnreadNotificationEvent">未読お知らせが有るときに呼び出されるイベント</param>
        /// <returns>起動時チェック処理を続行する場合はtrue、起動時チェック処理を終了する場合はfalse</returns>
        public bool ExistsNotificationCheck(ExistsUnreadNotificationEvent existsUnreadNotificationEvent)
        {
            // お知らせ情報取得APIを呼び出し、「未読お知らせ情報」を取得する
            UnreadNotice unreadNotice = null;
            try
            {
                unreadNotice = this.unreadNoticeRepository.GetUnreadNotice(
                this.userStatusRepository.GetStatus().DeviceId, this.volatileSettingRepository.GetVolatileSetting().AccessToken);

                if (unreadNotice.ExistsLatestNotice)
                {
                    // 「未読お知らせ有り」の場合の処理
                    existsUnreadNotificationEvent(unreadNotice.Total);
                }
            }
            catch (Exception e)
            {
                Logger.Warn(e, this.resourceWrapper.GetString("LOG_ERR_ExistsNotificationCheck_unreadNoticeRepository"));

                // APIでエラーが発生した場合、起動時チェック処理を終了する
                return false;
            }

            return true;
        }

        /// <summary>
        /// 他端末コピーチェック
        /// </summary>
        /// <param name="detectionFontCopyEvent">他端末からフォントがコピーされていたときに呼び出されるイベント</param>
        /// <returns>起動時チェック処理を続行する場合はtrue、起動時チェック処理を終了する場合はfalse</returns>
        public bool FontCopyCheck(DetectionFontCopyEvent detectionFontCopyEvent)
        {
            Logger.Warn($"[INFO] FontCopyCheck");

            // ユーザID取得APIを呼び出し、ユーザIDを取得する
            string userId = null;
            try
            {
                UserId userIdData = this.fontSecurityRepository.GetUserId(
                this.userStatusRepository.GetStatus().DeviceId, this.volatileSettingRepository.GetVolatileSetting().AccessToken);
                userId = this.FormatFontID7digit0padding(userIdData.ToString());
            }
            catch (Exception e)
            {
                Logger.Warn(e, this.resourceWrapper.GetString("LOG_ERR_FontCopyCheck_fontSecurityRepository_GetUserId"));

                // APIでエラーが発生した場合、起動時チェック処理を終了する
                return false;
            }

            // フォントコピー検知フラグ
            bool existCopiedFont = false;

            // フォント一覧の取得
            var fonts = this.userFontsSettingRepository.GetUserFontsSetting().Fonts;

            // ユーザーフォントフォルダ内にあるフォントについて、それぞれ処理を行う
            foreach (string filePath in this.GetFilePathsIntUserFontFile())
            {
                // [フォント：フォント一覧]に同じファイル名のデータが存在するか確認する
                Font userFont = fonts.FirstOrDefault(f => f.Path == filePath);
                if (userFont != null && !userFont.IsLETS)
                {
                    // 同名フォントが存在し、かつ「LETSフォント」ではない場合、次のファイルにスキップする
                    continue;
                }

                // フォントのユーザIDを取得し、自身のユーザIDと比較する
                FontIdInfo fontInfo = this.fontInfoRepository.GetFontInfo(filePath);
                string fontUserId = this.FormatFontID7digit0padding(fontInfo.UserId);
                if (!userId.Equals(fontUserId))
                {
                    existCopiedFont = true;

                    // メモリに「通知あり」と設定し、アイコンを変更
                    VolatileSetting volatileSetting = this.volatileSettingRepository.GetVolatileSetting();
                    volatileSetting.IsNoticed = true;
                    detectionFontCopyEvent();

                    try
                    {
                        this.fontSecurityRepository.PostFontFileCopyDetection(
                            this.userStatusRepository.GetStatus().DeviceId,
                            this.volatileSettingRepository.GetVolatileSetting().AccessToken,
                            userId.ToString(),
                            fontUserId,
                            fontInfo.DeviceId,
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    catch (Exception e)
                    {
                        Logger.Warn(e, this.resourceWrapper.GetString("LOG_ERR_FontCopyCheck_fontSecurityRepository_PostFontFileCopyDetection"));

                        // APIでエラーが発生した場合、起動時チェック処理を終了する
                        return false;
                    }
                }
            }

            if (!existCopiedFont)
            {
                this.fontSecurityRepository.NotifyVerifiedFonts(
                            this.userStatusRepository.GetStatus().DeviceId,
                            this.volatileSettingRepository.GetVolatileSetting().AccessToken);
            }

            return true;
        }

        /// <summary>
        /// サーバから削除されたフォントの削除チェック
        /// </summary>
        /// <returns>起動時チェック処理を続行する場合はtrue、起動時チェック処理を終了する場合はfalse</returns>
        public bool DeletedFontCheck()
        {
            // APIから削除されたフォント情報を取得する
            IList<InstallFont> deletedFontInformations = null;
            try
            {
                deletedFontInformations = this.fontManagerService.GetDeletedFontInformations();
            }
            catch (Exception e)
            {
                Logger.Warn(e, this.resourceWrapper.GetString("LOG_ERR_DeletedFontCheck_GetInstallFontInformations"));

                // APIでエラーが発生した場合、起動時チェック処理を終了する
                return false;
            }

            if (deletedFontInformations == null || deletedFontInformations.Count == 0)
            {
                // 削除されたフォントが1件も無ければ処理しない
                return true;
            }

            // サーバから削除されているフォントをマシンからもアンインストールする
            IList<Font> userFonts = this.userFontsSettingRepository.GetUserFontsSetting().Fonts;
            foreach (InstallFont deletedFont in deletedFontInformations)
            {
                Font userFont = userFonts.Where(fonts => this.FormatUserID6digit0padding(fonts.Id) == this.FormatUserID6digit0padding(deletedFont.FontId)).FirstOrDefault();
                if (userFont == null)
                {
                    // 削除されたフォントがマシン上になければ次のフォントにスキップ
                    continue;
                }

                // フォントをアンインストールする
                this.fontManagerService.Uninstall(userFont);
            }

            return true;
        }

        /// <summary>
        /// 起動時フォントチェック
        /// </summary>
        /// <param name="contracts">契約情報</param>
        /// <param name="detectionFontCopyEvent">コピーされたフォント発見時の処理</param>
        private async Task StartupFontCheck(IList<Contract> contracts, DetectionFontCopyEvent detectionFontCopyEvent)
        {
            Logger.Debug("StartupFontCheck:Start");

            // フォントアクティブ/ディアクティブ情報の同期
            Logger.Debug("StartupFontCheck:Synchronize:Start");
            this.fontManagerService.Synchronize(true);

            // 契約切れフォントのディアクティベート
            Logger.Debug("StartupFontCheck:DeactivateExpiredFonts:Start");
            this.fontManagerService.DeactivateExpiredFonts(contracts);

            // サーバから削除されたフォントの削除チェック
            Logger.Debug("StartupFontCheck:DeletedFontCheck:Start");
            this.DeletedFontCheck();

            // wait download
            bool doSecondCheck = false;
            while (!doSecondCheck)
            {
                if (this.fontManagerService.GetIsFirstDownloadCompleted())
                {
                    // 契約切れフォントのディアクティベート
                    Logger.Debug("StartupFontCheck:2nd:DeactivateExpiredFonts:Start");
                    this.fontManagerService.DeactivateExpiredFonts(contracts);

                    // サーバから削除されたフォントの削除チェック
                    Logger.Debug("StartupFontCheck:2nd:DeletedFontCheck:Start");
                    this.DeletedFontCheck();

                    // 他端末コピーチェック
                    Logger.Debug("StartupFontCheck:2nd:FontCopyCheck:Start");
                    this.FontCopyCheck(detectionFontCopyEvent);

                    doSecondCheck = true;
                    break;
                }

                await Task.Delay(1000);
            }

            Logger.Debug("StartupFontCheck:End");
        }

        /// <summary>
        /// 起草指定されているバージョンのプログラムがインストールされているか
        /// </summary>
        private bool CheckVersionInstalled(string version)
        {
            // 起動指定バージョンのプログラムフォルダが存在する場合、そちらのクライアントアプリケーションで再起動する
            string versionDirectoryPath = this.applicationVersionService.GetTargetVerisonDirectory(version);
            if (Directory.Exists(versionDirectoryPath))
            {
                if (File.Exists(Path.Combine(versionDirectoryPath, "LETS.exe")))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 指定の更新情報と、共通保存が持つ更新情報を確認し、既に更新プログラムがダウンロードされているか確認する
        /// </summary>
        /// <param name="clientApplicationUpdateInfomation">更新情報</param>
        /// <returns>ダウンロード済みである場合はtrue、そうでなければfalse</returns>
        private bool CheckDownloadCompleted(ClientApplicationUpdateInfomation clientApplicationUpdateInfomation)
        {
            // 共通保存が更新情報を保持していなければ、ダウンロードしていない
            Installer installer = this.applicationRuntimeRepository.GetApplicationRuntime().NextVersionInstaller;
            if (installer == null || string.IsNullOrEmpty(installer.Version))
            {
                return false;
            }

            // APIから取得した更新情報と、共通保存情報の更新情報が一致しなければダウンロードしていない
            string targetVersion = clientApplicationUpdateInfomation.ClientApplicationVersion.Version;
            string cacheVersion = installer.Version;
            if (!targetVersion.Equals(cacheVersion))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 全端末情報を取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <returns>ユーザに紐づく全端末情報(削除済みデータを除く)</returns>
        private IList<Device> GetAllDevices(string deviceId)
        {
            try
            {
                string accessToken = this.volatileSettingRepository.GetVolatileSetting().AccessToken;
                return this.devicesRepository.GetAllDevices(deviceId, accessToken);
            }
            catch (Exception e)
            {
                string message = this.resourceWrapper.GetString("LOG_WARN_StartupService_GetAllDevicesException");
                Logger.Warn(e, message);
                throw new GetAllDevicesException(message, e);
            }
        }

        /// <summary>
        /// 契約情報を取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <returns>契約情報の集合体</returns>
        private ContractsResult GetContractsAggregate(string deviceId)
        {
            try
            {
                string accessToken = this.volatileSettingRepository.GetVolatileSetting().AccessToken;
                return new ContractsResult(this.contractsAggregateRepository.GetContractsAggregate(deviceId, accessToken));
            }
            catch (Exception e)
            {
                // 取得に失敗した場合はキャッシュから情報を取得する
                Logger.Warn(e, this.resourceWrapper.GetString("LOG_WARN_StartupService_GetContractsAggregateException"));
                return new ContractsResult(this.contractsAggregateRepository.GetContractsAggregate(), true);
            }
        }

        /// <summary>
        /// ユーザーフォントフォルダに存在するファイルのパスを取得する
        /// </summary>
        /// <returns>ユーザーフォントフォルダに存在するファイルパスの集合体</returns>
        private IEnumerable<string> GetFilePathsIntUserFontFile()
        {
            var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var userFontsDir = @$"{local}\Microsoft\Windows\Fonts";
            if (!Directory.Exists(userFontsDir))
            {
                // 空のリストを返す
                return new List<string>();
            }

            return Directory.EnumerateFiles(userFontsDir, "*", SearchOption.TopDirectoryOnly);
        }

        /// <summary>
        /// ユーザIDを6桁0埋めにフォーマットする
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>フォーマットしたユーザID</returns>
        private string FormatUserID6digit0padding(string id)
        {
            string formatedId = "000000" + id;
            return formatedId.Substring(formatedId.Length - 6);
        }

        /// <summary>
        /// フォントIDを7桁0埋めにフォーマットする
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>フォーマットしたフォントID</returns>
        private string FormatFontID7digit0padding(string id)
        {
            string formatedId = "0000000" + id;
            return formatedId.Substring(formatedId.Length - 7);
        }
    }
}

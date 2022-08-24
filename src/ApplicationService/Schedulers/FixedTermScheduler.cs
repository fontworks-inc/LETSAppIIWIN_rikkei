using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Security.AccessControl;
using System.Security.Principal;
using ApplicationService.Interfaces;
using Core.Entities;
using Core.Interfaces;
using NLog;

namespace ApplicationService.Schedulers
{
    /// <summary>
    /// 定期確認処理を行うスケジューラ
    /// </summary>
    public class FixedTermScheduler : SchedulerBase, IFixedTermScheduler
    {
        /// <summary>
        /// n時間以上経過している時の閾値
        /// </summary>
        /// <remarks>前回処理時から設定した時間以上経過してい場合に処理を開始する</remarks>
        private static readonly int ElapsedHours = 24;

        /// <summary>
        /// ミリ秒 => 秒に変換するための乗数
        /// </summary>
        private static readonly int MillisecondMultiplier = 1000;

        /// <summary>
        /// メモリで保持する情報を格納するリポジトリ
        /// </summary>
        private IVolatileSettingRepository volatileSettingRepository;

        /// <summary>
        /// ユーザ別ステータス情報を格納するリポジトリ
        /// </summary>
        private IUserStatusRepository userStatusRepository;

        /// <summary>
        /// 起動時処理に関するサービス
        /// </summary>
        private IStartupService startupService;

        /// <summary>
        /// 通知受信機能を格納するリポジトリ
        /// </summary>
        private IReceiveNotificationRepository receiveNotificationRepository;

        /// <summary>
        /// フォント管理に関する処理を行うサービス
        /// </summary>
        private IFontManagerService fontManagerService;

        /// <summary>
        /// フォント管理に関する処理を行うサービス
        /// </summary>
        private IDeviceModeService deviceModeService;

        /// <summary>
        /// 契約情報を格納するリポジトリ
        /// </summary>
        private IContractsAggregateRepository contractsAggregateRepository;

        private ApplicationSetting applicationSetting;

        /// <summary>
        /// クライアントアプリを終了するために呼び出されるイベント
        /// </summary>
        private ShutdownClientApplicationRequiredEvent shutdownClientApplicationRequiredEvent;

        /// <summary>
        /// 強制アップデートを実施するイベント
        /// </summary>
        private ForceUpdateEvent forceUpdateEvent;

        /// <summary>
        /// ダウンロード完了時に実施するイベント
        /// </summary>
        private DownloadCompletedEvent downloadCompletedEvent;

        /// <summary>
        /// 更新プログラムがダウンロード済みのときに呼び出されるイベント
        /// </summary>
        private ExistsUpdateProgramEvent existsUpdateProgramEvent;

        /// <summary>
        /// 更新プログラムのダウンロードを実施するイベント
        /// </summary>
        private StartDownloadEvent startDownloadEvent;

        /// <summary>
        /// 自デバイスの情報が含まれていない場合に呼び出されるイベント
        /// </summary>
        private NotContainsDeviceEvent notContainsDeviceEvent;

        /// <summary>
        /// 未読お知らせが有るときに呼び出されるイベント
        /// </summary>
        /// <param name="numberOfUnreadMessages">未読件数</param>
        private ExistsUnreadNotificationEvent existsUnreadNotificationEvent;

        /// <summary>
        /// 他端末からフォントがコピーされていたときに呼び出されるイベント
        /// </summary>
        private DetectionFontCopyEvent detectionFontCopyEvent;

        /// <summary>
        /// 多重起動チェック情報
        /// </summary>
        private MultiplePreventionInfo multipleInfo;

        /// <summary>
        /// フォントの同期処理を行うべきかどうか
        /// </summary>
        private bool shouldSynchronize;

        private double originalInterval;
        private DateTime lastScheduledEvent = new DateTime(0);

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="interval">実行間隔(秒)</param>
        /// <param name="exceptionNotify">例外発生時の通知処理</param>
        /// <param name="resourceWrapper">文言の取得を行うインスタンス</param>
        /// <param name="volatileSettingRepository">メモリで保持する情報を格納するリポジトリ</param>
        /// <param name="userStatusRepository">ユーザ別ステータス情報を格納するリポジトリ</param>
        /// <param name="startupService">起動時処理に関するサービス</param>
        /// <param name="receiveNotificationRepository">通知受信機能を格納するリポジトリ</param>
        /// <param name="fontManagerService">フォント管理に関する処理を行うサービス</param>
        /// <param name="contractsAggregateRepository">契約情報を格納するリポジトリ</param>
        /// <param name="applicationSetting">アプリケーション情報</param>
        /// <param name="shutdownClientApplicationRequiredEvent">クライアントアプリを終了するイベント</param>
        /// <param name="forceUpdateEvent">強制アップデートを実施するイベント</param>
        /// <param name="downloadCompletedEvent">ダウンロード完了時に実施するイベント</param>
        /// <param name="existsUpdateProgramEvent">更新プログラムがダウンロード済みのときに呼び出されるイベント</param>
        /// <param name="startDownloadEvent">更新プログラムのダウンロードを実施するイベント</param>
        /// <param name="notContainsDeviceEvent">自デバイスの情報が端末情報に含まれていない場合に呼び出されるイベント</param>
        /// <param name="existsUnreadNotificationEvent">未読お知らせが有るときに呼び出されるイベント</param>
        /// <param name="detectionFontCopyEvent">他端末からフォントがコピーされていたときに呼び出されるイベント</param>
        /// <param name="multipleInfo">多重起動チェック情報</param>
        public FixedTermScheduler(
            double interval,
            ExceptionNotify exceptionNotify,
            IResourceWrapper resourceWrapper,
            IVolatileSettingRepository volatileSettingRepository,
            IUserStatusRepository userStatusRepository,
            IStartupService startupService,
            IReceiveNotificationRepository receiveNotificationRepository,
            IFontManagerService fontManagerService,
            IDeviceModeService deviceModeService,
            IContractsAggregateRepository contractsAggregateRepository,
            ApplicationSetting applicationSetting,
            ShutdownClientApplicationRequiredEvent shutdownClientApplicationRequiredEvent,
            ForceUpdateEvent forceUpdateEvent,
            DownloadCompletedEvent downloadCompletedEvent,
            ExistsUpdateProgramEvent existsUpdateProgramEvent,
            StartDownloadEvent startDownloadEvent,
            NotContainsDeviceEvent notContainsDeviceEvent,
            ExistsUnreadNotificationEvent existsUnreadNotificationEvent,
            DetectionFontCopyEvent detectionFontCopyEvent,
            MultiplePreventionInfo multipleInfo)
            : base(60 * MillisecondMultiplier, exceptionNotify, resourceWrapper)
        {
            this.originalInterval = interval;
            this.volatileSettingRepository = volatileSettingRepository;
            this.userStatusRepository = userStatusRepository;
            this.startupService = startupService;
            this.receiveNotificationRepository = receiveNotificationRepository;
            this.fontManagerService = fontManagerService;
            this.deviceModeService = deviceModeService;
            this.contractsAggregateRepository = contractsAggregateRepository;
            this.applicationSetting = applicationSetting;

            this.shutdownClientApplicationRequiredEvent = shutdownClientApplicationRequiredEvent;
            this.forceUpdateEvent = forceUpdateEvent;
            this.downloadCompletedEvent = downloadCompletedEvent;
            this.existsUpdateProgramEvent = existsUpdateProgramEvent;
            this.startDownloadEvent = startDownloadEvent;
            this.notContainsDeviceEvent = notContainsDeviceEvent;
            this.existsUnreadNotificationEvent = existsUnreadNotificationEvent;
            this.detectionFontCopyEvent = detectionFontCopyEvent;
            this.multipleInfo = multipleInfo;

            this.shouldSynchronize = false;

            // ネットワーク接続状況変更イベント
            NetworkChange.NetworkAvailabilityChanged +=
                new NetworkAvailabilityChangedEventHandler(this.NetworkChange_NetworkAvailabilityChanged);
        }

        /// <summary>
        /// 定期間隔で実行されるイベント
        /// </summary>
        protected override void ScheduledEvent()
        {
            Logger.Debug(this.ResourceWrapper.GetString("LOG_INFO_FixedTermScheduler_ScheduledEvent_Start"));

            // 以下の処理中にGCを防ぐため、ここでGCをかけておく
            System.GC.Collect();

            // 特定の条件を満たさない限りはフォントの同期処理を実行しないよう設定する
            this.shouldSynchronize = false;

            VolatileSetting volatileSetting = this.volatileSettingRepository.GetVolatileSetting();

            // フォントチェンジメッセージ送信
            this.fontManagerService.BroadcastFontChange();

            // ログアウト情報出力
            this.OutputLogout();

            // ユーザーステータスの保存
            this.userStatusRepository.SaveStatus(this.userStatusRepository.GetStatus());

            if (volatileSetting.IsCheckedStartup)
            {
                UserStatus userStatusWk = this.userStatusRepository.GetStatus();
                if (userStatusWk.IsLoggingIn)
                {
                    if (!this.receiveNotificationRepository.IsConnected())
                    {
                        Logger.Info(this.ResourceWrapper.GetString("LOG_INFO_FixedTermScheduler_ScheduledEvent_ReceiveNotificationStart"));
                        this.receiveNotificationRepository.Start(volatileSetting.AccessToken, this.userStatusRepository.GetStatus().DeviceId);

                        if (!this.receiveNotificationRepository.IsConnected())
                        {
                            this.shouldSynchronize = true;
                            this.FontSynchronize(); // 通知サーバへの再接続時にフォント同期処理を実行する
                        }
                    }

                    if (this.receiveNotificationRepository.IsConnected())
                    {
                        // アクセストークンが変わっている or 前回接続時より1時間経過している場合、再接続を行うため、一度切断する
                        if ((volatileSetting.AccessToken.CompareTo(this.receiveNotificationRepository.ConnectedAccessToken()) != 0)
                            || (this.receiveNotificationRepository.ConnectedTime().AddHours(1).CompareTo(DateTime.Now) < 0))
                        {
                            this.receiveNotificationRepository.Stop();
                        }
                    }
                }

                // 前回実行時間から、interval秒過ぎていなければ抜ける
                if (DateTime.Now < this.lastScheduledEvent.AddMilliseconds(this.originalInterval * MillisecondMultiplier))
                {
                    Logger.Debug(this.ResourceWrapper.GetString("LOG_INFO_FixedTermScheduler_ScheduledEvent_End"));
                    return;
                }
            }

            // [起動チェック処理]が「未処理」のとき、または[起動時チェック日時]からn時間以上経過しているときのみ処理を行う
            if (!volatileSetting.IsCheckedStartup
                || ((DateTime)volatileSetting.CheckedStartupAt).AddHours(ElapsedHours).CompareTo(DateTime.Now) <= 0)
            {
                if (this.userStatusRepository.GetStatus().IsDeviceMode)
                {
                    IList<string> messageList = this.deviceModeService.FixedTermCheck(false);
                    if (messageList.Count > 0)
                    {
                        foreach (string message in messageList)
                        {
                            Logger.Warn(message);
                        }
                    }

                    return;
                }

                // 起動時チェック処理を実行する
                if (this.startupService.IsCheckedStartup(
                    this.shutdownClientApplicationRequiredEvent,
                    this.forceUpdateEvent,
                    this.downloadCompletedEvent,
                    this.existsUpdateProgramEvent,
                    this.startDownloadEvent,
                    this.notContainsDeviceEvent,
                    this.existsUnreadNotificationEvent,
                    this.detectionFontCopyEvent,
                    this.multipleInfo))
                {
                    Logger.Info(this.ResourceWrapper.GetString("LOG_INFO_FixedTermScheduler_ScheduledEvent_IsCheckedStartup_True"));

                    // 起動時チェックが正常に終了し、通知受信開始処理が実行されていない場合は処理を開始する
                    if (!this.receiveNotificationRepository.IsConnected())
                    {
                        Logger.Info(this.ResourceWrapper.GetString("LOG_INFO_FixedTermScheduler_ScheduledEvent_ReceiveNotificationStart"));
                        this.receiveNotificationRepository.Start(volatileSetting.AccessToken, this.userStatusRepository.GetStatus().DeviceId);
                    }
                }
                else
                {
                    Logger.Info(this.ResourceWrapper.GetString("LOG_INFO_FixedTermScheduler_ScheduledEvent_IsCheckedStartup_False"));

                    // 契約切れフォントのディアクティベート」を行う
                    this.fontManagerService.DeactivateExpiredFonts(this.contractsAggregateRepository.GetContractsAggregate().Contracts);
                }
            }

            // [ユーザー別保存：ログイン状態]が「ログイン中」かつ[メモリ：起動チェック処理]が「処理済み」
            // かつ(前回チェック時から30分以上経過または[メモリ：通信状態]が「オフライン中」)のとき
            UserStatus userStatus = this.userStatusRepository.GetStatus();
            bool preOnline = volatileSetting.IsConnected;
            if (userStatus.IsLoggingIn && volatileSetting.IsCheckedStartup
                && (DateTime.Now >= this.lastScheduledEvent.AddMilliseconds(this.originalInterval * MillisecondMultiplier)
                    || !volatileSetting.IsConnected))
            {
                Logger.Debug("ログイン状態確認処理を実行する");

                // ログイン状態確認処理を実行する
                if (this.startupService.ConfirmLoginStatus(userStatus.DeviceId, this.notContainsDeviceEvent))
                {
                    Logger.Debug(this.ResourceWrapper.GetString("LOG_INFO_FixedTermScheduler_ScheduledEvent_ShouldSynchronize"));

                    // 結果が正常かつ「ログイン中」時
                    // オフライン→オンラインに変わったと判断してフォントの同期処理を実行
                    if (!preOnline)
                    {
                        this.shouldSynchronize = true;
                        this.FontSynchronize();
                    }
                }

                // フォントファイルチェックを行う
                this.fontManagerService.CheckFontsList();
            }

            this.lastScheduledEvent = DateTime.Now;

            Logger.Debug(this.ResourceWrapper.GetString("LOG_INFO_FixedTermScheduler_ScheduledEvent_End"));
        }

        /// <summary>
        /// ネットワーク接続状況変更イベント
        /// </summary>
        /// <param name="sender">送信元</param>
        /// <param name="e">イベントオブジェクト</param>
        private void NetworkChange_NetworkAvailabilityChanged(
            object sender, NetworkAvailabilityEventArgs e)
        {
            if (e.IsAvailable && this.shouldSynchronize)
            {
                Logger.Info(this.ResourceWrapper.GetString("LOG_INFO_FixedTermScheduler_NetworkChange_NetworkAvailabilityChanged_ShouldSynchronize"));

                // オフライン→オンラインに変わったとき かつ 定期間隔で実行されるイベントで実行すべきと判断されている場合
                // フォントの同期処理を行う
                this.FontSynchronize();
            }
        }

        /// <summary>
        /// フォント同期処理実行
        /// </summary>
        private void FontSynchronize()
        {
            if (this.shouldSynchronize)
            {
                // フォントの同期処理を行う
                this.fontManagerService.Synchronize(false);

                // 実行後、falseに設定
                this.shouldSynchronize = false;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "<保留中>")]
        private static string preAccessToken = string.Empty;

        private void OutputLogout()
        {
            VolatileSetting vSetting = this.volatileSettingRepository.GetVolatileSetting();

            string accessToken = vSetting.AccessToken;

            if (preAccessToken == accessToken)
            {
                return;
            }

            preAccessToken = accessToken;

            // ユーザー名の取得
            string username = Environment.UserName.Replace(' ', '_');

            // ホームドライブの取得
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string homedrive = appPath.Substring(0, appPath.IndexOf("\\"));

            // LETSフォルダ
            //string letsfolder = $@"{homedrive}\ProgramData\Fontworks\LETS";
            string programdataFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string letsfolder = $@"{programdataFolder}\Fontworks\LETS";

            // ユーザー名一覧を出力する
            string userlist = Path.Combine(letsfolder, "userlist.txt");
            if (System.IO.File.Exists(userlist))
            {
                this.SetHidden(userlist, false);
                string[] users = File.ReadAllLines(userlist);
                bool isExist = false;
                foreach (string u in users)
                {
                    if (u == username)
                    {
                        isExist = true;
                        break;
                    }
                }

                if (!isExist)
                {
                    File.AppendAllText(userlist, username + "\n");
                }
            }
            else
            {
                File.AppendAllText(userlist, username + "\n");
                this.SetFileAccessEveryone(userlist);
            }

            this.SetHidden(userlist, true);
        }

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

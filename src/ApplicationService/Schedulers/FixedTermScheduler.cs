using System;
using System.Net.NetworkInformation;
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
        /// 契約情報を格納するリポジトリ
        /// </summary>
        private IContractsAggregateRepository contractsAggregateRepository;

        /// <summary>
        /// 自デバイスの情報が端末情報に含まれていない場合に呼び出されるイベント
        /// </summary>
        private NotContainsDeviceEvent notContainsDeviceEvent;

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
        /// <param name="notContainsDeviceEvent">自デバイスの情報が端末情報に含まれていない場合に呼び出されるイベント</param>
        public FixedTermScheduler(
            double interval,
            ExceptionNotify exceptionNotify,
            IResourceWrapper resourceWrapper,
            IVolatileSettingRepository volatileSettingRepository,
            IUserStatusRepository userStatusRepository,
            IStartupService startupService,
            IReceiveNotificationRepository receiveNotificationRepository,
            IFontManagerService fontManagerService,
            IContractsAggregateRepository contractsAggregateRepository,
            NotContainsDeviceEvent notContainsDeviceEvent)
            : base(60 * MillisecondMultiplier, exceptionNotify, resourceWrapper)
        {
            //this.originalInterval = interval;
            this.originalInterval = 300;    // 5分にしておく
            this.volatileSettingRepository = volatileSettingRepository;
            this.userStatusRepository = userStatusRepository;
            this.startupService = startupService;
            this.receiveNotificationRepository = receiveNotificationRepository;
            this.fontManagerService = fontManagerService;
            this.contractsAggregateRepository = contractsAggregateRepository;
            this.notContainsDeviceEvent = notContainsDeviceEvent;
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
            Logger.Info(this.ResourceWrapper.GetString("LOG_INFO_FixedTermScheduler_ScheduledEvent_Start"));

            // 特定の条件を満たさない限りはフォントの同期処理を実行しないよう設定する
            this.shouldSynchronize = false;

            VolatileSetting volatileSetting = this.volatileSettingRepository.GetVolatileSetting();

            // フォントチェンジメッセージ送信
            this.fontManagerService.BroadcastFontChange();

            if (volatileSetting.IsCheckedStartup)
            {
                // 前回実行時間から、interval秒過ぎていなければ抜ける
                if (DateTime.Now < this.lastScheduledEvent.AddMilliseconds(this.originalInterval * MillisecondMultiplier))
                {
                    return;
                }
            }

            // [起動チェック処理]が「未処理」のとき、または[起動時チェック日時]からn時間以上経過しているときのみ処理を行う
            if (!volatileSetting.IsCheckedStartup || ((DateTime)volatileSetting.CheckedStartupAt)
                .AddHours(ElapsedHours).CompareTo(DateTime.Now) >= 0)
            {
                // 起動時チェック処理を実行する
                if (this.startupService.IsCheckedStartup(this.notContainsDeviceEvent))
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

            // [ユーザー別保存：ログイン状態]が「ログイン中」かつ[メモリ：通信状態]が「オフライン中」で、[メモリ：起動チェック処理]が「処理済み」のとき
            UserStatus userStatus = this.userStatusRepository.GetStatus();
            if (userStatus.IsLoggingIn && !volatileSetting.IsConnected && volatileSetting.IsCheckedStartup)
            {
                // ログイン状態確認処理を実行する
                if (this.startupService.ConfirmLoginStatus(userStatus.DeviceId, this.notContainsDeviceEvent))
                {
                    Logger.Info(this.ResourceWrapper.GetString("LOG_INFO_FixedTermScheduler_ScheduledEvent_ShouldSynchronize"));

                    // 結果が正常かつ「ログイン中」時
                    // オフライン→オンラインに変わったときにフォントの同期処理を実行するよう設定する
                    this.shouldSynchronize = true;
                }
            }

            this.lastScheduledEvent = DateTime.Now;

            Logger.Info(this.ResourceWrapper.GetString("LOG_INFO_FixedTermScheduler_ScheduledEvent_End"));
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
                this.fontManagerService.Synchronize(false);

                // 実行後、falseに設定
                this.shouldSynchronize = false;
            }
        }
    }
}

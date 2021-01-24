using System;
using System.Collections.Generic;
using System.Linq;
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
        /// フォント管理に関する処理を行うサービス
        /// </summary>
        private IFontManagerService fontManagerService = null;

        /// <summary>
        /// メモリで保持する情報を格納するリポジトリ
        /// </summary>
        private IVolatileSettingRepository volatileSettingRepository = null;

        /// <summary>
        /// 契約情報を格納するリポジトリ
        /// </summary>
        private IContractsAggregateRepository contractsAggregateRepository = null;

        /// <summary>
        /// 契約情報を格納するキャッシュリポジトリ
        /// </summary>
        private IContractsAggregateRepository contractsAggregateCacheRepository = null;

        /// <summary>
        /// 端末情報を格納するリポジトリ
        /// </summary>
        private IDevicesRepository devicesRepository = null;

        /// <summary>
        /// ユーザ別ステータス情報を格納するリポジトリ
        /// </summary>
        private IUserStatusRepository userStatusRepository = null;

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="resourceWrapper">文言の取得を行うインスタンス</param>
        /// <param name="fontManagerService">フォント管理に関する処理を行うサービス</param>
        /// <param name="volatileSettingRepository">メモリで保持する情報を格納するリポジトリ</param>
        /// <param name="contractsAggregateRepository">契約情報を格納するリポジトリ</param>
        /// <param name="contractsAggregateCacheRepository">契約情報を格納するキャッシュリポジトリ</param>
        /// <param name="devicesRepository">端末情報を格納するリポジトリ</param>
        /// <param name="userStatusRepository">ユーザ別ステータス情報を格納するリポジトリ</param>
        public StartupService(
            IResourceWrapper resourceWrapper,
            IFontManagerService fontManagerService,
            IVolatileSettingRepository volatileSettingRepository,
            IContractsAggregateRepository contractsAggregateRepository,
            IContractsAggregateRepository contractsAggregateCacheRepository,
            IDevicesRepository devicesRepository,
            IUserStatusRepository userStatusRepository)
        {
            this.resourceWrapper = resourceWrapper;
            this.fontManagerService = fontManagerService;
            this.volatileSettingRepository = volatileSettingRepository;
            this.contractsAggregateRepository = contractsAggregateRepository;
            this.contractsAggregateCacheRepository = contractsAggregateCacheRepository;
            this.devicesRepository = devicesRepository;
            this.userStatusRepository = userStatusRepository;
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
        /// <param name="notContainsDeviceEvent">自デバイスの情報が端末情報に含まれていない場合に呼び出されるイベント</param>
        /// <returns>チェック結果を返す</returns>
        public bool IsCheckedStartup(NotContainsDeviceEvent notContainsDeviceEvent)
        {
            try
            {
                Logger.Info(this.resourceWrapper.GetString("LOG_INFO_StartupService_IsCheckedStartup_Start"));

                // 開始時に起動時チェック処理を「未設定」に設定する
                this.volatileSettingRepository.GetVolatileSetting().IsCheckedStartup = false;

                if (this.fontManagerService == null
                    || this.contractsAggregateRepository == null
                    || this.contractsAggregateCacheRepository == null
                    || this.userStatusRepository == null)
                {
                    throw new InvalidOperationException(this.resourceWrapper.GetString("LOG_ERR_StartupService_IsCheckedStartup_InvalidOperationException"));
                }

                // ログイン状態確認処理を実行し、ログアウト中になる場合は以後の起動時チェックを行わない
                UserStatus userStatus = this.userStatusRepository.GetStatus();
                if (!userStatus.IsLoggingIn || !this.ConfirmLoginStatus(userStatus.DeviceId, notContainsDeviceEvent))
                {
                    Logger.Info(this.resourceWrapper.GetString("LOG_INFO_StartupService_IsCheckedStartup_LoggingOut"));
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
                }

                // 契約切れフォントのディアクティベート
                this.fontManagerService.DeactivateExpiredFonts(contractsResult.ContractsAggregate.Contracts);

                // フォントアクティブ/ディアクティブ情報の同期
                this.fontManagerService.Synchronize(true);

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
                    Logger.Info(string.Format("ConfirmLoginStatus:デバイスリストが存在しない場合はログアウト状態と判断する", ""));
                    return false;   // デバイスリストが存在しない場合はログアウト状態と判断する
                }

                if (!devices.Any(device => device.DeviceId == deviceId))
                {
                    // 自デバイスの情報が含まれていない場合、イベントを実行する
                    Logger.Info(string.Format("ConfirmLoginStatus:自デバイスの情報が含まれていない場合、イベントを実行する", ""));
                    notContainsDeviceEvent();
                    return false;
                }

                Logger.Info(string.Format("ConfirmLoginStatus:trye", ""));
                return true;
            }
            catch (GetAllDevicesException)
            {
                // 全端末情報を取得で例外が発生した場合は失敗扱い
                Logger.Info(string.Format("ConfirmLoginStatus:全端末情報を取得で例外が発生した場合は失敗扱い", ""));
                return false;
            }
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
                return new ContractsResult(this.contractsAggregateCacheRepository.GetContractsAggregate(), true);
            }
        }
    }
}

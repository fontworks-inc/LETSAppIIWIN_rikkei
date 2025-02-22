﻿using System;
using System.Net;
using ApplicationService.Interfaces;
using Core.Entities;
using Core.Interfaces;
using NLog;
using Prism.Ioc;

namespace ApplicationService.Authentication
{
    /// <summary>
    /// 認証サービス
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>
        /// 最初のログインフラグ
        /// </summary>
        private static bool isFirstLogined = false;

        /// <summary>
        /// 文言の取得を行うインスタンス
        /// </summary>
        private readonly IResourceWrapper resourceWrapper = null;

        /// <summary>
        /// 認証情報を格納するリポジトリ
        /// </summary>
        private readonly IAuthenticationInformationRepository authenticationInformationRepository = null;

        /// <summary>
        /// 設定情報(デバイスモード)を格納するリポジトリ
        /// </summary>
        private readonly IDeviceModeSettingRepository deviceModeSettingRepository = null;

        /// <summary>
        /// 端末情報を格納するリポジトリ
        /// </summary>
        private readonly IDevicesRepository devicesRepository = null;

        /// <summary>
        /// ユーザ別ステータス情報を格納するリポジトリ
        /// </summary>
        private readonly IUserStatusRepository userStatusRepository = null;

        /// <summary>
        /// 通知受信機能を格納するリポジトリ
        /// </summary>
        private readonly IReceiveNotificationRepository receiveNotificationRepository = null;

        /// <summary>
        /// メモリ上で保持する情報のリポジトリ
        /// </summary>
        private readonly IVolatileSettingRepository volatileSettingRepository = null;

        /// <summary>
        /// お客様情報のリポジトリ
        /// </summary>
        private readonly ICustomerRepository customerRepository = null;

        /// <summary>
        /// フォント管理サービス
        /// </summary>
        private readonly IFontManagerService fontManagerService = null;

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="containerProvider">コンテナプロバイダ</param>
        public AuthenticationService(IContainerProvider containerProvider)
        {
            this.resourceWrapper = containerProvider.Resolve<IResourceWrapper>();
            this.authenticationInformationRepository = containerProvider.Resolve<IAuthenticationInformationRepository>();
            this.devicesRepository = containerProvider.Resolve<IDevicesRepository>();
            this.userStatusRepository = containerProvider.Resolve<IUserStatusRepository>();
            this.volatileSettingRepository = containerProvider.Resolve<IVolatileSettingRepository>();
            this.receiveNotificationRepository = containerProvider.Resolve<IReceiveNotificationRepository>();
            this.volatileSettingRepository = containerProvider.Resolve<IVolatileSettingRepository>();
            this.customerRepository = containerProvider.Resolve<ICustomerRepository>();
            this.fontManagerService = containerProvider.Resolve<IFontManagerService>();
            this.deviceModeSettingRepository = containerProvider.Resolve<IDeviceModeSettingRepository>();
        }

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="resourceWrapper">文言の取得を行うインスタンス</param>
        /// <param name="authenticationInformationRepository">認証情報を格納するリポジトリ</param>
        /// <param name="devicesRepository">端末情報を格納するリポジトリ</param>
        /// <param name="userStatusRepository">ユーザ別ステータス情報を格納するリポジトリ</param>
        /// <param name="volatileSettingRepository">メモリ上で保持する情報のリポジトリ</param>
        /// <param name="deviceModeSettingRepository">設定情報(デバイスモード)のリポジトリ</param>
        /// <remarks>ログイン処理用</remarks>
        public AuthenticationService(
            IResourceWrapper resourceWrapper,
            IAuthenticationInformationRepository authenticationInformationRepository,
            IDevicesRepository devicesRepository,
            IUserStatusRepository userStatusRepository,
            IVolatileSettingRepository volatileSettingRepository,
            IDeviceModeSettingRepository deviceModeSettingRepository)
        {
            this.resourceWrapper = resourceWrapper;
            this.authenticationInformationRepository = authenticationInformationRepository;
            this.devicesRepository = devicesRepository;
            this.userStatusRepository = userStatusRepository;
            this.volatileSettingRepository = volatileSettingRepository;
            this.deviceModeSettingRepository = deviceModeSettingRepository;
        }

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="resourceWrapper">文言の取得を行うインスタンス</param>
        /// <param name="authenticationInformationRepository">認証情報を格納するリポジトリ</param>
        /// <param name="userStatusRepository">ユーザ別ステータス情報を格納するリポジトリ</param>
        /// <param name="receiveNotificationRepository">通知受信機能を格納するリポジトリ</param>
        /// <param name="volatileSettingRepository">メモリ上で保持する情報のリポジトリ</param>
        /// <param name="customerRepository">お客様情報のリポジトリ</param>
        /// <param name="fontManagerService">フォント管理サービス</param>
        /// <param name="deviceModeSettingRepository">設定情報(デバイスモード)のリポジトリ</param>
        /// <remarks>ログアウト処理用</remarks>
        public AuthenticationService(
            IResourceWrapper resourceWrapper,
            IAuthenticationInformationRepository authenticationInformationRepository,
            IUserStatusRepository userStatusRepository,
            IReceiveNotificationRepository receiveNotificationRepository,
            IVolatileSettingRepository volatileSettingRepository,
            ICustomerRepository customerRepository,
            IFontManagerService fontManagerService,
            IDeviceModeSettingRepository deviceModeSettingRepository)
        {
            this.resourceWrapper = resourceWrapper;
            this.authenticationInformationRepository = authenticationInformationRepository;
            this.userStatusRepository = userStatusRepository;
            this.receiveNotificationRepository = receiveNotificationRepository;
            this.volatileSettingRepository = volatileSettingRepository;
            this.customerRepository = customerRepository;
            this.fontManagerService = fontManagerService;
            this.deviceModeSettingRepository = deviceModeSettingRepository;
        }

        /// <summary>
        /// ログインする
        /// </summary>
        /// <param name="mailAddress">メールアドレス</param>
        /// <param name="password">パスワード</param>
        /// <returns>認証情報</returns>
        public AuthenticationInformationResponse Login(string mailAddress, string password)
        {
            Logger.Info("AuthenticationService:Login Enter");
            if (this.devicesRepository == null)
            {
                throw new InvalidOperationException(this.resourceWrapper.GetString("LOG_ERR_AuthenticationService_Login_InvalidOperationException"));
            }

            // アカウント認証を行う
            try
            {
                AuthenticationInformationResponse authenticationInformationResponse = this.authenticationInformationRepository.AuthenticateAccount(mailAddress, password);
                ResponseCode code = authenticationInformationResponse.GetResponseCode();
                if (code == ResponseCode.Succeeded)
                {
                    //authenticationInformationResponse.Data.GroupType = 1;   //デバイスモードデバッグ用
                    if (authenticationInformationResponse.Data.GroupType == 1)
                    {
                        // グループ区分を判定し、デバイスモードならば、ここで終了
                        System.Environment.SetEnvironmentVariable("LETS_DEVICE_MODE", "TRUE");
                        AuthenticationInformation authenticationInformation = authenticationInformationResponse.Data;
                        DeviceModeSetting deviceModeSetting = this.deviceModeSettingRepository.GetDeviceModeSetting();
                        deviceModeSetting.OfflineDeviceID = authenticationInformation.OfflineDeviceId;
                        deviceModeSetting.LicenseDecryptionKey = authenticationInformation.LicenseDecryptionKey;
                        deviceModeSetting.IndefiniteAccessToken = authenticationInformation.IndefiniteAccessToken;
                        this.deviceModeSettingRepository.SaveDeviceModeSetting(deviceModeSetting);
                        UserStatus wuserStatus = this.userStatusRepository.GetStatus();
                        wuserStatus.IsDeviceMode = true;
                        this.userStatusRepository.SaveStatus(wuserStatus);
                        return authenticationInformationResponse;
                    }
                }
                else
                {
                    return authenticationInformationResponse;
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.StackTrace);
            }

            // ログインするときは、接続状態をONにしておく
            this.volatileSettingRepository.GetVolatileSetting().IsConnected = true;

            // ユーザ別保存：デバイスIDを取得
            Logger.Info("AuthenticationService:Login ユーザ別保存：デバイスIDを取得");
            var userStatus = this.userStatusRepository.GetStatus();
            var deviceId = userStatus.DeviceId;
            var deviceKey = userStatus.DeviceKey;

            if (string.IsNullOrEmpty(deviceKey))
            {
                // デバイスキーが保存されていない場合、作成する
                Guid guid = System.Guid.NewGuid();
                deviceKey = guid.ToString("N");
                userStatus.DeviceKey = deviceKey;
                this.userStatusRepository.SaveStatus(userStatus);
            }

            // 配信サービスよりデバイスIDを発行
            Logger.Info("AuthenticationService:Login 配信サービスよりデバイスIDを発行");
            var user = new User()
            {
                MailAddress = mailAddress,
                Password = password,
                HostName = Dns.GetHostName(),
                OSUserName = Environment.UserName,
            };
            try
            {
                deviceId = this.devicesRepository.GetDeviceId(user, deviceKey);
            }
            catch (SystemException ret)
            {
                try
                {
                    return new AuthenticationInformationResponse()
                    {
                        Code = int.Parse(ret.Message),
                    };
                }
                catch (Exception)
                {
                    return new AuthenticationInformationResponse()
                    {
                        Message = ret.Message,
                        Code = -1,
                    };
                }
            }

            // ユーザ別保存：デバイスIDに保存
            Logger.Info("AuthenticationService:Login ユーザ別保存：デバイスIDに保存");
            userStatus.DeviceId = deviceId;
            this.userStatusRepository.SaveStatus(userStatus);

            // ログイン処理を実行
            Logger.Info("AuthenticationService:Login ログイン処理を実行");
            return this.authenticationInformationRepository.Login(
                    deviceId, mailAddress, password);
        }

        /// <summary>
        /// ログイン情報を保存する（ログイン完了処理）
        /// </summary>
        /// <param name="authenticationInformation">認証情報</param>
        /// <remarks>ログイン完了処理でのデータ保存部分</remarks>
        public void SaveLoginInfo(AuthenticationInformation authenticationInformation)
        {
            // [メモリ：アクセストークン]に「アクセストークン」を保存
            var volatileSetting = this.volatileSettingRepository.GetVolatileSetting();
            volatileSetting.AccessToken = authenticationInformation.AccessToken;
            volatileSetting.RefreshToken = authenticationInformation.RefreshToken;

            Logger.Debug("AccessToken = " + volatileSetting.AccessToken, string.Empty);
            Logger.Debug("UserAgent = " + volatileSetting.UserAgent, string.Empty);
            Logger.Debug("RefreshToken = " + volatileSetting.RefreshToken, string.Empty);

            // ユーザ別保存情報取得
            var userStatus = this.userStatusRepository.GetStatus();

            // [ユーザー別保存]に「リフレッシュトークン」と「ログイン中」を保存
            userStatus.RefreshToken = authenticationInformation.RefreshToken;
            userStatus.IsLoggingIn = true;

            // リフレッシュトークン次回取得日時に現在日時+7日+(0～6日)を設定
            int addDays = 7 + new Random().Next(7);
            userStatus.RefreshTokenUpdateSchedule = DateTime.Now.AddDays(addDays);

            this.userStatusRepository.SaveStatus(userStatus);

            if (!isFirstLogined)
            {
                // ユーザー配下のフォントフォルダ
                var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var userFontsDir = @$"{local}\Microsoft\Windows\Fonts";

                // フォント一覧の更新
                this.fontManagerService.UpdateFontsList(userFontsDir);
            }

            isFirstLogined = true;

            volatileSetting.IsCheckedStartup = false;
        }

        /// <summary>
        /// ログアウトする
        /// </summary>
        /// <param name="isCallApi">ログアウトAPIを呼び出すかどうか</param>
        /// <returns>ログアウト処理の成功時にtrue、それ以外はfalse</returns>
        public bool Logout(bool isCallApi)
        {
            if (this.receiveNotificationRepository == null
                || this.customerRepository == null
                || this.fontManagerService == null)
            {
                throw new InvalidOperationException(this.resourceWrapper.GetString("LOG_ERR_AuthenticationService_Logout_InvalidOperationException"));
            }

            // ユーザ別保存：デバイスIDを取得
            var userStatus = this.userStatusRepository.GetStatus();
            var deviceId = userStatus.DeviceId;

            // [ユーザー別保存：ログイン状態]に「ログアウト」を保存する
            userStatus.IsLoggingIn = false;
            this.userStatusRepository.SaveStatus(userStatus);

            // アクセストークン取得
            var accessToken = this.volatileSettingRepository.GetVolatileSetting().AccessToken;

            if (isCallApi)
            {
                try
                {
                    // ログアウト処理を実行
                    this.authenticationInformationRepository.Logout(deviceId, accessToken);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex);
                }
            }

            // [フォント：フォント情報]でアクティベートされているLETSフォントをディアクティベート
            this.fontManagerService.DeactivateSettingFonts();

            // 通知停止
            this.receiveNotificationRepository.Stop();

            // [ユーザー別保存：ログイン状態]に「ログアウト」を保存する
            userStatus.IsLoggingIn = false;
            userStatus.DeviceId = string.Empty; // デバイスIDをクリアする
            this.userStatusRepository.SaveStatus(userStatus);

            //// お客様情報のクリア
            this.customerRepository.SaveCustomer(new Customer());

            return true;
        }
    }
}

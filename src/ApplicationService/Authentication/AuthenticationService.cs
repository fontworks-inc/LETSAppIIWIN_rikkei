using System;
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
        }

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="resourceWrapper">文言の取得を行うインスタンス</param>
        /// <param name="authenticationInformationRepository">認証情報を格納するリポジトリ</param>
        /// <param name="devicesRepository">端末情報を格納するリポジトリ</param>
        /// <param name="userStatusRepository">ユーザ別ステータス情報を格納するリポジトリ</param>
        /// <param name="volatileSettingRepository">メモリ上で保持する情報のリポジトリ</param>
        /// <remarks>ログイン処理用</remarks>
        public AuthenticationService(
            IResourceWrapper resourceWrapper,
            IAuthenticationInformationRepository authenticationInformationRepository,
            IDevicesRepository devicesRepository,
            IUserStatusRepository userStatusRepository,
            IVolatileSettingRepository volatileSettingRepository)
        {
            this.resourceWrapper = resourceWrapper;
            this.authenticationInformationRepository = authenticationInformationRepository;
            this.devicesRepository = devicesRepository;
            this.userStatusRepository = userStatusRepository;
            this.volatileSettingRepository = volatileSettingRepository;
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
        /// <remarks>ログアウト処理用</remarks>
        public AuthenticationService(
            IResourceWrapper resourceWrapper,
            IAuthenticationInformationRepository authenticationInformationRepository,
            IUserStatusRepository userStatusRepository,
            IReceiveNotificationRepository receiveNotificationRepository,
            IVolatileSettingRepository volatileSettingRepository,
            ICustomerRepository customerRepository,
            IFontManagerService fontManagerService)
        {
            this.resourceWrapper = resourceWrapper;
            this.authenticationInformationRepository = authenticationInformationRepository;
            this.userStatusRepository = userStatusRepository;
            this.receiveNotificationRepository = receiveNotificationRepository;
            this.volatileSettingRepository = volatileSettingRepository;
            this.customerRepository = customerRepository;
            this.fontManagerService = fontManagerService;
        }

        /// <summary>
        /// ログインする
        /// </summary>
        /// <param name="mailAddress">メールアドレス</param>
        /// <param name="password">パスワード</param>
        /// <returns>認証情報</returns>
        public AuthenticationInformationResponse Login(string mailAddress, string password)
        {
            Logger.Debug("AuthenticationService:Login Enter");
            if (this.devicesRepository == null)
            {
                throw new InvalidOperationException(this.resourceWrapper.GetString("LOG_ERR_AuthenticationService_Login_InvalidOperationException"));
            }

            // ログインするときは、接続状態をONにしておく
            this.volatileSettingRepository.GetVolatileSetting().IsConnected = true;

            // ユーザ別保存：デバイスIDを取得
            Logger.Debug("AuthenticationService:Login ユーザ別保存：デバイスIDを取得");
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
            Logger.Debug("AuthenticationService:Login 配信サービスよりデバイスIDを発行");
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
                return new AuthenticationInformationResponse()
                {
                    Code = int.Parse(ret.Message),
                };
            }

            // ユーザ別保存：デバイスIDに保存
            Logger.Debug("AuthenticationService:Login ユーザ別保存：デバイスIDに保存");
            userStatus.DeviceId = deviceId;
            this.userStatusRepository.SaveStatus(userStatus);

            // ログイン処理を実行
            Logger.Debug("AuthenticationService:Login ログイン処理を実行");
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

            // [フォント：フォント情報]でアクティベートされているLETSフォントをディアクティベート
            this.fontManagerService.DeactivateSettingFonts();

            // ユーザ別保存：デバイスIDを取得
            var userStatus = this.userStatusRepository.GetStatus();
            var deviceId = userStatus.DeviceId;

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

            // 通知停止
            this.receiveNotificationRepository.Stop();

            // [ユーザー別保存：ログイン状態]に「ログアウト」を保存する
            userStatus.IsLoggingIn = false;
            userStatus.DeviceId = string.Empty; // デバイスIDをクリアする
            this.userStatusRepository.SaveStatus(userStatus);

            // お客様情報の削除
            this.customerRepository.Delete();

            return true;
        }
    }
}

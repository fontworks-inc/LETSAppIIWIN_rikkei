using System;
using System.Linq;
using System.Net;
using ApplicationService.Interfaces;
using Core.Entities;
using Core.Interfaces;
using NLog;
using OS.Interfaces;
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
        /// フォント情報のリポジトリ
        /// </summary>
        private readonly IUserFontsSettingRepository userFontsSettingRepository = null;

        /// <summary>
        /// フォントアクティベートサービス
        /// </summary>
        private readonly IFontActivationService fontActivationService = null;

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
        }

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="resourceWrapper">文言の取得を行うインスタンス</param>
        /// <param name="authenticationInformationRepository">認証情報を格納するリポジトリ</param>
        /// <param name="devicesRepository">端末情報を格納するリポジトリ</param>
        /// <param name="userStatusRepository">ユーザ別ステータス情報を格納するリポジトリ</param>
        /// <param name="volatileSettingRepository">メモリ上で保持する情報のリポジトリ</param>
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
        /// <param name="userFontsSettingRepository">フォント情報のリポジトリ</param>
        /// <param name="fontActivationService">フォントアクティベートサービス</param>
        /// <remarks>ログアウト処理用</remarks>
        public AuthenticationService(
            IResourceWrapper resourceWrapper,
            IAuthenticationInformationRepository authenticationInformationRepository,
            IUserStatusRepository userStatusRepository,
            IReceiveNotificationRepository receiveNotificationRepository,
            IVolatileSettingRepository volatileSettingRepository,
            IUserFontsSettingRepository userFontsSettingRepository,
            IFontActivationService fontActivationService)
        {
            this.resourceWrapper = resourceWrapper;
            this.authenticationInformationRepository = authenticationInformationRepository;
            this.userStatusRepository = userStatusRepository;
            this.receiveNotificationRepository = receiveNotificationRepository;
            this.volatileSettingRepository = volatileSettingRepository;
            this.userFontsSettingRepository = userFontsSettingRepository;
            this.fontActivationService = fontActivationService;
        }

        /// <summary>
        /// ログインする
        /// </summary>
        /// <param name="mailAddress">メールアドレス</param>
        /// <param name="password">パスワード</param>
        /// <returns>認証情報</returns>
        public AuthenticationInformationResponse Login(string mailAddress, string password)
        {
            // ユーザ別保存：デバイスIDを取得
            var userStatus = this.userStatusRepository.GetStatus();
            var deviceId = userStatus.DeviceId;

            // デバイスIDの値がない場合
            if (string.IsNullOrEmpty(deviceId))
            {
                // 配信サービスよりデバイスIDを発行
                var user = new User()
                {
                    MailAddress = mailAddress,
                    Password = password,
                    HostName = Dns.GetHostName(),
                    OSUserName = Environment.UserName,
                };
                deviceId = this.devicesRepository.GetDeviceId(user);

                // ユーザ別保存：デバイスIDに保存
                userStatus.DeviceId = deviceId;
                this.userStatusRepository.SaveStatus(userStatus);
            }

            // ログイン処理を実行
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

            // ユーザ別保存情報取得
            var userStatus = this.userStatusRepository.GetStatus();

            // [ユーザー別保存]に「リフレッシュトークン」と「ログイン中」を保存
            userStatus.RefreshToken = authenticationInformation.RefreshToken;
            userStatus.IsLoggingIn = true;
            this.userStatusRepository.SaveStatus(userStatus);
        }

        /// <summary>
        /// ログアウトする
        /// </summary>
        /// <returns>ログアウト処理の成功時にtrue、それ以外はfalse</returns>
        public bool Logout()
        {
            if (this.receiveNotificationRepository == null
                || this.userFontsSettingRepository == null
                || this.fontActivationService == null)
            {
                throw new InvalidOperationException(this.resourceWrapper.GetString("LOG_ERR_AuthenticationService_Logout_InvalidOperationException"));
            }

            // ユーザ別保存：デバイスIDを取得
            var userStatus = this.userStatusRepository.GetStatus();
            var deviceId = userStatus.DeviceId;

            // アクセストークン取得
            var accessToken = this.volatileSettingRepository.GetVolatileSetting().AccessToken;

            try
            {
                // ログアウト処理を実行
                this.authenticationInformationRepository.Logout(deviceId, accessToken);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex);
                return false;
            }

            // 通知停止
            this.receiveNotificationRepository.Stop();

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

            // [フォント：フォント情報]の更新は、
            // fontActivationService.Deactivate内で実施されるためこの位置では保存しない

            // [ユーザー別保存：ログイン状態]に「ログアウト」を保存する
            userStatus.IsLoggingIn = false;
            this.userStatusRepository.SaveStatus(userStatus);

            return true;
        }
    }
}

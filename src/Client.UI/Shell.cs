using System;
using System.IO;
using System.Windows;
using ApplicationService.Authentication;
using ApplicationService.Fonts;
using ApplicationService.Interfaces;
using ApplicationService.Startup;
using Client.UI.Components;
using Client.UI.Interfaces;
using Client.UI.Wrappers;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.API;
using Infrastructure.File;
using Infrastructure.Memory;
using NLog;
using OS.Interfaces;
using OS.Services;
using Prism.Ioc;
using Prism.Unity;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace Client.UI
{
    /// <summary>
    /// アプリケーションに関する設定を行う
    /// </summary>
    public class Shell : PrismApplication
    {
        /// <summary>
        /// ロガー。
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>
        /// 共通設定フォルダ
        /// </summary>
        private static readonly string ApplicationSettingFolder = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "..", "config");

        /// <summary>
        /// ユーザーデータフォルダ
        /// </summary>
        private static readonly string UserDataDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Fontworks", "LETS");

        /// <summary>
        /// アプリケーションコンポーネント
        /// </summary>
        private ComponentManager componentManager;

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        public Shell()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        /// <summary>
        /// DIコンテナに登録する型を指定する
        /// </summary>
        /// <param name="containerRegistry">コンテナレジストリ</param>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // メモリ上で保存する情報
            var volatileSettingMemoryRepository = new VolatileSettingMemoryRepository();
            containerRegistry.RegisterInstance<IVolatileSettingRepository>(volatileSettingMemoryRepository);

            // 共通設定情報
            var applicationSettingRepository = new ApplicationSettingFileRepository(Path.Combine(ApplicationSettingFolder, "appsettings.json"));
            containerRegistry.RegisterInstance<IApplicationSettingRepository>(applicationSettingRepository);

            // 共通保存情報
            containerRegistry.RegisterInstance<IApplicationRuntimeRepository>(
                new ApplicationRuntimeFileRepository(Path.Combine(ApplicationSettingFolder, "appruntime.json")));

            // ユーザー別保存情報
            var userStatusFileRepository = new UserStatusFileRepository(Path.Combine(UserDataDirectory, "status.dat"));
            containerRegistry.RegisterInstance<IUserStatusRepository>(userStatusFileRepository);

            // フォント情報
            containerRegistry.RegisterInstance<IUserFontsSettingRepository>(
                new UserFontsSettingFileRepository(Path.Combine(UserDataDirectory, "fonts.dat")));

            // 契約情報
            var contractsAggregateRepository = new ContractsAggregateAPIRepositoryMock();
            containerRegistry.RegisterInstance<IContractsAggregateRepository>(contractsAggregateRepository);

            // キャッシュ情報 FUNCTION_08_02_01(お客様情報取得APIのレスポンス)
            containerRegistry.RegisterInstance<ICustomerRepository>(
                new CustomerFileRepository(Path.Combine(UserDataDirectory, "FUNCTION_08_02_01.dat")));

            // キャッシュ情報 FUNCTION_08_05_02(クライアントアプリの起動Ver情報取得APIのレスポンス)
            containerRegistry.RegisterInstance<IClientApplicationVersionRepository>(
                new ClientApplicationVersionFileRepository(Path.Combine(UserDataDirectory, "FUNCTION_08_05_02.dat")));

            // APIConfiguration
            ApplicationSetting applicationSetting = applicationSettingRepository.GetSetting();
            var apiConfiguration = new APIConfiguration(applicationSetting.FontDeliveryServerUri);

            // 認証情報(モック)
            containerRegistry.RegisterInstance<IAuthenticationInformationRepository>(
               new AuthenticationInformationRepositoryMock(apiConfiguration));

            // URL情報(モック)
            containerRegistry.RegisterInstance<IUrlRepository>(new UrlAPIRepositoryMock(apiConfiguration));

            // 端末情報(モック)
            var devicesRepository = new DevicesRepositoryMock(apiConfiguration);
            containerRegistry.RegisterInstance<IDevicesRepository>(devicesRepository);

            // フォント管理サービス(モック)
            var fontManagerService = new FontManagerServiceMock();
            containerRegistry.RegisterInstance<IFontManagerService>(fontManagerService);

            // フォントのアクティベート通知サービス
            var fontNotificationService = new FontNotificationService(fontManagerService);
            containerRegistry.RegisterInstance<IFontNotificationService>(fontNotificationService);

            // フォントのアクティベートサービス(モック)
            containerRegistry.Register<IFontActivationService, FontActivationServiceMock>();

            // 通知受信処理(モック)
            containerRegistry.RegisterInstance<IReceiveNotificationRepository>(
                new ReceiveNotificationAPIRepositoryMock(
                    apiConfiguration,
                    userStatusFileRepository,
                    fontNotificationService));

            // 各種画面で利用する情報
            var resourceWrapper = new ResourceWrapper();
            containerRegistry.RegisterInstance<IResourceWrapper>(resourceWrapper);
            containerRegistry.RegisterInstance<ILoginWindowWrapper>(new LoginWindowWrapper());

            // 認証サービス
            containerRegistry.RegisterInstance<IAuthenticationService>(
                new AuthenticationService((Application.Current as PrismApplication).Container));

            // キャッシュ情報 FUNCTION_08_03_02(契約情報取得APIのレスポンス)
            var contractsAggregateFileRepository = new ContractsAggregateFileRepository(Path.Combine(UserDataDirectory, "FUNCTION_08_03_02.dat"));

            // 起動時処理サービス
            containerRegistry.RegisterInstance<IStartupService>(new StartupService(
                resourceWrapper,
                fontManagerService,
                volatileSettingMemoryRepository,
                contractsAggregateRepository,
                contractsAggregateFileRepository,
                devicesRepository));
        }

        /// <summary>
        /// 起動時処理
        /// </summary>
        /// <param name="e">StartupEventArgs</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // アプリケーションコンポーネントを生成
            this.componentManager = new ComponentManager();

            // コンテナに登録したオブジェクトを取得する
            IContainerProvider container = (Current as PrismApplication).Container;

            // 起動処理

            // TODO 「5. クイックメニュー設定」の「ログイン中の場合」の処理を記載後、SetLoginStatusを適切な場所に移動してください
            // アカウント情報を表示する
            var customerRepository = container.Resolve<ICustomerRepository>();
            var customer = customerRepository.GetCustomer();
            this.componentManager.MenuLoginStatus.SetLoginStatus(customer);

            // 状態によってクイックメニューの表示を切替
            this.componentManager.MenuUpdateStatus.Hide();
            this.componentManager.MenuDownloadStatus.Hide();
            this.componentManager.ApplicationIcon.SetNormalMode();

            this.componentManager.Show();
        }

        /// <summary>
        /// 終了時処理
        /// </summary>
        /// <param name="e">ExitEventArgs</param>
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            this.componentManager.Dispose();
        }

        /// <summary>
        /// アプリケーションでメインに使用するWindowを設定する
        /// </summary>
        /// <returns>画面は表示しないため、nullを返す</returns>
        protected override Window CreateShell()
        {
            return null;
        }

        /// <summary>
        /// グローバル例外への対応確認
        /// 未処理例外を処理する
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">UnhandledExceptionEventArgs</param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // 例外発生時、エラーを通知してアプリケーションを終了する
            var exception = e.ExceptionObject as Exception;
            ExceptionHandling(exception);

            // アプリケーション終了処理
            Shell shell = (Shell)(System.Windows.Application.Current as PrismApplication);
            shell.Shutdown();
        }

        /// <summary>
        /// グローバル例外への対応確認
        /// 例外発生時処理
        /// </summary>
        /// <param name="exception">例外</param>
        private static void ExceptionHandling(Exception exception)
        {
            // 例外発生時の処理のため、コンテナを経由せず直接インスタンスを生成する
            var resourceWrapper = new ResourceWrapper();
            string message = resourceWrapper.GetString("APP_ERR_UnhandledException");
            Logger.Error(exception, message);

            var xml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText04);
            var stringElements = xml.GetElementsByTagName("text");
            stringElements[0].AppendChild(xml.CreateTextNode("LETS"));
            stringElements[1].AppendChild(xml.CreateTextNode(message));

            // アプリアイコンは実行ファイルと同じディレクトリにコピーする(コンテンツ－常にコピーする)
            var images = xml.GetElementsByTagName("image");
            var imagePath = "file:///" + Path.GetFullPath("ICON_APP.ico");
            ((Windows.Data.Xml.Dom.XmlElement)images[0]).SetAttribute("src", imagePath);

            // 表示間隔をロングに(デフォルトはショート)
            IXmlNode toastNode = xml.SelectSingleNode("/toast");
            ((Windows.Data.Xml.Dom.XmlElement)toastNode).SetAttribute("duration", "short");

            var appId = "LETS";
            var toast = new ToastNotification(xml);
            ToastNotificationManager.CreateToastNotifier(appId).Show(toast);
        }

        /// <summary>
        /// 起動時チェック処理
        /// </summary>
        /// <param name="container">DIコンテナ</param>
        /// <returns>チェック結果を返す</returns>
        private bool IsCheckedStartup(IContainerProvider container)
        {
            var startupService = container.Resolve<IStartupService>();
            var userStatusRepository = container.Resolve<IUserStatusRepository>();
            UserStatus userStatus = userStatusRepository.GetStatus();
            return startupService.IsCheckedStartup(userStatus.DeviceId, this.componentManager.Notice, this.componentManager.ForcedLogout);
        }
    }
}

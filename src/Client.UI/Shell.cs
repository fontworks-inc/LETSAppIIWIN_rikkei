﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Windows;
using ApplicationService.Authentication;
using ApplicationService.DeviceMode;
using ApplicationService.Fonts;
using ApplicationService.Interfaces;
using ApplicationService.Schedulers;
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

namespace Client.UI
{
    /// <summary>
    /// アプリケーションに関する設定を行う
    /// </summary>
    public class Shell : PrismApplication
    {
        /// <summary>
        /// ロガー
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
            Logger.Debug("Shell#Construct:Enter");

            try
            {
                // グローバル例外に対応するイベントハンドラを追加（WPF用）
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                // グローバル例外に対応するイベントハンドラを追加（Windowsフォーム用）
                System.Windows.Forms.Application.ThreadException +=
                    new ThreadExceptionEventHandler(Application_ThreadException);
            }
            catch (Exception e)
            {
                Logger.Error(e.StackTrace);
            }

            Logger.Debug("Shell#Construct:Exit");
        }

        /// <summary>
        /// 多重起動防止情報
        /// </summary>
        public static MultiplePreventionInfo MultiplePrevention { get; set; } = new Core.Entities.MultiplePreventionInfo();

        /// <summary>
        /// DIコンテナに登録する型を指定する
        /// </summary>
        /// <param name="containerRegistry">コンテナレジストリ</param>
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            Logger.Debug("Shell#RegisterTypes:Enter");

            {
                string selfpath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                System.IO.FileInfo fi = new System.IO.FileInfo(selfpath);

                System.Diagnostics.Process pro = new System.Diagnostics.Process();

                pro.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
                pro.StartInfo.Arguments = @"/c ver";
                pro.StartInfo.CreateNoWindow = true;
                pro.StartInfo.UseShellExecute = false;
                pro.StartInfo.RedirectStandardOutput = true;

                pro.Start();
                string output = pro.StandardOutput.ReadToEnd();

                string ver = string.Empty;
                try
                {
                    Assembly asm = Assembly.GetExecutingAssembly();
                    ver = $"(Ver{asm.GetName().Version})";
                }
                catch (Exception)
                {
                }

                Logger.Warn(string.Empty);
                Logger.Warn("=====================================================");
                Logger.Warn($"LETS.exe{ver}:CreationTime=" + fi.CreationTime);
                Logger.Warn(output);
                this.SetLogAccessEveryone();
            }

            // メモリ上で保存する情報
            Logger.Debug("Shell#RegisterTypes:メモリ上で保存する情報");
            var volatileSettingMemoryRepository = new VolatileSettingMemoryRepository();
            containerRegistry.RegisterInstance<IVolatileSettingRepository>(volatileSettingMemoryRepository);
            volatileSettingMemoryRepository.GetVolatileSetting().ClientApplicationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LETS.exe");

            // 共通設定情報
            Logger.Debug("Shell#RegisterTypes:共通設定情報");
            var applicationSettingRepository = new ApplicationSettingFileRepository(Path.Combine(ApplicationSettingFolder, "appsettings.json"));
            containerRegistry.RegisterInstance<IApplicationSettingRepository>(applicationSettingRepository);

            // プロキシ認証情報
            Logger.Debug("Shell#RegisterTypes:プロキシ認証情報");
            var proxyAuthSettingRepository = new ProxyAuthSettingFileRepository(Path.Combine(ApplicationSettingFolder, "puroxyauth.json"));
            containerRegistry.RegisterInstance<IProxyAuthSettingRepository>(proxyAuthSettingRepository);

            // 共通保存情報
            Logger.Debug("Shell#RegisterTypes:共通保存情報");
            var applicationRuntimeRepository = new ApplicationRuntimeFileRepository(Path.Combine(ApplicationSettingFolder, "appruntime.json"));
            containerRegistry.RegisterInstance<IApplicationRuntimeRepository>(applicationRuntimeRepository);

            // ユーザー別保存情報
            Logger.Debug("Shell#RegisterTypes:ユーザー別保存情報");
            var userStatusFileRepository = new UserStatusFileRepository(Path.Combine(UserDataDirectory, "status.dat"));
            containerRegistry.RegisterInstance<IUserStatusRepository>(userStatusFileRepository);
            volatileSettingMemoryRepository.GetVolatileSetting().RefreshToken = userStatusFileRepository.GetStatus().RefreshToken;
            Logger.Debug("RefreshToken=" + volatileSettingMemoryRepository.GetVolatileSetting().RefreshToken);

            string deviceid = userStatusFileRepository.GetStatus().DeviceId;
            Logger.Debug("deviceid=" + deviceid);

            // APIConfiguration
            Logger.Debug("Shell#RegisterTypes:APIConfiguration");
            ApplicationSetting applicationSetting = applicationSettingRepository.GetSetting();
            var apiConfiguration = new APIConfiguration(applicationSetting.FontDeliveryServerUri, applicationSetting.NotificationServerUri, applicationSetting.FixedTermConfirmationInterval, applicationSetting.CommunicationRetryCount, proxyAuthSettingRepository);

            // フォント情報
            Logger.Debug("Shell#RegisterTypes:フォント情報");
            var userFontsSettingFileRepository = new UserFontsSettingFileRepository(Path.Combine(UserDataDirectory, "fonts.dat"));
            containerRegistry.RegisterInstance<IUserFontsSettingRepository>(userFontsSettingFileRepository);

            // 各種画面で利用する情報
            Logger.Debug("Shell#RegisterTypes:各種画面で利用する情報");
            var resourceWrapper = new ResourceWrapper();
            containerRegistry.RegisterInstance<IResourceWrapper>(resourceWrapper);
            containerRegistry.RegisterInstance<ILoginWindowWrapper>(new LoginWindowWrapper());

            // 認証情報
            Logger.Debug("Shell#RegisterTypes:認証情報");
            var authenticationInformationRepository = new AuthenticationInformationAPIRepository(apiConfiguration);
            containerRegistry.RegisterInstance<IAuthenticationInformationRepository>(authenticationInformationRepository);

            // URL情報
            Logger.Debug("Shell#RegisterTypes:URL情報");
            containerRegistry.RegisterInstance<IUrlRepository>(new UrlAPIRepository(apiConfiguration));

            // フォント情報
            Logger.Debug("Shell#RegisterTypes:フォント情報");
            FontsAPIRepository fontsAPIRepository = new FontsAPIRepository(apiConfiguration);
            containerRegistry.RegisterInstance<IFontsRepository>(fontsAPIRepository);

            // 端末情報
            Logger.Debug("Shell#RegisterTypes:端末情報");
            var devicesRepository = new DevicesAPIRepository(apiConfiguration);
            containerRegistry.RegisterInstance<IDevicesRepository>(devicesRepository);

            // フォントの内部情報リポジトリ
            Logger.Debug("Shell#RegisterTypes:フォントの内部情報リポジトリ");
            var fontInfoRepository = new FontFileRepository(resourceWrapper);
            containerRegistry.Register<IFontFileRepository, FontFileRepository>();

            // デバイスモード時設定ファイル
            // 設定情報(デバイスモード時)
            Logger.Debug("Shell#RegisterTypes:設定情報(デバイスモード時)");
            //Logger.Warn("Shell#RegisterTypes:設定情報(デバイスモード時)");
            var deviceModeSettingRepository = new DeviceModeSettingRepository(Path.Combine(ApplicationSettingFolder, "dev-setting.dat"));
            containerRegistry.RegisterInstance<IDeviceModeSettingRepository>(deviceModeSettingRepository);

            // フォント情報(デバイスモード時)
            Logger.Debug("Shell#RegisterTypes:フォント情報(デバイスモード時)");
            var deviceModeFontListRepository = new DeviceModeFontListRepository(Path.Combine(ApplicationSettingFolder, "dev-fonts.dat"));
            containerRegistry.RegisterInstance<IDeviceModeFontListRepository>(deviceModeFontListRepository);

            // ライセンス情報(デバイスモード時)
            Logger.Debug("Shell#RegisterTypes:ライセンス情報(デバイスモード時)");
            var deviceModeLicenseInfoAPIRepository = new DeviceModeLisenceInfoAPIRepository(apiConfiguration);
            var deviceModeLicenseInfoRepository = new DeviceModeLicenseInfoRepository(Path.Combine(ApplicationSettingFolder, "dev-license.dat"), deviceModeLicenseInfoAPIRepository);
            containerRegistry.RegisterInstance<IDeviceModeLicenseInfoRepository>(deviceModeLicenseInfoRepository);

            // フォントのアクティベートサービス
            Logger.Debug("Shell#RegisterTypes:フォントのアクティベートサービス");
            var fontActivationService = new FontActivationService(userFontsSettingFileRepository, userStatusFileRepository, fontInfoRepository);
            containerRegistry.Register<IFontActivationService, FontActivationService>();

            // 未読お知らせ情報
            Logger.Debug("Shell#RegisterTypes:未読お知らせ情報");
            var unreadNoticeRepository = new UnreadNoticeRepository(apiConfiguration);
            containerRegistry.RegisterInstance<IUnreadNoticeRepository>(unreadNoticeRepository);

            // フォントセキュリティ情報を格納するリポジトリ
            Logger.Debug("Shell#RegisterTypes:フォントセキュリティ情報を格納するリポジトリ");
            var fontSecurityRepository = new FontSecurityAPIRepository(apiConfiguration);
            containerRegistry.RegisterInstance<IFontSecurityRepository>(fontSecurityRepository);

            // クライアントアプリケーションバージョン情報(API)
            Logger.Debug("Shell#RegisterTypes:クライアントアプリケーションバージョン情報(API)");
            var clientApplicationVersionAPIRepository = new ClientApplicationVersionAPIRepository(apiConfiguration);
            containerRegistry.RegisterInstance<IClientApplicationVersionRepository>(clientApplicationVersionAPIRepository);

            // プログラムバージョン取得サービス
            Logger.Debug("Shell#RegisterTypes:プログラムバージョン取得サービス");
            var applicationVersionService = new ApplicationVersionService(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LETS.exe"));
            containerRegistry.RegisterInstance<IApplicationVersionService>(applicationVersionService);

            // フォント管理サービス
            Logger.Debug("Shell#RegisterTypes:フォント管理サービス");
            var fontManagerService = new FontManagerService(
                resourceWrapper,
                applicationSettingRepository,
                volatileSettingMemoryRepository,
                userFontsSettingFileRepository,
                userStatusFileRepository,
                fontsAPIRepository,
                fontActivationService,
                fontInfoRepository);
            containerRegistry.RegisterInstance<IFontManagerService>(fontManagerService);

            // フォントのアクティベート通知サービス
            Logger.Debug("Shell#RegisterTypes:フォントのアクティベート通知サービス");
            var fontNotificationService = new FontNotificationService(fontManagerService);
            containerRegistry.RegisterInstance<IFontNotificationService>(fontNotificationService);

            // 通知受信処理
            Logger.Debug("Shell#RegisterTypes:通知受信処理");
            var receiveNotificationRepository = new ReceiveNotificationAPIRepository(
                apiConfiguration,
                userStatusFileRepository,
                fontNotificationService);
            containerRegistry.RegisterInstance<IReceiveNotificationRepository>(receiveNotificationRepository);

            // キャッシュ情報 FUNCTION_08_02_01(お客様情報取得APIのレスポンス)
            Logger.Debug("Shell#RegisterTypes:キャッシュ情報 FUNCTION_08_02_01(お客様情報取得APIのレスポンス)");
            containerRegistry.RegisterInstance<ICustomerRepository>(
                new CustomerFileRepository(Path.Combine(UserDataDirectory, "FUNCTION_08_02_01.dat"), new CustomerAPIRepository(apiConfiguration)));

            // キャッシュ情報 FUNCTION_08_03_02(契約情報取得APIのレスポンス)
            Logger.Debug("Shell#RegisterTypes:キャッシュ情報 FUNCTION_08_03_02(契約情報取得APIのレスポンス)");
            var contractsAggregateFileRepository = new ContractsAggregateFileRepository(Path.Combine(UserDataDirectory, "FUNCTION_08_03_02.dat"), new ContractsAggregateAPIRepository(apiConfiguration));

            // キャッシュ情報 FUNCTION_08_05_02(クライアントアプリの起動Ver情報取得APIのレスポンス)
            Logger.Debug("Shell#RegisterTypes:キャッシュ情報 FUNCTION_08_05_02(クライアントアプリの起動Ver情報取得APIのレスポンス)");
            var clientApplicationVersionFileRepository = new ClientApplicationVersionFileRepository(Path.Combine(UserDataDirectory, "FUNCTION_08_05_02.dat"), clientApplicationVersionAPIRepository);

            // 認証サービス
            Logger.Debug("Shell#RegisterTypes:認証サービス");
            containerRegistry.RegisterInstance<IAuthenticationService>(
                new AuthenticationService((Application.Current as PrismApplication).Container));

            // 更新プログラムダウンロードサービス
            Logger.Debug("Shell#RegisterTypes:更新プログラムダウンロードサービス");
            var applicationDownloadService = new ApplicationDownloadService(resourceWrapper, volatileSettingMemoryRepository, applicationRuntimeRepository, (IAPIConfiguration)apiConfiguration);
            containerRegistry.RegisterInstance<IApplicationDownloadService>(applicationDownloadService);

            // プロセス実施サービス
            Logger.Debug("Shell#RegisterTypes:プロセス実施サービス");
            var startProcessService = new StartProcessService(resourceWrapper);
            containerRegistry.RegisterInstance<IStartProcessService>(startProcessService);

            // プログラムアップデートサービス
            Logger.Debug("Shell#RegisterTypes:プログラムアップデートサービス");
            var applicationUpdateService = new ApplicationUpdateService(resourceWrapper, volatileSettingMemoryRepository, clientApplicationVersionFileRepository, applicationRuntimeRepository, startProcessService);
            containerRegistry.RegisterInstance<IApplicationUpdateService>(applicationUpdateService);

            // デバイスモードサービス
            Logger.Debug("Shell#RegisterTypes:デバイスモードサービス");
            var deviceModeService = new DeviceModeService(deviceModeSettingRepository, deviceModeFontListRepository, deviceModeLicenseInfoRepository, deviceModeLicenseInfoAPIRepository, startProcessService, fontInfoRepository);
            containerRegistry.RegisterInstance<IDeviceModeService>(deviceModeService);

            // アプリケーションコンポーネントを生成
            Logger.Debug("Shell#RegisterTypes:アプリケーションコンポーネントを生成");
            this.componentManager = new ComponentManager();
            this.componentManager.SetMultiplePreventionInfo(MultiplePrevention);
            var componentManagerWrapper = new ComponentManagerWrapper();
            componentManagerWrapper.Manager = this.componentManager;
            containerRegistry.RegisterInstance<IComponentManagerWrapper>(componentManagerWrapper);

            fontManagerService.ShowErrorDialogEvent = (string text, string caption) =>
            {
                Current.Dispatcher.Invoke(() =>
                {
                    ToastNotificationWrapper.Show(text, caption);
                });
            };
            fontManagerService.FontStartDownloadEvent = (InstallFont font, double compFileSize, double totalFileSize) =>
            {
                Current.Dispatcher.Invoke(() =>
                {
                    this.componentManager.StartFontDownload(font, compFileSize, totalFileSize);
                });
            };
            fontManagerService.FontDownloadCompletedEvent = (IList<InstallFont> fontList) =>
             {
                 Current.Dispatcher.Invoke(() =>
                 {
                     this.componentManager.FontDownloadCompleted(fontList);
                 });
             };

            fontManagerService.FontDownloadFailedEvent = (InstallFont font) =>
            {
                Current.Dispatcher.Invoke(() =>
                {
                    this.componentManager.FontDownloadFailed(font);
                });
            };

            fontManagerService.FontUpdateComplateEvent = () =>
            {
                Current.Dispatcher.Invoke(() =>
                {
                    this.componentManager.FontUpdateComplete();
                });
            };

            fontManagerService.FontDownloadCancelledEvent = (InstallFont font) =>
            {
                Current.Dispatcher.Invoke(() =>
                {
                    this.componentManager.FontDownloadCancelled(font);
                });
            };

            // 起動時処理サービス
            Logger.Debug("Shell#RegisterTypes:起動時処理サービス");
            var startupService = new StartupService(
                resourceWrapper,
                applicationVersionService,
                startProcessService,
                fontManagerService,
                volatileSettingMemoryRepository,
                applicationRuntimeRepository,
                userFontsSettingFileRepository,
                contractsAggregateFileRepository,
                devicesRepository,
                userStatusFileRepository,
                clientApplicationVersionFileRepository,
                unreadNoticeRepository,
                fontSecurityRepository,
                fontInfoRepository,
                authenticationInformationRepository);
            containerRegistry.RegisterInstance<IStartupService>(startupService);

            //startupService.ShowErrorDialogEvent = (string text, string caption) =>
            //{
            //    Current.Dispatcher.Invoke(() =>
            //    {
            //        ToastNotificationWrapper.Show(text, caption);
            //    });
            //};

            // 定期確認処理
            Logger.Debug("Shell#RegisterTypes:定期確認処理");
            containerRegistry.RegisterInstance<IFixedTermScheduler>(
                new FixedTermScheduler(
                    applicationSetting.FixedTermConfirmationInterval,
                    (Exception exception) =>
                    {
                        // 例外発生時の処理(UIスレッドで実行)
                        Current.Dispatcher.Invoke(() =>
                        {
                            ExceptionNotifier.Notify(exception);
                            this.Shutdown();
                        });
                    },
                    resourceWrapper,
                    volatileSettingMemoryRepository,
                    userStatusFileRepository,
                    startupService,
                    receiveNotificationRepository,
                    fontManagerService,
                    deviceModeService,
                    contractsAggregateFileRepository,
                    applicationSetting,
                    () =>
                    {
                        // 強制ログアウト画面の表示(UIスレッドで実行)
                        Current.Dispatcher.Invoke(() =>
                        {
                            this.componentManager.ForcedLogout();
                        });
                    },
                    () => Current.Dispatcher.Invoke(this.componentManager.ForcedUpdate),
                    () => Current.Dispatcher.Invoke(this.componentManager.UpdateProgramDownloadCompleted),
                    () => Current.Dispatcher.Invoke(this.componentManager.IsUpdated),
                    () => Current.Dispatcher.Invoke(this.componentManager.StartUpdateProgramDownload),
                    () => Current.Dispatcher.Invoke(this.componentManager.ForcedLogout),
                    (numberOfUnreadMessages) => Current.Dispatcher.Invoke(() => this.componentManager.ShowNotification(numberOfUnreadMessages)),
                    () => Current.Dispatcher.Invoke(() => this.componentManager.SetIcon()),
                    MultiplePrevention));

            // APIに強制ログアウト処理を持たせる
            Logger.Debug("Shell#RegisterTypes:APIに強制ログアウト処理を持たせる");
            apiConfiguration.ForceLogout = () =>
            {
                Current.Dispatcher.Invoke(() =>
                {
                    this.componentManager.ForcedLogout();
                });
            };

            Logger.Debug("Shell#RegisterTypes:Exit");
        }

        /// <summary>
        /// 起動時処理
        /// </summary>
        /// <param name="e">StartupEventArgs</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            Logger.Debug("Shell#OnStartup:Enter");

            base.OnStartup(e);

            // コンテナに登録したオブジェクトを取得する
            IContainerProvider container = (Current as PrismApplication).Container;

            // ウィンドウメッセージ受信用画面を表示
            this.componentManager.Show();

            // フォント管理
            var fontService = container.Resolve<IFontManagerService>();

            var userStatusRepository = container.Resolve<IUserStatusRepository>();

            this.OutputClearUserData();

            // [ユーザー別保存：デバイスモード]を取得する
            Logger.Debug("Shell#OnStartup:ユーザー別保存：デバイスモード]を取得する");
            if (userStatusRepository.GetStatus().IsDeviceMode)
            {
                // コンテナに登録したオブジェクトを取得する
                var deviceModeService = container.Resolve<IDeviceModeService>();
                try
                {
                    IList<string> messageList = deviceModeService.FixedTermCheck(true);
                    if (messageList.Count > 0)
                    {
                        foreach (string message in messageList)
                        {
                            ToastNotificationWrapper.Show("LETSオフライン専用アプリ", message);
                        }
                    }

                    var applicationRuntimeRepository = container.Resolve<IApplicationRuntimeRepository>();
                    var nextVersionInstaller = applicationRuntimeRepository.GetApplicationRuntime().NextVersionInstaller;
                    if (nextVersionInstaller != null && nextVersionInstaller.DownloadStatus == DownloadStatus.Update)
                    {
                        var volatileSettingMemoryRepository = container.Resolve<IVolatileSettingRepository>();
                        volatileSettingMemoryRepository.GetVolatileSetting().IsUpdated = true;
                        ToastNotificationWrapper.Show("LETSオフライン専用アプリ", "アップデート完了");

                        // アップデート完了時に共通保存情報をリセットする
                        Logger.Debug("OnStartup Exit:ApplicationRuntime:Reset");
                        applicationRuntimeRepository.SaveApplicationRuntime(new ApplicationRuntime());
                    }
                }
                catch (InvalidOperationException invalidEx)
                {
                    // アプリケーションを終了する
                    Logger.Debug("Shell#OnStartup:アプリケーションを終了する");
                    Logger.Error(invalidEx.StackTrace);
                    ToastNotificationWrapper.Show("LETSオフライン専用アプリ", invalidEx.Message);
                    Shell shell = (Shell)(System.Windows.Application.Current as PrismApplication);
                    shell.Shutdown();
                }
                catch (Exception ex)
                {
                    Logger.Debug("Shell#OnStartup:FixedTermCheck:Exception");
                    Logger.Error(ex.StackTrace);
                }

                Logger.Debug("Shell#OnStartup:return");

                return;
            }

            // [ユーザー別保存：ログイン状態]を取得する
            Logger.Debug("Shell#OnStartup:[ユーザー別保存：ログイン状態]を取得する");
            Logger.Debug(string.Format("OnStartup:[ユーザー別保存：ログイン状態]を取得する", string.Empty));
            if (userStatusRepository.GetStatus().IsLoggingIn)
            {
                // 状態表示：ログイン中を表示する
                Logger.Debug("Shell#OnStartup:状態表示：ログイン中を表示する");
                this.componentManager.QuickMenu.ShowLoginStatus();

                // アップデート状態確認：アップデート完了を表示する
                Logger.Debug("Shell#OnStartup:アップデート状態確認：アップデート完了を表示する");
                var applicationRuntimeRepository = container.Resolve<IApplicationRuntimeRepository>();
                var nextVersionInstaller = applicationRuntimeRepository.GetApplicationRuntime().NextVersionInstaller;
                if (nextVersionInstaller != null && nextVersionInstaller.DownloadStatus == DownloadStatus.Update)
                {
                    var volatileSettingMemoryRepository = container.Resolve<IVolatileSettingRepository>();
                    volatileSettingMemoryRepository.GetVolatileSetting().IsUpdated = true;
                    this.componentManager.SetIcon();
                    this.componentManager.QuickMenu.ShowDownloadStatus();
                    this.componentManager.QuickMenu.MenuUpdateStatus.SetCompleted();
                    this.componentManager.QuickMenu.Manager.UpdateCompleted();

                    // アップデート完了時に共通保存情報をリセットする
                    Logger.Debug("OnStartup Exit:ApplicationRuntime:Reset");
                    applicationRuntimeRepository.SaveApplicationRuntime(new ApplicationRuntime());
                }

                // クイックメニューを設定
                Logger.Debug("Shell#OnStartup:クイックメニューを設定");
                this.componentManager.ApplicationIcon.Enabled = true;

                // ユーザー配下のフォントフォルダ
                Logger.Debug("Shell#OnStartup:ユーザー配下のフォントフォルダ");
                var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var userFontsDir = @$"{local}\Microsoft\Windows\Fonts";

                // フォント一覧の更新
                Logger.Debug("Shell#OnStartup:フォント一覧の更新");
                fontService.UpdateFontsList(userFontsDir);

                // 起動時チェック処理
                Logger.Debug("Shell#OnStartup:起動時チェック処理");
                var receiveNotificationRepository = container.Resolve<IReceiveNotificationRepository>();
                var volatileSettingRepository = container.Resolve<IVolatileSettingRepository>();
                var startupService = container.Resolve<IStartupService>();
                if (startupService.IsCheckedStartup(
                    (System.Windows.Application.Current as PrismApplication).Shutdown,
                    this.componentManager.ForcedUpdate,
                    this.componentManager.UpdateProgramDownloadCompleted,
                    this.componentManager.IsUpdated,
                    this.componentManager.StartUpdateProgramDownload,
                    this.componentManager.ForcedLogout,
                    this.componentManager.ShowNotification,
                    () => this.componentManager.SetIcon(),
                    MultiplePrevention))
                {
                    // 起動時チェック処理が処理済みとなった場合、通知受信処理を開始する
                    Logger.Debug("Shell#OnStartup:起動時チェック処理が処理済みとなった場合、通知受信処理を開始する");
                    receiveNotificationRepository.Start(volatileSettingRepository.GetVolatileSetting().AccessToken, userStatusRepository.GetStatus().DeviceId);
                }
            }
            else
            {
                // ログアウト中の場合、LETSフォントをディアクティベートし、[フォント：フォント一覧]に保存
                Logger.Debug("Shell#OnStartup:ログアウト中の場合、LETSフォントをディアクティベートし、[フォント：フォント一覧]に保存");
                Logger.Debug("OnStartup:ログアウト中の場合、LETSフォントをディアクティベートし、[フォント：フォント一覧]に保存");
                fontService.DeactivateSettingFonts();
            }

            // アイコン表示ルールに従いアイコンを設定
            Logger.Debug("Shell#OnStartup:アイコン表示ルールに従いアイコンを設定");
            this.componentManager.SetIcon();

            // 定期確認処理の開始
            Logger.Debug("Shell#OnStartup:定期確認処理の開始");
            var fixedTermScheduler = container.Resolve<IFixedTermScheduler>();
            fixedTermScheduler.Start();

            Logger.Debug("Shell#OnStartup:Exit");
        }

        private static bool isOutputClearUserData = false;
        private static string userRegID = string.Empty;
        private static string userProfileImagePath = string.Empty;

        private void OutputClearUserData()
        {
            Logger.Debug("Shell#OutputClearUserData:Enter");

            Logger.Info("OutputClearUserData:Enter");
            try
            {
                if (isOutputClearUserData)
                {
                    Logger.Info("OutputClearUserData:Exit(Already Outputed");
                    return;
                }

                // ユーザデータ削除バッチを出力する
                // ユーザレジストリIDを取得する
                string userregid = this.GetUserRegID();

                // ホームドライブの取得
                string appPath = AppDomain.CurrentDomain.BaseDirectory;
                string homedrive = appPath.Substring(0, appPath.IndexOf("\\"));

                // LETSフォルダ
                //string letsfolder = $@"{homedrive}\ProgramData\Fontworks\LETS";
                string programdataFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                string letsfolder = $@"{programdataFolder}\Fontworks\LETS";

                string userDataDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Fontworks", "LETS");

                string clearUserDataPath = Path.Combine(letsfolder, $"clearuserdata_{userregid}.bat");
                if (System.IO.File.Exists(clearUserDataPath))
                {
                    this.SetHidden(clearUserDataPath, false);
                }

                System.IO.File.WriteAllText(clearUserDataPath, "REM ユーザー削除" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
                this.writeBatRunas(clearUserDataPath);
                System.IO.File.AppendAllText(clearUserDataPath, $"rd /s /q {userDataDirectory}\n");
                System.IO.File.AppendAllText(clearUserDataPath, @"Del /F ""%~dp0%~nx0""" + "\n");
                this.SetFileAccessEveryone(clearUserDataPath);
                this.SetHidden(clearUserDataPath, true);

                isOutputClearUserData = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace);
            }
            Logger.Info("OutputClearUserData:Exit");

            Logger.Debug("Shell#OutputClearUserData:Exit");
        }

        private void writeBatRunas(string fnm)
        {
            //File.AppendAllText(fnm, "cd /d %~dp0" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
            //File.AppendAllText(fnm, "whoami /priv | find \"SeDebugPrivilege\" > nul" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
            //File.AppendAllText(fnm, "if %errorlevel% neq 0 (" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
            //File.AppendAllText(fnm, "      @powershell start-process %~0 -verb runas" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
            //File.AppendAllText(fnm, "      exit" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
            //File.AppendAllText(fnm, "  )" + Environment.NewLine, System.Text.Encoding.GetEncoding("shift_jis"));
        }

        /// <summary>
        /// ユーザのレジストリIDを取得する
        /// </summary>
        private string GetUserRegID()
        {
            Logger.Debug("Shell#GetUserRegID:Enter");

            string username = Environment.UserName;

            if (string.IsNullOrEmpty(userRegID))
            {
                try
                {
                    Logger.Debug($"GetUserRegID:username={username}");
                    string regpath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList";
                    var regkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(regpath);
                    string[] subkeys = regkey.GetSubKeyNames();
                    regkey.Close();
                    foreach (string k in subkeys)
                    {
                        Logger.Debug($"GetUserRegID:subkey={k}");
                        var subregkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey($@"{regpath}\{k}");
                        string profileImagePath = (string)subregkey.GetValue("ProfileImagePath");
                        subregkey.Close();
                        if (!string.IsNullOrEmpty(profileImagePath))
                        {
                            string[] paths = profileImagePath.Split('\\');
                            string uname = paths[paths.Length - 1];
                            Logger.Debug($"GetUserRegID:uname={uname}");
                            if (string.Compare(uname, username, true) == 0)
                            {
                                userRegID = k;
                                userProfileImagePath = profileImagePath;
                                Logger.Debug($"userProfileImagePath:{userProfileImagePath}");
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Debug("GetUserRegID:" + ex.StackTrace);
                }
            }

            if (string.IsNullOrEmpty(userRegID))
            {
                userRegID = username.Replace(' ', '_');
            }

            Logger.Debug("Shell#GetUserRegID:Exit");

            return userRegID;
        }

        /// <summary>
        /// ファイルの隠し属性を設定/解除する
        /// </summary>
        /// <param name="filepath">ファイルパス</param>
        /// <param name="isHidden">隠し属性フラグ</param>
        private void SetHidden(string filepath, bool isHidden)
        {
            Logger.Debug("Shell#SetHidden:Enter");

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

            Logger.Debug("Shell#SetHidden:Exit");
        }

        private void SetFileAccessEveryone(string path)
        {
            Logger.Debug("Shell#SetFileAccessEveryone:Enter");

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

            Logger.Debug("Shell#SetFileAccessEveryone:Exit");
        }

        /// <summary>
        /// 終了時処理
        /// </summary>
        /// <param name="e">ExitEventArgs</param>
        protected override void OnExit(ExitEventArgs e)
        {
            Logger.Debug("Shell#OnExit:Enter");

            base.OnExit(e);

            this.componentManager.Dispose();

            Logger.Debug("Shell#OnExit:Exit");

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
        /// 未処理例外を処理する（WPF用）
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">UnhandledExceptionEventArgs</param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Debug("Shell#CurrentDomain_UnhandledException:Enter");

            // 例外発生時、エラーを通知する
            var exception = e.ExceptionObject as Exception;
            ExceptionNotifier.Notify(exception);

            // アプリケーションを終了する
            Shell shell = (Shell)(System.Windows.Application.Current as PrismApplication);
            shell.Shutdown();

            Logger.Debug("Shell#CurrentDomain_UnhandledException:Exit");
        }

        /// <summary>
        /// グローバル例外への対応確認
        /// 未処理例外を処理する（Windowsフォーム用）
        /// </summary>
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            Logger.Debug("Shell#Application_ThreadException:Enter");

            // 例外発生時、エラーを通知する
            ExceptionNotifier.Notify(e.Exception);

            // アプリケーションを終了する
            Shell shell = (Shell)(System.Windows.Application.Current as PrismApplication);
            shell.Shutdown();

            Logger.Debug("Shell#Application_ThreadException:Exit");
        }

        /// <summary>
        /// ログファイルアクセス権の設定
        /// </summary>
        private void SetLogAccessEveryone()
        {
            Logger.Debug("Shell#SetLogAccessEveryone:Enter");

            try
            {
                // ホームドライブの取得
                string appPath = AppDomain.CurrentDomain.BaseDirectory;
                string homedrive = appPath.Substring(0, appPath.IndexOf("\\"));

                // ログフォルダ
                //string letsfolder = $@"{homedrive}\ProgramData\Fontworks\LETS\config";
                string programdataFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                string letsfolder = $@"{programdataFolder}\Fontworks\LETS\config";

                // ログファイル名
                string normallog = Path.Combine(letsfolder, "LETS.log");
                string debuglog = Path.Combine(letsfolder, "LETS-debug.log");

                FileSystemAccessRule rule = new FileSystemAccessRule(
                    new NTAccount("everyone"),
                    FileSystemRights.FullControl,
                    AccessControlType.Allow);

                var sec = new FileSecurity();
                sec.AddAccessRule(rule);

                if (!File.Exists(normallog))
                {
                    File.Create(normallog);
                }

                System.IO.FileSystemAclExtensions.SetAccessControl(new FileInfo(normallog), sec);

                if (File.Exists(debuglog))
                {
                    System.IO.FileSystemAclExtensions.SetAccessControl(new FileInfo(debuglog), sec);
                }
            }
            catch (Exception e)
            {
                // NOP
                Logger.Error(e.StackTrace);
            }

            Logger.Debug("Shell#SetLogAccessEveryone:Exit");
        }
    }
}

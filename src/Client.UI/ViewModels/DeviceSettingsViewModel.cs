using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ApplicationService.Interfaces;
using Client.UI.Components;
using Client.UI.Interfaces;
using Client.UI.Views;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using NLog;
using Prism.Commands;
using Prism.Mvvm;

namespace Client.UI.ViewModels
{
    /// <summary>
    /// ３台目NG画面ビューモデル
    /// </summary>
    /// <remarks>画面ID：APP_05_01</remarks>
    public class DeviceSettingsViewModel : BindableBase
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>
        /// ユーザ別ステータス情報を格納するリポジトリ
        /// </summary>
        private readonly IUserStatusRepository userStatusRepository;

        /// <summary>
        /// メモリ上で保存する情報を格納するリポジトリ
        /// </summary>
        private readonly IVolatileSettingRepository volatileSettingRepository;

        /// <summary>
        /// 端末情報を格納するリポジトリ
        /// </summary>
        private readonly IDevicesRepository devicesRepository;

        /// <summary>
        /// 認証情報
        /// </summary>
        private readonly AuthenticationInformation authenticationInformation;

        /// <summary>
        /// (メイン)ログイン画面
        /// </summary>
        private readonly ILoginWindowWrapper loginWindow;

        /// <summary>
        /// アプリケーションコンポーネント
        /// </summary>
        private readonly ComponentManager componentManager;

        /// <summary>
        /// リソース読込み
        /// </summary>
        private readonly IResourceWrapper resouceWrapper;

        /// <summary>
        /// 端末情報一覧
        /// </summary>
        private IList<Device> devices;

        /// <summary>
        /// 次へボタンの文字色
        /// </summary>
        private Brush nextButtonForeground;

        /// <summary>
        /// 次へボタンの背景色
        /// </summary>
        private Brush nextButtonBackground;

        /// <summary>
        /// 次へボタンが押下可能かどうか
        /// </summary>
        private bool nextButtonIsEnabled;

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="loginWindowWrapper">LoginWindowのラッパー</param>
        /// <param name="resouceWrapper">Resourceのラッパー</param>
        /// <param name="componentManagerWrapper">ComponentManagerWrapperのラッパー</param>
        /// <param name="userStatusRepository">ユーザ別ステータス情報を格納するリポジトリ</param>
        /// <param name="volatileSettingRepository">メモリ上で保存する情報を格納するリポジトリ</param>
        /// <param name="devicesRepository">端末情報を格納するリポジトリ</param>
        public DeviceSettingsViewModel(
            ILoginWindowWrapper loginWindowWrapper,
            IResourceWrapper resouceWrapper,
            IComponentManagerWrapper componentManagerWrapper,
            IUserStatusRepository userStatusRepository,
            IVolatileSettingRepository volatileSettingRepository,
            IDevicesRepository devicesRepository)
        {
            this.loginWindow = loginWindowWrapper;
            this.resouceWrapper = resouceWrapper;
            this.componentManager = componentManagerWrapper.Manager;

            this.userStatusRepository = userStatusRepository;
            this.volatileSettingRepository = volatileSettingRepository;
            this.devicesRepository = devicesRepository;

            this.authenticationInformation = this.loginWindow.GetAuthenticationInformation();

            // 端末一覧を初期化
            if (!this.InitializeDeviceList())
            {
                return;
            }

            this.NextButtonClick = new DelegateCommand(this.OnNextButtonClick, this.CanExecute);

            Logger.Info(this.resouceWrapper.GetString("LOG_INFO_DeviceSettingsViewModel_Start"));
        }

        /// <summary>
        /// 次へボタンクリックコマンド
        /// </summary>
        public DelegateCommand NextButtonClick { get; }

        /// <summary>
        /// 次へボタンの文字色
        /// </summary>
        public Brush NextButtonForeground
        {
            get { return this.nextButtonForeground; }
            set { this.SetProperty(ref this.nextButtonForeground, value); }
        }

        /// <summary>
        /// 次へボタンの背景色
        /// </summary>
        public Brush NextButtonBackground
        {
            get { return this.nextButtonBackground; }
            set { this.SetProperty(ref this.nextButtonBackground, value); }
        }

        /// <summary>
        /// 次へボタンが押下可能かどうか
        /// </summary>
        public bool NextButtonIsEnabled
        {
            get
            {
                return this.nextButtonIsEnabled;
            }

            set
            {
                if (this.nextButtonIsEnabled == value)
                {
                    return;
                }
                else
                {
                    this.nextButtonIsEnabled = value;
                }

                this.NextButtonClick.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// 端末情報一覧
        /// </summary>
        public IList<Device> Devices
        {
            get { return this.devices; }
            set { this.SetProperty(ref this.devices, value); }
        }

        /// <summary>
        /// 端末リストビュー項目一覧
        /// </summary>
        public IList<DeviceViewModel> DevicesSource { get; private set; }

        /// <summary>
        /// ロゴ画像ソース
        /// </summary>
        public ImageSource ImageLogo
        {
            get
            {
                return this.resouceWrapper.GetImageSource("IMG_LOGO");
            }
        }

        /// <summary>
        /// 処理タイトル
        /// </summary>
        public string ProcessTitle
        {
            get
            {
                return this.resouceWrapper.GetString("APP_05_01_PROC_TITLE");
            }
        }

        /// <summary>
        /// 処理説明
        /// </summary>
        public string ProcessDescription
        {
            get
            {
                return this.resouceWrapper.GetString("APP_05_01_PROC_DESCRIPTION");
            }
        }

        /// <summary>
        /// 端末一覧タイトル
        /// </summary>
        public string DeviceListTitle
        {
            get
            {
                return this.resouceWrapper.GetString("APP_05_01_DEVICE_LIST_TITLE");
            }
        }

        /// <summary>
        /// 下部説明
        /// </summary>
        public string ProcessDescription2
        {
            get
            {
                return this.resouceWrapper.GetString("APP_05_01_PROC_DESCRIPTION2");
            }
        }

        /// <summary>
        /// 次へボタン名
        /// </summary>
        public string NextButtonTitle
        {
            get
            {
                return this.resouceWrapper.GetString("APP_05_01_BTN_NEXT");
            }
        }

        /// <summary>
        /// 次へボタンが押下可能かどうか
        /// </summary>
        /// <returns>次へボタン実行可否</returns>
        public bool CanExecute()
        {
            if (this.DevicesSource.Any(device => device.IsLoggingOut))
            {
                this.NextButtonForeground = Brushes.Black;
                this.NextButtonIsEnabled = true;
            }
            else
            {
                this.NextButtonForeground = Brushes.Gray;
                this.NextButtonIsEnabled = false;
            }

            return this.NextButtonIsEnabled;
        }

        /// <summary>
        /// 次へボタンクリック時の処理
        /// </summary>
        private void OnNextButtonClick()
        {
            Logger.Info(string.Format(
            this.resouceWrapper.GetString("LOG_INFO_DeviceSettingsViewModel_ButtonClick"),
            this.resouceWrapper.GetString("APP_05_01_BTN_NEXT")));

            // 自端末を使用中に設定する
            var newAuthenticationInformation = this.ActivateUserDevice();
            if (newAuthenticationInformation == null)
            {
                return;
            }

            try
            {
                // ログイン完了処理を実行
                this.componentManager.LoginCompleted(newAuthenticationInformation);

                // ログイン完了画面に遷移
                this.loginWindow.NavigationService.Navigate(new LoginCompleted());
            }
            catch (Exception ex)
            {
                // 配信サーバアクセスでエラーが発生したときは、画面を閉じ以後の処理を行わない
                var exception = new Exception(this.resouceWrapper.GetString("LOG_ERR_DeviceSettingsViewModel_LoginCompleted_Exception"), ex);
                Logger.Error(exception);
                this.loginWindow.Close();
            }
        }

        /// <summary>
        /// 自端末を使用中に設定する
        /// </summary>
        /// <returns>認証情報</returns>
        private AuthenticationInformation ActivateUserDevice()
        {
            try
            {
                // 自端末を利用可能にする
                var deviceId = this.userStatusRepository.GetStatus().DeviceId;
                var newAuthenticationInformation = this.devicesRepository.ActivateDevice(
                    deviceId, this.authenticationInformation.AccessToken);

                return newAuthenticationInformation;
            }
            catch (InvalidResponseCodeException ex)
            {
                // レスポンスコードが正常以外の場合、エラーメッセージを表示
                string errorMessage = this.resouceWrapper.GetString("APP_05_01_ERR_02");
                MessageBox.Show(errorMessage);
                Logger.Error(ex, errorMessage);
                return null;
            }
            catch (Exception ex)
            {
                // 配信サーバアクセスでエラーが発生したときは、画面を閉じ以後の処理を行わない
                var exception = new Exception(this.resouceWrapper.GetString("LOG_ERR_DeviceSettingsViewModel_ActivateUserDevice_Exception"), ex);
                Logger.Error(exception);
                this.loginWindow.Close();
                return null;
            }
        }

        /// <summary>
        /// 端末一覧を初期化する
        /// </summary>
        /// <returns>成否</returns>
        private bool InitializeDeviceList()
        {
            try
            {
                // 使用中の端末情報を取得
                var deviceId = this.userStatusRepository.GetStatus().DeviceId;
                this.devices = this.devicesRepository.GetAllDevices(deviceId, this.authenticationInformation.AccessToken);

                // 使用中の端末一覧を表示
                this.DevicesSource = new List<DeviceViewModel>();
                foreach (var device in this.devices)
                {
                    this.DevicesSource.Add(new DeviceViewModel(
                        this.loginWindow,
                        this.resouceWrapper,
                        this.devicesRepository,
                        this.userStatusRepository,
                        this.volatileSettingRepository,
                        this,
                        device));
                }

                return true;
            }
            catch (Exception ex)
            {
                // 配信サーバアクセスでエラーが発生したときは、画面を閉じ以後の処理を行わない
                var exception = new Exception(this.resouceWrapper.GetString("LOG_ERR_DeviceSettingsViewModel_InitializeDeviceList_Exception"), ex);
                Logger.Error(exception);
                this.loginWindow.Close();
                return false;
            }
        }
    }
}

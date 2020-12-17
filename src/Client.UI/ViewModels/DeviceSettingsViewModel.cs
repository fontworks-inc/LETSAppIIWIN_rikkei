using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Client.UI.Interfaces;
using Client.UI.Views;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
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
        /// (メイン)ログイン画面
        /// </summary>
        private ILoginWindowWrapper loginWindow;

        /// <summary>
        /// リソース読込み
        /// </summary>
        private IResourceWrapper resouceWrapper;

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="loginWindowWrapper">LoginWindowのラッパー</param>
        /// <param name="resouceWrapper">Resourceのラッパー</param>
        /// <param name="userStatusRepository">ユーザ別ステータス情報を格納するリポジトリ</param>
        /// <param name="volatileSettingRepository">メモリ上で保存する情報を格納するリポジトリ</param>
        /// <param name="devicesRepository">端末情報を格納するリポジトリ</param>
        public DeviceSettingsViewModel(
            ILoginWindowWrapper loginWindowWrapper,
            IResourceWrapper resouceWrapper,
            IUserStatusRepository userStatusRepository,
            IVolatileSettingRepository volatileSettingRepository,
            IDevicesRepository devicesRepository)
        {
            this.loginWindow = loginWindowWrapper;
            this.resouceWrapper = resouceWrapper;

            this.userStatusRepository = userStatusRepository;
            this.volatileSettingRepository = volatileSettingRepository;
            this.devicesRepository = devicesRepository;

            this.authenticationInformation = this.loginWindow.GetAuthenticationInformation();

            this.NextButtonClick = new DelegateCommand(this.OnNextButtonClick, this.CanExecute);

            // 端末一覧を初期化
            this.InitializeDeviceList();
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
            try
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
            catch (Exception)
            {
                // 【TODO：ログ出力】
                // 予期せぬ例外発生時は、ログイン画面を閉じる
                this.loginWindow.Close();
                return false;
            }
        }

        /// <summary>
        /// 次へボタンクリック時の処理
        /// </summary>
        private void OnNextButtonClick()
        {
            // 自端末を使用中に設定する
            var newAuthenticationInformation = this.ActivateUserDevice();
            if (newAuthenticationInformation == null)
            {
                return;
            }

            // ログイン完了処理
            if (this.CompleteLoginProcess(newAuthenticationInformation))
            {
                // ログイン完了画面に遷移
                this.loginWindow.NavigationService.Navigate(new LoginCompleted());
            }
        }

        /// <summary>
        /// 自端末を使用中に設定する
        /// </summary>
        /// <returns>成否</returns>
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
            catch (InvalidResponseCodeException)
            {
                // レスポンスコードが正常以外の場合、エラーメッセージを表示
                MessageBox.Show(this.resouceWrapper.GetString("APP_05_01_ERR_02"));

                // 【TODO：ログ出力】
                // ログイン画面を閉じる
                this.loginWindow.Close();
                return null;
            }
            catch (Exception)
            {
                // 【TODO：ログ出力】
                // 予期せぬ例外発生時は、ログイン画面を閉じる
                this.loginWindow.Close();
                return null;
            }
        }

        /// <summary>
        /// ログイン完了処理を実行
        /// </summary>
        /// <param name="newAuthenticationInformation">認証情報</param>
        /// <returns>成否</returns>
        private bool CompleteLoginProcess(AuthenticationInformation newAuthenticationInformation)
        {
            try
            {
                // ログイン完了処理を実行【TODO:ログイン完了処理】
                return true;
            }
            catch (Exception)
            {
                // 【TODO：ログ出力】
                // 予期せぬ例外発生時は、ログイン画面を閉じる
                this.loginWindow.Close();
                return false;
            }
        }

        /// <summary>
        /// 端末一覧を初期化する
        /// </summary>
        private void InitializeDeviceList()
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
            }
            catch (Exception)
            {
                // 【TODO：ログ出力】
                // 予期せぬ例外発生時は、ログイン画面を閉じる
                this.loginWindow.Close();
            }
        }
    }
}

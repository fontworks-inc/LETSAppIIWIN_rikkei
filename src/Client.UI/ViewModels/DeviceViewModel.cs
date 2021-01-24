using System;
using System.Windows;
using System.Windows.Media;
using Client.UI.Interfaces;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using NLog;
using Prism.Commands;
using Prism.Mvvm;

namespace Client.UI.ViewModels
{
    /// <summary>
    /// 端末一覧項目ビューモデル
    /// </summary>
    /// <remarks>端末設定画面の端末一覧項目</remarks>
    public class DeviceViewModel : BindableBase
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>
        /// メモリ上で保存する情報を格納するリポジトリ
        /// </summary>
        private readonly IVolatileSettingRepository volatileSettingRepository;

        /// <summary>
        /// ユーザ別ステータス情報を格納するリポジトリ
        /// </summary>
        private readonly IUserStatusRepository userStatusRepository;

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
        /// リソース読込み
        /// </summary>
        private readonly IResourceWrapper resouceWrapper;

        /// <summary>
        /// 端末設定画面ViewModel
        /// </summary>
        private readonly DeviceSettingsViewModel settingsViewModel;

        /// <summary>
        /// ログアウトボタンの表示状態
        /// </summary>
        private Visibility visibilityLogout;

        /// <summary>
        /// ログアウト済みラベルの表示状態
        /// </summary>
        private Visibility visibilityLoggedout;

        /// <summary>
        /// 端末情報
        /// </summary>
        private Device device;

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="loginWindowWrapper">LoginWindowのラッパー</param>
        /// <param name="resouceWrapper">Resourceのラッパー</param>
        /// <param name="devicesRepository">端末情報を格納するリポジトリ</param>
        /// <param name="userStatusRepository">ユーザ別ステータス情報を格納するリポジトリ</param>
        /// <param name="volatileSettingRepository">メモリ上で保存する情報を格納するリポジトリ</param>
        /// <param name="settingsViewModel">端末設定画面ViewModel</param>
        /// <param name="device">端末情報</param>
        public DeviceViewModel(
            ILoginWindowWrapper loginWindowWrapper,
            IResourceWrapper resouceWrapper,
            IDevicesRepository devicesRepository,
            IUserStatusRepository userStatusRepository,
            IVolatileSettingRepository volatileSettingRepository,
            DeviceSettingsViewModel settingsViewModel,
            Device device)
        {
            this.loginWindow = loginWindowWrapper;
            this.resouceWrapper = resouceWrapper;

            this.volatileSettingRepository = volatileSettingRepository;
            this.userStatusRepository = userStatusRepository;
            this.devicesRepository = devicesRepository;

            this.authenticationInformation = this.loginWindow.GetAuthenticationInformation();

            this.settingsViewModel = settingsViewModel;
            this.device = device;

            this.visibilityLogout = Visibility.Visible;
            this.visibilityLoggedout = Visibility.Collapsed;
            this.IsLoggingOut = false;

            this.LogoutButtonClick = new DelegateCommand(this.OnLogoutButtonClick);
        }

        /// <summary>
        /// 端末種類アイコンソース
        /// </summary>
        public ImageSource DiviceType
        {
            get
            {
                var resouceId = string.Empty;
                try
                {
                    switch (this.device.GetOSType())
                    {
                        case OSType.Windows:
                            resouceId = "IMG_DEVICE_PC";
                            break;
                        case OSType.MacOS:
                            resouceId = "IMG_DEVICE_PC";
                            break;
                        default:
                            resouceId = "IMG_DEVICE_PC";
                            break;
                    }
                }
                catch (InvalidOSTypeException ex)
                {
                    // OS種類が不正な場合、ログを出力
                    Logger.Warn(ex, string.Format(
                        this.resouceWrapper.GetString("LOG_WARN_DeviceViewModel_InvalidOSTypeException"), this.Name));

                    // 【Phase1】ではPCのみであるため、PC用アイコンを設定して続行
                    resouceId = "IMG_DEVICE_PC";
                }

                return this.resouceWrapper.GetImageSource(resouceId);
            }
        }

        /// <summary>
        /// ログアウトボタンクリックコマンド
        /// </summary>
        public DelegateCommand LogoutButtonClick { get; }

        /// <summary>
        /// 端末名（ホスト名＋OSユーザ名）
        /// </summary>
        public string Name
        {
            get
            {
                var deviceName = $"{this.device.HostName} {this.device.OSUserName}";
                return deviceName;
            }
        }

        /// <summary>
        /// ログアウトボタン名
        /// </summary>
        public string LogoutButtonTitle
        {
            get
            {
                return this.resouceWrapper.GetString("APP_05_01_DEVICE_LIST_LOGOUG");
            }
        }

        /// <summary>
        /// ログアウトボタンの表示状態
        /// </summary>
        public Visibility VisibilityLogout
        {
            get { return this.visibilityLogout; }
            set { this.SetProperty(ref this.visibilityLogout, value); }
        }

        /// <summary>
        /// ログアウト済みラベル名
        /// </summary>
        public string LoggedoutLabel
        {
            get
            {
                return this.resouceWrapper.GetString("APP_05_01_DEVICE_LIST_LOGGEDOUG");
            }
        }

        /// <summary>
        /// ログアウト済みラベルの表示状態
        /// </summary>
        public Visibility VisibilityLoggedout
        {
            get
            {
                return this.visibilityLoggedout;
            }

            set
            {
                this.SetProperty(ref this.visibilityLoggedout, value);
            }
        }

        /// <summary>
        /// 端末をログアウトしたかどうか
        /// </summary>
        public bool IsLoggingOut { get; private set; }

        /// <summary>
        /// ログアウトボタンクリック時の処理
        /// </summary>
        public void OnLogoutButtonClick()
        {
            Logger.Info(string.Format(
            this.resouceWrapper.GetString("LOG_INFO_DeviceViewModel_ButtonClick"),
            this.resouceWrapper.GetString("APP_09_01_BTN_LOGOUT"),
            this.Name));

            // 該当端末を解除する
            if (!this.DeactivateDevice())
            {
                return;
            }

            // 該当端末のログアウトボタンをログアウト済みに変更する
            this.VisibilityLogout = Visibility.Collapsed;
            this.VisibilityLoggedout = Visibility.Visible;
            this.IsLoggingOut = true;

            // 「次へ」ボタンを有効にする
            this.settingsViewModel.CanExecute();
        }

        /// <summary>
        /// 該当端末を解除する
        /// </summary>
        /// <returns>成否</returns>
        private bool DeactivateDevice()
        {
            try
            {
                // 該当端末を解除する
                this.devicesRepository.DeactivateDevice(
                    this.device.UserDeviceId, this.authenticationInformation.AccessToken, this.device.DeviceId);

                return true;
            }
            catch (InvalidResponseCodeException ex)
            {
                // レスポンスコードが正常以外の場合、エラーメッセージを表示
                string errorMessage = this.resouceWrapper.GetString("APP_05_01_ERR_01");
                MessageBox.Show(errorMessage);
                Logger.Error(ex, errorMessage);
                return false;
            }
            catch (Exception ex)
            {
                // 配信サーバアクセスでエラーが発生したときは、画面を閉じ以後の処理を行わない
                var exception = new Exception(
                    string.Format(
                        this.resouceWrapper.GetString("LOG_ERR_DeviceViewModel_DeactivateDevice_Exception"),
                        this.Name), ex);
                Logger.Error(exception);
                this.loginWindow.Close();
                return false;
            }
        }
    }
}

using System;
using System.Windows;
using System.Windows.Media;
using ApplicationService.Interfaces;
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
    /// 二要素認証画面ビューモデル
    /// </summary>
    /// <remarks>画面ID：APP_04_01_2fa, APP_04_01_2fa_err</remarks>
    public class TwoFactAuthenticationViewModel : BindableBase
    {
        /// <summary>
        /// 認証サービス
        /// </summary>
        private readonly IAuthenticationService authenticationService;

        /// <summary>
        /// 認証情報を格納するリポジトリ
        /// </summary>
        private readonly IAuthenticationInformationRepository authenticationInformationRepository;

        /// <summary>
        /// ユーザ別ステータス情報を格納するリポジトリ
        /// </summary>
        private readonly IUserStatusRepository userStatusRepository;

        /// <summary>
        /// 認証コード
        /// </summary>
        private string authenticationCode;

        /// <summary>
        /// 送信ボタンの文字色
        /// </summary>
        private Brush sendButtonForeground;

        /// <summary>
        /// 送信ボタンが押下可能かどうか
        /// </summary>
        private bool sendButtonIsEnabled;

        /// <summary>
        /// エラーメッセージ
        /// </summary>
        private string errorMessage;

        /// <summary>
        /// エラーメッセージ表示状態
        /// </summary>
        private Visibility errorMessageVisibility;

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
        /// <param name="authenticationInformationRepository">認証情報を格納するリポジトリ</param>
        /// <param name="userStatusRepository">ユーザ別ステータス情報を格納するリポジトリ</param>
        /// <param name="authenticationService">認証サービス</param>
        public TwoFactAuthenticationViewModel(
            ILoginWindowWrapper loginWindowWrapper,
            IResourceWrapper resouceWrapper,
            IAuthenticationInformationRepository authenticationInformationRepository,
            IUserStatusRepository userStatusRepository,
            IAuthenticationService authenticationService)
        {
            this.loginWindow = loginWindowWrapper;
            this.resouceWrapper = resouceWrapper;

            this.authenticationInformationRepository = authenticationInformationRepository;
            this.userStatusRepository = userStatusRepository;
            this.authenticationService = authenticationService;

            this.errorMessage = string.Empty;
            this.errorMessageVisibility = Visibility.Hidden;

            this.SendButtonClick = new DelegateCommand(this.OnSendButtonClick, this.CanSend);
        }

        /// <summary>
        /// 送信するボタンクリックコマンド
        /// </summary>
        public DelegateCommand SendButtonClick { get; }

        /// <summary>
        /// 認証コード
        /// </summary>
        public string AuthenticationCode
        {
            get
            {
                return this.authenticationCode;
            }

            set
            {
                this.SetProperty(ref this.authenticationCode, value);
                this.CanSend();
            }
        }

        /// <summary>
        /// 送信ボタンの文字色
        /// </summary>
        public Brush SendButtonForeground
        {
            get { return this.sendButtonForeground; }
            set { this.SetProperty(ref this.sendButtonForeground, value); }
        }

        /// <summary>
        /// 送信ボタンが押下可能かどうか
        /// </summary>
        public bool SendButtonIsEnabled
        {
            get
            {
                return this.sendButtonIsEnabled;
            }

            set
            {
                if (this.sendButtonIsEnabled == value)
                {
                    return;
                }
                else
                {
                    this.sendButtonIsEnabled = value;
                }

                this.SendButtonClick.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// エラーメッセージ
        /// </summary>
        public string ErrorMessage
        {
            get { return this.errorMessage; }
            set { this.SetProperty(ref this.errorMessage, value); }
        }

        /// <summary>
        /// エラーメッセージ表示状態
        /// </summary>
        public Visibility ErrorMessageVisibility
        {
            get { return this.errorMessageVisibility; }
            set { this.SetProperty(ref this.errorMessageVisibility, value); }
        }

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
                return this.resouceWrapper.GetString("APP_04_01_2fa_PROC_TITLE");
            }
        }

        /// <summary>
        /// 処理説明
        /// </summary>
        public string ProcessDescription
        {
            get
            {
                return this.resouceWrapper.GetString("APP_04_01_2fa_PROC_DESCRIPTION");
            }
        }

        /// <summary>
        /// 項目名(認証コード)
        /// </summary>
        public string AuthenticationCodeLabel
        {
            get
            {
                return this.resouceWrapper.GetString("APP_04_01_2fa_LBL_AUTHENTICATION_CODE");
            }
        }

        /// <summary>
        /// 送信ボタン名
        /// </summary>
        public string SendButtonTitle
        {
            get
            {
                return this.resouceWrapper.GetString("APP_04_01_2fa_BTN_SEND");
            }
        }

        /// <summary>
        /// 送信ボタンが押下可能かどうか
        /// </summary>
        private bool CanSend()
        {
            if (!string.IsNullOrEmpty(this.AuthenticationCode))
            {
                this.SendButtonForeground = Brushes.White;
                this.SendButtonIsEnabled = true;
            }
            else
            {
                this.SendButtonForeground = Brushes.Gray;
                this.SendButtonIsEnabled = false;
            }

            return this.SendButtonIsEnabled;
        }

        /// <summary>
        /// 送信するボタンクリック時の処理
        /// </summary>
        private void OnSendButtonClick()
        {
            try
            {
                // ユーザ別保存：デバイスIDを取得
                var deviceId = this.userStatusRepository.GetStatus().DeviceId;

                // ２要素認証処理を実行
                var authenticationInformation = this.authenticationInformationRepository
                    .TwoFactAuthentication(deviceId, this.AuthenticationCode);

                var responseCode = authenticationInformation.GetResponseCode();
                var responseMessage = authenticationInformation.Message;

                switch (responseCode)
                {
                    case ResponseCode.Succeeded:
                        // ログイン完了処理を実行【TODO:ログイン完了処理】
                        // ログイン完了画面に遷移
                        this.loginWindow.NavigationService.Navigate(new LoginCompleted());
                        break;
                    case ResponseCode.AuthenticationFailed:
                        // 認証エラー
                        this.ErrorMessage = this.resouceWrapper.GetString("APP_04_01_2fa_ERR_01");
                        this.ErrorMessageVisibility = Visibility.Visible;
                        break;
                    case ResponseCode.TwoFACodeHasExpired:
                        // ２要素認証コード有効期限切れエラー
                        this.ErrorMessage = this.resouceWrapper.GetString("APP_04_01_2fa_ERR_03");
                        this.ErrorMessageVisibility = Visibility.Visible;
                        break;

                    // case ResponseCode.InvalidArgument:
                    //    // 引数不正 ※機能仕様書に記載がないため対応不明
                    //    this.ErrorMessage = this.resouceWrapper.GetString("APP_04_01_2fa_ERR_02");
                    //    this.ErrorMessageVisibility = Visibility.Visible;
                    //    break;
                    case ResponseCode.MaximumNumberOfDevicesInUse:
                        // 端末設定画面に遷移
                        this.loginWindow.SetAuthenticationInformation(authenticationInformation.Data);
                        this.loginWindow.NavigationService.Navigate(new DeviceSettings());
                        break;
                    default:
                        // その他
                        this.ErrorMessage = string.Format(this.resouceWrapper.GetString("APP_04_ERR_EXCEPTION"), responseMessage);
                        this.ErrorMessageVisibility = Visibility.Visible;
                        break;
                }
            }
            catch (InvalidResponseCodeException ex)
            {
                // その他のレスポンスコードの場合、エラーメッセージを表示
                this.ErrorMessage = string.Format(this.resouceWrapper.GetString("APP_04_ERR_EXCEPTION"), ex.Message);
                this.ErrorMessageVisibility = Visibility.Visible;
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
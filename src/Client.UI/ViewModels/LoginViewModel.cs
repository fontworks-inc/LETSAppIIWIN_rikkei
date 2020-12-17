﻿using System;
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
    /// ログイン画面ビューモデル
    /// </summary>
    /// <remarks>画面ID：APP_04_01, APP_04_01_err</remarks>
    public class LoginViewModel : BindableBase
    {
        /// <summary>
        /// 認証サービス
        /// </summary>
        private readonly IAuthenticationService authenticationService;

        /// <summary>
        /// メールアドレス
        /// </summary>
        private string mailAddress;

        /// <summary>
        /// パスワード
        /// </summary>
        private string password;

        /// <summary>
        /// ログインボタンの文字色
        /// </summary>
        private Brush loginButtonForeground;

        /// <summary>
        /// ログインボタンが押下可能かどうか
        /// </summary>
        private bool loginButtonIsEnabled;

        /// <summary>
        /// エラーメッセージ
        /// </summary>
        private string errorMessage;

        /// <summary>
        /// エラーメッセージ表示状態
        /// </summary>
        private Visibility errorMessageVisibility;

        /// <summary>
        /// エラーメッセージ(メールアドレス)
        /// </summary>
        private string mailAddressErrorMessage;

        /// <summary>
        /// エラーメッセージ表示状態(メールアドレス)
        /// </summary>
        private Visibility mailAddressErrorMessageVisibility;

        /// <summary>
        /// エラーメッセージ(パスワード)
        /// </summary>
        private string passwordErrorMessage;

        /// <summary>
        /// エラーメッセージ表示状態(パスワード)
        /// </summary>
        private Visibility passwordErrorMessageVisibility;

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
        /// <param name="authenticationService">認証サービス</param>
        public LoginViewModel(
            ILoginWindowWrapper loginWindowWrapper,
            IResourceWrapper resouceWrapper,
            IAuthenticationService authenticationService)
        {
            this.loginWindow = loginWindowWrapper;
            this.resouceWrapper = resouceWrapper;

            this.authenticationService = authenticationService;

            this.errorMessageVisibility = Visibility.Hidden;
            this.mailAddressErrorMessageVisibility = Visibility.Hidden;
            this.passwordErrorMessageVisibility = Visibility.Hidden;

            this.ResetPasswordPageLinkClick = new DelegateCommand(this.OnResetPasswordPageLinkClick);
            this.AccountRegistrationPageLinkClick = new DelegateCommand(this.OnAccountRegistrationPageLinkClick);
            this.LoginButtonClick = new DelegateCommand(this.OnLoginButtonClick, this.CanLogin);
        }

        /// <summary>
        /// ログイン画面起動時処理コマンド
        /// </summary>
        public DelegateCommand LoginPageLoaded { get; }

        /// <summary>
        /// パスワード再設定ページへのリンククリックコマンド
        /// </summary>
        public DelegateCommand ResetPasswordPageLinkClick { get; }

        /// <summary>
        /// 会員登録するボタンクリックコマンド
        /// </summary>
        public DelegateCommand AccountRegistrationPageLinkClick { get; }

        /// <summary>
        /// ログインボタン押下時
        /// </summary>
        public DelegateCommand LoginButtonClick { get; private set; }

        /// <summary>
        /// メールアドレス
        /// </summary>
        public string MailAddress
        {
            get
            {
                return this.mailAddress;
            }

            set
            {
                this.SetProperty(ref this.mailAddress, value);
                this.CanLogin();
            }
        }

        /// <summary>
        /// パスワード
        /// </summary>
        public string Password
        {
            get
            {
                return this.password;
            }

            set
            {
                this.SetProperty(ref this.password, value);
                this.CanLogin();
            }
        }

        /// <summary>
        /// ログインボタンの文字色
        /// </summary>
        public Brush LoginButtonForeground
        {
            get { return this.loginButtonForeground; }
            set { this.SetProperty(ref this.loginButtonForeground, value); }
        }

        /// <summary>
        /// ログインボタンが押下可能かどうか
        /// </summary>
        public bool LoginButtonIsEnabled
        {
            get
            {
                return this.loginButtonIsEnabled;
            }

            set
            {
                if (this.loginButtonIsEnabled == value)
                {
                    return;
                }
                else
                {
                    this.loginButtonIsEnabled = value;
                }

                this.LoginButtonClick.RaiseCanExecuteChanged();
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
        /// エラーメッセージ(メールアドレス)
        /// </summary>
        public string MailAddressErrorMessage
        {
            get { return this.mailAddressErrorMessage; }
            set { this.SetProperty(ref this.mailAddressErrorMessage, value); }
        }

        /// <summary>
        /// エラーメッセージ表示状態(メールアドレス)
        /// </summary>
        public Visibility MailAddressErrorMessageVisibility
        {
            get { return this.mailAddressErrorMessageVisibility; }
            set { this.SetProperty(ref this.mailAddressErrorMessageVisibility, value); }
        }

        /// <summary>
        /// エラーメッセージ(パスワード)
        /// </summary>
        public string PasswordErrorMessage
        {
            get { return this.passwordErrorMessage; }
            set { this.SetProperty(ref this.passwordErrorMessage, value); }
        }

        /// <summary>
        /// エラーメッセージ表示状態(パスワード)
        /// </summary>
        public Visibility PasswordErrorMessageVisibility
        {
            get { return this.passwordErrorMessageVisibility; }
            set { this.SetProperty(ref this.passwordErrorMessageVisibility, value); }
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
                return this.resouceWrapper.GetString("APP_04_01_PROC_TITLE");
            }
        }

        /// <summary>
        /// 項目名(メールアドレスID)
        /// </summary>
        public string MailAdressLabel
        {
            get
            {
                return this.resouceWrapper.GetString("APP_04_01_LBL_MAILADRESS");
            }
        }

        /// <summary>
        /// 項目名(パスワード)
        /// </summary>
        public string PasswordLabel
        {
            get
            {
                return this.resouceWrapper.GetString("APP_04_01_LBL_PASSWORD");
            }
        }

        /// <summary>
        /// パスワード再設定画面案内
        /// </summary>
        public string ResetPasswordPageLabel
        {
            get
            {
                return this.resouceWrapper.GetString("APP_04_01_LBL_RESET_PASSWORD_PAGE");
            }
        }

        /// <summary>
        /// パスワード登録画面リンク
        /// </summary>
        public string ResetPasswordPageLinkLabel
        {
            get
            {
                return this.resouceWrapper.GetString("APP_04_LNK_HERE");
            }
        }

        /// <summary>
        /// アカウント登録画面案内
        /// </summary>
        public string RegistrationPageLabel
        {
            get
            {
                return this.resouceWrapper.GetString("APP_04_01_LBL_REGISTRATION_PAGE");
            }
        }

        /// <summary>
        /// アカウント登録画面リンク
        /// </summary>
        public string RegistrationPageLinkLabel
        {
            get
            {
                return this.resouceWrapper.GetString("APP_04_LNK_HERE");
            }
        }

        /// <summary>
        /// ログインボタン名
        /// </summary>
        public string LoginButtonTitle
        {
            get
            {
                return this.resouceWrapper.GetString("APP_04_01_BTN_LOGIN");
            }
        }

        /// <summary>
        /// ログインボタンが押下可能かどうか
        /// </summary>
        private bool CanLogin()
        {
            if (!string.IsNullOrEmpty(this.MailAddress) && !string.IsNullOrEmpty(this.Password))
            {
                this.LoginButtonForeground = Brushes.White;
                this.LoginButtonIsEnabled = true;
            }
            else
            {
                this.LoginButtonForeground = Brushes.Gray;
                this.LoginButtonIsEnabled = false;
            }

            return this.LoginButtonIsEnabled;
        }

        /// <summary>
        /// ログインボタンクリック時の処理
        /// </summary>
        private void OnLoginButtonClick()
        {
            try
            {
                // ログイン処理を実行
                var authenticationInformation = this.authenticationService.Login(
                    this.MailAddress, this.Password);

                var responseCode = authenticationInformation.GetResponseCode();
                var responseMessage = authenticationInformation.Message;

                switch (responseCode)
                {
                    case ResponseCode.Succeeded:
                        // ログイン完了処理を実行【TODO:ログイン完了処理】
                        // ログイン完了画面に遷移
                        this.loginWindow.NavigationService.Navigate(new LoginCompleted());
                        break;
                    case ResponseCode.TwoFAIsRequired:
                        // 二段階認証画面に遷移
                        this.loginWindow.NavigationService.Navigate(new TwoFactAuthentication());
                        break;

                    // case ResponseCode.InvalidArgument:
                    //    // 引数不正 ※機能仕様書に記載がないため対応不明
                    //    this.MailAddressErrorMessage = this.resouceWrapper.GetString("APP_04_01_ERR_02");
                    //    this.MailAddressErrorMessageVisibility = Visibility.Visible;
                    //    this.PasswordErrorMessage = this.resouceWrapper.GetString("APP_04_01_ERR_03");
                    //    this.PasswordErrorMessageVisibility = Visibility.Visible;
                    //    break;
                    case ResponseCode.AuthenticationFailed:
                        // 認証エラー
                        this.ErrorMessage = this.resouceWrapper.GetString("APP_04_01_ERR_01");
                        this.ErrorMessageVisibility = Visibility.Visible;
                        break;
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

        /// <summary>
        /// パスワード再設定ページへのリンククリック時の処理
        /// </summary>
        private void OnResetPasswordPageLinkClick()
        {
            MessageBox.Show("パスワード再設定ページを表示");
        }

        /// <summary>
        /// 会員登録ページへのリンククリック時の処理
        /// </summary>
        private void OnAccountRegistrationPageLinkClick()
        {
            MessageBox.Show("会員登録ページを表示");
        }
    }
}

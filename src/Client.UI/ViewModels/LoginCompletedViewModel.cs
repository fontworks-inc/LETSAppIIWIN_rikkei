using System;
using System.Windows.Media;
using Client.UI.Entities;
using Client.UI.Exceptions;
using Client.UI.Interfaces;
using Core.Entities;
using Core.Interfaces;
using NLog;
using Prism.Commands;
using Prism.Mvvm;

namespace Client.UI.ViewModels
{
    /// <summary>
    /// ログイン完了画面ビューモデル
    /// </summary>
    /// <remarks>画面ID：APP_06_01</remarks>
    public class LoginCompletedViewModel : BindableBase
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>
        /// (メイン)ログイン画面
        /// </summary>
        private ILoginWindowWrapper loginWindow;

        /// <summary>
        /// リソース読込み
        /// </summary>
        private IResourceWrapper resouceWrapper;

        /// <summary>
        /// URLアドレスを格納するリポジトリ
        /// </summary>
        private IUrlRepository urlRepository;

        /// <summary>
        /// メモリで保持する情報を格納するリポジトリ
        /// </summary>
        private IVolatileSettingRepository volatileSettingRepository;

        /// <summary>
        /// ユーザ別ステータス情報を格納するリポジトリ
        /// </summary>
        private IUserStatusRepository userStatusRepository;

        /// <summary>
        /// ブラウザ
        /// </summary>
        private WebBrowser webBrowser;

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="loginWindowWrapper">LoginWindowのラッパー</param>
        /// <param name="resouceWrapper">Resourceのラッパー</param>
        /// <param name="urlRepository">URLアドレスを格納するリポジトリ</param>
        /// <param name="volatileSettingRepository">メモリで保持する情報を格納するリポジトリ</param>
        /// <param name="userStatusRepository">ユーザ別ステータス情報を格納するリポジトリ</param>
        public LoginCompletedViewModel(
            ILoginWindowWrapper loginWindowWrapper,
            IResourceWrapper resouceWrapper,
            IUrlRepository urlRepository,
            IVolatileSettingRepository volatileSettingRepository,
            IUserStatusRepository userStatusRepository)
        {
            this.loginWindow = loginWindowWrapper;
            this.resouceWrapper = resouceWrapper;
            this.urlRepository = urlRepository;
            this.volatileSettingRepository = volatileSettingRepository;
            this.userStatusRepository = userStatusRepository;

            this.webBrowser = new WebBrowser();

            this.FontsListButtonClick = new DelegateCommand(this.OnFontsListButtonClick);

            Logger.Info(string.Format(
                this.resouceWrapper.GetString("LOG_INFO_LoginCompletedViewModel_Start")));
        }

        /// <summary>
        /// フォント一覧へボタンクリックコマンド
        /// </summary>
        public DelegateCommand FontsListButtonClick { get; }

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
                return this.resouceWrapper.GetString("APP_06_01_PROC_TITLE");
            }
        }

        /// <summary>
        /// 処理説明
        /// </summary>
        public string ProcessDescription
        {
            get
            {
                return this.resouceWrapper.GetString("APP_06_01_PROC_DESCRIPTION");
            }
        }

        /// <summary>
        /// フォント一覧へボタン名
        /// </summary>
        public string FontsListButtonTitle
        {
            get
            {
                return this.resouceWrapper.GetString("APP_06_01_BTN_FONTLIST");
            }
        }

        /// <summary>
        /// フォント一覧ボタンクリック時の処理
        /// </summary>
        private void OnFontsListButtonClick()
        {
            Logger.Info(string.Format(
               this.resouceWrapper.GetString("LOG_INFO_LoginCompletedViewModel_ButtonClick"),
               this.resouceWrapper.GetString("APP_06_01_BTN_FONTLIST")));

            try
            {
                // フォント一覧画面をブラウザで表示
                string deviceId = this.userStatusRepository.GetStatus().DeviceId;
                string accessToken = this.volatileSettingRepository.GetVolatileSetting().AccessToken;
                this.webBrowser.Navigate(this.GetFontListPageUrl(deviceId, accessToken));
            }
            catch (GetFontListPageUrlException e)
            {
                // 配信サーバアクセスでエラーが発生したときは、画面を閉じ以後の処理を行わない
                Logger.Error(e.StackTrace);
            }
            finally
            {
                this.loginWindow.Close();
            }
        }

        /// <summary>
        /// フォント一覧画面URLを取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>フォント一覧画面URL</returns>
        private Url GetFontListPageUrl(string deviceId, string accessToken)
        {
            try
            {
                return this.urlRepository.GetFontListPageUrl(deviceId, accessToken);
            }
            catch (Exception e)
            {
                throw new GetFontListPageUrlException(this.resouceWrapper.GetString("LOG_ERR_LoginCompletedViewModel_GetFontListPageUrlException"), e);
            }
        }
    }
}

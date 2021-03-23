using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ApplicationService.Interfaces;
using Client.UI.Interfaces;
using Client.UI.Views;
using Client.UI.Wrappers;
using Core.Entities;
using Core.Interfaces;
using NLog;
using Prism.Ioc;
using Prism.Unity;

namespace Client.UI.Components
{
    /// <summary>
    /// アプリケーションのコンポーネントを管理するクラス
    /// ウィンドウメッセージ受信用画面
    /// </summary>
    public partial class ComponentManager : Form
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>
        /// 自ウィンドウハンドラ
        /// </summary>
        private static IntPtr myhWnd = IntPtr.Zero;

        /// <summary>
        /// アップデート確認画面を表示しているかどうか
        /// </summary>
        private static bool isShownForceUpdate = false;

        /// <summary>
        /// 認証サービス
        /// </summary>
        private readonly IAuthenticationService authenticationService;

        /// <summary>
        /// プログラムのアップデートを行うサービス
        /// </summary>
        private readonly IApplicationUpdateService applicationUpdateService;

        /// <summary>
        /// プログラムのダウンロードを行うサービスクラス
        /// </summary>
        private readonly IApplicationDownloadService applicationDownloadService;

        /// <summary>
        /// メモリで保持する情報を格納するリポジトリ
        /// </summary>
        private readonly IVolatileSettingRepository volatileSettingRepository;

        /// <summary>
        /// アプリケーション共通保存情報を格納するリポジトリ
        /// </summary>
        private readonly IApplicationRuntimeRepository applicationRuntimeRepository;

        /// <summary>
        /// お客様情報API
        /// </summary>
        private readonly ICustomerRepository customerRepository;

        /// <summary>
        /// ユーザ別ステータス情報を格納するリポジトリ
        /// </summary>
        private readonly IUserStatusRepository userStatusRepository;

        /// <summary>
        /// ログイン画面(メイン)ラッパー
        /// </summary>
        private readonly ILoginWindowWrapper loginWindowWrapper;

        /// <summary>
        /// リソース読込み
        /// </summary>
        private readonly IResourceWrapper resouceWrapper;

        /// <summary>
        /// フォント管理に関する処理を行うサービス
        /// </summary>
        private IFontManagerService fontManagerService = null;

        /// <summary>
        /// 多重起動防止情報
        /// </summary>
        private MultiplePreventionInfo multipleInfo = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ComponentManager()
            : this(new Container())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="container">IContainer</param>
        public ComponentManager(IContainer container)
        {
            this.components = container;
            this.components.Add(this);

            // ウィンドウメッセージ受信用画面を初期化(最小化、タスクバー非表示、透明化)
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.Opacity = 0;

            // コンテナに登録されている情報を取得
            IContainerProvider containerProvider = (System.Windows.Application.Current as PrismApplication).Container;
            this.resouceWrapper = containerProvider.Resolve<IResourceWrapper>();
            this.loginWindowWrapper = containerProvider.Resolve<ILoginWindowWrapper>();

            // 設定ファイルを読み込む
            this.volatileSettingRepository = containerProvider.Resolve<IVolatileSettingRepository>();
            this.userStatusRepository = containerProvider.Resolve<IUserStatusRepository>();
            this.applicationRuntimeRepository = containerProvider.Resolve<IApplicationRuntimeRepository>();

            // 認証サービス
            this.authenticationService = containerProvider.Resolve<IAuthenticationService>();

            // プログラムのアップデートを行うサービス
            this.applicationUpdateService = containerProvider.Resolve<IApplicationUpdateService>();

            // プログラムのダウンロードを行うサービス
            this.applicationDownloadService = containerProvider.Resolve<IApplicationDownloadService>();

            // フォント管理に関する処理を行うサービス
            this.fontManagerService = containerProvider.Resolve<IFontManagerService>();

            // お客様情報API
            this.customerRepository = containerProvider.Resolve<ICustomerRepository>();

            // ユーザ情報API
            this.userStatusRepository = containerProvider.Resolve<IUserStatusRepository>();

            // コンポーネントを初期化(タスクトレイアイコン、クイックメニュー)
            this.InitializeComponent();

            Task task = Task.Run(() =>
            {
                this.ExitLETS(this);
            });
        }

        /// <summary>
        /// アプリケーションアイコン
        /// </summary>
        public ApplicationIconComponent ApplicationIcon
        {
            get;
            private set;
        }

        /// <summary>
        /// クイックメニュー
        /// </summary>
        public QuickMenuComponent QuickMenu
        {
            get;
            private set;
        }

        /// <summary>
        /// アプリケーションのコンポーネントをすべて破棄する
        /// </summary>
        public new void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// IResourceWrapper を返す
        /// </summary>
        /// <returns>IResourceWrapper</returns>
        public IResourceWrapper GetResource()
        {
            return this.resouceWrapper;
        }

        /// <summary>
        /// ログイン画面を起動する
        /// </summary>
        public void ShowLoginWindow()
        {
            // タスクトレイアイコンを操作不可とする
            this.ApplicationIcon.Enabled = false;

            // ログイン画面を取得
            var loginWindow = this.loginWindowWrapper.Window;
            if (loginWindow == null)
            {
                // 初回表示時は、新規作成
                loginWindow = new LoginWindow();
            }

            // 非表示の場合は再表示する
            if (loginWindow.Visibility != System.Windows.Visibility.Visible)
            {
                loginWindow.ShowDialog();
            }

            // 最小化されている場合は通常表示に戻す
            if (loginWindow.WindowState == System.Windows.WindowState.Minimized)
            {
                loginWindow.WindowState = System.Windows.WindowState.Normal;
            }

            loginWindow.Activate();

            // タスクトレイアイコンを操作可能とする
            this.ApplicationIcon.Enabled = true;
        }

        /// <summary>
        /// ログイン完了処理
        /// </summary>
        /// <param name="authenticationInformation">認証情報</param>
        public void LoginCompleted(AuthenticationInformation authenticationInformation)
        {
            // ログイン情報をメモリ、ユーザー別保存情報に保存
            this.authenticationService.SaveLoginInfo(authenticationInformation);

            // クイックメニューの状態表示部にログイン中情報を表示
            this.QuickMenu.ShowLoginStatus();

            // アイコン表示ルールに従いアイコンを設定
            this.ApplicationIcon.SetIcon();
        }

        /// <summary>
        /// アプリケーションを終了する
        /// </summary>
        /// <param name="deactivateFonts">フォントをディアクティベートするかどうか</param>
        public void Exit(bool deactivateFonts)
        {
            try
            {
                if (deactivateFonts)
                {
                    // ログアウト処理を実行する
                    this.Logout();
                }
                else
                {
                    // Mutexを削除する
                    if (this.multipleInfo != null && this.multipleInfo.HasHandle)
                    {
                        this.multipleInfo.MutexInfo.ReleaseMutex();
                        this.multipleInfo.MutexInfo.Close();
                        this.multipleInfo.HasHandle = false;
                    }

                    // 更新後のプログラムを起動する
                    // ホームドライブの取得
                    string homedrive = Environment.GetEnvironmentVariable("HOMEDRIVE");

                    // LETSアプリの起動(ショートカットを実行する)
                    string shortcut = $@"{homedrive}\ProgramData\Microsoft\Windows\Start Menu\Programs\StartUp\LETS デスクトップアプリ.lnk";
                    Process.Start(new ProcessStartInfo("cmd", $"/c \"{shortcut}\"") { CreateNoWindow = true });
                }
            }
            catch (Exception ex)
            {
                Logger.Debug("Exit:" + ex.StackTrace);
            }

            // アプリケーションを終了
            System.Windows.Application.Current.Shutdown();
        }

        /// <summary>
        /// 強制アップデート
        /// </summary>
        public void ForcedUpdate()
        {
            if (isShownForceUpdate)
            {
                return;
            }

            try
            {
                isShownForceUpdate = true;

                // タスクトレイアイコンを操作不可とする
                this.ApplicationIcon.Enabled = false;

                // 強制アップデートダイアログを表示
                var forcedUpdateDialog = new ForceUpdateNotification();
                forcedUpdateDialog.ShowDialog();

                // アップデート処理を実施
                this.StartUpdate();

                // タスクトレイアイコンを操作可能とする
                this.ApplicationIcon.Enabled = true;
            }
            finally
            {
                isShownForceUpdate = false;
            }
        }

        /// <summary>
        /// アップデート開始
        /// </summary>
        public void StartUpdate()
        {
            // アップデート処理実施
            try
            {
                this.applicationUpdateService.Update();
            }
            catch (Exception e)
            {
                Logger.Error(e, this.resouceWrapper.GetString("FUNC_01_02_17_ERR_UpdateFailed"));
                ToastNotificationWrapper.Show(this.resouceWrapper.GetString("FUNC_01_02_17_ERR_UpdateFailed"), e.Message);
                return;
            }

            // クイックメニューの状態表示部にアップデート中を表示
            this.QuickMenu.ShowUpdateStatus();

            // アイコン表示ルールに従いアイコンを設定
            this.ApplicationIcon.SetIcon();
        }

        /// <summary>
        /// アップデート完了
        /// </summary>
        public void UpdateCompleted()
        {
            this.volatileSettingRepository.GetVolatileSetting().IsUpdated = true;

            // クイックメニューの状態表示部にアップデート完了を表示
            this.QuickMenu.ShowUpdateStatus();

            // アイコン表示ルールに従いアイコンを設定
            this.ApplicationIcon.SetIcon();
        }

        /// <summary>
        /// 更新プログラムダウンロード開始
        /// </summary>
        public void StartUpdateProgramDownload()
        {
            // ダウンロード処理実施
            this.applicationDownloadService.StartDownloading(this.UpdateProgramDownloadCompleted, this.ForcedUpdate);

            // アイコン表示ルールに従いアイコンを設定
            this.ApplicationIcon.SetIcon();
        }

        /// <summary>
        /// 更新プログラムダウンロード完了
        /// </summary>
        public void UpdateProgramDownloadCompleted()
        {
            // アイコン表示ルールに従いアイコンを設定
            this.ApplicationIcon.SetIcon();

            // クイックメニューに「アップデート」を表示
            this.QuickMenu.MenuUpdate.Show();
        }

        /// <summary>
        /// フォントダウンロード開始
        /// </summary>
        /// <param name="font">フォント</param>
        /// <param name="compFileSize">ダウンロード済みファイルサイズ</param>
        /// <param name="totalFileSize">合計ファイルサイズ</param>
        public void StartFontDownload(InstallFont font, double compFileSize, double totalFileSize)
        {
            // クイックメニューの状態表示部にダウンロード中を表示
            this.QuickMenu.ShowDownloadStatus();

            // クイックメニューにダウンロード進捗とファイル名を表示
            this.QuickMenu.MenuDownloadStatus.SetProgressStatus(font, compFileSize, totalFileSize);

            // アイコン表示ルールに従いアイコンを設定
            this.ApplicationIcon.SetIcon();
        }

        /// <summary>
        /// フォントダウンロード完了
        /// </summary>
        /// <param name="fontList">フォントリスト</param>
        public void FontDownloadCompleted(IList<InstallFont> fontList)
        {
            // ダウンロード完了でクイックメニューを「ダウンロード中」から「ログイン中」に変更
            this.QuickMenu.MenuDownloadStatus.Hide();
            this.QuickMenu.MenuLoginStatus.Show();

            // アイコン表示ルールに従いアイコンを設定
            this.ApplicationIcon.SetIcon();

            // 通知メッセージを表示
            string[] nameList = fontList.Select(font => font.DisplayFontName).ToArray();
            string text = string.Format(this.resouceWrapper.GetString("MENU_DOWNLOAD_COMPLETED_TEXT"), fontList.Count, string.Join("、", nameList));
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding enc = Encoding.GetEncoding("Shift_JIS");
            if (enc.GetByteCount(text) > 138)
            {
                text = text.Substring(0, enc.GetString(enc.GetBytes(text), 0, 136).Length) + "…";
            }

            string title = this.resouceWrapper.GetString("MENU_DOWNLOAD_COMPLETED_TITLE");
            ToastNotificationWrapper.Show(title, text);

            // アイコン表示ルールに従いアイコンを設定
            this.ApplicationIcon.SetIcon();
        }

        /// <summary>
        /// フォントダウンロード失敗
        /// </summary>
        /// <param name="font">フォント</param>
        public void FontDownloadFailed(InstallFont font)
        {
            string title = this.resouceWrapper.GetString("FUNC_01_03_01_NOTIFIED_FailedToDownloadFonts");
            string text = string.Format(this.resouceWrapper.GetString("FUNC_01_03_01_NOTIFIED_FailedToDownloadFonts_Text"), font.DisplayFontName);
            Logger.Error(text);
        }

        /// <summary>
        /// 更新あり
        /// </summary>
        public void IsUpdated()
        {
            // メモリに「更新有り」と設定
            VolatileSetting volatileSetting = this.volatileSettingRepository.GetVolatileSetting();
            volatileSetting.IsUpdated = true;

            // アイコン表示ルールに従いアイコンを設定
            this.ApplicationIcon.SetIcon();
        }

        /// <summary>
        /// お知らせ
        /// </summary>
        /// <param name="numberOfUnreadMessages">未読お知らせの件数(デフォルトは0)</param>
        public void ShowNotification(int numberOfUnreadMessages = 0)
        {
            // メモリに「通知有り」と設定
            VolatileSetting volatileSetting = this.volatileSettingRepository.GetVolatileSetting();
            volatileSetting.IsNoticed = true;

            // クイックメニューにお知らせを表示
            this.QuickMenu.MenuAnnouncePage.SetNumberOfUnreadMessages(numberOfUnreadMessages);

            // アイコン表示ルールに従いアイコンを設定
            this.ApplicationIcon.SetIcon();
        }

        /// <summary>
        /// ログアウト処理
        /// </summary>
        /// <param name="isCallApi">ログアウトAPIを呼び出すかどうか</param>
        public void Logout(bool isCallApi = true)
        {
            // ログアウト処理実行
            if (this.authenticationService.Logout(isCallApi))
            {
                // クイックメニューを解除する
                this.ApplicationIcon.RemoveQuickMenu();

                // アイコン表示ルールに従いアイコンを設定
                this.ApplicationIcon.SetIcon();
            }
        }

        /// <summary>
        /// 強制ログアウト処理
        /// </summary>
        public void ForcedLogout()
        {
            if (!this.userStatusRepository.GetStatus().IsLoggingIn)
            {
                return;
            }

            // タスクトレイアイコンを操作不可とする
            this.ApplicationIcon.Enabled = false;

            // 強制ログアウトダイアログを表示
            var forcedLogoutDialog = new ForceLogoutNotification();
            forcedLogoutDialog.ShowDialog();

            // ログアウト処理を実施
            this.Logout(false);

            // タスクトレイアイコンを操作可能とする
            this.ApplicationIcon.Enabled = true;
        }

        /// <summary>
        /// アイコン表示ルールに従いアイコンを設定
        /// </summary>
        /// <param name="selected">選択中かどうか（デフォルト：選択中でない）</param>
        public void SetIcon(bool selected = false)
        {
            // アイコン表示ルールに従いアイコンを設定
            this.ApplicationIcon.SetIcon(selected);
        }

        /// <summary>
        /// 多重起動チェック情報設定
        /// </summary>
        /// <param name="multipleInfo">多重起動チェック情報</param>
        public void SetMultiplePreventionInfo(MultiplePreventionInfo multipleInfo)
        {
            this.multipleInfo = multipleInfo;
        }

        /// <summary>
        /// ウィンドウメッセージを処理する
        /// </summary>
        /// <param name="m">ウィンドウメッセージ</param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // ユーザー定義メッセージを処理する
            WindowMessageType messageType = (WindowMessageType)m.Msg;
            if (myhWnd == IntPtr.Zero)
            {
                myhWnd = m.HWnd;
            }

            switch (messageType)
            {
                case WindowMessageType.General:
                    LParamType paramType = (LParamType)m.LParam;
                    switch (paramType)
                    {
                        case LParamType.LoadLoginWindow:
                            // ログインメッセージ
                            this.ShowLoginWindow();
                            break;

                        case LParamType.Shutdown:
                            // 終了メッセージ(ディアクティベートなし)
                            Logger.Debug("WndProc:LParamType.Shutdown");
                            this.Exit(false);
                            break;

                        case LParamType.DeactivateFontsAndShutdown:
                            // 終了メッセージ(ディアクティベートあり)
                            Logger.Debug("WndProc:LParamType.DeactivateFontsAndShutdown");
                            this.Exit(true);
                            break;

                        default:
                            break;
                    }

                    break;

                case WindowMessageType.ProgressOfUpdate:
                    // プログラムアップデート進捗メッセージ
                    int progressRate = (int)m.LParam;
                    Logger.Debug("WndProc:WindowMessageType.ProgressOfUpdate:" + progressRate.ToString());
                    this.QuickMenu.ShowUpdateStatus();
                    this.QuickMenu.MenuUpdateStatus.SetProgressStatus(progressRate);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// コンポーネント初期化処理
        /// </summary>
        private void InitializeComponent()
        {
            // アプリケーションタイトル（ウィンドウメッセージ受信用画面のタイトル）
            this.Text = "LETS";
            this.SuspendLayout();

            // タスクトレイアイコンの生成
            this.ApplicationIcon = new ApplicationIconComponent(this);

            // クイックメニューの生成
            this.QuickMenu = new QuickMenuComponent(this);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]

        /// <summary>
        /// ウィンドウを列挙する
        /// </summary>
        /// <param name="lpEnumFunc">コールバック関数</param>
        /// <param name="lparam">コールバック関数パラメタ</param>
#pragma warning disable SA1204 // Static elements should appear before instance elements
        private static extern bool EnumWindows(EnumWindowsDelegate lpEnumFunc, IntPtr lparam);
#pragma warning restore SA1204 // Static elements should appear before instance elements

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]

        /// <summary>
        /// ウィンドウのタイトルを取得する
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="lpString">タイトル文字列</param>
        /// <param name="nMaxCount">文字列最大長</param>
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]

        /// <summary>
        /// ウィンドウのクラス名を取得する
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="lpClassName">クラス名文字列</param>
        /// <param name="nMaxCount">文字列最大長</param>
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]

        /// <summary>
        /// ウィンドウのタイトル長さを取得する
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]

        /// <summary>
        /// ウィンドウのプロセスIDを取得する
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="lpdwProcessId">プロセスID</param>
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        /// <summary>
        /// LETSが存在したか
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "<保留中>")]
        private static bool isExitLETS = false;

        /// <summary>
        /// ウィンドウのプロセスIDを取得する
        /// </summary>
        /// <param name="compManager">コンポーネントマネージャ</param>
        private async void ExitLETS(ComponentManager compManager)
        {
            try
            {
                isExitLETS = false;
                while (!isExitLETS)
                {
                    EnumWindows(new EnumWindowsDelegate(this.EnumWindowCallBack), IntPtr.Zero);
                    if (isExitLETS)
                    {
                        return;
                    }

                    await Task.Delay(3000);
                }
            }
            catch (Exception ex)
            {
                Logger.Debug("ExitLETS:" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "<保留中>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1615:Element return value should be documented", Justification = "<保留中>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:Elements should be documented", Justification = "<保留中>")]
        public delegate bool EnumWindowsDelegate(IntPtr hWnd, IntPtr lparam);

        /// <summary>
        /// ウィンドウ列挙のコールバック関数
        /// </summary>
        /// <param name="hWnd">ウィンドウハンドル</param>
        /// <param name="lparam">コールバックパラメタ</param>
        private bool EnumWindowCallBack(IntPtr hWnd, IntPtr lparam)
        {
            try
            {
                // ウィンドウのタイトルの長さを取得する
                int textLen = GetWindowTextLength(hWnd);
                if (textLen <= 0)
                {
                    return true;
                }

                // ウィンドウのタイトルを取得する
                StringBuilder tsb = new StringBuilder(textLen + 1);
                GetWindowText(hWnd, tsb, tsb.Capacity);
                if (!tsb.ToString().Contains("LETS-Ver"))
                {
                    return true;
                }

                // ウィンドウのクラス名を取得する
                StringBuilder csb = new StringBuilder(256);
                GetClassName(hWnd, csb, csb.Capacity);
                if (!csb.ToString().Contains("#32770"))
                {
                    return true;
                }

                // プロセスIDからプロセス名を取得する
                string procname = string.Empty;
                try
                {
                    int pid;
                    GetWindowThreadProcessId(hWnd, out pid);
                    Process p = Process.GetProcessById(pid);
                    procname = p.ProcessName;
                    if (!procname.Contains("msiexec"))
                    {
                        return true;
                    }
                }
                catch (Exception)
                {
                    // NOP
                }

                isExitLETS = true;
                if (myhWnd != IntPtr.Zero)
                {
                    SendMessage(myhWnd, 0x8001, 0, 3);
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message + "\n" + ex.StackTrace);
            }

            return true;
        }
    }
}

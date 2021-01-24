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
using System.Windows.Interop;
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
        /// 認証サービス
        /// </summary>
        private readonly IAuthenticationService authenticationService;

        /// <summary>
        /// お客様情報API
        /// </summary>
        private readonly ICustomerRepository customerRepository;

        /// <summary>
        /// ユーザー別保存情報
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

            // 認証サービス
            this.authenticationService = containerProvider.Resolve<IAuthenticationService>();

            // フォント管理に関する処理を行うサービス
            this.fontManagerService = containerProvider.Resolve<IFontManagerService>();

            // お客様情報API
            this.customerRepository = containerProvider.Resolve<ICustomerRepository>();

            // お客様情報API
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

            // 最前面にする
            loginWindow.Topmost = true;

            // 非表示の場合は再表示する
            if (loginWindow.Visibility != System.Windows.Visibility.Visible)
            {
                loginWindow.ShowDialog();
            }

            // 最小化されている場合は通常表示に戻す
            if (loginWindow.WindowState == System.Windows.WindowState.Minimized)
            {
                loginWindow.WindowState = System.Windows.WindowState.Normal;
                loginWindow.Topmost = false;
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
            if (deactivateFonts)
            {
                // ログアウト処理を実行する
                this.Logout();

                // 「LETSフォント」のフォントファイルパスを出力する
                this.fontManagerService.OutputLetsFontsList();
            }

            // アプリケーションを終了
            System.Windows.Application.Current.Shutdown();
        }

        /// <summary>
        /// 強制アップデート【Phase2】
        /// </summary>
        public void ForcedUpdate()
        {
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

        /// <summary>
        /// アップデート開始【Phase2】
        /// </summary>
        public void StartUpdate()
        {
            // クイックメニューの状態表示部にアップデート中を表示
            this.QuickMenu.ShowUpdateStatus();

            // アイコン表示ルールに従いアイコンを設定
            this.ApplicationIcon.SetIcon();
        }

        /// <summary>
        /// アップデート完了【Phase2】
        /// </summary>
        public void UpdateCompleted()
        {
            // アイコン表示ルールに従いアイコンを設定
            this.ApplicationIcon.SetIcon();
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
        }

        /// <summary>
        /// ログアウト処理
        /// </summary>
        public void Logout()
        {
            // ログアウト処理実行
            if (this.authenticationService.Logout())
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
            // タスクトレイアイコンを操作不可とする
            this.ApplicationIcon.Enabled = false;

            // 強制ログアウトダイアログを表示
            var forcedLogoutDialog = new ForceLogoutNotification();
            forcedLogoutDialog.ShowDialog();

            // ログアウト処理を実施
            this.Logout();

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

        private static IntPtr MyhWnd = IntPtr.Zero;
        /// <summary>
        /// ウィンドウメッセージを処理する
        /// </summary>
        /// <param name="m">ウィンドウメッセージ</param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            // ユーザー定義メッセージを処理する
            WindowMessageType messageType = (WindowMessageType)m.Msg;
            if (MyhWnd == IntPtr.Zero)
            {
                MyhWnd = m.HWnd;
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
                            this.Exit(false);
                            break;

                        case LParamType.DeactivateFontsAndShutdown:
                            // 終了メッセージ(ディアクティベートあり)
                            this.Exit(true);
                            break;

                        default:
                            break;
                    }

                    break;

                case WindowMessageType.ProgressOfUpdate:
                    // プログラムアップデート進捗メッセージ【Phase2】
                    int progressRate = (int)m.LParam;
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
        public extern static bool EnumWindows(EnumWindowsDelegate lpEnumFunc,
            IntPtr lparam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd,
            StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetClassName(IntPtr hWnd,
            StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        private static bool isExitLETS = false;
        private ComponentManager compManager;
        private void ExitLETS(ComponentManager compManager)
        {
            this.compManager = compManager;
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

                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Logger.Debug("ExitLETS:" + ex.Message);
            }
        }

        public delegate bool EnumWindowsDelegate(IntPtr hWnd, IntPtr lparam);

        private bool EnumWindowCallBack(IntPtr hWnd, IntPtr lparam)
        {
            try
            {
                // ウィンドウのタイトルの長さを取得する
                int textLen = GetWindowTextLength(hWnd);
                if (textLen > 0)
                {
                    // ウィンドウのタイトルを取得する
                    StringBuilder tsb = new StringBuilder(textLen + 1);
                    GetWindowText(hWnd, tsb, tsb.Capacity);

                    // ウィンドウのクラス名を取得する
                    StringBuilder csb = new StringBuilder(256);
                    GetClassName(hWnd, csb, csb.Capacity);

                    // プロセスIDからプロセス名を取得する
                    int pid;
                    GetWindowThreadProcessId(hWnd, out pid);
                    Process p = Process.GetProcessById(pid);
                    string procname = p.ProcessName;

                    if (tsb.ToString().Contains("LETS-Ver") && csb.ToString().Contains("#32770") && procname.Contains("msiexec"))
                    {
                        Logger.Debug("EnumWindowCallBack:" + tsb.ToString());
                        Logger.Debug("EnumWindowCallBack:ProcName=" + procname);
                        isExitLETS = true;
                        if (MyhWnd != IntPtr.Zero)
                        {
                            SendMessage(MyhWnd, 0x8001, 0, 3);
                        }

                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.Message);
            }

            return true;
        }

    }
}

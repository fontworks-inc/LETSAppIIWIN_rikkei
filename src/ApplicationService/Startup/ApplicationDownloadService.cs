using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using ApplicationService.Interfaces;
using Core.Entities;
using Core.Interfaces;
using Microsoft.Win32;
using NLog;

namespace ApplicationService.Startup
{
    /// <summary>
    /// プログラムのダウンロードを行うサービス
    /// </summary>
    public class ApplicationDownloadService : IApplicationDownloadService
    {
        /// <summary>
        /// 更新プログラムのファイル名
        /// </summary>
        private static readonly string FileNameTamplate = "LETS-Ver{0}.zip";

        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>
        /// 文言の取得を行うインスタンス
        /// </summary>
        private IResourceWrapper resourceWrapper = null;

        /// <summary>
        /// メモリで保持する情報を格納するリポジトリ
        /// </summary>
        private IVolatileSettingRepository volatileSettingRepository;

        /// <summary>
        /// 共通保存情報情報を格納するリポジトリ
        /// </summary>
        private IApplicationRuntimeRepository applicationRuntimeRepository;

        /// <summary>
        /// APIアクセスの設定情報を格納するリポジトリ
        /// </summary>
        private IAPIConfiguration aPIConfiguration;

        /// <summary>
        /// ダウンロード完了時に呼び出されるComponent側のイベント
        /// </summary>
        private DownloadCompletedComponentEvent downloadCompletedComponentEvent = null;

        /// <summary>
        /// アップデートチェック時に時に呼び出されるComponent側のイベント
        /// </summary>
        private ForceUpdateEvent forceUpdateEvent = null;

        /// <summary>
        /// ダウンロードメソッドのクライアント
        /// </summary>
        private WebClient webClient;

        /// <summary>
        /// タイムアウトまでの時間
        /// </summary>
        private int timeoutMillisecond;

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        public ApplicationDownloadService()
        {
        }

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="resourceWrapper">文言の取得を行うインスタンス</param>
        /// <param name="volatileSettingRepository">メモリで保持する情報を格納するリポジトリ</param>
        /// <param name="applicationRuntimeRepository">共通保存情報情報を格納するリポジトリ</param>
        /// <param name="timeoutMillisecond">タイムアウトまでの時間（デフォルトでは60秒）</param>
        /// <param name="aPIConfiguration">APIアクセスの設定</param>
        public ApplicationDownloadService(
            IResourceWrapper resourceWrapper,
            IVolatileSettingRepository volatileSettingRepository,
            IApplicationRuntimeRepository applicationRuntimeRepository,
            IAPIConfiguration aPIConfiguration,
            int timeoutMillisecond = 600000)
        {
            this.resourceWrapper = resourceWrapper;
            this.volatileSettingRepository = volatileSettingRepository;
            this.applicationRuntimeRepository = applicationRuntimeRepository;
            this.aPIConfiguration = aPIConfiguration;
            this.timeoutMillisecond = timeoutMillisecond;
        }

        /// <summary>
        /// プログラムのダウンロードを開始
        /// </summary>
        /// <param name="downloadCompletedComponentEvent">ダウンロード完了時に呼び出されるComponent側のイベント</param>
        /// <param name="forceUpdateEvent">アップデートチェック時に時に呼び出されるComponent側のイベント</param>
        public void StartDownloading(DownloadCompletedComponentEvent downloadCompletedComponentEvent, ForceUpdateEvent forceUpdateEvent)
        {
            // コンポーネントのイベントを設定する
            this.downloadCompletedComponentEvent = downloadCompletedComponentEvent;
            this.forceUpdateEvent = forceUpdateEvent;

            // 更新プログラムをダウンロードする
            this.Download();

            // [共通保存：更新プログラム情報.ダウンロード状態]に「ダウンロード中」を設定する
            ApplicationRuntime applicationRuntime = this.applicationRuntimeRepository.GetApplicationRuntime();
            applicationRuntime.NextVersionInstaller.DownloadStatus = DownloadStatus.Running;
            this.applicationRuntimeRepository.SaveApplicationRuntime(applicationRuntime);

            // メモリに「ダウンロード中」を設定する
            VolatileSetting volatileSetting = this.volatileSettingRepository.GetVolatileSetting();
            volatileSetting.CompletedDownload = false;
            volatileSetting.IsDownloading = true;
        }

        /// <summary>
        /// プログラムのダウンロードを完了
        /// </summary>
        /// <param name="downloadCompletedComponentEvent">ダウンロード完了時に呼び出されるComponent側のイベント</param>
        /// <param name="forceUpdateEvent">強制アップデートチェック時に時に呼び出されるComponent側のイベント</param>
        public void CompleteDownloading(DownloadCompletedComponentEvent downloadCompletedComponentEvent = null, ForceUpdateEvent forceUpdateEvent = null)
        {
            // 引数で渡されている場合、コンポーネントのイベントを設定する
            if (downloadCompletedComponentEvent != null)
            {
                this.downloadCompletedComponentEvent = downloadCompletedComponentEvent;
            }

            if (forceUpdateEvent != null)
            {
                this.forceUpdateEvent = forceUpdateEvent;
            }

            // [共通保存：更新プログラム情報.ダウンロード状態]に「ダウンロード完了」と設定する
            ApplicationRuntime applicationRuntime = this.applicationRuntimeRepository.GetApplicationRuntime();
            applicationRuntime.NextVersionInstaller.DownloadStatus = DownloadStatus.Completed;
            this.applicationRuntimeRepository.SaveApplicationRuntime(applicationRuntime);

            // メモリの「ダウンロード中」を削除する
            VolatileSetting volatileSetting = this.volatileSettingRepository.GetVolatileSetting();
            volatileSetting.IsDownloading = false;

            // [共通保存：更新プログラム情報.強制/任意]が「強制」か「任意」かで処理を切り替える
            if (this.applicationRuntimeRepository.GetApplicationRuntime().NextVersionInstaller.ApplicationUpdateType)
            {
                // 「強制」の場合、「強制アップデートチェック」の処理を行う
                StartupService startupService = new StartupService(null, null, null, null, this.volatileSettingRepository, this.applicationRuntimeRepository, null, null, null, null, null, null, null, null);
                startupService.ForceUpdateCheck(this.forceUpdateEvent, () => { });
            }
            else
            {
                // 「任意」の場合、メモリに「ダウンロード完了」を設定し、アイコン・メニューを設定する
                volatileSetting.CompletedDownload = true;
                this.downloadCompletedComponentEvent();
            }
        }

        /// <summary>
        /// ダウンロードを実行
        /// </summary>
        private void Download()
        {
            Installer installer = this.applicationRuntimeRepository.GetApplicationRuntime().NextVersionInstaller;

            // ダウンロードURL
            Uri url = new Uri(installer.Url);
            IWebProxy proxyServer = this.aPIConfiguration.GetWebProxy(installer.Url);

            // ダウンロード先のファイルパス
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string homedrive = appPath.Substring(0, appPath.IndexOf("\\"));
            //string dirPath = Path.Combine($@"{homedrive}\ProgramData\Fontworks\LETS", "LETS-Ver" + installer.Version);
            string programdataFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string dirPath = Path.Combine($@"{programdataFolder}\Fontworks\LETS", "LETS-Ver" + installer.Version);
            string fileName = string.Format(FileNameTamplate, installer.Version);
            string filePath = Path.Combine(dirPath, fileName);
            if (!Directory.Exists(dirPath))
            {
                // ダウンロードフォルダが存在しなければ作成する
                Directory.CreateDirectory(dirPath);
            }

            // WebClientの作成
            if (this.webClient == null)
            {
                this.webClient = new MyWebClient(this.timeoutMillisecond);
                if (proxyServer != null)
                {
                    this.webClient.Proxy = proxyServer;
                }

                // イベントハンドラの作成
                this.webClient.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(this.NotifyDownloading);
            }

            Logger.Info(this.resourceWrapper.GetString("LOG_INFO_ApplicationDownloadService_Start"));

            // ダウンロードを開始
            this.webClient.DownloadFileAsync(url, filePath);
        }

        /// <summary>
        /// ダウンロード完了を通知する
        /// </summary>
        private void NotifyDownloading(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            // ダウンロード完了時に解放する
            this.webClient.Dispose();

            if (e.Error == null)
            {
                // 正常に終了した場合は完了処理
                Logger.Info(this.resourceWrapper.GetString("LOG_INFO_ApplicationDownloadService_Success"));

                this.CompleteDownloading();
            }
            else
            {
                // エラー処理
                Logger.Error(this.resourceWrapper.GetString("LOG_ERROR_ApplicationDownloadService_Fail"));

                // [共通保存：更新プログラム情報]を空にする（ダウンロード失敗後、更新情報が残ったままだと再ダウンロードが行われないため）
                this.applicationRuntimeRepository.SaveApplicationRuntime(new ApplicationRuntime());

                // メモリの「ダウンロード中」を削除する
                VolatileSetting volatileSetting = this.volatileSettingRepository.GetVolatileSetting();
                volatileSetting.IsDownloading = false;
            }
        }

        private class MyWebClient : WebClient
        {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="timeout">タイムアウトまでの時間</param>
            public MyWebClient(int timeout)
            {
                this.Timeout = timeout;
            }

            /// <summary>
            /// タイムアウトまでの時間(ms)
            /// </summary>
            public int Timeout { get; set; }

            /// <summary>
            /// 指定したリソースの WebRequest オブジェクトを返します。
            /// </summary>
            /// <param name="address">アドレス</param>
            protected override WebRequest GetWebRequest(Uri address)
            {
                var request = base.GetWebRequest(address);
                if (request != null)
                {
                    request.Timeout = this.Timeout;
                }

                return request;
            }
        }
    }
}

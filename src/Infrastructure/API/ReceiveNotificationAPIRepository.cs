using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Memory;
using Microsoft.Win32;
using Newtonsoft.Json;
using NLog;

namespace Infrastructure.API
{
    /// <summary>
    /// 通知受信機能を格納するリポジトリのクラス
    /// </summary>
    public class ReceiveNotificationAPIRepository : APIRepositoryBase, IReceiveNotificationRepository
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
        /// フォントのアクティベート通知のサービス
        /// </summary>
        /// <remarks>フォントアクティベートを受信後、該当するメソッドを呼ぶことで通知する</remarks>
        private readonly IFontNotificationService fontNotificationService;

        /// <summary>
        /// HTTP Client インスタンス.
        /// </summary>
        private HttpClient client = null;

        /// <summary>
        /// Stream Reader. 非同期でメッセージを読み込む.
        /// </summary>
        private StreamReader streamReader;

        /// <summary>
        /// 購読中であることを表すフラグ.
        /// </summary>
        private bool subscribed = false;

        /// <summary>
        /// 処理機能
        /// </summary>
        private Action<List<string>> emitter;

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="apiConfiguration">設定情報</param>
        /// <param name="userStatusRepository">ユーザ別ステータス情報を格納するリポジトリ</param>
        /// <param name="fontNotificationService">フォントのアクティベート通知のサービス</param>
        public ReceiveNotificationAPIRepository(
            APIConfiguration apiConfiguration,
            IUserStatusRepository userStatusRepository,
            IFontNotificationService fontNotificationService)
            : base(apiConfiguration)
        {
            this.userStatusRepository = userStatusRepository;
            this.fontNotificationService = fontNotificationService;
            this.emitter = this.MessageHandler;
        }

        /// <summary>
        /// SSE接続を開始する
        /// </summary>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>SSE接続の成否</returns>
        public bool Start(string accessToken, string deviceid)
        {
            if (this.client == null)
            {
                HttpClientHandler ch = new HttpClientHandler();
                string proxyserver = this.GetProxyServer();
                if (proxyserver != null)
                {
                    ch.Proxy = new WebProxy(proxyserver);
                    ch.UseProxy = true;
                    this.client = new HttpClient(ch);
                }
                else {
                    this.client = new HttpClient();
                }
                this.client.BaseAddress = new Uri(this.NotifyBasePath);
                this.client.Timeout = Timeout.InfiniteTimeSpan;
            }

            // var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:3000" + "/notifications");
            //var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:3000" + "/sse2");
            ApplicationSetting setting = new ApplicationSetting();
            //var request = new HttpRequestMessage(HttpMethod.Get, setting.NotificationServerUri + "/notifications");
            var request = new HttpRequestMessage(HttpMethod.Get, this.NotifyBasePath + "/notifications");

            request.Headers.Add("User-Agent", this.GetUserAgent());
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("Accept", "text/event-stream");
            request.Headers.Add("Cache-Control", "no-cache,no-store");
            request.Headers.Add("X-LETS-DEVICEID", deviceid);
            if (accessToken != null)
            {
                request.Headers.Add("Authorization", $"Bearer {accessToken}");
            }

            Task.Run(() => this.Sbscriber(this.client, request));

            return true;
        }

        private string GetUserAgent()
        {
            VolatileSetting vSetting = new VolatileSettingMemoryRepository().GetVolatileSetting();

            if (!string.IsNullOrEmpty(vSetting.UserAgent))
            {
                return vSetting.UserAgent;
            }

            // アプリバージョンの取得
            var output = string.Empty;
            var appver = string.Empty;
            var apppath = vSetting.ClientApplicationPath;

            if (System.IO.File.Exists(apppath))
            {
                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(apppath);
                appver = versionInfo.ProductVersion;
            }

            System.Diagnostics.Process pro = new System.Diagnostics.Process();

            pro.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
            pro.StartInfo.Arguments = @"/c ver";
            pro.StartInfo.CreateNoWindow = true;
            pro.StartInfo.UseShellExecute = false;
            pro.StartInfo.RedirectStandardOutput = true;

            pro.Start();
            output = pro.StandardOutput.ReadToEnd();

            MatchCollection matches = Regex.Matches(output, @"\d+\.\d+\.\d+\.\d+");
            foreach (Match match in matches)
            {
                output = string.Concat("LETS/", appver, " (Win ", match.Value, ")");
            }

            vSetting.UserAgent = output;
            return vSetting.UserAgent;
        }


        /// <summary>
        /// SSE接続を停止する
        /// </summary>
        public void Stop()
        {
            this.subscribed = false;
        }

        /// <summary>
        /// 接続中か確認する
        /// </summary>
        /// <returns>接続中か</returns>
        public bool IsConnected()
        {
            return this.subscribed;
        }

        /// <summary>
        /// SSE 購読受信スレッド.
        /// </summary>
        /// <param name="client">HTTP Clientインスタンス.</param>
        /// <param name="request">httpリクエスト</param>
        private async Task Sbscriber(HttpClient client, HttpRequestMessage request)
        {
            try
            {
                //Console.WriteLine("Establishing connection");
                Logger.Info("Establishing connection", string.Empty);
                this.subscribed = true;
                var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                // SSE サーバに対して GETリクエスト　(非同期stream)
                using (this.streamReader = new StreamReader(await response.Content.ReadAsStreamAsync()))
                {
                    // メッセージ受信バッファを準備する.
                    var messageList = new List<string>();

                    // 購読が終了依頼されるか、セッションの切断があるまで続ける.
                    while (!this.streamReader.EndOfStream && this.subscribed)
                    {
                        // 非同期モードで1行受信。受信するまで待機.
                        var message = await this.streamReader.ReadLineAsync();

                        if (message.Length == 0)
                        {
                            // 改行のみの場合はイベント区切り.
                            // メッセージをemitterにより通知
                            this.emitter(messageList);

                            // メッセージ受信バッファを作成し、次のイベントへの準備を行う.
                            messageList = new List<string>();
                        }
                        else
                        {
                            // メッセージをバッファに保存する.
                            messageList.Add(message);
                        }
                    }

                    //Console.WriteLine("Connection Closed");
                    Logger.Info("Connection Closed", string.Empty);
                    this.subscribed = false;
                    messageList.Clear();
                    this.emitter(messageList);
                }
            }
            catch (Exception ex)
            {
                Logger.Info($"Error: {ex.Message}", string.Empty);
                this.subscribed = false;
                this.emitter(new List<string>());
            }
        }

        /// <summary>
        /// 受信メッセージによる処理分岐
        /// </summary>
        /// <param name="messageList">受信メッセージ</param>
        private void MessageHandler(List<string> messageList)
        {
            if (!this.subscribed)
            {
                return;
            }
            else
            {
                var sseMessage = SseMessage.BuildMessage(messageList);
                if (!sseMessage.IsCommentOnly())
                {
                    ActivateFont font = JsonConvert.DeserializeObject<ActivateFont>(sseMessage.Data);
                    switch (sseMessage.EventType)
                    {
                        case "font-activate":
                            Logger.Info("font-activate", string.Empty);
                            this.fontNotificationService.Activate(font);
                            break;
                        case "font-deactivate":
                            Logger.Info("font-deactivate", string.Empty);
                            this.fontNotificationService.Deactivate(font.FontId);
                            break;
                        case "font-all-uninstall":
                            Logger.Info("font-all-uninstall", string.Empty);
                            this.fontNotificationService.AllUninstall();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private string GetProxyServer()
        {
            string proxyserver = null;

            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings");
                proxyserver = (string)key.GetValue("ProxyServer");

                if (proxyserver == null || proxyserver == "")
                {
                    Process p = new Process();
                    // コマンドプロンプトと同じように実行します
                    p.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
                    p.StartInfo.Arguments = "/c " + "netsh winhttp show proxy"; // 実行するファイル名（コマンド）
                    p.StartInfo.CreateNoWindow = true; // コンソール・ウィンドウは開かない
                    p.StartInfo.UseShellExecute = false; // シェル機能を使用しない
                    p.StartInfo.RedirectStandardOutput = true;
                    p.Start();
                    string cmdresult = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();

                    var lines = cmdresult.Replace("\r\n", "\n").Split(new[] { '\n', '\r' });
                    foreach (string line in lines)
                    {
                        var words = line.Replace("  ", " ").Split(' ');
                        string preWord = "";
                        foreach (string w in words)
                        {
                            if (preWord == "サーバー:")
                            {
                                proxyserver = w;
                            }
                            preWord = w;
                        }
                    }

                }
            }
            catch (Exception e)
            {
                //Proxy設定に失敗したら握りつぶす
            }

            return proxyserver;
        }
    }
}

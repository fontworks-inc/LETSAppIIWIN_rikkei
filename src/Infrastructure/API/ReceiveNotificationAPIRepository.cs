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
        /// 購読中になった時間
        /// </summary>
        private DateTime subscribedTime = new DateTime(0);

        /// <summary>
        /// 購読に使用したアクセストークン
        /// </summary>
        private string subscribedAccessToken = string.Empty;

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
        /// <param name="deviceid">デバイスID</param>
        /// <returns>SSE接続の成否</returns>
        public bool Start(string accessToken, string deviceid)
        {
            if (this.client == null)
            {
                HttpClientHandler ch = new HttpClientHandler();
                UserStatus userStatus = this.userStatusRepository.GetStatus();
                IWebProxy proxyserver = this.APIConfiguration.GetWebProxy(this.NotifyBasePath);
                if (proxyserver != null)
                {
                    ch.Proxy = proxyserver;
                }

                this.client = new HttpClient(ch);

                this.client.BaseAddress = new Uri(this.NotifyBasePath);
                this.client.Timeout = Timeout.InfiniteTimeSpan;
            }

            ApplicationSetting setting = new ApplicationSetting();
            var request = new HttpRequestMessage(HttpMethod.Get, this.NotifyBasePath + "/notifications");

            request.Headers.Add("User-Agent", this.GetUserAgent());
            request.Headers.Add("Connection", "keep-alive");
            request.Headers.Add("Accept", "text/event-stream");
            request.Headers.Add("Cache-Control", "no-cache");
            request.Headers.Add("X-LETS-DEVICEID", deviceid);
            if (accessToken != null)
            {
                request.Headers.Add("Authorization", $"Bearer {accessToken}");
                this.subscribedAccessToken = accessToken;
            }

            int? lastEventId = this.userStatusRepository.GetStatus().LastEventId;
            if (lastEventId != null)
            {
                request.Headers.Add("Last-Event-ID", lastEventId.ToString());
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

            var useragent = this.APIConfiguration.GetUserAgent(vSetting.ClientApplicationPath);

            vSetting.UserAgent = useragent;
            return vSetting.UserAgent;
        }

        /// <summary>
        /// SSE接続を停止する
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:Elements should be ordered by access", Justification = "<保留中>")]
        public void Stop()
        {
            if (this.subscribed)
            {
                try
                {
                    this.streamReader.Close();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.StackTrace);
                }
            }

            this.subscribed = false;
            Logger.Debug("SSE Stop:subscribed = false");
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
        /// 接続した時間を返す
        /// </summary>
        /// <returns>接続開始日時</returns>
        public DateTime ConnectedTime()
        {
            return this.subscribedTime;
        }

        /// <summary>
        /// 接続時に使用したアクセストークンを返す
        /// </summary>
        /// <returns>接続開始時アクセストークン</returns>
        public string ConnectedAccessToken()
        {
            return this.subscribedAccessToken;
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
                Logger.Debug("Establishing connection");
                this.subscribed = true;
                this.subscribedTime = DateTime.Now;
                var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();
                Logger.Debug("response.EnsureSuccessStatusCode");

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

                    Logger.Debug("Connection Closed:subscribed = false");
                    this.subscribed = false;
                    messageList.Clear();
                    this.emitter(messageList);
                }
            }
            catch (Exception ex)
            {
                Logger.Debug($"Connection Error:subscribed = false {ex.StackTrace}");
                this.subscribed = false;
                this.emitter(new List<string>());
            }

            Logger.Debug("Sbscriber Exit:subscribed = false");
            this.subscribed = false;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "<保留中>")]
        private static Queue<SseMessage> sseQue = new Queue<SseMessage>();
        private static Task sseproctask = null;

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
                    Logger.Debug("NotifyMessage:" + sseMessage.Data);
                    sseQue.Enqueue(sseMessage);

                    if (sseproctask != null)
                    {
                        if (sseproctask.IsCompleted)
                        {
                            sseproctask = Task.Run(() =>
                            {
                                this.SseMessageProc();
                            });
                        }
                    }
                    else
                    {
                        sseproctask = Task.Run(() =>
                        {
                            this.SseMessageProc();
                        });
                    }
                }
            }
        }

        private void SseMessageProc()
        {
            string eventID = string.Empty;
            while (sseQue.Count > 0)
            {
                SseMessage sseMessage = sseQue.Dequeue();

                Logger.Debug("sseMessageProc:NotifyMessage:" + sseMessage.Data);
                eventID = sseMessage.Id;

                ActivateFont font = JsonConvert.DeserializeObject<ActivateFont>(sseMessage.Data);
                switch (sseMessage.EventType)
                {
                    case "font-activate":
                        Logger.Info($"font-activate:{font.FontId}:{font.DisplayFontName}");
                        this.fontNotificationService.Activate(font);
                        break;
                    case "font-deactivate":
                        Logger.Info($"font-deactivate:{font.FontId}");
                        this.fontNotificationService.Deactivate(font.FontId);
                        break;
                    case "font-all-uninstall":
                        Logger.Info("font-all-uninstall");
                        this.fontNotificationService.AllUninstall();
                        break;
                    default:
                        break;
                }
            }

            if (!string.IsNullOrEmpty(eventID))
            {
                UserStatus userStatus = this.userStatusRepository.GetStatus();
                try
                {
                    userStatus.LastEventId = int.Parse(eventID);
                    this.userStatusRepository.SaveStatus(userStatus);
                }
                catch (Exception)
                {
                    // Parseに失敗したら設定しない
                }
            }
        }
    }
}

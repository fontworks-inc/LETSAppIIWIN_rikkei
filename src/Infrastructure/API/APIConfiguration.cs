namespace Infrastructure.API
{
    /// <summary>
    /// API接続に利用する設定情報
    /// </summary>
    public class APIConfiguration
    {
        /// <summary>
        /// 強制ログアウトを実施するイベント
        /// </summary>
        public delegate void ForceLogoutEvent();

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="basePath">API接続先のベースURL</param>
        /// <param name="notifyBasePath">通知サーバーのURL</param>
        /// <param name="fixedTermConfirmationInterval">定期確認間隔</param>
        /// <param name="communicationRetryCount">リトライ回数</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "<保留中>")]
        public APIConfiguration(string basePath, string notifyBasePath, int fixedTermConfirmationInterval, int communicationRetryCount)
        {
            this.BasePath = basePath;
            this.NotifyBasePath = notifyBasePath;
            this.FixedTermConfirmationInterval = fixedTermConfirmationInterval;
            this.CommunicationRetryCount = communicationRetryCount;
        }

        /// <summary>
        /// API接続先のベースURL
        /// </summary>
        /// <example>"http://api/v1</example>
        public string BasePath { get; }

        /// <summary>
        /// 通知サーバーのURL
        /// </summary>
        public string NotifyBasePath { get; }

        /// <summary>
        /// 定期確認間隔
        /// </summary>
        public int FixedTermConfirmationInterval { get; }

        /// <summary>
        /// リトライ回数
        /// </summary>
        public int CommunicationRetryCount { get; }

        /// <summary>
        /// 強制ログアウトを実施するイベント
        /// </summary>
        public ForceLogoutEvent ForceLogout { get; set; }
    }
}

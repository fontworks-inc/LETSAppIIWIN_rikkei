namespace Infrastructure.API
{
    /// <summary>
    /// API接続に利用する設定情報
    /// </summary>
    public class APIConfiguration
    {
        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="basePath">API接続先のベースURL</param>
        /// <param name="userAgent">User-Agent</param>
        public APIConfiguration(string basePath, string notifyBasePath)
        {
            this.BasePath = basePath;
            this.NotifyBasePath = notifyBasePath;
        }

        /// <summary>
        /// API接続先のベースURL
        /// </summary>
        /// <example>"http://api/v1</example>
        public string BasePath { get; }

        public string NotifyBasePath { get; }
    }
}

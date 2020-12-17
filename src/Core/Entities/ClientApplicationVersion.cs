namespace Core.Entities
{
    /// <summary>
    /// クライアントアプリケーションの起動バージョン情報を表すクラス
    /// </summary>
    public class ClientApplicationVersion
    {
        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        public ClientApplicationVersion()
        {
        }

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="appId">アプリID</param>
        /// <param name="version">アプリバージョン</param>
        /// <param name="url">URL</param>
        public ClientApplicationVersion(string appId, string version, string url)
        {
            this.AppId = appId;
            this.Version = version;
            this.Url = url;
        }

        /// <summary>
        /// アプリID
        /// </summary>
        public string AppId { get; set; } = string.Empty;

        /// <summary>
        /// アプリバージョン
        /// </summary>
        public string Version { get; set; } = string.Empty;

        /// <summary>
        /// URL
        /// </summary>
        public string Url { get; set; } = string.Empty;
    }
}

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
        public APIConfiguration(string basePath)
        {
            this.BasePath = basePath;
        }

        /// <summary>
        /// API接続先のベースURL
        /// </summary>
        /// <example>"http://api/v1</example>
        public string BasePath { get; }
    }
}

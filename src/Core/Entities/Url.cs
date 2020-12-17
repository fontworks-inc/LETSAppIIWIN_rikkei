namespace Core.Entities
{
    /// <summary>
    /// URLを表すクラス
    /// </summary>
    public class Url
    {
        private readonly string url;

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="url">URL</param>
        public Url(string url)
        {
            this.url = url;
        }

        /// <summary>
        /// URLを取得する
        /// </summary>
        /// <returns>URL</returns>
        public override string ToString()
        {
            return this.url;
        }
    }
}

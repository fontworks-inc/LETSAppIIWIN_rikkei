namespace Core.Entities
{
    /// <summary>
    /// ダウンロード状態
    /// </summary>
    public enum DownloadStatus
    {
        /// <summary>
        /// ダウンロード中
        /// </summary>
        Running = 1,

        /// <summary>
        /// ダウンロード完了
        /// </summary>
        Completed = 2,

        /// <summary>
        /// アップデータ実行
        /// </summary>
        Update = 3,
    }
}

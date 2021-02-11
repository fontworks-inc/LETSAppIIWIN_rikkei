namespace ApplicationService.Interfaces
{
    /// <summary>
    /// ダウンロード完了時に呼び出されるComponent側のイベント
    /// </summary>
    public delegate void DownloadCompletedComponentEvent();

    /// <summary>
    /// プログラムのダウンロードを行うサービスのインターフェース
    /// </summary>
    public interface IApplicationDownloadService
    {
        /// <summary>
        /// プログラムのダウンロードを開始
        /// </summary>]
        /// <param name="downloadCompletedComponentEvent">ダウンロード完了時に呼び出されるComponent側のイベント</param>
        /// <param name="forceUpdateEvent">アップデートチェック時に時に呼び出されるComponent側のイベント</param>
        void StartDownloading(DownloadCompletedComponentEvent downloadCompletedComponentEvent = null, ForceUpdateEvent forceUpdateEvent = null);

        /// <summary>
        /// プログラムのダウンロードを完了
        /// </summary>
        /// <param name="downloadCompletedComponentEvent">ダウンロード完了時に呼び出されるComponent側のイベント</param>
        /// <param name="forceUpdateEvent">アップデートチェック時に時に呼び出されるComponent側のイベント</param>
        void CompleteDownloading(DownloadCompletedComponentEvent downloadCompletedComponentEvent = null, ForceUpdateEvent forceUpdateEvent = null);
    }
}

namespace ApplicationService.Interfaces
{
    /// <summary>
    /// クライアントアプリを終了するために呼び出されるイベント
    /// </summary>
    public delegate void ShutdownClientApplicationRequiredEvent();

    /// <summary>
    /// 強制アップデートを実施するイベント
    /// </summary>
    public delegate void ForceUpdateEvent();

    /// <summary>
    /// ダウンロード完了時に実施するイベント
    /// </summary>
    public delegate void DownloadCompletedEvent();

    /// <summary>
    /// 更新プログラムがダウンロード済みのときに呼び出されるイベント
    /// </summary>
    public delegate void ExistsUpdateProgramEvent();

    /// <summary>
    /// 更新プログラムのダウンロードを実施するイベント
    /// </summary>
    public delegate void StartDownloadEvent();

    /// <summary>
    /// 自デバイスの情報が含まれていない場合に呼び出されるイベント
    /// </summary>
    public delegate void NotContainsDeviceEvent();

    /// <summary>
    /// 未読お知らせが有るときに呼び出されるイベント
    /// </summary>
    /// <param name="numberOfUnreadMessages">未読件数</param>
    public delegate void ExistsUnreadNotificationEvent(int numberOfUnreadMessages);

    /// <summary>
    /// 他端末からフォントがコピーされていたときに呼び出されるイベント
    /// </summary>
    public delegate void DetectionFontCopyEvent();

    /// <summary>
    /// 起動時処理に関するサービスのインターフェース
    /// </summary>
    public interface IStartupService
    {
        /// <summary>
        /// 起動時チェック処理
        /// </summary>
        /// <param name="shutdownClientApplicationRequiredEvent">クライアントアプリを終了するイベント</param>
        /// <param name="forceUpdateEvent">強制アップデートを実施するイベント</param>
        /// <param name="downloadCompletedEvent">ダウンロード完了時に実施するイベント</param>
        /// <param name="existsUpdateProgramEvent">更新プログラムがダウンロード済みのときに呼び出されるイベント</param>
        /// <param name="startDownloadEvent">更新プログラムのダウンロードを実施するイベント</param>
        /// <param name="notContainsDeviceEvent">自デバイスの情報が端末情報に含まれていない場合に呼び出されるイベント</param>
        /// <param name="existsUnreadNotificationEvent">未読お知らせが有るときに呼び出されるイベント</param>
        /// <param name="detectionFontCopyEvent">他端末からフォントがコピーされていたときに呼び出されるイベント</param>
        /// <returns>チェック結果を返す</returns>
        bool IsCheckedStartup(
            ShutdownClientApplicationRequiredEvent shutdownClientApplicationRequiredEvent,
            ForceUpdateEvent forceUpdateEvent,
            DownloadCompletedEvent downloadCompletedEvent,
            ExistsUpdateProgramEvent existsUpdateProgramEvent,
            StartDownloadEvent startDownloadEvent,
            NotContainsDeviceEvent notContainsDeviceEvent,
            ExistsUnreadNotificationEvent existsUnreadNotificationEvent,
            DetectionFontCopyEvent detectionFontCopyEvent);

        /// <summary>
        /// 強制アップデートチェック
        /// </summary>
        /// <param name="forceUpdateEvent">強制アップデートを実施するイベント</param>
        /// <param name="downloadCompletedEvent">ダウンロード完了時に実施するイベント</param>
        void ForceUpdateCheck(ForceUpdateEvent forceUpdateEvent, DownloadCompletedEvent downloadCompletedEvent);

        /// <summary>
        /// 起動指定バージョンチェック
        /// </summary>
        /// <returns>再起動が必要な場合true、不要な場合false</returns>
        bool StartingVersionCheck();

        /// <summary>
        /// 次バージョンチェック
        /// </summary>
        /// <param name="existsUpdateProgramEvent">更新プログラムがダウンロード済みのときに呼び出されるイベント</param>
        /// <param name="startDownloadEvent">更新プログラムのダウンロードを実施するイベント</param>
        /// <returns>起動時チェック処理を続行する場合はtrue、起動時チェック処理を終了する場合はfalse</returns>
        bool NextVersionCheck(ExistsUpdateProgramEvent existsUpdateProgramEvent, StartDownloadEvent startDownloadEvent);

        /// <summary>
        /// ログイン状態確認処理
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="notContainsDeviceEvent">自デバイスの情報が含まれていない場合に呼び出されるイベント</param>
        /// <returns>ログイン中の時にtrue, ログアウト時にfalseを返す</returns>
        bool ConfirmLoginStatus(
            string deviceId,
            NotContainsDeviceEvent notContainsDeviceEvent);

        /// <summary>
        /// お知らせ有無チェック
        /// </summary>
        /// <param name="existsUnreadNotificationEvent">未読お知らせが有るときに呼び出されるイベント</param>
        /// <returns>起動時チェック処理を続行する場合はtrue、起動時チェック処理を終了する場合はfalse</returns>
        bool ExistsNotificationCheck(ExistsUnreadNotificationEvent existsUnreadNotificationEvent);

        /// <summary>
        /// 他端末コピーチェック
        /// </summary>
        /// <param name="detectionFontCopyEvent">他端末からフォントがコピーされていたときに呼び出されるイベント</param>
        /// <returns>起動時チェック処理を続行する場合はtrue、起動時チェック処理を終了する場合はfalse</returns>
        bool FontCopyCheck(DetectionFontCopyEvent detectionFontCopyEvent);

        /// <summary>
        /// サーバから削除されたフォントの削除チェック
        /// </summary>
        /// <returns>起動時チェック処理を続行する場合はtrue、起動時チェック処理を終了する場合はfalse</returns>
        bool DeletedFontCheck();
    }
}

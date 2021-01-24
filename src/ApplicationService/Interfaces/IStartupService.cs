namespace ApplicationService.Interfaces
{
    /// <summary>
    /// 自デバイスの情報が含まれていない場合に呼び出されるイベント
    /// </summary>
    public delegate void NotContainsDeviceEvent();

    /// <summary>
    /// 起動時処理に関するサービスのインターフェース
    /// </summary>
    public interface IStartupService
    {
        /// <summary>
        /// 起動時チェック処理
        /// </summary>
        /// <param name="notContainsDeviceEvent">自デバイスの情報が端末情報に含まれていない場合に呼び出されるイベント</param>
        /// <returns>チェック結果を返す</returns>
        bool IsCheckedStartup(NotContainsDeviceEvent notContainsDeviceEvent);

        /// <summary>
        /// ログイン状態確認処理
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="notContainsDeviceEvent">自デバイスの情報が含まれていない場合に呼び出されるイベント</param>
        /// <returns>ログイン中の時にtrue, ログアウト時にfalseを返す</returns>
        bool ConfirmLoginStatus(
            string deviceId,
            NotContainsDeviceEvent notContainsDeviceEvent);
    }
}

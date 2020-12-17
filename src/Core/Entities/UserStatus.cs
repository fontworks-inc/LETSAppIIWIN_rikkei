namespace Core.Entities
{
    /// <summary>
    /// ユーザ別ステータス情報を表すクラス
    /// </summary>
    public class UserStatus
    {
        /// <summary>
        /// デバイスID
        /// </summary>
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>
        /// ログイン状態
        /// </summary>
        /// <remarks>ログイン中の時にtrue, ログアウト時にfalseを返す</remarks>
        public bool IsLoggingIn { get; set; } = false;

        /// <summary>
        /// リフレッシュトークン
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// SSE接続時に使用するLast-Event-ID
        /// </summary>
        public int? LastEventId { get; set; } = null;
    }
}

using System;

namespace Core.Entities
{
    /// <summary>
    /// ユーザ別ステータス情報を表すクラス
    /// </summary>
    public class UserStatus
    {
        /// <summary>
        /// デバイスキー
        /// </summary>
        public string DeviceKey { get; set; } = string.Empty;

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
        /// リフレッシュトークン次回取得日時
        /// </summary>
        public DateTime RefreshTokenUpdateSchedule { get; set; } = DateTime.MaxValue;

        /// <summary>
        /// SSE接続時に使用するLast-Event-ID
        /// </summary>
        public int? LastEventId { get; set; } = null;

        /// <summary>
        /// デバイスモード
        /// </summary>
        /// <remarks>デバイスモードの時にtrueを返す</remarks>
        public bool IsDeviceMode { get; set; } = false;

    }
}

using System;
using System.Collections.Generic;

namespace Core.Entities
{
    /// <summary>
    /// メモリで保持する情報を表すクラス
    /// </summary>
    public class VolatileSetting
    {
        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        public VolatileSetting()
        {
        }

        /// <summary>
        /// 起動時チェック処理
        /// </summary>
        /// <remarks>処理済みの時true, 未処理時にfalseを返す</remarks>
        public bool IsCheckedStartup { get; set; } = false;

        /// <summary>
        /// 起動時チェック日時
        /// </summary>
        public DateTime? CheckedStartupAt { get; set; } = null;

        /// <summary>
        /// アクセストークン
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// 通信状態
        /// </summary>
        /// <remarks>オンライン中の時にtrue, オフライン時にfalseを返す</remarks>
        public bool IsConnected { get; set; } = false;

        /// <summary>
        /// 前回アクセス日時
        /// </summary>
        public DateTime? LastAccessAt { get; set; } = null;

        /// <summary>
        /// 通知状態
        /// </summary>
        /// <remarks>通知ありの時にtrue, なしの時にfalseを返す</remarks>
        public bool IsNoticed { get; set; } = false;

        /// <summary>
        /// 更新状態
        /// </summary>
        /// <remarks>更新ありの時にtrue, なしの時にfalseを返す</remarks>
        public bool IsUpdated { get; set; } = false;

        /// <summary>
        /// ダウンロード中かどうか
        /// </summary>
        /// <remarks>ダウンロード中時にtrueを返す</remarks>
        public bool IsDownloading { get; set; } = false;

        /// <summary>
        /// ダウンロード完了済みかどうか
        /// </summary>
        /// <remarks>ダウンロード完了時にtrueを返す</remarks>
        public bool CompletedDownload { get; set; } = false;

        /// <summary>
        /// アップデート中かどうか
        /// </summary>
        /// <remarks>アップデート中であればtrueを返す</remarks>
        public bool IsUpdating { get; set; } = false;

        /// <summary>
        /// インストール対象フォント
        /// </summary>
        public IList<InstallFont> InstallTargetFonts { get; set; } = new List<InstallFont>();

        /// <summary>
        /// 通知フォントリスト
        /// </summary>
        public IList<InstallFont> NotificationFonts { get; set; } = new List<InstallFont>();

        /// <summary>
        /// User-Agent
        /// </summary>
        public string UserAgent { get; set; } = string.Empty;

        /// <summary>
        /// RefreshToken
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// クライアントアプリケーションのパス
        /// </summary>
        public string ClientApplicationPath { get; set; } = string.Empty;

        /// <summary>
        /// ProxyServer
        /// </summary>
        public string ProxyServer { get; set; } = string.Empty;

        /// <summary>
        /// フォント同期実行
        /// </summary>
        public bool IsFontUpdating { get; set; } = false;

    }
}

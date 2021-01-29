using System;

namespace Core.Entities
{
    /// <summary>
    /// インストーラー情報を表すクラス
    /// </summary>
    public class Installer
    {
        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        public Installer()
        {
        }

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="downloadStatus">ダウンロード状態</param>
        /// <param name="version">バージョン</param>
        /// <param name="applicationUpdateType">強制/任意</param>
        /// <param name="url">URL</param>
        public Installer(DownloadStatus downloadStatus, string version, bool applicationUpdateType, string url)
        {
            this.DownloadStatus = downloadStatus;
            this.Version = version;
            this.ApplicationUpdateType = applicationUpdateType;
            this.Url = url;
        }

        /// <summary>
        /// ダウンロード状態
        /// </summary>
        public DownloadStatus DownloadStatus { get; set; }

        /// <summary>
        /// バージョン
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 強制/任意
        /// </summary>
        /// <remarks>強制の時にtrue, 任意の時にfalseを返す</remarks>
        public bool ApplicationUpdateType { get; set; }

        /// <summary>
        /// URL
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// このオブジェクトが指定されたオブジェクトと等しいかどうかを判定する
        /// </summary>
        /// <param name="obj">比較対象のオブジェクト</param>
        /// <returns>このオブジェクトが指定されたオブジェクトと等しい場合はtrue、それ以外はfalse</returns>
        public override bool Equals(object obj)
        {
            return obj is Installer installer &&
                   this.Version == installer.Version;
        }

        /// <summary>
        /// このオブジェクトのハッシュコードを取得する
        /// </summary>
        /// <returns>このオブジェクトのハッシュコード</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Version);
        }
    }
}

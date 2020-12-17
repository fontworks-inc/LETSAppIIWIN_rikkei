using System;

namespace Core.Entities
{
    /// <summary>
    /// インストール用フォント情報を表すクラス
    /// </summary>
    public class InstallFontBase
    {
        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="id">フォントID</param>
        /// <param name="displayName">表示用フォント名</param>
        /// <param name="fileSize">ファイルサイズ</param>>
        /// <param name="version">バージョン</param>
        public InstallFontBase(string id, string displayName, float fileSize, string version)
        {
            this.Id = id;
            this.DisplayName = displayName;
            this.FileSize = fileSize;
            this.Version = version;
        }

        /// <summary>
        /// フォントID
        /// </summary>
        /// <remarks>LETSフォントのみを対象として情報を保存する</remarks>
        public string Id { get; }

        /// <summary>
        /// 表示用フォント名
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// ファイルサイズ
        /// </summary>
        public float FileSize { get; }

        /// <summary>
        /// バージョン
        /// </summary>
        public string Version { get; }

        /// <summary>
        /// このオブジェクトが指定されたオブジェクトと等しいかどうかを判定する
        /// </summary>
        /// <param name="obj">比較対象のオブジェクト</param>
        /// <returns>このオブジェクトが指定されたオブジェクトと等しい場合はtrue、それ以外はfalse</returns>
        public override bool Equals(object obj)
        {
            return obj is InstallFontBase font &&
                   this.Id == font.Id &&
                   this.Version == font.Version;
        }

        /// <summary>
        /// このオブジェクトのハッシュコードを取得する
        /// </summary>
        /// <returns>このオブジェクトのハッシュコード</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Id, this.Version);
        }
    }
}

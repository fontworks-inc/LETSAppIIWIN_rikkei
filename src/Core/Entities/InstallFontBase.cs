using System;
using System.Collections.Generic;

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
        public InstallFontBase()
        {
        }

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="fontId">フォントID</param>
        /// <param name="displayFontName">表示用フォント名</param>
        /// <param name="fileSize">ファイルサイズ</param>>
        /// <param name="version">バージョン</param>
        /// <param name="isFreemium">フリーミアムフォントフラグ</param>
        /// <param name="contractIds">契約ID</param>
        public InstallFontBase(string fontId, string displayFontName, float fileSize, string version, bool isFreemium, List<string> contractIds)
        {
            this.FontId = fontId;
            this.DisplayFontName = displayFontName;
            this.FileSize = fileSize;
            this.Version = version;
            this.IsFreemium = isFreemium;
            this.ContractIds = contractIds;
        }

        /// <summary>
        /// フォントID
        /// </summary>
        /// <remarks>LETSフォントのみを対象として情報を保存する</remarks>
        public string FontId { get; set; }

        /// <summary>
        /// 表示用フォント名
        /// </summary>
        public string DisplayFontName { get; set; }

        /// <summary>
        /// ファイルサイズ
        /// </summary>
        public float FileSize { get; set; }

        /// <summary>
        /// バージョン
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// フリーミアムフォントフラグ
        /// </summary>
        public bool IsFreemium { get; set; }

        /// <summary>
        /// 契約ID
        /// </summary>
        public List<string> ContractIds { get; set; }

        /// <summary>
        /// このオブジェクトが指定されたオブジェクトと等しいかどうかを判定する
        /// </summary>
        /// <param name="obj">比較対象のオブジェクト</param>
        /// <returns>このオブジェクトが指定されたオブジェクトと等しい場合はtrue、それ以外はfalse</returns>
        public override bool Equals(object obj)
        {
            return obj is InstallFontBase font &&
                   this.FontId == font.FontId &&
                   this.Version == font.Version;
        }

        /// <summary>
        /// このオブジェクトのハッシュコードを取得する
        /// </summary>
        /// <returns>このオブジェクトのハッシュコード</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.FontId, this.Version);
        }
    }
}

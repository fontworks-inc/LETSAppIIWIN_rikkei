using System;
using System.Collections.Generic;

namespace Core.Entities
{
    /// <summary>
    /// フォントを表すクラス
    /// </summary>
    public class InstallFont : InstallFontBase
    {
        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        public InstallFont()
            : base()
        {
        }

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="userFontId">ユーザーフォントID</param>
        /// <param name="activateFlg">アクティベートフラグ</param>
        /// <param name="id">フォントID</param>
        /// <param name="displayName">表示用フォント名</param>
        /// <param name="fileName">ファイル名</param>
        /// <param name="fileSize">ファイルサイズ</param>
        /// <param name="version">バージョン</param>
        /// <param name="needFontVersionUpdate">フォントバージョンアップ要否</param>
        /// <param name="isAvailableFont">フォント利用可否</param>
        /// <param name="isFreemium">フリーミアムフォントフラグ</param>
        /// <param name="contractIds">契約ID</param>
        public InstallFont(string userFontId, bool activateFlg, string id, string displayName, string fileName, float fileSize, string version, bool needFontVersionUpdate, bool isAvailableFont, bool isFreemium, List<string> contractIds)
            : base(id, displayName, fileSize, version, isFreemium, contractIds)
        {
            this.UserFontId = userFontId;
            this.ActivateFlg = activateFlg;
            this.FileName = fileName;
            this.NeedFontVersionUpdate = needFontVersionUpdate;
            this.IsAvailableFont = isAvailableFont;
        }

        /// <summary>
        /// ユーザーフォントID
        /// </summary>
        /// <remarks>ユーザーＩＤやデバイスＩＤを埋め込んだフォント</remarks>
        public string UserFontId { get; set; }

        /// <summary>
        /// アクティベートフラグ
        /// </summary>
        /// <remarks>
        /// アクティベート状態の時にtrue, ディアクティベート状態の時にfalseを返す
        /// </remarks>
        public bool ActivateFlg { get; set; }

        /// <summary>
        /// ファイル名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// フォントバージョンアップ要否
        /// </summary>
        /// <remarks>
        /// ユーザの端末にインストール済みのフォントのバージョンと「フォント」テーブルのフォント「バージョン」を比較し、
        /// 後者が新しければtrue, そうでなければfalseを返す
        /// </remarks>
        public bool NeedFontVersionUpdate { get; set; }

        /// <summary>
        /// フォント利用可否
        /// </summary>
        /// <remarks>
        /// 当該フォントが利用可能の時にtrue, 削除され利用できなければfalseを返す
        /// </remarks>
        public bool IsAvailableFont { get; set; }

        /// <summary>
        /// フォントファイルパス
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// このオブジェクトが指定されたオブジェクトと等しいかどうかを判定する
        /// </summary>
        /// <param name="obj">比較対象のオブジェクト</param>
        /// <returns>このオブジェクトが指定されたオブジェクトと等しい場合はtrue、それ以外はfalse</returns>
        public override bool Equals(object obj)
        {
            return obj is InstallFont font &&
                  this.DisplayFontName.Equals(font.DisplayFontName) &&
                   this.Version.Equals(font.Version);
        }

        /// <summary>
        /// このオブジェクトのハッシュコードを取得する
        /// </summary>
        /// <returns>このオブジェクトのハッシュコード</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.DisplayFontName, this.Version);
        }
    }
}

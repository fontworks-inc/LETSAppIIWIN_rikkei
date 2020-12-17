using System;
using System.Collections.Generic;

namespace Core.Entities
{
    /// <summary>
    /// フォントを表すクラス
    /// </summary>
    public class Font
    {
        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        public Font()
        {
        }

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="id">フォントID</param>
        /// <param name="path">ファイルパス</param>
        /// <param name="isLETS">LETSフォントかどうかを表す</param>
        /// <param name="isActivated">アクティベート状態</param>
        /// <param name="displayName">表示用フォント名</param>
        /// <param name="version">バージョン</param>
        /// <param name="registryKey">レジストリキー</param>
        /// <param name="contractIds">契約IDリスト</param>
        public Font(string id, string path, bool isLETS, bool? isActivated, string displayName, string version, string registryKey, IList<string> contractIds)
        {
            this.Id = id;
            this.Path = path;
            this.IsLETS = isLETS;
            this.IsActivated = isActivated;
            this.DisplayName = displayName;
            this.Version = version;
            this.RegistryKey = registryKey;
            this.ContractIds = contractIds;
        }

        /// <summary>
        /// フォントID
        /// </summary>
        /// <remarks>LETSフォントのみを対象として情報を保存する</remarks>
        public string Id { get; set; }

        /// <summary>
        /// ファイルパス
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// LETSフォントかどうかを表す
        /// </summary>
        /// <remarks>LETSフォントの時にtrue, LETSフォント以外の時にfalseを返す</remarks>
        public bool IsLETS { get; set; }

        /// <summary>
        /// アクティベート状態
        /// </summary>
        /// <remarks>
        /// アクティベート状態の時にtrue, ディアクティベート状態の時にfalseを返す
        /// LETSフォントのみを対象として情報を保存する
        /// </remarks>
        public bool? IsActivated { get; set; }

        /// <summary>
        /// 表示用フォント名
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// バージョン
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 契約IDリスト
        /// </summary>
        public IList<string> ContractIds { get; set; }

        /// <summary>
        /// レジストリキー
        /// </summary>
        /// <remarks>アクティベート時に登録したレジストリのキー</remarks>
        public string RegistryKey { get; set; }

        /// <summary>
        /// このオブジェクトが指定されたオブジェクトと等しいかどうかを判定する
        /// </summary>
        /// <param name="obj">比較対象のオブジェクト</param>
        /// <returns>このオブジェクトが指定されたオブジェクトと等しい場合はtrue、それ以外はfalse</returns>
        public override bool Equals(object obj)
        {
            return obj is Font font &&
                   this.DisplayName == font.DisplayName &&
                   this.Version == font.Version;
        }

        /// <summary>
        /// このオブジェクトのハッシュコードを取得する
        /// </summary>
        /// <returns>このオブジェクトのハッシュコード</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.DisplayName, this.Version);
        }
    }
}

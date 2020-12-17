namespace Core.Entities
{
    /// <summary>
    /// アクティベートフォント情報を表すクラス
    /// </summary>
    public class ActivateFont : InstallFontBase
    {
        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="id">フォントID</param>
        /// <param name="displayName">表示用フォント名</param>
        /// <param name="fileSize">ファイルサイズ</param>>
        /// <param name="version">バージョン</param>
        public ActivateFont(string id, string displayName, float fileSize, string version)
            : base(id, displayName, fileSize, version)
        {
        }
    }
}
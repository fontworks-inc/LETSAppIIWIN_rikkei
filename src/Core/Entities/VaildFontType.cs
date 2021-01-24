namespace Core.Entities
{
    /// <summary>
    /// 取得するフォントの適用種別を表すクラス
    /// </summary>
    public enum VaildFontType
    {
        /// <summary>
        /// 過去にインストールしたフォントのうち利用可能なフォント情報のみ取得
        /// </summary>
        AvailableFonts,

        /// <summary>
        /// 過去にインストールしたフォントのうち削除されたフォント情報のみ取得
        /// </summary>
        DeletedFonts,

        /// <summary>
        /// 過去にインストールした全フォント情報の取得
        /// </summary>
        Both,
    }
}

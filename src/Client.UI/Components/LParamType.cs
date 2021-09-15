namespace Client.UI.Components
{
    /// <summary>
    /// ウィンドウメッセージ処理 ユーザー定義メッセージのLParamの種類
    /// </summary>
    public enum LParamType
    {
        /// <summary>
        /// ログイン画面を起動
        /// </summary>
        LoadLoginWindow = 1,

        /// <summary>
        /// アプリケーションを終了して再起動
        /// </summary>
        Restart = 2,

        /// <summary>
        /// すべてのフォントをディアクティベートして
        /// アプリケーションを終了
        /// </summary>
        DeactivateFontsAndShutdown = 3,

        /// <summary>
        /// アプリケーションを終了
        /// </summary>
        Shutdown = 4,
    }
}

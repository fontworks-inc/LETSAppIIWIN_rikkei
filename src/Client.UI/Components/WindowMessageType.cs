namespace Client.UI.Components
{
    /// <summary>
    /// ウィンドウメッセージ処理 ユーザー定義メッセージのメッセージの種類
    /// </summary>
    public enum WindowMessageType
    {
        /// <summary>
        /// 一般的なメッセージ
        /// </summary>
        General = 0x8001,

        /// <summary>
        /// プログラムアップデート進捗メッセージ
        /// </summary>
        ProgressOfUpdate = 0x8002,
    }
}

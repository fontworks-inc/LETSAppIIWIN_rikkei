namespace ApplicationService.Interfaces
{
    /// <summary>
    /// 指定のプロセスを実施するサービスのインターフェース
    /// </summary>
    public interface IStartProcessService
    {
        /// <summary>
        /// 指定パスのプロセスを管理者権限で実施する
        /// </summary>
        /// <param name="directoryPath">実行するフォルダのパス</param>
        /// <param name="fileName">ファイル名</param>
        /// <param name="arg">コマンド引数</param>
        /// <param name="runAdmin">管理者権限実行フラグ</param>
        void StartProcessAdministrator(string directoryPath, string fileName, string[] arg, bool runAdmin = true);
    }
}

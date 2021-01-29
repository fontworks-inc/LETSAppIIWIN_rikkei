using System;
using System.Collections.Generic;
using System.Text;

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
        void StartProcessAdministrator(string directoryPath, string fileName, string[] arg);
    }
}

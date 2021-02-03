using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using ApplicationService.Interfaces;
using Core.Interfaces;
using NLog;

namespace ApplicationService.Startup
{
    /// <summary>
    /// 指定のプロセスを実施するサービス
    /// </summary>
    public class StartProcessService : IStartProcessService
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>
        /// 文言の取得を行うインスタンス
        /// </summary>
        private IResourceWrapper resourceWrapper = null;

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="resourceWrapper">文言の取得を行うインスタンス</param>
        public StartProcessService(IResourceWrapper resourceWrapper)
        {
            this.resourceWrapper = resourceWrapper;
        }

        /// <summary>
        /// 指定パスのプロセスを管理者権限で実施する
        /// </summary>
        /// <param name="directoryPath">実行するフォルダのパス</param>
        /// <param name="fileName">ファイル名</param>
        /// <param name="arg">コマンド引数(null可能)</param>
        /// <param name="runAdmin">管理者権限実行フラグ</param>
        public void StartProcessAdministrator(string directoryPath, string fileName, string[] arg, bool runAdmin)
        {
            var proc = new System.Diagnostics.Process();

            proc.StartInfo.FileName = Path.Combine(directoryPath, fileName);
            if (runAdmin)
            {
                proc.StartInfo.Verb = "RunAs";
            }

            proc.StartInfo.UseShellExecute = true;

            if (arg != null && arg.Length != 0)
            {
                proc.StartInfo.Arguments = string.Join(" ", arg);
            }

            try
            {
                proc.Start();
            }
            catch (Win32Exception ex)
            {
                Logger.Error(ex, this.resourceWrapper.GetString("LOG_ERR_StartProcessService_StartProcessAdministrator_Win32Exception") + proc.StartInfo.FileName);
                throw new Win32Exception(this.resourceWrapper.GetString("LOG_ERR_StartProcessService_StartProcessAdministrator_Win32Exception") + proc.StartInfo.FileName);
            }
        }
    }
}

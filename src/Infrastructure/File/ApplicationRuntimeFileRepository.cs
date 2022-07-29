using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text.Json;
using Core.Entities;
using Core.Interfaces;
using NLog;

namespace Infrastructure.File
{
    /// <summary>
    /// 共通保存情報を格納するファイルリポジトリ
    /// </summary>
    public class ApplicationRuntimeFileRepository : TextFileRepositoryBase, IApplicationRuntimeRepository
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        public ApplicationRuntimeFileRepository(string filePath)
            : base(filePath)
        {
        }

        /// <summary>
        /// 共通保存情報を取得する
        /// </summary>
        /// <returns>アプリケーション設定情報</returns>
        public ApplicationRuntime GetApplicationRuntime()
        {
            if (System.IO.File.Exists(this.FilePath))
            {
                // ファイルが存在する場合、内容を返す
                string jsonString = this.ReadAll();
                return JsonSerializer.Deserialize<ApplicationRuntime>(jsonString);
            }
            else
            {
                // ファイルが存在しない場合、新規のオブジェクトを返す
                ApplicationRuntime applicationRuntime = new ApplicationRuntime();
                return applicationRuntime;
            }
        }

        /// <summary>
        /// アプリケーション共通保存情報を保存する
        /// </summary>
        /// <param name="applicationRuntime">アプリケーション共通保存情報</param>
        public void SaveApplicationRuntime(ApplicationRuntime applicationRuntime)
        {
            this.WriteAll(JsonSerializer.Serialize(applicationRuntime));
            this.SetFileAccessEveryone(this.FilePath);
        }

        private void SetFileAccessEveryone(string path)
        {
            try
            {
                FileSystemAccessRule rule = new FileSystemAccessRule(
                    new NTAccount("everyone"),
                    FileSystemRights.FullControl,
                    AccessControlType.Allow);

                var sec = new FileSecurity();
                sec.AddAccessRule(rule);
                System.IO.FileSystemAclExtensions.SetAccessControl(new FileInfo(path), sec);
            }
            catch (Exception)
            {
                // NOP
            }
        }
    }
}

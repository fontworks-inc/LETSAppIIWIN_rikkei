using System;
using System.Text.Json;
using Core.Entities;
using Core.Interfaces;
using NLog;

namespace Infrastructure.File
{
    /// <summary>
    /// ユーザ別ステータス情報を格納するファイルリポジトリ
    /// </summary>
    public class UserStatusFileRepository : EncryptFileBase, IUserStatusRepository
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        private static object saveLockObject = new object();

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        public UserStatusFileRepository(string filePath)
            : base(filePath)
        {
        }

        /// <summary>
        /// ユーザ別ステータス情報を取得する
        /// </summary>
        /// <returns>ユーザ別ステータス情報</returns>
        public UserStatus GetStatus()
        {
            if (System.IO.File.Exists(this.FilePath))
            {
                // ファイルが存在する場合、内容を返す
                try
                {
                    string jsonString = this.ReadAll();
                    return JsonSerializer.Deserialize<UserStatus>(jsonString);
                }
                catch (Exception ex)
                {
                    Logger.Debug("UserStatus:" + ex.StackTrace);
                    return new UserStatus();
                }
            }
            else
            {
                // ファイルが存在しない場合、新規のオブジェクトを返す
                return new UserStatus();
            }
        }

        /// <summary>
        /// ユーザ別ステータス情報を保存する
        /// </summary>
        /// <param name="status">ユーザ別ステータス情報</param>
        public void SaveStatus(UserStatus status)
        {
            lock (saveLockObject)
            {
                this.WriteAll(JsonSerializer.Serialize(status));
            }
        }
    }
}

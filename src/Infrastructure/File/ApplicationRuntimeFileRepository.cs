using System.Text.Json;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.File
{
    /// <summary>
    /// 共通保存情報を格納するファイルリポジトリ
    /// </summary>
    public class ApplicationRuntimeFileRepository : TextFileRepositoryBase, IApplicationRuntimeRepository
    {
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
                return new ApplicationRuntime();
            }
        }

        /// <summary>
        /// アプリケーション共通保存情報を保存する
        /// </summary>
        /// <param name="applicationRuntime">アプリケーション共通保存情報</param>
        public void SaveApplicationRuntime(ApplicationRuntime applicationRuntime)
        {
            this.WriteAll(JsonSerializer.Serialize(applicationRuntime));
        }
    }
}

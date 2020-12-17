using System.Text.Json;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.File
{
    /// <summary>
    /// クライアントアプリの起動Ver情報を格納するファイルリポジトリ
    /// </summary>
    public class ClientApplicationVersionFileRepository : EncryptFileBase, IClientApplicationVersionRepository
    {
        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        public ClientApplicationVersionFileRepository(string filePath)
            : base(filePath)
        {
        }

        /// <summary>
        /// クライアントアプリケーションの起動バージョン情報を取得する
        /// </summary>
        /// <returns>ユーザ別フォント情報</returns>
        public ClientApplicationVersion GetClientApplicationVersion()
        {
            if (System.IO.File.Exists(this.FilePath))
            {
                // ファイルが存在する場合、内容を返す
                string jsonString = this.ReadAll();
                return JsonSerializer.Deserialize<ClientApplicationVersion>(jsonString);
            }
            else
            {
                // ファイルが存在しない場合、新規のオブジェクトを返す
                return new ClientApplicationVersion();
            }
        }

        /// <summary>
        /// クライアントアプリケーションの起動バージョン情報を保存する
        /// </summary>
        /// <param name="clientApplicationVersion">ユーザ別フォント情報</param>
        public void SaveClientApplicationVersion(ClientApplicationVersion clientApplicationVersion)
        {
            this.WriteAll(JsonSerializer.Serialize(clientApplicationVersion));
        }
    }
}

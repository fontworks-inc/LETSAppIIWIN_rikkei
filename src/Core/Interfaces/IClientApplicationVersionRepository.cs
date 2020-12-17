using Core.Entities;

namespace Core.Interfaces
{
    /// <summary>
    /// クライアントアプリケーションの起動バージョン情報を格納するリポジトリのインターフェイス
    /// </summary>
    public interface IClientApplicationVersionRepository
    {
        /// <summary>
        /// クライアントアプリケーションの起動バージョン情報を取得する
        /// </summary>
        /// <returns>ユーザ別フォント情報</returns>
        ClientApplicationVersion GetClientApplicationVersion();

        /// <summary>
        /// クライアントアプリケーションの起動バージョン情報を保存する
        /// </summary>
        /// <param name="clientApplicationVersion">ユーザ別フォント情報</param>
        void SaveClientApplicationVersion(ClientApplicationVersion clientApplicationVersion);
    }
}

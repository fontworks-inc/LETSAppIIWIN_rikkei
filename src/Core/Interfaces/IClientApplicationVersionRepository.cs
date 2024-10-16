using Core.Entities;

namespace Core.Interfaces
{
    /// <summary>
    /// クライアントアプリケーションの起動バージョン情報を格納するリポジトリのインターフェイス
    /// </summary>
    public interface IClientApplicationVersionRepository
    {
        /// <summary>
        /// クライアントアプリケーションの更新情報を取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>更新情報</returns>
        ClientApplicationUpdateInfomation GetClientApplicationUpdateInfomation(string deviceId, string accessToken);

        /// <summary>
        /// クライアントアプリケーションの起動バージョン情報を取得する
        /// </summary>
        /// <returns>ユーザ別フォント情報</returns>
        ClientApplicationVersion GetClientApplicationVersion();

        /// <summary>
        /// クライアントアプリケーションの起動バージョン情報を取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>クライアントアプリバージョン情報</returns>
        ClientApplicationVersion GetClientApplicationVersion(string deviceId, string accessToken);

        /// <summary>
        /// デバイスモードクライアントアプリケーションの更新情報を取得する
        /// </summary>
        /// <returns>更新情報</returns>
        ClientApplicationUpdateInfomation GetClientApplicationDeviceUpdateInfomation(string offlineDeviceId, string indefiniteAccessToken);

        /// <summary>
        /// クライアントアプリケーションの起動バージョン情報を保存する
        /// </summary>
        /// <param name="clientApplicationVersion">ユーザ別フォント情報</param>
        void SaveClientApplicationVersion(ClientApplicationVersion clientApplicationVersion);
    }
}

using System;
using System.Text.Json;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.API;
using NLog;

namespace Infrastructure.File
{
    /// <summary>
    /// クライアントアプリの起動Ver情報を格納するファイルリポジトリ
    /// </summary>
    public class ClientApplicationVersionFileRepository : EncryptFileBase, IClientApplicationVersionRepository
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        private ClientApplicationVersionAPIRepository clientApplicationVersionAPIRepository;

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="clientApplicationVersionAPIRepository">クライアントアプリの起動Ver情報を取得するAPIリポジトリ</param>
        public ClientApplicationVersionFileRepository(string filePath, ClientApplicationVersionAPIRepository clientApplicationVersionAPIRepository)
            : base(filePath)
        {
            this.clientApplicationVersionAPIRepository = clientApplicationVersionAPIRepository;
        }

        /// <summary>
        /// クライアントアプリケーションの更新情報を取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>更新情報</returns>
        public ClientApplicationUpdateInfomation GetClientApplicationUpdateInfomation(string deviceId, string accessToken)
        {
            ClientApplicationUpdateInfomation clientApplicationUpdateInfomation = this.clientApplicationVersionAPIRepository.GetClientApplicationUpdateInfomation(deviceId, accessToken);

            return clientApplicationUpdateInfomation;
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
                try
                {
                    string jsonString = this.ReadAll();
                    return JsonSerializer.Deserialize<ClientApplicationVersion>(jsonString);
                }
                catch (Exception ex)
                {
                    Logger.Error("GetClientApplicationVersion:" + ex.StackTrace);
                    return new ClientApplicationVersion();
                }
            }
            else
            {
                // ファイルが存在しない場合、新規のオブジェクトを返す
                return new ClientApplicationVersion();
            }
        }

        /// <summary>
        /// クライアントアプリケーションの起動バージョン情報を取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>ユーザ別フォント情報</returns>
        public ClientApplicationVersion GetClientApplicationVersion(string deviceId, string accessToken)
        {
            ClientApplicationVersion clientApplicationVersion = this.clientApplicationVersionAPIRepository.GetClientApplicationVersion(deviceId, accessToken);
            if (clientApplicationVersion != null)
            {
                this.SaveClientApplicationVersion(clientApplicationVersion);
            }

            return this.GetClientApplicationVersion();
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

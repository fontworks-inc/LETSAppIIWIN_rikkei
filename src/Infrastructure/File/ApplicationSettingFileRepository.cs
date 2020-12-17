﻿using System.Text.Json;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.File
{
    /// <summary>
    /// アプリケーション設定情報を格納するファイルリポジトリ
    /// </summary>
    public class ApplicationSettingFileRepository : FileRepositoryBase, IApplicationSettingRepository
    {
        /// <summary>
        /// フォント配信サーバURI デフォルト値
        /// </summary>
        private static readonly string FontDeliveryServerUri = "https://158.101.75.134";

        /// <summary>
        /// 通知サーバURI デフォルト値
        /// </summary>
        private static readonly string NotificationServerUri = "https://158.101.75.134";

        /// <summary>
        /// 通信リトライ回数 デフォルト値
        /// </summary>
        private static readonly int CommunicationRetryCount = 10;

        /// <summary>
        /// フォント配信サーバURI デフォルト値
        /// </summary>
        private static readonly int FixedTermConfirmationInterval = 1800;

        /// <summary>
        /// フォント配信サーバURI デフォルト値
        /// </summary>
        private static readonly int FontCalculationFactor = 3;

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        public ApplicationSettingFileRepository(string filePath)
            : base(filePath)
        {
        }

        /// <summary>
        /// アプリケーション設定情報を取得する
        /// </summary>
        /// <returns>アプリケーション設定情報</returns>
        public ApplicationSetting GetSetting()
        {
            if (System.IO.File.Exists(this.FilePath))
            {
                // ファイルが存在する場合、内容を返す
                string jsonString = this.ReadAll();
                return JsonSerializer.Deserialize<ApplicationSetting>(jsonString);
            }
            else
            {
                // ファイルが存在しない場合、定数を持つ新規のオブジェクトを返す
                return new ApplicationSetting(
                    FontDeliveryServerUri,
                    NotificationServerUri,
                    CommunicationRetryCount,
                    FixedTermConfirmationInterval,
                    FontCalculationFactor);
            }
        }
    }
}

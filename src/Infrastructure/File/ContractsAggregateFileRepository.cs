using System;
using System.Text.Json;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.API;
using NLog;

namespace Infrastructure.File
{
    /// <summary>
    /// 契約情報取得処理のレスポンスを扱うファイルリポジトリ
    /// </summary>
    public class ContractsAggregateFileRepository : EncryptFileBase, IContractsAggregateRepository
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        private ContractsAggregateAPIRepository contractsAggregateAPIRepository;

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        public ContractsAggregateFileRepository(string filePath, ContractsAggregateAPIRepository contractsAggregateAPIRepository)
            : base(filePath)
        {
            this.contractsAggregateAPIRepository = contractsAggregateAPIRepository;
        }

        /// <summary>
        /// 契約情報を取得する
        /// </summary>
        /// <returns>契約情報の集合体</returns>
        public ContractsAggregate GetContractsAggregate()
        {
            if (System.IO.File.Exists(this.FilePath))
            {
                // ファイルが存在する場合、内容を返す
                try
                {
                    string jsonString = this.ReadAll();
                    return JsonSerializer.Deserialize<ContractsAggregate>(jsonString);
                }
                catch (Exception ex)
                {
                    Logger.Error("GetContractsAggregate:" + ex.StackTrace);
                    return new ContractsAggregate();
                }
            }
            else
            {
                // ファイルが存在しない場合、新規のオブジェクトを返す
                return new ContractsAggregate();
            }
        }

        /// <summary>
        /// 契約情報を取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>契約情報の集合体</returns>
        /// <remarks>API呼び出し時のみ実装</remarks>
        public ContractsAggregate GetContractsAggregate(string deviceId, string accessToken)
        {
            ContractsAggregate contractsAggregate = this.contractsAggregateAPIRepository.GetContractsAggregate(deviceId, accessToken);
            this.SaveContractsAggregate(contractsAggregate);

            return contractsAggregate;
        }

        /// <summary>
        /// 契約情報を保存する
        /// </summary>
        /// <param name="contractsAggregate">契約情報の集合体</param>
        public void SaveContractsAggregate(ContractsAggregate contractsAggregate)
        {
            this.WriteAll(JsonSerializer.Serialize(contractsAggregate));
        }
    }
}

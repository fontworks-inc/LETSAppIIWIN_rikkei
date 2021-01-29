using System;
using System.Collections.Generic;
using Core.Entities;
using Core.Interfaces;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Infrastructure.API
{
    /// <summary>
    /// 契約情報を格納するAPIリポジトリモック
    /// </summary>
    /// <remarks>FUNCTION_08_03_02(契約情報取得API)</remarks>
    public class ContractsAggregateAPIRepository : APIRepositoryBase, IContractsAggregateRepository
    {
        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="apiConfiguration">設定情報</param>
        public ContractsAggregateAPIRepository(APIConfiguration apiConfiguration)
            : base(apiConfiguration)
        {
        }

        /// <summary>
        /// 契約情報を取得する
        /// </summary>
        /// <returns>契約情報の集合体</returns>
        /// <remarks>APIリポジトリではこのメソッドは実装しない</remarks>
        public ContractsAggregate GetContractsAggregate()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 契約情報を保存する
        /// </summary>
        /// <param name="contractsAggregate">契約情報の集合体</param>
        /// <remarks>APIリポジトリではこのメソッドは実装しない</remarks>
        public void SaveContractsAggregate(ContractsAggregate contractsAggregate)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 契約情報を取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>契約情報の集合体</returns>
        public ContractsAggregate GetContractsAggregate(string deviceId, string accessToken)
        {
            ContractsAggregate response = new ContractsAggregate();

            // APIの引数の値をセット(個別処理)
            this.ApiParam[APIParam.DeviceId] = deviceId;
            this.ApiParam[APIParam.AccessToken] = accessToken;

            // API通信を行う(リトライ込み)を行う（共通処理）
            try
            {
                this.Invoke(this.CallGetContractsAPI);

                // 戻り値のセット（個別処理）
                var ret = new ContractResponse(this.ApiResponse);
                if (ret.Code == (int)ResponseCode.Succeeded)
                {
                    response.NeedContractRenewal = ret.Data.NeedContractRenewal;
                    Action<ContractResponseContract> action = item =>
                    {
                        var contract = new Contract();
                        contract.ContractId = item.ContractId;
                        try
                        {
                            contract.ContractEndDate = DateTime.Parse(item.ContractEndDate);
                        }
                        catch (Exception ex)
                        {
                            contract.ContractEndDate = new DateTime(0);
                        }
                        response.Contracts.Add(contract);
                    };
                    ret.Data.Contracts.ForEach(action);
                }
                else
                {
                    throw new ApiException(ret.Code, ret.Message);
                }
            }
            catch (ApiException)
            {
                // 通信に失敗or通信しなかった
                throw;
            }

            return response;
        }

        /// <summary>
        /// 契約情報取得の呼び出し
        /// </summary>
        private void CallGetContractsAPI()
        {
            Configuration config = new Configuration();
            config.BasePath = this.BasePath;
            config.UserAgent = (string)this.ApiParam[APIParam.UserAgent];
            config.AccessToken = (string)this.ApiParam[APIParam.AccessToken];
            ContractApi apiInstance = new ContractApi(config);
            this.ApiResponse = apiInstance.GetContracts((string)this.ApiParam[APIParam.DeviceId], config.UserAgent);
        }
    }
}

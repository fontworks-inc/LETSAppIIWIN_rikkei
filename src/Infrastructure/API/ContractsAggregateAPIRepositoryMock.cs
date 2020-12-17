using System;
using System.Collections.Generic;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.API
{
    /// <summary>
    /// 契約情報を格納するAPIリポジトリモック
    /// </summary>
    /// <remarks>FUNCTION_08_03_02(契約情報取得API)</remarks>
    public class ContractsAggregateAPIRepositoryMock : IContractsAggregateRepository
    {
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
            return new ContractsAggregate(true, new List<Contract>()
            {
                new Contract("AAA1234", DateTime.Parse("2021/12/08 15:38")),
                new Contract("ABC999", DateTime.Parse("2022/12/08 15:38")),
            });
        }
    }
}

using Core.Entities;

namespace Core.Interfaces
{
    /// <summary>
    /// 契約情報を格納するリポジトリのインターフェイス
    /// </summary>
    public interface IContractsAggregateRepository
    {
        /// <summary>
        /// 契約情報を取得する
        /// </summary>
        /// <returns>契約情報の集合体</returns>
        ContractsAggregate GetContractsAggregate();

        /// <summary>
        /// 契約情報を取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>契約情報の集合体</returns>
        ContractsAggregate GetContractsAggregate(string deviceId, string accessToken);

        /// <summary>
        /// 契約情報を保存する
        /// </summary>
        /// <param name="contractsAggregate">ユーザ別フォント情報</param>
        void SaveContractsAggregate(ContractsAggregate contractsAggregate);
    }
}

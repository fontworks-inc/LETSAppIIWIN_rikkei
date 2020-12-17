using System.Collections.Generic;

namespace Core.Entities
{
    /// <summary>
    /// 契約情報の集合体を表すクラス
    /// </summary>
    public class ContractsAggregate
    {
        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        public ContractsAggregate()
        {
        }

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="needContractRenewal">契約更新要否</param>
        /// <param name="contracts">契約情報</param>
        public ContractsAggregate(bool needContractRenewal, IList<Contract> contracts)
        {
            this.NeedContractRenewal = needContractRenewal;
            this.Contracts = contracts;
        }

        /// <summary>
        /// 契約更新要否
        /// </summary>
        public bool NeedContractRenewal { get; set; } = false;

        /// <summary>
        /// 契約情報
        /// </summary>
        public IList<Contract> Contracts { get; set; } = new List<Contract>();
    }
}

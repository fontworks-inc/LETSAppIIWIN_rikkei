using System;

namespace Core.Entities
{
    /// <summary>
    /// 契約情報を表すクラス
    /// </summary>
    public class Contract
    {
        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        public Contract()
        {
        }

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="contractId">契約ID</param>
        /// <param name="contractEndDate">契約終了日</param>
        public Contract(string contractId, DateTime contractEndDate)
        {
            this.ContractId = contractId;
            this.ContractEndDate = contractEndDate;
        }

        /// <summary>
        /// 契約ID
        /// </summary>
        public string ContractId { get; set; }

        /// <summary>
        /// 契約終了日
        /// </summary>
        public DateTime ContractEndDate { get; set; }

        /// <summary>
        /// このオブジェクトが指定されたオブジェクトと等しいかどうかを判定する
        /// </summary>
        /// <param name="obj">比較対象のオブジェクト</param>
        /// <returns>このオブジェクトが指定されたオブジェクトと等しい場合はtrue、それ以外はfalse</returns>
        public override bool Equals(object obj)
        {
            return obj is Contract contract &&
                   this.ContractId == contract.ContractId;
        }

        /// <summary>
        /// このオブジェクトのハッシュコードを取得する
        /// </summary>
        /// <returns>このオブジェクトのハッシュコード</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.ContractId);
        }
    }
}

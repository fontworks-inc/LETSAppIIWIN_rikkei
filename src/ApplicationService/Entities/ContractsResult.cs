using Core.Entities;

namespace ApplicationService.Entities
{
    /// <summary>
    /// 契約情報取得結果を表すクラス
    /// </summary>
    internal class ContractsResult
    {
        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="contractsAggregate">契約情報の集合体</param>
        internal ContractsResult(ContractsAggregate contractsAggregate)
        {
            this.ContractsAggregate = contractsAggregate;
            this.IsCashed = false;
        }

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="contractsAggregate">契約情報の集合体</param>
        /// <param name="isCashed">キャッシュから取得したどうかを表す</param>
        internal ContractsResult(ContractsAggregate contractsAggregate, bool isCashed)
        {
            this.ContractsAggregate = contractsAggregate;
            this.IsCashed = isCashed;
        }

        /// <summary>
        /// 契約情報の集合体
        /// </summary>
        internal ContractsAggregate ContractsAggregate { get; }

        /// <summary>
        /// キャッシュから取得したどうかを表す
        /// </summary>
        /// <remarks>キャッシュから取得した場合true, そうでない場合false</remarks>
        internal bool IsCashed { get; }
    }
}

using Core.Entities;

namespace Core.Interfaces
{
    /// <summary>
    /// お客様情報を扱うリポジトリのインターフェイス
    /// </summary>
    public interface ICustomerRepository
    {
        /// <summary>
        /// お客様情報を取得する
        /// </summary>
        /// <returns>お客様情報</returns>
        Customer GetCustomer();

        /// <summary>
        /// お客様情報を保存する
        /// </summary>
        /// <param name="customer">お客様情報</param>
        void SaveCustomer(Customer customer);

        /// <summary>
        /// 削除する
        /// </summary>
        void Delete();
    }
}

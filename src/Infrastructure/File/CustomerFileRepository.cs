using System.Text.Json;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.File
{
    /// <summary>
    /// お客様情報を格納するファイルリポジトリ
    /// </summary>
    public class CustomerFileRepository : EncryptFileBase, ICustomerRepository
    {
        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        public CustomerFileRepository(string filePath)
            : base(filePath)
        {
        }

        /// <summary>
        /// お客様情報を取得する
        /// </summary>
        /// <returns>お客様情報</returns>
        public Customer GetCustomer()
        {
            if (System.IO.File.Exists(this.FilePath))
            {
                // ファイルが存在する場合、内容を返す
                string jsonString = this.ReadAll();
                return JsonSerializer.Deserialize<Customer>(jsonString);
            }
            else
            {
                // ファイルが存在しない場合、新規のオブジェクトを返す
                return new Customer();
            }
        }

        /// <summary>
        /// お客様情報を保存する
        /// </summary>
        /// <param name="customer">お客様情報</param>
        public void SaveCustomer(Customer customer)
        {
            this.WriteAll(JsonSerializer.Serialize(customer));
        }
    }
}

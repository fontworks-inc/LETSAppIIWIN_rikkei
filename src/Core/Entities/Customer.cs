using System;

namespace Core.Entities
{
    /// <summary>
    /// お客様情報を表すクラス
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        public Customer()
        {
        }

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="mailAddress">メールアドレス</param>
        /// <param name="lastName">姓</param>
        /// <param name="firstName">名</param>
        public Customer(string mailAddress, string lastName, string firstName)
        {
            this.MailAddress = mailAddress;
            this.LastName = lastName;
            this.FirstName = firstName;
        }

        /// <summary>
        /// メールアドレス
        /// </summary>
        public string MailAddress { get; set; } = string.Empty;

        /// <summary>
        /// 姓
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// 名
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// このオブジェクトが指定されたオブジェクトと等しいかどうかを判定する
        /// </summary>
        /// <param name="obj">比較対象のオブジェクト</param>
        /// <returns>このオブジェクトが指定されたオブジェクトと等しい場合はtrue、それ以外はfalse</returns>
        public override bool Equals(object obj)
        {
            return obj is Customer customer &&
                   this.MailAddress == customer.MailAddress;
        }

        /// <summary>
        /// このオブジェクトのハッシュコードを取得する
        /// </summary>
        /// <returns>このオブジェクトのハッシュコード</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.MailAddress);
        }
    }
}

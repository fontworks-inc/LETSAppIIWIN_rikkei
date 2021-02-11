namespace Core.Entities
{
    /// <summary>
    /// ユーザIDを表すクラス
    /// </summary>
    public class UserId
    {
        private readonly string userId;

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="userId">ユーザID</param>
        public UserId(string userId)
        {
            this.userId = userId;
        }

        /// <summary>
        /// ユーザIDを取得する
        /// </summary>
        /// <returns>ユーザID</returns>
        public override string ToString()
        {
            return this.userId;
        }
    }
}

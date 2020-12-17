namespace Core.Entities
{
    /// <summary>
    /// 利用者を表すクラス
    /// </summary>
    public class User
    {
        /// <summary>
        /// メールアドレス
        /// </summary>
        public string MailAddress { get; set; } = string.Empty;

        /// <summary>
        /// パスワード
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// ホスト名
        /// </summary>
        public string HostName { get; set; } = string.Empty;

        /// <summary>
        /// OSユーザ名
        /// </summary>
        public string OSUserName { get; set; } = string.Empty;
    }
}

namespace Core.Entities
{
    /// <summary>
    /// 認証情報のレスポンス情報を表すクラス
    /// </summary>
    public class AuthenticationInformationResponse : ResponseBase<AuthenticationInformation>
    {
        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        public AuthenticationInformationResponse()
        {
        }

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="code">レスポンスコード</param>
        /// <param name="message">メッセージ</param>
        /// <param name="data">データ</param>
        public AuthenticationInformationResponse(
            int code, string message, AuthenticationInformation data)
            : base(code, message, data)
        {
        }
    }
}

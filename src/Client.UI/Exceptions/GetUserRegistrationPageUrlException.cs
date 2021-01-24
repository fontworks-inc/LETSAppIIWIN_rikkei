using System;

namespace Client.UI.Exceptions
{
    /// <summary>
    /// 会員登録ページURL取得処理に失敗した際にスローされる例外クラス
    /// </summary>
    public class GetUserRegistrationPageUrlException : Exception
    {
        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        public GetUserRegistrationPageUrlException()
            : base()
        {
        }

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        public GetUserRegistrationPageUrlException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        /// <param name="inner">内部例外</param>
        public GetUserRegistrationPageUrlException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

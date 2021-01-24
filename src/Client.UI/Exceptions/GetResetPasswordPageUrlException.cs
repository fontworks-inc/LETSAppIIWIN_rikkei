using System;

namespace Client.UI.Exceptions
{
    /// <summary>
    /// パスワード再設定ページURL取得処理に失敗した際にスローされる例外クラス
    /// </summary>
    public class GetResetPasswordPageUrlException : Exception
    {
        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        public GetResetPasswordPageUrlException()
            : base()
        {
        }

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        public GetResetPasswordPageUrlException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        /// <param name="inner">内部例外</param>
        public GetResetPasswordPageUrlException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

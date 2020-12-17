using System;
using System.Runtime.Serialization;

namespace Core.Exceptions
{
    /// <summary>
    /// レスポンスコードがその他のエラーであった場合の例外
    /// </summary>
    public class InvalidResponseCodeException : Exception
    {
        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        public InvalidResponseCodeException()
            : base()
        {
        }

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="message">例外メッセージ</param>
        public InvalidResponseCodeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="message">例外メッセージ</param>
        /// <param name="innerException">内部例外</param>
        public InvalidResponseCodeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="info">スローされている例外に関するシリアル化済みオブジェクト データを保持している SerializationInfo</param>
        /// <param name="context">転送元または転送先についてのコンテキスト情報を含む StreamingContext</param>
        protected InvalidResponseCodeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

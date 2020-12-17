using System;
using System.Runtime.Serialization;

namespace Core.Exceptions
{
    /// <summary>
    /// OSタイプが不正であった場合の例外
    /// </summary>
    public class InvalidOSTypeException : Exception
    {
        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        public InvalidOSTypeException()
            : base()
        {
        }

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="message">例外メッセージ</param>
        public InvalidOSTypeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="message">例外メッセージ</param>
        /// <param name="innerException">内部例外</param>
        public InvalidOSTypeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="info">スローされている例外に関するシリアル化済みオブジェクト データを保持している SerializationInfo</param>
        /// <param name="context">転送元または転送先についてのコンテキスト情報を含む StreamingContext</param>
        protected InvalidOSTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}

using System;
using Core.Exceptions;

namespace Core.Entities
{
    /// <summary>
    /// レスポンスを表すクラス
    /// </summary>
    /// <typeparam name="T">データに格納するEntityの型</typeparam>
    public class ResponseBase<T>
    {
        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        public ResponseBase()
        {
        }

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="code">レスポンスコード</param>
        /// <param name="message">メッセージ</param>
        /// <param name="data">データ</param>
        public ResponseBase(int code, string message, T data)
        {
            this.Code = code;
            this.Message = message;
            this.Data = data;
        }

        /// <summary>
        /// レスポンスコード
        /// </summary>
        public int Code { get; set; } = 0;

        /// <summary>
        /// メッセージ
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// データ
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// レスポンスコードを取得する
        /// </summary>
        /// <returns>レスポンスコード</returns>
        public ResponseCode GetResponseCode()
        {
            if (Enum.IsDefined(typeof(ResponseCode), this.Code))
            {
                return (ResponseCode)this.Code;
            }
            else
            {
                throw new InvalidResponseCodeException(this.Message);
            }
        }

        /// <summary>
        /// このオブジェクトが指定されたオブジェクトと等しいかどうかを判定する
        /// </summary>
        /// <param name="obj">比較対象のオブジェクト</param>
        /// <returns>このオブジェクトが指定されたオブジェクトと等しい場合はtrue、それ以外はfalse</returns>
        public override bool Equals(object obj)
        {
            return obj is ResponseBase<T> response &&
                   this.Code == response.Code &&
                   this.Message == response.Message &&
                   this.Data.Equals(response.Data);
        }

        /// <summary>
        /// このオブジェクトのハッシュコードを取得する
        /// </summary>
        /// <returns>このオブジェクトのハッシュコード</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Code);
        }
    }
}

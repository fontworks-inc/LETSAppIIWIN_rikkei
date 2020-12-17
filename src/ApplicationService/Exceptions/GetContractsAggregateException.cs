using System;

namespace ApplicationService.Exceptions
{
    /// <summary>
    /// 契約情報取得処理に失敗した際にスローされる例外クラス
    /// </summary>
    public class GetContractsAggregateException : Exception
    {
        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        public GetContractsAggregateException()
            : base()
        {
        }

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        public GetContractsAggregateException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        /// <param name="inner">内部例外</param>
        public GetContractsAggregateException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

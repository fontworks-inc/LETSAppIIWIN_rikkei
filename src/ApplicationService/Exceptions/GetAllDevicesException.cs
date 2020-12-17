using System;

namespace ApplicationService.Exceptions
{
    /// <summary>
    /// 全端末情報取得処理に失敗した際にスローされる例外クラス
    /// </summary>
    public class GetAllDevicesException : Exception
    {
        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        public GetAllDevicesException()
            : base()
        {
        }

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        public GetAllDevicesException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        /// <param name="inner">内部例外</param>
        public GetAllDevicesException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

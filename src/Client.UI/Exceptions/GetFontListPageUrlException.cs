using System;

namespace Client.UI.Exceptions
{
    /// <summary>
    /// フォント一覧画面URL取得処理に失敗した際にスローされる例外クラス
    /// </summary>
    public class GetFontListPageUrlException : Exception
    {
        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        public GetFontListPageUrlException()
            : base()
        {
        }

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        public GetFontListPageUrlException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        /// <param name="inner">内部例外</param>
        public GetFontListPageUrlException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}

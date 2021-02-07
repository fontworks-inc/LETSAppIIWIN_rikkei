using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    /// <summary>
    /// SSEメッセージ
    /// </summary>
    public class SseMessage
    {
        private class KeyValue
        {
            public string Key { get; set; }

            public string Value { get; set; }
        }

        /// <summary>
        /// 受信したSSEイベント文字列のリストからSSEメッセージを取り出す
        /// </summary>
        /// <param name="messageList">受信したSSEメッセージ文字列(各1行)のリスト</param>
        /// <returns>SseMessage </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "<保留中>")]
        public static SseMessage BuildMessage(List<string> messageList)
        {
            var id = string.Empty;
            var eventType = string.Empty;
            var data = new StringBuilder();
            var retry = string.Empty;

            messageList.ForEach(message =>
            {
                var trimedMessage = message.Trim();
                var keyValue = GetKeyValue(trimedMessage);
                switch (keyValue.Key)
                {
                    case "id":
                        id = keyValue.Value;
                        break;
                    case "event":
                        eventType = keyValue.Value;
                        break;
                    case "data":
                        data.Append(keyValue.Value);
                        break;
                    case "retry":
                        retry = keyValue.Value;
                        break;
                    default:
                        break;
                }
            });
            return new SseMessage(id, eventType, data.ToString(), retry);
        }

        /// <summary>
        /// Retry (msec)を取得する.
        /// SSE では 再接続Retry の待ち時間がretry: にmsecで指定されている.
        /// </summary>
        /// <returns>
        /// 設定されていれば待ち時間(sec)を返す.<br/>
        /// 設定されていない場合には-1を返す.
        /// </returns>
        public int RetryValue()
        {
            if (int.TryParse(this._retry, out var value))
            {
                return value;
            }

            return -1;
        }

        /// <summary>
        /// id を数値として取得する.
        /// </summary>
        /// <returns>
        /// 数値が設定されていればidを返す.<br/>
        /// 設定されていない、あるいは数値でない場合には-1を返す.
        /// </returns>
        public int IdValue()
        {
            if (int.TryParse(this._id, out var value))
            {
                return value;
            }

            return -1;
        }

        /// <summary>
        /// 文字列の取得
        /// </summary>
        /// <returns>出力用文字列</returns>
        public override string ToString()
        {
            return $"id: {this.Id}  event: {this.EventType}  data: {this.Data}";
        }

        /// <summary>
        /// コメントかの判断
        /// </summary>
        /// <returns>コメントではあればtrue</returns>
        public bool IsCommentOnly()
        {
            return this._retry.Length == 0 && this._id.Length == 0 && this._data.Length == 0;
        }

        private static string GetValue(string message, string key) => message.Trim().Replace(key, string.Empty).Trim();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="id">key id: の値</param>
        /// <param name="eventType">key event: の値</param>
        /// <param name="data">key data: の値</param>
        /// <param name="retry">key retry: の値</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "<保留中>")]
        private SseMessage(string id, string eventType, string data, string retry)
        {
            this._id = id;
            this._eventType = eventType;
            this._data = data;
            this._retry = retry;
        }

        private static KeyValue GetKeyValue(string message)
        {
            var result = new KeyValue();
            var trimed = message.Trim();
            var keyEndPosition = trimed.IndexOf(':');

            if (keyEndPosition < 0)
            {
                result.Key = string.Empty;
                result.Value = string.Empty;
                return result;
            }

            result.Key = trimed.Substring(0, keyEndPosition).Trim();
            result.Value = trimed.Substring(keyEndPosition + 1).Trim();
            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "<保留中>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309:Field names should not begin with underscore", Justification = "<保留中>")]
        private readonly string _retry;

        /// <summary>
        /// リトライ
        /// </summary>
        public string Retry => this._retry;

        /// <summary>
        /// イベントタイプ
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "<保留中>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309:Field names should not begin with underscore", Justification = "<保留中>")]
        private readonly string _eventType;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:Elements should be documented", Justification = "<保留中>")]
        public string EventType => this._eventType;

        /// <summary>
        /// data
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "<保留中>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309:Field names should not begin with underscore", Justification = "<保留中>")]
        private readonly string _data;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:Elements should be documented", Justification = "<保留中>")]
        public string Data => this._data;

        /// <summary>
        /// id
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:Elements should appear in the correct order", Justification = "<保留中>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1309:Field names should not begin with underscore", Justification = "<保留中>")]
        private readonly string _id;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:Elements should be documented", Justification = "<保留中>")]
        public string Id => this._id;
    }
}

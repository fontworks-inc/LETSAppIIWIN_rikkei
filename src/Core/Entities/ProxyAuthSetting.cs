using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    /// <summary>
    /// プロキシ認証情報を表すクラス
    /// </summary>
    public class ProxyAuthSetting
    {
        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        public ProxyAuthSetting()
        {
        }

        /// <summary>
        /// プロキシ認証ID
        /// </summary>
        public string ID { get; set; } = string.Empty;

        /// <summary>
        /// プロキシ認証パスワード
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Core.Interfaces
{
    /// <summary>
    /// APIアクセスの設定インタフェース
    /// </summary>
    public interface IAPIConfiguration
    {
        /// <summary>
        /// プロキシを取得する
        /// </summary>
        /// <param name="targeturi">接続先URI</param>
        /// <returns>プロキシ情報</returns>
        IWebProxy GetWebProxy(string targeturl);

        /// <summary>
        /// UserAgentを取得する
        /// </summary>
        /// <param name="apppath">アプリケーションパス</param>
        /// <returns>UserAgent</returns>
        string GetUserAgent(string apppath);
    }
}

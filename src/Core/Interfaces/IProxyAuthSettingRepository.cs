using System;
using System.Collections.Generic;
using System.Text;
using Core.Entities;

namespace Core.Interfaces
{
    /// <summary>
    /// プロキシ認証情報を格納するリポジトリのインターフェイス
    /// </summary>
    public interface IProxyAuthSettingRepository
    {
        /// <summary>
        /// プロキシ認証情報を取得する
        /// </summary>
        /// <returns>プロキシ認証情報情報</returns>
        ProxyAuthSetting GetSetting();
    }
}

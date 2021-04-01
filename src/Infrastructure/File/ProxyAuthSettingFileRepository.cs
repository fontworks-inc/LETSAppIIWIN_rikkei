using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.File
{
    public class ProxyAuthSettingFileRepository : TextFileRepositoryBase, IProxyAuthSettingRepository
    {
        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        public ProxyAuthSettingFileRepository(string filePath)
            : base(filePath)
        {
        }

        /// <summary>
        /// プロキシ認証設定情報を取得する
        /// </summary>
        /// <returns>プロキシ認証設定情報</returns>
        public ProxyAuthSetting GetSetting()
        {
            if (System.IO.File.Exists(this.FilePath))
            {
                // ファイルが存在する場合、内容を返す
                string jsonString = this.ReadAll();
                return JsonSerializer.Deserialize<ProxyAuthSetting>(jsonString);
            }
            else
            {
                // ファイルが存在しない場合、定数を持つ新規のオブジェクトを返す
                return new ProxyAuthSetting();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.File
{
    /// <summary>
    /// フォント情報一覧(デバイスモード時)を格納するリポジトリ
    /// </summary>
    public class DeviceModeFontListRepository : TextFileRepositoryBase, IDeviceModeFontListRepository
    {
        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        public DeviceModeFontListRepository(string filePath)
            : base(filePath)
        {
        }

        /// <summary>
        /// フォント情報一覧(デバイスモード時)を取得する
        /// </summary>
        /// <returns>フォント情報一覧(デバイスモード時)</returns>
        public DeviceModeFontList GetDeviceModeFontList()
        {
            if (System.IO.File.Exists(this.FilePath))
            {
                // ファイルが存在する場合、内容を返す
                string jsonString = this.ReadAll();
                return JsonSerializer.Deserialize<DeviceModeFontList>(jsonString);
            }
            else
            {
                // ファイルが存在しない場合、新規のオブジェクトを返す
                return new DeviceModeFontList();
            }
        }

        /// <summary>
        /// フォント情報一覧(デバイスモード時)を保存する
        /// </summary>
        /// <param name="deviceModeFontList">フォント情報一覧(デバイスモード時)</param>
        public void SaveDeviceModeFontList(DeviceModeFontList deviceModeFontList)
        {
            this.WriteAll(JsonSerializer.Serialize(deviceModeFontList));
        }
    }
}

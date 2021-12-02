using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Core.Entities;
using Core.Interfaces;
using NLog;

namespace Infrastructure.File
{
    /// <summary>
    /// ライセンス情報(デバイスモード時)を格納するファイルリポジトリ
    /// </summary>
    public class DeviceModeLicenseInfoRepository : EncryptFileBase, IDeviceModeLicenseInfoRepository
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        private static object saveLockObject = new object();

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        public DeviceModeLicenseInfoRepository(string filePath)
            : base(filePath)
        {
        }

        /// <summary>
        /// 設定情報(デバイスモード時)を取得する
        /// </summary>
        /// <returns>設定情報(デバイスモード時)</returns>
        public Core.Entities.DeviceModeLicenseInfo GetDeviceModeLicenseInfo()
        {
            if (System.IO.File.Exists(this.FilePath))
            {
                // ファイルが存在する場合、内容を返す
                try
                {
                    string jsonString = this.ReadAll();
                    return JsonSerializer.Deserialize<DeviceModeLicenseInfo>(jsonString);
                }
                catch (Exception ex)
                {
                    Logger.Error("UserStatus:" + ex.StackTrace);
                    return new DeviceModeLicenseInfo();
                }
            }
            else
            {
                // ファイルが存在しない場合、新規のオブジェクトを返す
                return new DeviceModeLicenseInfo();
            }
        }

        /// <summary>
        /// 設定情報(デバイスモード時)を保存する
        /// </summary>
        /// <param name="info">設定情報(デバイスモード時)</param>
        public void SaveDeviceModeLicenseInfo(Core.Entities.DeviceModeLicenseInfo info)
        {
            lock (saveLockObject)
            {
                this.WriteAll(JsonSerializer.Serialize(info));
            }
        }
    }
}

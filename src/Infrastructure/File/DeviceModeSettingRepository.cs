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
    /// 設定情報(デバイスモード時)を格納するファイルリポジトリ
    /// </summary>
    public class DeviceModeSettingRepository : EncryptFileBase, IDeviceModeSettingRepository
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
        public DeviceModeSettingRepository(string filePath)
            : base(filePath)
        {
        }

        public bool Exists()
        {
            return System.IO.File.Exists(this.FilePath);
        }

        /// <summary>
        /// 設定情報(デバイスモード時)を取得する
        /// </summary>
        /// <returns>設定情報(デバイスモード時)</returns>
        public Core.Entities.DeviceModeSetting GetDeviceModeSetting()
        {
            Logger.Debug($"GetDeviceModeSetting:Enter");
            Logger.Warn($"GetDeviceModeSetting:{this.FilePath}");

            if (System.IO.File.Exists(this.FilePath))
            {
                // ファイルが存在する場合、内容を返す
                try
                {
                    Logger.Debug($"GetDeviceModeSetting:ファイルが存在する場合、内容を返す");
                    Logger.Warn($"GetDeviceModeSetting:ファイルが存在する場合、内容を返す");
                    string jsonString = this.ReadAll();
                    return JsonSerializer.Deserialize<DeviceModeSetting>(jsonString);
                }
                catch (Exception ex)
                {
                    Logger.Error("GetDeviceModeSetting:" + ex.StackTrace);
                    return new DeviceModeSetting();
                }
            }
            else
            {
                // ファイルが存在しない場合、新規のオブジェクトを返す
                Logger.Debug($"GetDeviceModeSetting:ファイルが存在しない場合、新規のオブジェクトを返す");
                DeviceModeSetting deviceModeSetting = new DeviceModeSetting();
                Logger.Warn($"DeviceModeSetting 新規作成");
#if COMPLETELY_OFFLINE
                Logger.Warn($"GetDeviceModeSetting:完全オフラインモード");
                deviceModeSetting.IsCompletelyOffline = true;  //完全オフラインモードをtrueにする
                this.SaveDeviceModeSetting(deviceModeSetting);
#endif
                return deviceModeSetting;
            }
        }

        /// <summary>
        /// 設定情報(デバイスモード時)を保存する
        /// </summary>
        /// <param name="setting">設定情報(デバイスモード時)</param>
        public void SaveDeviceModeSetting(Core.Entities.DeviceModeSetting setting)
        {
            Logger.Warn($"SaveDeviceModeSetting:{setting}");
            lock (saveLockObject)
            {
                this.WriteAll(JsonSerializer.Serialize(setting));
            }
        }
    }
}

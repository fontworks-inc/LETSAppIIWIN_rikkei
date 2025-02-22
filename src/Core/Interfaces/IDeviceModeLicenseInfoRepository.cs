﻿using System;
using System.Collections.Generic;
using System.Text;
using Core.Entities;

namespace Core.Interfaces
{
    /// <summary>
    /// ライセンス情報(デバイスモード時)を格納するリポジトリのインターフェイス
    /// </summary>
    public interface IDeviceModeLicenseInfoRepository
    {
        /// <summary>
        /// 設定情報(デバイスモード時)を取得する
        /// </summary>
        /// <returns>設定情報(デバイスモード時)</returns>
        DeviceModeLicenseInfo GetDeviceModeLicenseInfo();

        /// <summary>
        /// 設定情報(デバイスモード時)を取得する
        /// </summary>
        /// <returns>設定情報(デバイスモード時)</returns>
        DeviceModeLicenseInfo GetDeviceModeLicenseInfo(bool fromOnline, string offlineDeviceId, string indefiniteAccessToken, string licenceFileKeyPath, string licenseDecryptionKey);

        /// <summary>
        /// 設定情報(デバイスモード時)を保存する
        /// </summary>
        /// <param name="setting">設定情報(デバイスモード時)</param>
        void SaveDeviceModeLicenseInfo(DeviceModeLicenseInfo setting);

        DeviceModeLicenseInfo CreateLicenseInfoFromJsonText(string jsonText);
    }
}

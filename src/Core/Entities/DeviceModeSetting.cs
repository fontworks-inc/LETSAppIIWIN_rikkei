using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    /// <summary>
    /// 設定情報(デバイスモード時)を表すクラス
    /// </summary>
    public class DeviceModeSetting
    {
        /// <summary>
        /// オフラインデバイスID
        /// </summary>
        public string OfflineDeviceID { get; set; } = string.Empty;

        /// <summary>
        /// ライセンス復号キー
        /// </summary>
        public string LicenseDecodeKey { get; set; } = string.Empty;

        /// <summary>
        /// 無期限アクセストークン
        /// </summary>
        public string EternalAccessToken { get; set; } = string.Empty;

        /// <summary>
        /// ライセンスキーファイルパス
        /// </summary>
        public string LicenceFileKeyPath { get; set; } = string.Empty;

        /// <summary>
        /// フォントファイルパス
        /// </summary>
        public string FontFilePath { get; set; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    /// <summary>
    /// ライセンス情報(デバイスモード時)を表すクラス
    /// </summary>
    public class DeviceModeLicenseInfo
    {
        /// <summary>
        /// ライセンス一覧
        /// </summary>
        public IList<DeviceModeLicense> DeviceModeLicenceList { get; set; } = new List<DeviceModeLicense>();

        /// <summary>
        /// ZIPパスワード
        /// </summary>
        public string ZipPassword { get; set; } = string.Empty;
    }
}

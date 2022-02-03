using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Entities
{
    public class DeviceModeLicenceOfflineInfo
    {

        /// <summary>
        /// デバイスID
        /// </summary>
        public int? device_id { get; set; }

        /// <summary>
        /// ライセンス一覧
        /// </summary>
        public IList<DeviceModeLicenseOffline> licenses { get; set; } = new List<DeviceModeLicenseOffline>();

        /// <summary>
        /// ZIPパスワード
        /// </summary>
        public string zip_password { get; set; } = string.Empty;
    }
}

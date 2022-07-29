using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Entities
{
    /// <summary>
    /// デバイスモードライセンスオフライン
    /// </summary>
    public class DeviceModeLicenseOffline
    {
        /// <summary>
        /// LETS種別ID
        /// </summary>
        public int? lets_type_id { get; set; }

        /// <summary>
        /// LETS種別名
        /// </summary>
        public string lets_type_name { get; set; } = string.Empty;

        /// <summary>
        /// 有効期限
        /// </summary>
        public DateTime expiration_date { get; set; }
    }
}

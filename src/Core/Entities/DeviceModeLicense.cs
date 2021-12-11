using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    /// <summary>
    /// ライセンス(デバイスモード時)を表すクラス
    /// </summary>
    public class DeviceModeLicense
    {
        /// <summary>
        /// LETS種別
        /// </summary>
        public int? LetsKind { get; set; }

        /// <summary>
        /// LETS種別名
        /// </summary>
        public string LetsKindName { get; set; } = string.Empty;

        /// <summary>
        /// 有効期限
        /// </summary>
        public DateTime ExpireDate { get; set; }
    }
}

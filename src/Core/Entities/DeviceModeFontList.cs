using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    /// <summary>
    /// フォント一覧(デバイスモード時)を表すクラス
    /// </summary>
    public class DeviceModeFontList
    {
        /// <summary>
        /// フォント一覧(デバイスモード時)
        /// </summary>
        public IList<DeviceModeFontInfo> Fonts { get; set; } = new List<DeviceModeFontInfo>();
    }
}

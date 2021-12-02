using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    /// <summary>
    /// フォント(デバイスモード時)を表すクラス
    /// </summary>

    public class DeviceModeFontInfo
    {
        /// <summary>
        /// フォントファイルパス
        /// </summary>
        public string FontFilePath { get; set; } = string.Empty;

        /// <summary>
        /// LETS種別
        /// </summary>
        public int? LetsKind { get; set; }

        /// <summary>
        /// レジストリキー
        /// </summary>
        /// <remarks>アクティベート時に登録したレジストリのキー</remarks>
        public string RegistryKey { get; set; } = string.Empty;

        /// <summary>
        /// 削除対象
        /// </summary>
        /// <remarks>削除対象の時にTRUE, 起動時に削除するフォント</remarks>>
        public bool? IsRemove { get; set; }

    }
}

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
        /// フォント(デバイスモード時)を表すクラスのコンストラクタ
        /// </summary>
        public DeviceModeFontInfo()
        {
            this.FontFilePath = string.Empty;
            this.RegistryKey = string.Empty;
            this.LetsKind = 0;
            this.IsRemove = false;
        }

        /// <summary>
        /// フォント(デバイスモード時)を表すクラスのコンストラクタ
        /// </summary>
        /// <param name="fontFilePath">フォントファイルパス</param>
        /// <param name="registryKey">レジストリキー</param>
        /// <param name="letsKind">LETS種別</param>
        public DeviceModeFontInfo(string fontFilePath, string registryKey, int letsKind)
        {
            this.FontFilePath = fontFilePath;
            this.RegistryKey = registryKey;
            this.LetsKind = letsKind;
            this.IsRemove = false;
        }

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

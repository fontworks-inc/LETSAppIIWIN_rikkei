using System.Runtime.InteropServices;

namespace Core.Interfaces
{
    /// <summary>
    /// フォント情報のリポジトリインターフェイス
    /// </summary>
    public interface IFontFileRepository
    {
        /// <summary>
        /// フォントに設定された情報を取得する
        /// </summary>
        /// <param name="fontFilePath">フォントファイルパス</param>
        /// <returns>認証情報</returns>
        FontIdInfo GetFontInfo(string fontFilePath);
    }

    /// <summary>
    /// フォントID情報
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct FontIdInfo
    {
        /// <summary>
        /// 名称情報
        /// </summary>
        public NameInfo NameInfo;

        /// <summary>
        /// ID入り名称
        /// Size=k_license_desc_len+1
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 46)]
        public string IdInName;

        /// <summary>
        /// ユーザーID
        /// Size=k_n_user_id_digits+1
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string UserId;

        /// <summary>
        /// デバイスID
        /// Size=k_n_device_id_hex_digits+1
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 17)]
        public string DeviceId;

        /// <summary>
        /// ユーザーフォントの種類
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public int UserFontVers;

        /// <summary>
        /// LETS種別
        /// </summary>
        [MarshalAs(UnmanagedType.I4)]
        public int LetsKind;

        /// <summary>
        /// 顧客ID
        /// Size=k_n_customer_id_digits+1
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string CustomerId;
    }

    /// <summary>
    /// フォントIDに関する情報
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct Ids
    {
        /// <summary>
        /// ID入り読み込み名称
        /// Size=k_license_desc_len+1
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 46)]
        public string IdInReadableName;

        /// <summary>
        /// フォントID
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 7)]
        public string FontId;
    }

    /// <summary>
    /// フォント名称に関する情報
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct NameInfo
    {
        /// <summary>
        /// 表示名
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string UniqueName;

        /// <summary>
        /// バージョン情報
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string Version;

        /// <summary>
        /// フォントIDに関する情報
        /// </summary>
        public Ids Ids;
    }
}

using Core.Exceptions;

namespace Core.Entities
{
    /// <summary>
    /// 端末を表すクラス
    /// </summary>
    public class Device
    {
        /// <summary>
        /// ユーザデバイスID
        /// </summary>
        public string UserDeviceId { get; set; } = string.Empty;

        /// <summary>
        /// デバイスID
        /// </summary>
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>
        /// ホスト名
        /// </summary>
        public string HostName { get; set; } = string.Empty;

        /// <summary>
        /// OSユーザ名
        /// </summary>
        public string OSUserName { get; set; } = string.Empty;

        /// <summary>
        /// OSタイプ
        /// </summary>
        public string OSType { get; set; } = string.Empty;

        /// <summary>
        /// OSバージョン
        /// </summary>
        public string OSVersion { get; set; } = string.Empty;

        /// <summary>
        /// OSタイプを取得する
        /// </summary>
        /// <returns>OSタイプ</returns>
        public OSType GetOSType()
        {
            switch (this.OSType)
            {
                case "Win":
                    return Entities.OSType.Windows;
                case "Mac":
                    return Entities.OSType.MacOS;
                default:
                    throw new InvalidOSTypeException();
            }
        }
    }
}

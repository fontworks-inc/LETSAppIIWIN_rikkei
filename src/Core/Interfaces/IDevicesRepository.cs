using System.Collections.Generic;
using Core.Entities;

namespace Core.Interfaces
{
    /// <summary>
    /// 端末情報を格納するリポジトリのインターフェイス
    /// </summary>
    public interface IDevicesRepository
    {
        /// <summary>
        /// 全端末情報を取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>ユーザに紐づく全端末情報(削除済みデータを除く)</returns>
        /// <remarks>FUNCTION_08_01_04(端末情報取得API)</remarks>
        IList<Device> GetAllDevices(string deviceId, string accessToken);

        /// <summary>
        /// 端末を利用解除する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <param name="targetDeviceId">端末解除対象デバイスID</param>
        /// <remarks>FUNCTION_08_01_05(端末解除API)</remarks>
        void DeactivateDevice(string deviceId, string accessToken, string targetDeviceId);

        /// <summary>
        /// 端末を利用可能にする
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>認証情報</returns>
        /// <remarks>FUNCTION_08_01_06(端末使用API)</remarks>
        AuthenticationInformation ActivateDevice(string deviceId, string accessToken);

        /// <summary>
        /// デバイスIDを発行する
        /// </summary>
        /// <param name="user">ユーザ情報</param>
        /// <param name="deviceKey">デバイスキー</param>
        /// <returns>デバイスID</returns>
        /// <remarks>FUNCTION_08_01_12(デバイスID発行API)</remarks>
        string GetDeviceId(User user, string deviceKey);
    }
}

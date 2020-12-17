using System;
using System.Collections.Generic;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.API
{
    /// <summary>
    /// 端末情報を格納するAPIリポジトリ
    /// </summary>
    public class DevicesAPIRepository : APIRepositoryBase, IDevicesRepository
    {
        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="apiConfiguration">設定情報</param>
        public DevicesAPIRepository(APIConfiguration apiConfiguration)
            : base(apiConfiguration)
        {
        }

        /// <summary>
        /// 全端末情報を取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>ユーザに紐づく全端末情報(削除済みデータを除く)</returns>
        /// <remarks>FUNCTION_08_01_04(端末情報取得API)</remarks>
        public IList<Device> GetAllDevices(string deviceId, string accessToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 端末を利用解除する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <param name="targetDeviceId">端末解除対象デバイスID</param>
        /// <remarks>FUNCTION_08_01_05(端末解除API)</remarks>
        public void DeactivateDevice(string deviceId, string accessToken, string targetDeviceId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 端末を利用可能にする
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>認証情報</returns>
        /// <remarks>FUNCTION_08_01_06(端末使用API)</remarks>
        public AuthenticationInformation ActivateDevice(string deviceId, string accessToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// デバイスIDを発行する
        /// </summary>
        /// <param name="user">ユーザ情報</param>
        /// <returns>デバイスID</returns>
        /// <remarks>FUNCTION_08_01_12(デバイスID発行API)</remarks>
        public string GetDeviceId(User user)
        {
            throw new NotImplementedException();
        }
    }
}

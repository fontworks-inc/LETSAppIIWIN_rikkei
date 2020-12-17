using System;
using System.Collections.Generic;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;

namespace Infrastructure.API
{
    /// <summary>
    /// 端末情報を格納するAPIリポジトリのモック
    /// </summary>
    public class DevicesRepositoryMock : APIRepositoryBase, IDevicesRepository
    {
        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="apiConfiguration">設定情報</param>
        public DevicesRepositoryMock(APIConfiguration apiConfiguration)
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
            // 正常(２件)
            return this.GetAllDevicesTestData();

            //// 異常(２件未満)
            // return this.GetAllDevicesTestDataLessThan2();

            //// 異常(３件以上)
            // return this.GetAllDevicesTestDataMoreThan2();

            //// 異常(OS種類が不正)
            // return this.GetAllDevicesTestDataInvalidOSType();

            //// レスポンスコードが成功以外の場合
            // throw new InvalidResponseCodeException("message");

            //// 配信サーバアクセスエラー
            // throw new Exception("server access error.");
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
            //// レスポンスコードが成功以外の場合
            // throw new InvalidResponseCodeException("invalid response");

            //// 配信サーバアクセスエラー
            // throw new Exception("server access error.");
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
            // 正常
            return new AuthenticationInformation()
            {
                AccessToken = "access_token_01",
                RefreshToken = "refresh_token_01",
            };

            //// レスポンスコードが成功以外の場合
            // throw new InvalidResponseCodeException("message");

            //// 配信サーバアクセスエラー
            // throw new Exception("server access error.");
        }

        /// <summary>
        /// デバイスIDを発行する
        /// </summary>
        /// <param name="user">ユーザ情報</param>
        /// <returns>デバイスID</returns>
        /// <remarks>FUNCTION_08_01_12(デバイスID発行API)</remarks>
        public string GetDeviceId(User user)
        {
            // 正常
            return "device_id_test_001";

            //// レスポンスコードが成功以外の場合
            // throw new InvalidResponseCodeException("message");

            //// 配信サーバアクセスエラー
            // throw new Exception("server access error.");
        }

        /// <summary>
        /// 全端末情報テストデータ(正常)を取得
        /// </summary>
        private IList<Device> GetAllDevicesTestData()
        {
            var devices = new List<Device>();
            devices.Add(new Device()
            {
                UserDeviceId = "user_device_id_test",
                DeviceId = "device_id_test_a",
                HostName = "HomeUse",
                OSUserName = "taro",
                OSType = "Win",
                OSVersion = "10",
            });
            devices.Add(new Device()
            {
                UserDeviceId = "user_device_id_test",
                DeviceId = "device_id_test_b",
                HostName = "OfficeUse",
                OSUserName = "Afro Taro",
                OSType = "Mac",
                OSVersion = "XXXX",
            });
            return devices;
        }

        /// <summary>
        /// 全端末情報テストデータ(異常(２件未満))を取得
        /// </summary>
        private IList<Device> GetAllDevicesTestDataLessThan2()
        {
            var devices = new List<Device>();
            devices.Add(new Device()
            {
                UserDeviceId = "user_device_id_test",
                DeviceId = "device_id_test_a",
                HostName = "HomeUse",
                OSUserName = "taro",
                OSType = "Win",
                OSVersion = "10",
            });
            return devices;
        }

        /// <summary>
        /// 全端末情報テストデータ(異常(３件以上))を取得
        /// </summary>
        private IList<Device> GetAllDevicesTestDataMoreThan2()
        {
            var devices = new List<Device>();
            devices.Add(new Device()
            {
                UserDeviceId = "user_device_id_test",
                DeviceId = "device_id_test_a",
                HostName = "HomeUse",
                OSUserName = "taro",
                OSType = "Win",
                OSVersion = "10",
            });
            devices.Add(new Device()
            {
                UserDeviceId = "user_device_id_test",
                DeviceId = "device_id_test_b",
                HostName = "OfficeUse",
                OSUserName = "Afro Taro",
                OSType = "Win",
                OSVersion = "10",
            });
            devices.Add(new Device()
            {
                UserDeviceId = "user_device_id_test",
                DeviceId = "device_id_test_c",
                HostName = "HomeUse2",
                OSUserName = "taro3",
                OSType = "Mac",
                OSVersion = "XX.XX",
            });
            return devices;
        }

        /// <summary>
        /// 全端末情報テストデータ(異常(OS種類が不正))を取得
        /// </summary>
        private IList<Device> GetAllDevicesTestDataInvalidOSType()
        {
            var devices = new List<Device>();
            devices.Add(new Device()
            {
                UserDeviceId = "user_device_id_test",
                DeviceId = "device_id_test_a",
                HostName = "HomeUse",
                OSUserName = "taro",
                OSType = "Win",
                OSVersion = "10",
            });
            devices.Add(new Device()
            {
                UserDeviceId = "user_device_id_test",
                DeviceId = "device_id_test_d",
                HostName = "SmartPhone",
                OSUserName = "Afro Taro",
                OSType = "Android",
                OSVersion = "XX",
            });
            return devices;
        }
    }
}

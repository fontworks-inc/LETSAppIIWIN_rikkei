using System;
using System.Collections.Generic;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OS.Interfaces;

namespace ApplicationService.Authentication.Tests
{
    /// <summary>
    /// 認証サービスのテスト
    /// </summary>
    [TestClass]
    public class AuthenticationServiceTests
    {
        /// <summary>
        /// ログイン処理のテスト(正常：ユーザー別保存：デバイスIDが存在する場合)
        /// </summary>
        [TestMethod]
        public void LoginTest_SuccessGetUserStatusDeviceId()
        {
            // モックを作成
            var resourceWrapper = new Mock<IResourceWrapper>();
            var authenticationInformationRepository = new Mock<IAuthenticationInformationRepository>();
            var devicesRepository = new Mock<IDevicesRepository>();
            var userStatusRepository = new Mock<IUserStatusRepository>();
            var volatileSettingRepository = new Mock<IVolatileSettingRepository>();
            var service = new AuthenticationService(
                resourceWrapper.Object,
                authenticationInformationRepository.Object,
                devicesRepository.Object,
                userStatusRepository.Object,
                volatileSettingRepository.Object);

            // ユーザ別ステータス情報
            var userStatus = new UserStatus() { DeviceId = "device_id_test_001" };
            userStatusRepository.Setup(repos => repos.GetStatus()).Returns(userStatus);

            // 認証情報
            var excepted = new AuthenticationInformationResponse()
                        { Code = (int)ResponseCode.Succeeded, Data = new AuthenticationInformation("access_token_01", "refresh_token_01") };
            authenticationInformationRepository.Setup(repos => repos.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                                               .Returns(excepted);

            // ログイン実行
            var actual = service.Login("mailAddress", "password");

            Assert.AreEqual(excepted.Data.AccessToken, actual.Data.AccessToken);
            Assert.AreEqual(excepted.Data.RefreshToken, actual.Data.RefreshToken);
        }

        /// <summary>
        /// ログイン処理のテスト(正常：配信サービスからデバイスIDを発行した場合)
        /// </summary>
        [TestMethod]
        public void LoginTest_SuccessGetDeviceId()
        {
            // モックを作成
            var resourceWrapper = new Mock<IResourceWrapper>();
            var authenticationInformationRepository = new Mock<IAuthenticationInformationRepository>();
            var devicesRepository = new Mock<IDevicesRepository>();
            var userStatusRepository = new Mock<IUserStatusRepository>();
            var volatileSettingRepository = new Mock<IVolatileSettingRepository>();
            var service = new AuthenticationService(
                resourceWrapper.Object,
                authenticationInformationRepository.Object,
                devicesRepository.Object,
                userStatusRepository.Object,
                volatileSettingRepository.Object);

            // ユーザ別ステータス情報
            userStatusRepository.Setup(repos => repos.GetStatus()).Returns(new UserStatus());

            // 端末情報
            var deviceId = "device_id_test_001";
            devicesRepository.Setup(repos => repos.GetDeviceId(It.IsAny<User>()))
                             .Returns(deviceId);

            // 認証情報
            var excepted = new AuthenticationInformationResponse()
                        { Code = (int)ResponseCode.Succeeded, Data = new AuthenticationInformation("access_token_01", "refresh_token_01") };
            authenticationInformationRepository.Setup(repos => repos.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                                               .Returns(excepted);

            // ログイン実行
            var actual = service.Login("mailAddress", "password");

            Assert.AreEqual(excepted.Data.AccessToken, actual.Data.AccessToken);
            Assert.AreEqual(excepted.Data.RefreshToken, actual.Data.RefreshToken);
        }

        /// <summary>
        /// ログイン処理のテスト(異常：配信サービスアクセスエラー(デバイスID発行API実行時))
        /// </summary>
        [TestMethod]
        public void LoginTest_ErrorDevice()
        {
            // モックを作成
            var resourceWrapper = new Mock<IResourceWrapper>();
            var authenticationInformationRepository = new Mock<IAuthenticationInformationRepository>();
            var devicesRepository = new Mock<IDevicesRepository>();
            var userStatusRepository = new Mock<IUserStatusRepository>();
            var volatileSettingRepository = new Mock<IVolatileSettingRepository>();
            var service = new AuthenticationService(
                resourceWrapper.Object,
                authenticationInformationRepository.Object,
                devicesRepository.Object,
                userStatusRepository.Object,
                volatileSettingRepository.Object);

            // ユーザ別ステータス情報
            userStatusRepository.Setup(repos => repos.GetStatus()).Returns(new UserStatus());

            // 端末情報
            devicesRepository.Setup(repos => repos.GetDeviceId(It.IsAny<User>()))
                             .Throws(new Exception("server access error. - GetDeviceId"));

            // ログイン実行
            Assert.ThrowsException<Exception>(() => service.Login("mailAddress", "password"));
        }

        /// <summary>
        /// ログイン処理のテスト(２要素認証要求)
        /// </summary>
        [TestMethod]
        public void LoginTest_TwoFAIsRequired()
        {
            // モックを作成
            var resourceWrapper = new Mock<IResourceWrapper>();
            var authenticationInformationRepository = new Mock<IAuthenticationInformationRepository>();
            var devicesRepository = new Mock<IDevicesRepository>();
            var userStatusRepository = new Mock<IUserStatusRepository>();
            var volatileSettingRepository = new Mock<IVolatileSettingRepository>();
            var service = new AuthenticationService(
                resourceWrapper.Object,
                authenticationInformationRepository.Object,
                devicesRepository.Object,
                userStatusRepository.Object,
                volatileSettingRepository.Object);

            // ユーザ別ステータス情報
            userStatusRepository.Setup(repos => repos.GetStatus()).Returns(new UserStatus() { DeviceId = "device_id_test_001" });

            // 認証情報
            var excepted = new AuthenticationInformationResponse() { Code = (int)ResponseCode.TwoFAIsRequired };
            authenticationInformationRepository.Setup(repos => repos.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                                               .Returns(excepted);

            // ログイン実行
            var actual = service.Login("mailAddress", "password");

            Assert.AreEqual(ResponseCode.TwoFAIsRequired, actual.GetResponseCode());
        }

        /// <summary>
        /// ログイン処理のテスト(引数不正)
        /// </summary>
        [TestMethod]
        public void LoginTest_InvalidArgument()
        {
            // モックを作成
            var resourceWrapper = new Mock<IResourceWrapper>();
            var authenticationInformationRepository = new Mock<IAuthenticationInformationRepository>();
            var devicesRepository = new Mock<IDevicesRepository>();
            var userStatusRepository = new Mock<IUserStatusRepository>();
            var volatileSettingRepository = new Mock<IVolatileSettingRepository>();
            var service = new AuthenticationService(
                resourceWrapper.Object,
                authenticationInformationRepository.Object,
                devicesRepository.Object,
                userStatusRepository.Object,
                volatileSettingRepository.Object);

            // ユーザ別ステータス情報
            userStatusRepository.Setup(repos => repos.GetStatus()).Returns(new UserStatus() { DeviceId = "device_id_test_001" });

            // 認証情報
            var excepted = new AuthenticationInformationResponse() { Code = (int)ResponseCode.InvalidArgument };
            authenticationInformationRepository.Setup(repos => repos.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                                               .Returns(excepted);

            // ログイン実行
            var actual = service.Login("mailAddress", "password");

            Assert.AreEqual(ResponseCode.InvalidArgument, actual.GetResponseCode());
        }

        /// <summary>
        /// ログイン処理のテスト(認証エラー)
        /// </summary>
        [TestMethod]
        public void LoginTest_AuthenticationFailed()
        {
            // モックを作成
            var resourceWrapper = new Mock<IResourceWrapper>();
            var authenticationInformationRepository = new Mock<IAuthenticationInformationRepository>();
            var devicesRepository = new Mock<IDevicesRepository>();
            var userStatusRepository = new Mock<IUserStatusRepository>();
            var volatileSettingRepository = new Mock<IVolatileSettingRepository>();
            var service = new AuthenticationService(
                resourceWrapper.Object,
                authenticationInformationRepository.Object,
                devicesRepository.Object,
                userStatusRepository.Object,
                volatileSettingRepository.Object);

            // ユーザ別ステータス情報
            userStatusRepository.Setup(repos => repos.GetStatus()).Returns(new UserStatus() { DeviceId = "device_id_test_001" });

            // 認証情報
            var excepted = new AuthenticationInformationResponse() { Code = (int)ResponseCode.AuthenticationFailed };
            authenticationInformationRepository.Setup(repos => repos.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                                               .Returns(excepted);

            // ログイン実行
            var actual = service.Login("mailAddress", "password");

            Assert.AreEqual(ResponseCode.AuthenticationFailed, actual.GetResponseCode());
        }

        /// <summary>
        /// ログイン処理のテスト(２要素認証コード有効期限切れエラー)
        /// </summary>
        [TestMethod]
        public void LoginTest_TwoFACodeHasExpired()
        {
            // モックを作成
            var resourceWrapper = new Mock<IResourceWrapper>();
            var authenticationInformationRepository = new Mock<IAuthenticationInformationRepository>();
            var devicesRepository = new Mock<IDevicesRepository>();
            var userStatusRepository = new Mock<IUserStatusRepository>();
            var volatileSettingRepository = new Mock<IVolatileSettingRepository>();
            var service = new AuthenticationService(
                resourceWrapper.Object,
                authenticationInformationRepository.Object,
                devicesRepository.Object,
                userStatusRepository.Object,
                volatileSettingRepository.Object);

            // ユーザ別ステータス情報
            userStatusRepository.Setup(repos => repos.GetStatus()).Returns(new UserStatus() { DeviceId = "device_id_test_001" });

            // 認証情報
            var excepted = new AuthenticationInformationResponse() { Code = (int)ResponseCode.TwoFACodeHasExpired };
            authenticationInformationRepository.Setup(repos => repos.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                                               .Returns(excepted);

            // ログイン実行
            var actual = service.Login("mailAddress", "password");

            Assert.AreEqual(ResponseCode.TwoFACodeHasExpired, actual.GetResponseCode());
        }

        /// <summary>
        /// ログイン処理のテスト(同時使用デバイス数の上限エラー)
        /// </summary>
        [TestMethod]
        public void LoginTest_MaximumNumberOfDevicesInUse()
        {
            // モックを作成
            var resourceWrapper = new Mock<IResourceWrapper>();
            var authenticationInformationRepository = new Mock<IAuthenticationInformationRepository>();
            var devicesRepository = new Mock<IDevicesRepository>();
            var userStatusRepository = new Mock<IUserStatusRepository>();
            var volatileSettingRepository = new Mock<IVolatileSettingRepository>();
            var service = new AuthenticationService(
                resourceWrapper.Object,
                authenticationInformationRepository.Object,
                devicesRepository.Object,
                userStatusRepository.Object,
                volatileSettingRepository.Object);

            // ユーザ別ステータス情報
            userStatusRepository.Setup(repos => repos.GetStatus()).Returns(new UserStatus() { DeviceId = "device_id_test_001" });

            // 認証情報
            var excepted = new AuthenticationInformationResponse() { Code = (int)ResponseCode.MaximumNumberOfDevicesInUse };
            authenticationInformationRepository.Setup(repos => repos.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                                               .Returns(excepted);

            // ログイン実行
            var actual = service.Login("mailAddress", "password");

            Assert.AreEqual(ResponseCode.MaximumNumberOfDevicesInUse, actual.GetResponseCode());
        }

        /// <summary>
        /// ログイン処理のテスト(異常：処理対象外のレスポンスコード)
        /// </summary>
        [TestMethod]
        public void LoginTest_InvalidResponseCodeException()
        {
            // モックを作成
            var resourceWrapper = new Mock<IResourceWrapper>();
            var authenticationInformationRepository = new Mock<IAuthenticationInformationRepository>();
            var devicesRepository = new Mock<IDevicesRepository>();
            var userStatusRepository = new Mock<IUserStatusRepository>();
            var volatileSettingRepository = new Mock<IVolatileSettingRepository>();
            var service = new AuthenticationService(
                resourceWrapper.Object,
                authenticationInformationRepository.Object,
                devicesRepository.Object,
                userStatusRepository.Object,
                volatileSettingRepository.Object);

            // ユーザ別ステータス情報
            userStatusRepository.Setup(repos => repos.GetStatus()).Returns(new UserStatus() { DeviceId = "device_id_test_001" });

            // 認証情報
            var excepted = new AuthenticationInformationResponse() { Code = 9999 };
            authenticationInformationRepository.Setup(repos => repos.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                                               .Throws(new InvalidResponseCodeException());

            // ログイン実行
            Assert.ThrowsException<InvalidResponseCodeException>(() => service.Login("mailAddress", "password"));
        }

        /// <summary>
        /// ログイン処理のテスト(異常：配信サービスアクセスエラー(ログインAPI実行時))
        /// </summary>
        [TestMethod]
        public void LoginTest_ErrorLogin()
        {
            // モックを作成
            var resourceWrapper = new Mock<IResourceWrapper>();
            var authenticationInformationRepository = new Mock<IAuthenticationInformationRepository>();
            var devicesRepository = new Mock<IDevicesRepository>();
            var userStatusRepository = new Mock<IUserStatusRepository>();
            var volatileSettingRepository = new Mock<IVolatileSettingRepository>();
            var service = new AuthenticationService(
                resourceWrapper.Object,
                authenticationInformationRepository.Object,
                devicesRepository.Object,
                userStatusRepository.Object,
                volatileSettingRepository.Object);

            // ユーザ別ステータス情報
            userStatusRepository.Setup(repos => repos.GetStatus()).Returns(new UserStatus() { DeviceId = "device_id_test_001" });

            // 認証情報
            authenticationInformationRepository.Setup(repos => repos.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                                               .Throws(new Exception("server access error.- Login"));

            // ログイン実行
            Assert.ThrowsException<Exception>(() => service.Login("mailAddress", "password"));
        }

        /// <summary>
        /// ログイン完了処理のテスト
        /// </summary>
        [TestMethod]
        public void LoginCompletedTest_Success()
        {
            // モックを作成
            var resourceWrapper = new Mock<IResourceWrapper>();
            var authenticationInformationRepository = new Mock<IAuthenticationInformationRepository>();
            var devicesRepository = new Mock<IDevicesRepository>();
            var userStatusRepository = new Mock<IUserStatusRepository>();
            var volatileSettingRepository = new Mock<IVolatileSettingRepository>();
            var service = new AuthenticationService(
                resourceWrapper.Object,
                authenticationInformationRepository.Object,
                devicesRepository.Object,
                userStatusRepository.Object,
                volatileSettingRepository.Object);

            // ユーザ別ステータス情報
            var userStatus = new UserStatus()
            {
                DeviceId = "device_id_test_001",
                RefreshToken = string.Empty,
                IsLoggingIn = false,
            };
            userStatusRepository.Setup(repos => repos.GetStatus()).Returns(userStatus);

            // 更新後
            var updateUserStatus = userStatus;
            updateUserStatus.RefreshToken = "TokenR";
            updateUserStatus.IsLoggingIn = true;
            userStatusRepository.Setup(repos => repos.SaveStatus(updateUserStatus));

            // 認証情報
            var volatileSetting = new VolatileSetting()
            {
                AccessToken = string.Empty,
            };
            volatileSettingRepository.Setup(repos => repos.GetVolatileSetting())
                                               .Returns(volatileSetting);

            // ログイン完了処理実行
            var authenticationInformation = new AuthenticationInformation("TokenA", "TokenR");
            service.SaveLoginInfo(authenticationInformation);

            // 呼び出し確認
            userStatusRepository.Verify(x => x.GetStatus(), Times.AtLeastOnce());
            volatileSettingRepository.Verify(x => x.GetVolatileSetting(), Times.AtLeastOnce());

            // 保存確認
            userStatusRepository.Verify(x => x.SaveStatus(updateUserStatus), Times.AtLeastOnce());
        }

        /// <summary>
        /// ログアウト処理のテスト
        /// </summary>
        [TestMethod]
        public void LogoutTest_Success()
        {
            // モックを作成
            var resourceWrapper = new Mock<IResourceWrapper>();
            var authenticationInformationRepository = new Mock<IAuthenticationInformationRepository>();
            var userStatusRepository = new Mock<IUserStatusRepository>();
            var receiveNotificationRepository = new Mock<IReceiveNotificationRepository>();
            var volatileSettingRepository = new Mock<IVolatileSettingRepository>();
            var userFontsSettingRepository = new Mock<IUserFontsSettingRepository>();
            var fontActivationService = new Mock<IFontActivationService>();

            var service = new AuthenticationService(
                resourceWrapper.Object,
                authenticationInformationRepository.Object,
                userStatusRepository.Object,
                receiveNotificationRepository.Object,
                volatileSettingRepository.Object,
                userFontsSettingRepository.Object,
                fontActivationService.Object);

            // ユーザ別ステータス情報
            var userStatus = new UserStatus() { DeviceId = "device_id", IsLoggingIn = true };
            userStatusRepository.Setup(repos => repos.GetStatus()).Returns(userStatus);
            var logoutStates = new UserStatus() { DeviceId = "device_id", IsLoggingIn = false };
            userStatusRepository.Setup(repos => repos.SaveStatus(logoutStates));

            // メモリ保存情報
            var volatileSetting = new VolatileSetting() { AccessToken = "access_token_01" };
            volatileSettingRepository.Setup(repos => repos.GetVolatileSetting()).Returns(volatileSetting);

            // フォント情報
            var userFonts = new UserFontsSetting()
            {
                Fonts = new List<Font>()
                {
                    new Font("id_1", @"C:\S\font.ttf", true, true, "表示名", "1.1.1", "fontRegistry", new List<string>() { "contractIds1" }),
                },
            };
            userFontsSettingRepository.Setup(repos => repos.GetUserFontsSetting()).Returns(userFonts);

            var logoutFonts = new UserFontsSetting()
            {
                Fonts = new List<Font>()
                {
                    new Font("id_1", @"C:\S\font.ttf", true, false, "表示名", "1.1.1", "fontRegistry", new List<string>() { "contractIds1" }),
                },
            };
            userFontsSettingRepository.Setup(repos => repos.SaveUserFontsSetting(logoutFonts));

            // ディアクティベート
            fontActivationService.Setup(repos => repos.Deactivate(userFonts.Fonts[0]));

            // ログアウト実行
            var actual = service.Logout();

            Assert.IsTrue(actual);

            // 呼び出し確認
            userStatusRepository.Verify(x => x.GetStatus(), Times.AtLeastOnce());
            volatileSettingRepository.Verify(x => x.GetVolatileSetting(), Times.AtLeastOnce());
            fontActivationService.Verify(x => x.Deactivate(userFonts.Fonts[0]), Times.AtLeastOnce());
            authenticationInformationRepository.Verify(x => x.Logout(userStatus.DeviceId, volatileSetting.AccessToken), Times.AtLeastOnce());
            userStatusRepository.Verify(x => x.SaveStatus(It.IsAny<UserStatus>()), Times.AtLeastOnce());
        }

        /// <summary>
        /// ログアウト処理のテスト(異常：ログイン用コンストラクタ)
        /// </summary>
        [TestMethod]
        public void LogoutTest_InvalidOperationError()
        {
            // モックを作成
            var resourceWrapper = new Mock<IResourceWrapper>();
            var authenticationInformationRepository = new Mock<IAuthenticationInformationRepository>();
            var devicesRepository = new Mock<IDevicesRepository>();
            var userStatusRepository = new Mock<IUserStatusRepository>();
            var volatileSettingRepository = new Mock<IVolatileSettingRepository>();
            var service = new AuthenticationService(
                resourceWrapper.Object,
                authenticationInformationRepository.Object,
                devicesRepository.Object,
                userStatusRepository.Object,
                volatileSettingRepository.Object);

            // ログアウト実行
            Assert.ThrowsException<InvalidOperationException>(() => service.Logout());
        }

        /// <summary>
        /// ログアウト処理のテスト(認証情報リポジトリでエラー発生）
        /// </summary>
        [TestMethod]
        public void LogoutTest_Error()
        {
            // モックを作成
            var resourceWrapper = new Mock<IResourceWrapper>();
            var authenticationInformationRepository = new Mock<IAuthenticationInformationRepository>();
            var userStatusRepository = new Mock<IUserStatusRepository>();
            var receiveNotificationRepository = new Mock<IReceiveNotificationRepository>();
            var volatileSettingRepository = new Mock<IVolatileSettingRepository>();
            var userFontsSettingRepository = new Mock<IUserFontsSettingRepository>();
            var fontActivationService = new Mock<IFontActivationService>();

            var service = new AuthenticationService(
                resourceWrapper.Object,
                authenticationInformationRepository.Object,
                userStatusRepository.Object,
                receiveNotificationRepository.Object,
                volatileSettingRepository.Object,
                userFontsSettingRepository.Object,
                fontActivationService.Object);

            // 認証情報
            authenticationInformationRepository.Setup(
                repos => repos.Logout(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());

            // ユーザ別ステータス情報
            var userStatus = new UserStatus() { DeviceId = "device_id", IsLoggingIn = true };
            userStatusRepository.Setup(repos => repos.GetStatus()).Returns(userStatus);
            var logoutStates = new UserStatus() { DeviceId = "device_id", IsLoggingIn = false };
            userStatusRepository.Setup(repos => repos.SaveStatus(logoutStates));

            // メモリ保存情報
            var volatileSetting = new VolatileSetting() { AccessToken = "access_token_01" };
            volatileSettingRepository.Setup(repos => repos.GetVolatileSetting()).Returns(volatileSetting);

            // フォント情報
            var userFonts = new UserFontsSetting()
            {
                Fonts = new List<Font>()
                {
                    new Font("id_1", @"C:\S\font.ttf", true, true, "表示名", "1.1.1", "fontRegistry", new List<string>() { "contractIds1" }),
                },
            };
            userFontsSettingRepository.Setup(repos => repos.GetUserFontsSetting()).Returns(userFonts);

            var logoutFonts = new UserFontsSetting()
            {
                Fonts = new List<Font>()
                {
                    new Font("id_1", @"C:\S\font.ttf", true, false, "表示名", "1.1.1", "fontRegistry", new List<string>() { "contractIds1" }),
                },
            };
            userFontsSettingRepository.Setup(repos => repos.SaveUserFontsSetting(logoutFonts));

            // ディアクティベート
            fontActivationService.Setup(repos => repos.Deactivate(userFonts.Fonts[0]));

            // ログアウト実行
            var actual = service.Logout();

            Assert.IsFalse(actual);

            // 下記の2つは実行される
            userStatusRepository.Verify(x => x.GetStatus(), Times.AtLeastOnce());
            volatileSettingRepository.Verify(x => x.GetVolatileSetting(), Times.AtLeastOnce());

            userStatusRepository.Verify(x => x.SaveStatus(logoutStates), Times.Never());
        }
    }
}
using System;
using System.Collections.Generic;
using ApplicationService.Interfaces;
using Core.Entities;
using Core.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ApplicationService.Startup.Tests
{
    /// <summary>
    /// 起動時処理に関するサービスクラス
    /// </summary>
    [TestClass]
    public class StartupServiceTests
    {
        /// <summary>
        /// ログイン状態確認処理のテスト 自デバイスの情報が含まれている場合
        /// </summary>
        [TestMethod]
        public void ConfirmLoginStatusTest_Successful()
        {
            // モックの作成
            var resourceWrapper = new Mock<IResourceWrapper>();
            var volatileSettingRepository = new Mock<IVolatileSettingRepository>();
            volatileSettingRepository.Setup(repos => repos.GetVolatileSetting())
                 .Returns(new VolatileSetting() { AccessToken = "1234567890" });
            var devicesRepository = new Mock<IDevicesRepository>();
            devicesRepository.Setup(repos => repos.GetAllDevices(It.IsAny<string>(), It.IsAny<string>()))
                 .Returns(new List<Device>() { new Device() { DeviceId = "1234aaadwdewd" }, new Device() { DeviceId = "123456789" } });

            bool calledNotContainsDeviceEvent = false;

            // 自デバイスの情報が含まれている場合、trueを返す
            StartupService service = new StartupService(
                resourceWrapper.Object,
                volatileSettingRepository.Object,
                devicesRepository.Object);
            Assert.IsTrue(service.ConfirmLoginStatus("1234aaadwdewd", () =>
            {
                calledNotContainsDeviceEvent = true;
            }));

            // 自デバイスの情報が含まれてる場合、イベントが実行されないことを確認する
            Assert.IsFalse(calledNotContainsDeviceEvent);
        }

        /// <summary>
        /// ログイン状態確認処理のテスト 自デバイスの情報が含まれていない場合
        /// </summary>
        [TestMethod]
        public void ConfirmLoginStatusTest_Failed()
        {
            // モックの作成
            var resourceWrapper = new Mock<IResourceWrapper>();
            var volatileSettingRepository = new Mock<IVolatileSettingRepository>();
            volatileSettingRepository.Setup(repos => repos.GetVolatileSetting())
                 .Returns(new VolatileSetting() { AccessToken = "1234567890" });
            var devicesRepository = new Mock<IDevicesRepository>();
            devicesRepository.Setup(repos => repos.GetAllDevices(It.IsAny<string>(), It.IsAny<string>()))
                 .Returns(new List<Device>() { new Device() { DeviceId = "NoData" }, new Device() { DeviceId = "123456789" } });

            bool calledNotContainsDeviceEvent = false;

            // 自デバイスの情報が含まれていない場合、falseを返す
            StartupService service = new StartupService(
                resourceWrapper.Object,
                volatileSettingRepository.Object,
                devicesRepository.Object);
            Assert.IsFalse(service.ConfirmLoginStatus("1234aaadwdewd", () =>
            {
                calledNotContainsDeviceEvent = true;
            }));

            // 自デバイスの情報が含まれていない場合、イベントが実行されることを確認する
            Assert.IsTrue(calledNotContainsDeviceEvent);
        }

        /// <summary>
        /// ログイン状態確認処理のテスト 端末情報取得処理で例外発生時
        /// </summary>
        [TestMethod]
        public void ConfirmLoginStatusTest_Exception()
        {
            // モックの作成
            var resourceWrapper = new Mock<IResourceWrapper>();
            resourceWrapper.Setup(repos => repos.GetString(It.IsAny<string>()))
                 .Returns("ログイン状態確認時の全端末情報取得処理で例外が発生しました");

            var volatileSettingRepository = new Mock<IVolatileSettingRepository>();
            volatileSettingRepository.Setup(repos => repos.GetVolatileSetting())
                 .Returns(new VolatileSetting() { AccessToken = "1234567890" });
            var devicesRepository = new Mock<IDevicesRepository>();
            devicesRepository.Setup(repos => repos.GetAllDevices(It.IsAny<string>(), It.IsAny<string>()))
                 .Returns(() =>
                 {
                     throw new Exception();
                 });

            bool calledNotContainsDeviceEvent = false;

            // 自デバイスの情報が含まれていない場合、falseを返す
            StartupService service = new StartupService(
                resourceWrapper.Object,
                volatileSettingRepository.Object,
                devicesRepository.Object);
            Assert.IsFalse(service.ConfirmLoginStatus("1234aaadwdewd", () =>
            {
                calledNotContainsDeviceEvent = true;
            }));

            // 端末情報取得処理で例外発生時は、イベントが実行されないことを確認する
            Assert.IsFalse(calledNotContainsDeviceEvent);
        }

        /// <summary>
        /// 起動時チェック処理のテスト　正常系
        /// </summary>
        [TestMethod]
        public void IsCheckedStartupTest_Successful()
        {
            // モックの作成
            var resourceWrapper = new Mock<IResourceWrapper>();
            var fontManagerService = new Mock<IFontManagerService>();
            var contractsAggregateRepository = new Mock<IContractsAggregateRepository>();

            // 契約情報の戻り値設定
            var contractsAggregate = new ContractsAggregate()
            {
                NeedContractRenewal = false,
                Contracts = new List<Contract>
                {
                    new Contract("AAA1234", DateTime.Parse("2021/12/08 15:38")),
                    new Contract("ABC999", DateTime.Parse("2022/12/08 15:38")),
                },
            };
            contractsAggregateRepository.Setup(repos => repos.GetContractsAggregate(It.IsAny<string>(), It.IsAny<string>())).Returns(contractsAggregate);

            var contractsAggregateCacheRepository = new Mock<IContractsAggregateRepository>();
            var volatileSettingRepository = new Mock<IVolatileSettingRepository>();
            volatileSettingRepository.Setup(repos => repos.GetVolatileSetting())
                 .Returns(new VolatileSetting() { AccessToken = "1234567890" });
            var devicesRepository = new Mock<IDevicesRepository>();
            devicesRepository.Setup(repos => repos.GetAllDevices(It.IsAny<string>(), It.IsAny<string>()))
                 .Returns(new List<Device>() { new Device() { DeviceId = "1234aaadwdewd" }, new Device() { DeviceId = "123456789" } });

            StartupService service = new StartupService(
                resourceWrapper.Object,
                fontManagerService.Object,
                volatileSettingRepository.Object,
                contractsAggregateRepository.Object,
                contractsAggregateCacheRepository.Object,
                devicesRepository.Object);

            Assert.IsTrue(service.IsCheckedStartup("123456789", () => { }, () => { }));
            fontManagerService.Verify(repos => repos.Synchronize(true), Times.Once);
        }

        /// <summary>
        /// 起動時チェック処理のテスト　ログアウト中扱い
        /// </summary>
        [TestMethod]
        public void IsCheckedStartupTest_InLogout()
        {
            // モックの作成
            var resourceWrapper = new Mock<IResourceWrapper>();
            var fontManagerService = new Mock<IFontManagerService>();
            var contractsAggregateRepository = new Mock<IContractsAggregateRepository>();

            // 契約情報の戻り値設定
            var contractsAggregate = new ContractsAggregate()
            {
                NeedContractRenewal = false,
                Contracts = new List<Contract>
                {
                    new Contract("AAA1234", DateTime.Parse("2021/12/08 15:38")),
                    new Contract("ABC999", DateTime.Parse("2022/12/08 15:38")),
                },
            };
            contractsAggregateRepository.Setup(repos => repos.GetContractsAggregate(It.IsAny<string>(), It.IsAny<string>())).Returns(contractsAggregate);

            var contractsAggregateCacheRepository = new Mock<IContractsAggregateRepository>();
            var volatileSettingRepository = new Mock<IVolatileSettingRepository>();
            volatileSettingRepository.Setup(repos => repos.GetVolatileSetting())
                 .Returns(new VolatileSetting() { AccessToken = "1234567890" });
            var devicesRepository = new Mock<IDevicesRepository>();
            devicesRepository.Setup(repos => repos.GetAllDevices(It.IsAny<string>(), It.IsAny<string>()))
                 .Returns(() =>
                 {
                     throw new Exception();
                 });

            StartupService service = new StartupService(
                resourceWrapper.Object,
                fontManagerService.Object,
                volatileSettingRepository.Object,
                contractsAggregateRepository.Object,
                contractsAggregateCacheRepository.Object,
                devicesRepository.Object);

            Assert.IsFalse(service.IsCheckedStartup("123456789", () => { }, () => { }));
            fontManagerService.Verify(repos => repos.Synchronize(true), Times.Never);
        }

        /// <summary>
        /// 起動時チェック処理のテスト　キャッシュ利用時
        /// </summary>
        [TestMethod]
        public void IsCheckedStartupTest_UsedCache()
        {
            // モックの作成
            var resourceWrapper = new Mock<IResourceWrapper>();
            var fontManagerService = new Mock<IFontManagerService>();
            var contractsAggregateRepository = new Mock<IContractsAggregateRepository>();
            var contractsAggregateCacheRepository = new Mock<IContractsAggregateRepository>();

            // 契約情報の戻り値設定
            var contractsAggregate = new ContractsAggregate()
            {
                NeedContractRenewal = true,
                Contracts = new List<Contract>
                {
                    new Contract("AAA1234", DateTime.Parse("2021/12/08 15:38")),
                    new Contract("ABC999", DateTime.Parse("2022/12/08 15:38")),
                },
            };
            contractsAggregateRepository.Setup(repos => repos.GetContractsAggregate(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(() =>
                {
                    throw new Exception();
                });

            contractsAggregateCacheRepository.Setup(repos => repos.GetContractsAggregate()).Returns(contractsAggregate);

            var volatileSettingRepository = new Mock<IVolatileSettingRepository>();
            volatileSettingRepository.Setup(repos => repos.GetVolatileSetting())
                 .Returns(new VolatileSetting() { AccessToken = "1234567890" });
            var devicesRepository = new Mock<IDevicesRepository>();
            devicesRepository.Setup(repos => repos.GetAllDevices(It.IsAny<string>(), It.IsAny<string>()))
                 .Returns(new List<Device>() { new Device() { DeviceId = "1234aaadwdewd" }, new Device() { DeviceId = "123456789" } });

            StartupService service = new StartupService(
                resourceWrapper.Object,
                fontManagerService.Object,
                volatileSettingRepository.Object,
                contractsAggregateRepository.Object,
                contractsAggregateCacheRepository.Object,
                devicesRepository.Object);

            // ライセンス更新が必要な時に注入したイベントが実行されることを確認する
            Assert.IsTrue(service.IsCheckedStartup("123456789", () => { }, () => { }));
            fontManagerService.Verify(repos => repos.Synchronize(true), Times.Once);
        }

        /// <summary>
        /// 起動時チェック処理のテスト　ライセンス更新
        /// </summary>
        [TestMethod]
        public void IsCheckedStartupTest_UpdateLicense()
        {
            // モックの作成
            var resourceWrapper = new Mock<IResourceWrapper>();
            var fontManagerService = new Mock<IFontManagerService>();
            var contractsAggregateRepository = new Mock<IContractsAggregateRepository>();

            // 契約情報の戻り値設定
            var contractsAggregate = new ContractsAggregate()
            {
                NeedContractRenewal = true,
                Contracts = new List<Contract>
                {
                    new Contract("AAA1234", DateTime.Parse("2021/12/08 15:38")),
                    new Contract("ABC999", DateTime.Parse("2022/12/08 15:38")),
                },
            };
            contractsAggregateRepository.Setup(repos => repos.GetContractsAggregate(It.IsAny<string>(), It.IsAny<string>())).Returns(contractsAggregate);

            var contractsAggregateCacheRepository = new Mock<IContractsAggregateRepository>();
            var volatileSettingRepository = new Mock<IVolatileSettingRepository>();
            volatileSettingRepository.Setup(repos => repos.GetVolatileSetting())
                 .Returns(new VolatileSetting() { AccessToken = "1234567890" });
            var devicesRepository = new Mock<IDevicesRepository>();
            devicesRepository.Setup(repos => repos.GetAllDevices(It.IsAny<string>(), It.IsAny<string>()))
                 .Returns(new List<Device>() { new Device() { DeviceId = "1234aaadwdewd" }, new Device() { DeviceId = "123456789" } });

            StartupService service = new StartupService(
                resourceWrapper.Object,
                fontManagerService.Object,
                volatileSettingRepository.Object,
                contractsAggregateRepository.Object,
                contractsAggregateCacheRepository.Object,
                devicesRepository.Object);

            // ライセンス更新が必要な時に注入したイベントが実行されることを確認する
            bool calledContractUpdateRequiredEvent = false;
            Assert.IsTrue(service.IsCheckedStartup("123456789", () => { calledContractUpdateRequiredEvent = true; }, () => { }));
            Assert.IsTrue(calledContractUpdateRequiredEvent);
            fontManagerService.Verify(repos => repos.Synchronize(true), Times.Once);
        }

        /// <summary>
        /// 起動時チェック処理　利用しているコンストラクタが不正
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void IsCheckedStartupTest_InvalidConstructor()
        {
            // モックの作成
            var resourceWrapper = new Mock<IResourceWrapper>();
            var volatileSettingRepository = new Mock<IVolatileSettingRepository>();
            volatileSettingRepository.Setup(repos => repos.GetVolatileSetting())
                 .Returns(new VolatileSetting() { AccessToken = "1234567890" });
            var devicesRepository = new Mock<IDevicesRepository>();
            devicesRepository.Setup(repos => repos.GetAllDevices(It.IsAny<string>(), It.IsAny<string>()))
                 .Returns(new List<Device>() { new Device() { DeviceId = "1234aaadwdewd" }, new Device() { DeviceId = "123456789" } });

            StartupService service = new StartupService(
                resourceWrapper.Object,
                volatileSettingRepository.Object,
                devicesRepository.Object);

            service.IsCheckedStartup("123456789", () => { }, () => { });
        }
    }
}
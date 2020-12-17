using System.IO;
using System.Text.Json;
using Core.Entities;
using InfrastructureTests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infrastructure.File.Tests
{
    /// <summary>
    /// アプリケーション設定情報を格納するファイルリポジトリのテスト
    /// </summary>
    [TestClass]
    public class ApplicationSettingFileRepositoryTests
    {
        /// <summary>
        /// テストデータを格納するディレクトリのパス
        /// </summary>
        private static readonly string DataSettingFilePath = TestDataService.GetTestDataDirectory("ApplicationSetting");

        /// <summary>
        /// アプリケーション設定情報取得処理のテスト
        /// </summary>
        [TestMethod]
        public void GetSettingTest()
        {
            var repository = new ApplicationSettingFileRepository(Path.Combine(DataSettingFilePath, "appsettings.test.json"));
            ApplicationSetting setting = repository.GetSetting();

            Assert.AreEqual("http://api/v1", setting.FontDeliveryServerUri);
            Assert.AreEqual("http://api/v2", setting.NotificationServerUri);
            Assert.AreEqual(15, setting.CommunicationRetryCount);
            Assert.AreEqual(300, setting.FixedTermConfirmationInterval);
            Assert.AreEqual(5, setting.FontCalculationFactor);
        }

        /// <summary>
        /// アプリケーション設定情報のJSON形式が正しくない場合のテスト
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(JsonException))]
        public void GetSettingTest_Failed()
        {
            var repository = new ApplicationSettingFileRepository(Path.Combine(DataSettingFilePath, "appsettings.failed.json"));
            ApplicationSetting setting = repository.GetSetting();
        }

        /// <summary>
        /// アプリケーション設定情報のファイルが見つからない場合のテスト
        /// </summary>
        [TestMethod]
        public void GetSettingTest_NotFound()
        {
            var repository = new ApplicationSettingFileRepository(Path.Combine(DataSettingFilePath, "NotFound"));
            ApplicationSetting setting = repository.GetSetting();

            // プログラム内に保持する固定値が設定されることを確認する
            Assert.AreEqual("https://158.101.75.134", setting.FontDeliveryServerUri);
            Assert.AreEqual("https://158.101.75.134", setting.NotificationServerUri);
            Assert.AreEqual(10, setting.CommunicationRetryCount);
            Assert.AreEqual(1800, setting.FixedTermConfirmationInterval);
            Assert.AreEqual(3, setting.FontCalculationFactor);
        }
    }
}
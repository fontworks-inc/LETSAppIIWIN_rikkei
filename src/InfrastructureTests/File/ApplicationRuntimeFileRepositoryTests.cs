using System.IO;
using System.Text.Json;
using Core.Entities;
using InfrastructureTests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infrastructure.File.Tests
{
    /// <summary>
    /// 共通保存情報を格納するファイルリポジトリのテスト
    /// </summary>
    [TestClass]
    public class ApplicationRuntimeFileRepositoryTests
    {
        /// <summary>
        /// テストデータを格納するディレクトリのパス
        /// </summary>
        private static readonly string DataSettingFilePath = TestDataService.GetTestDataDirectory("ApplicationRuntime");

        /// <summary>
        /// 共通保存情報保存・取得処理のテスト
        /// </summary>
        [TestMethod]
        public void SaveApplicationRuntimeTestAndGetApplicationRuntimeTest()
        {
            // 任意の値を設定しファイルに保存する
            var repository = new ApplicationRuntimeFileRepository("appruntime.json");
            ApplicationRuntime expected = new ApplicationRuntime();
            expected.NextVersionInstaller = new Installer(DownloadStatus.Running, "1.0.0", true);
            repository.SaveApplicationRuntime(expected);

            // ファイルから値を取得する
            ApplicationRuntime actual = repository.GetApplicationRuntime();

            // 設定した値と取得した値が一致することを確認する
            Assert.AreEqual(expected.NextVersionInstaller.ApplicationUpdateType, actual.NextVersionInstaller.ApplicationUpdateType);
            Assert.AreEqual(expected.NextVersionInstaller.DownloadStatus, actual.NextVersionInstaller.DownloadStatus);
            Assert.AreEqual(expected.NextVersionInstaller.Version, actual.NextVersionInstaller.Version);
        }

        /// <summary>
        /// 共通保存情報のJSON形式が正しくない場合のテスト
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(JsonException))]
        public void GetApplicationRuntimeTest_Failed()
        {
            var repository = new ApplicationRuntimeFileRepository(Path.Combine(DataSettingFilePath, "appruntime.failed.json"));
            ApplicationRuntime runtime = repository.GetApplicationRuntime();
        }

        /// <summary>
        /// 共通保存情報のファイルが見つからない場合のテスト
        /// </summary>
        [TestMethod]
        public void GetApplicationRuntimeTest_NotFound()
        {
            var repository = new ApplicationRuntimeFileRepository(Path.Combine(DataSettingFilePath, "NotFound"));
            ApplicationRuntime actual = repository.GetApplicationRuntime();

            // プロパティを設定していないインスタンスが返ってくることを確認する
            Assert.IsNull(actual.NextVersionInstaller);
        }
    }
}
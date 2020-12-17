using System;
using System.IO;
using System.Text.RegularExpressions;
using Core.Entities;
using InfrastructureTests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infrastructure.File.Tests
{
    /// <summary>
    /// ユーザー別保存情報を格納するファイルリポジトリのテスト
    /// </summary>
    [TestClass]
    public class UserStatusFileRepositoryTests
    {
        /// <summary>
        /// テストデータを格納するディレクトリのパス
        /// </summary>
        private static readonly string DataSettingFilePath = TestDataService.GetTestDataDirectory("UserStatus");

        /// <summary>
        /// ユーザー別保存情報の保存・取得処理のテスト
        /// </summary>
        [TestMethod]
        public void SaveStatusTestAndGetStatusTest()
        {
            // 任意の値を設定しファイルに保存する
            var repository = new UserStatusFileRepository("status.dat");
            UserStatus expected = new UserStatus()
            {
                DeviceId = "aaaa123456",
                IsLoggingIn = true,
                LastEventId = 123555555,
                RefreshToken = "rgestrgij58544",
            };
            repository.SaveStatus(expected);

            // base64で暗号化されていることの確認
            string text = System.IO.File.ReadAllText("status.dat");
            text.Trim();
            Assert.IsTrue((text.Length % 4 == 0) && Regex.IsMatch(text, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None));

            // 復号化され、保存し値が読み取れることの確認
            UserStatus actual = repository.GetStatus();
            Assert.AreEqual(expected.DeviceId, actual.DeviceId);
            Assert.AreEqual(expected.IsLoggingIn, actual.IsLoggingIn);
            Assert.AreEqual(expected.LastEventId, actual.LastEventId);
            Assert.AreEqual(expected.RefreshToken, actual.RefreshToken);
        }

        /// <summary>
        /// ユーザー別保存情報が暗号化されていない場合のテスト
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void GetStatusTest_Failed()
        {
            var repository = new UserStatusFileRepository(Path.Combine(DataSettingFilePath, "status.test.failed.dat"));
            UserStatus entity = repository.GetStatus();
        }

        /// <summary>
        /// ユーザー別保存情報のファイルが見つからない場合のテスト
        /// </summary>
        [TestMethod]
        public void GetStatusTest_NotFound()
        {
            var repository = new UserStatusFileRepository(Path.Combine(DataSettingFilePath, "NotFound"));
            UserStatus actual = repository.GetStatus();

            // プロパティを設定していないインスタンスが返ってくることを確認する
            var expected = new UserStatus();
            Assert.AreEqual(expected.DeviceId, actual.DeviceId);
            Assert.AreEqual(expected.IsLoggingIn, actual.IsLoggingIn);
            Assert.AreEqual(expected.RefreshToken, actual.RefreshToken);
            Assert.AreEqual(expected.LastEventId, actual.LastEventId);
        }
    }
}
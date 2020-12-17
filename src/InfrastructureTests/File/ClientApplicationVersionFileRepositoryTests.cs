using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Entities;
using InfrastructureTests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infrastructure.File.Tests
{
    /// <summary>
    /// クライアントアプリの起動Ver情報を扱うリポジトリのテスト
    /// </summary>
    [TestClass]
    public class ClientApplicationVersionFileRepositoryTests
    {
        /// <summary>
        /// テストデータを格納するディレクトリのパス
        /// </summary>
        private static readonly string DataSettingFilePath = TestDataService.GetTestDataDirectory("ClientApplicationVersion");

        /// <summary>
        /// クライアントアプリの起動Ver情報の保存・取得処理のテスト
        /// </summary>
        [TestMethod]
        public void SaveClientApplicationVersionAndGetClientApplicationVersionTest()
        {
            // 任意の値を設定しファイルに保存する
            var repository = new ClientApplicationVersionFileRepository("FUNCTION_08_05_02.dat");
            ClientApplicationVersion expected = new ClientApplicationVersion("1234AAAA", "1.0.2", "http://aaaa.co.jp");
            repository.SaveClientApplicationVersion(expected);

            // base64で暗号化されていることの確認
            string text = System.IO.File.ReadAllText("FUNCTION_08_05_02.dat");
            text.Trim();
            Assert.IsTrue((text.Length % 4 == 0) && Regex.IsMatch(text, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None));

            // 復号化され、保存し値が読み取れることの確認
            ClientApplicationVersion actual = repository.GetClientApplicationVersion();
            Assert.AreEqual(expected.AppId, actual.AppId);
            Assert.AreEqual(expected.Version, actual.Version);
            Assert.AreEqual(expected.Url, actual.Url);
        }

        /// <summary>
        /// ファイルが暗号化されていない場合のテスト
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void GetClientApplicationVersionTest_Failed()
        {
            var repository = new ClientApplicationVersionFileRepository(Path.Combine(DataSettingFilePath, "FUNCTION_08_05_02.failed.dat"));
            ClientApplicationVersion entity = repository.GetClientApplicationVersion();
        }

        /// <summary>
        /// ファイルが見つからない場合のテスト
        /// </summary>
        [TestMethod]
        public void GetClientApplicationVersionTest_NotFound()
        {
            var repository = new ClientApplicationVersionFileRepository(Path.Combine(DataSettingFilePath, "NotFound"));
            ClientApplicationVersion actual = repository.GetClientApplicationVersion();

            // プロパティを設定していないインスタンスが返ってくることを確認する
            var expected = new ClientApplicationVersion();
            Assert.AreEqual(expected.AppId, actual.AppId);
            Assert.AreEqual(expected.Version, actual.Version);
            Assert.AreEqual(expected.Url, actual.Url);
        }
    }
}
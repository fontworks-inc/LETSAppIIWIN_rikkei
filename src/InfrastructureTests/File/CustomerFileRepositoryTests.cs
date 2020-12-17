using System;
using System.IO;
using System.Text.RegularExpressions;
using Core.Entities;
using InfrastructureTests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infrastructure.File.Tests
{
    /// <summary>
    /// お客様情報を格納するファイルリポジトリのテスト
    /// </summary>
    [TestClass]
    public class CustomerFileRepositoryTests
    {
        /// <summary>
        /// テストデータを格納するディレクトリのパス
        /// </summary>
        private static readonly string DataSettingFilePath = TestDataService.GetTestDataDirectory("Customer");

        /// <summary>
        /// お客様情報の保存・取得処理のテスト
        /// </summary>
        [TestMethod]
        public void SaveCustomerAndGetCustomerTest()
        {
            // 任意の値を設定しファイルに保存する
            var repository = new CustomerFileRepository("FUNCTION_08_02_01.dat");
            Customer expected = new Customer("aaabcd12344@aaa.co.jp", "ああああ", "いいいい");
            repository.SaveCustomer(expected);

            // base64で暗号化されていることの確認
            string text = System.IO.File.ReadAllText("FUNCTION_08_02_01.dat");
            text.Trim();
            Assert.IsTrue((text.Length % 4 == 0) && Regex.IsMatch(text, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None));

            // 復号化され、保存し値が読み取れることの確認
            Customer actual = repository.GetCustomer();
            Assert.AreEqual(expected.MailAddress, actual.MailAddress);
            Assert.AreEqual(expected.LastName, actual.LastName);
            Assert.AreEqual(expected.FirstName, actual.FirstName);
        }

        /// <summary>
        /// お客様情報が暗号化されていない場合のテスト
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void GetCustomerTest_Failed()
        {
            var repository = new CustomerFileRepository(Path.Combine(DataSettingFilePath, "FUNCTION_08_02_01.failed.dat"));
            Customer entity = repository.GetCustomer();
        }

        /// <summary>
        /// お客様情報のファイルが見つからない場合のテスト
        /// </summary>
        [TestMethod]
        public void GetCustomerTest_NotFound()
        {
            var repository = new CustomerFileRepository(Path.Combine(DataSettingFilePath, "NotFound"));
            Customer actual = repository.GetCustomer();

            // プロパティを設定していないインスタンスが返ってくることを確認する
            var expected = new Customer();
            Assert.AreEqual(expected.MailAddress, actual.MailAddress);
            Assert.AreEqual(expected.LastName, actual.LastName);
            Assert.AreEqual(expected.FirstName, actual.FirstName);
        }

        /// <summary>
        /// ファイル削除のテスト
        /// </summary>
        [TestMethod]
        public void DeleteTest()
        {
            string target = Path.Combine(DataSettingFilePath, "FUNCTION_08_02_01.remove.dat");
            var repository = new CustomerFileRepository(target);

            // 削除実行する前はファイルがあることを確認する
            Assert.IsTrue(System.IO.File.Exists(target));

            // 削除実行
            repository.Delete();

            // 削除実行後はファイルがなくなっていることを確認する
            Assert.IsFalse(System.IO.File.Exists(target));
        }
    }
}
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
    /// 契約情報取得処理のレスポンスを扱うリポジトリのテスト
    /// </summary>
    [TestClass]
    public class ContractsAggregateFileRepositoryTests
    {
        /// <summary>
        /// テストデータを格納するディレクトリのパス
        /// </summary>
        private static readonly string DataSettingFilePath = TestDataService.GetTestDataDirectory("ContractsAggregate");

        /// <summary>
        /// 契約情報の集合体の保存・取得処理のテスト
        /// </summary>
        [TestMethod]
        public void SaveContractsAggregateAndGetContractsAggregateTest()
        {
            // 任意の値を設定しファイルに保存する
            var repository = new ContractsAggregateFileRepository("FUNCTION_08_03_02.dat");
            ContractsAggregate expected = new ContractsAggregate(true, new List<Contract>()
                {
                    new Contract("AAA1234", DateTime.Parse("2020/12/08 15:38")),
                    new Contract("ABC999", DateTime.Parse("2022/12/08 15:38")),
                });

            repository.SaveContractsAggregate(expected);

            // base64で暗号化されていることの確認
            string text = System.IO.File.ReadAllText("FUNCTION_08_03_02.dat");
            text.Trim();
            Assert.IsTrue((text.Length % 4 == 0) && Regex.IsMatch(text, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None));

            // 復号化され、保存し値が読み取れることの確認
            ContractsAggregate actual = repository.GetContractsAggregate();
            Assert.AreEqual(expected.NeedContractRenewal, actual.NeedContractRenewal);

            // 設定した値と取得した値が一致することを確認する
            Assert.IsTrue(actual.Contracts.Any());
            for (int i = 0; i < actual.Contracts.Count; i++)
            {
                Assert.IsTrue(expected.Contracts[i].Equals(actual.Contracts[i]));
            }
        }

        /// <summary>
        /// ファイルが暗号化されていない場合のテスト
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void GetContractsAggregateTest_Failed()
        {
            var repository = new ContractsAggregateFileRepository(Path.Combine(DataSettingFilePath, "FUNCTION_08_03_02.failed.dat"));
            ContractsAggregate entity = repository.GetContractsAggregate();
        }

        /// <summary>
        /// クラスで実装しないインターフェースのメソッドが呼ばれた場合のテスト
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(NotImplementedException))]
        public void GetContractsAggregateTest_NotImplementedException()
        {
            var repository = new ContractsAggregateFileRepository(Path.Combine(DataSettingFilePath, "FUNCTION_08_03_02.dat"));
            ContractsAggregate entity = repository.GetContractsAggregate("deviceId", "accessToken");
        }

        /// <summary>
        /// ファイルが見つからない場合のテスト
        /// </summary>
        [TestMethod]
        public void GetContractsAggregateTest_NotFound()
        {
            var repository = new ContractsAggregateFileRepository(Path.Combine(DataSettingFilePath, "NotFound"));
            ContractsAggregate actual = repository.GetContractsAggregate();

            // プロパティを設定していないインスタンスが返ってくることを確認する
            var expected = new ContractsAggregate();
            Assert.AreEqual(expected.NeedContractRenewal, actual.NeedContractRenewal);
            Assert.AreEqual(expected.Contracts.Count, 0);
        }
    }
}
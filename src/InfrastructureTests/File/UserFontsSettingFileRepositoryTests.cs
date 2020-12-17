using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Core.Entities;
using InfrastructureTests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infrastructure.File.Tests
{
    /// <summary>
    /// ユーザー別フォント情報を格納するファイルリポジトリのテスト
    /// </summary>
    [TestClass]
    public class UserFontsSettingFileRepositoryTests
    {
        /// <summary>
        /// テストデータを格納するディレクトリのパス
        /// </summary>
        private static readonly string DataSettingFilePath = TestDataService.GetTestDataDirectory("UserFontsSetting");

        /// <summary>
        /// ユーザー別フォント情報の保存・取得処理のテスト
        /// </summary>
        [TestMethod]
        public void SaveUserFontsSettingAndGetUserFontsSettingTest()
        {
            // 任意の値を設定しファイルに保存する
            var repository = new UserFontsSettingFileRepository("fonts.dat");
            UserFontsSetting expected = new UserFontsSetting();
            expected.Fonts = new List<Font>()
            {
                new Font("aaa", "c:\file", true, true, "テストフォント", "1.0.0", "aaabbbccc123", new List<string>() { "12345aaaa", "9876bbbbb" }),
                new Font(string.Empty, "c:\file2", false, null, "テストフォント2", "1.0.1", string.Empty, new List<string>() { "99999aaa", "55555cccc" }),
            };
            repository.SaveUserFontsSetting(expected);

            // ファイルから値を取得する
            UserFontsSetting actual = repository.GetUserFontsSetting();

            // 設定した値と取得した値が一致することを確認する
            Assert.IsTrue(actual.Fonts.Any());
            for (int i = 0; i < actual.Fonts.Count; i++)
            {
                Assert.IsTrue(expected.Fonts[i].Equals(actual.Fonts[i]));
            }
        }

        /// <summary>
        /// ユーザー別フォント情報のJSON形式が正しくない場合のテスト
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(JsonException))]
        public void GetUserFontsSettingTest_Failed()
        {
            var repository = new UserFontsSettingFileRepository(Path.Combine(DataSettingFilePath, "fonts.failed.dat"));
            UserFontsSetting setting = repository.GetUserFontsSetting();
        }

        /// <summary>
        /// ユーザー別フォント情報のファイルが見つからない場合のテスト
        /// </summary>
        [TestMethod]
        public void GetUserFontsSettingTest_NotFound()
        {
            var repository = new UserFontsSettingFileRepository(Path.Combine(DataSettingFilePath, "NotFound"));
            UserFontsSetting actual = repository.GetUserFontsSetting();

            // プロパティを設定していないインスタンスが返ってくることを確認する
            Assert.AreEqual(actual.Fonts.Count, 0);
        }
    }
}
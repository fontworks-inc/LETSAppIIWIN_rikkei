using System;
using System.Collections.Generic;
using Core.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infrastructure.Memory.Tests
{
    /// <summary>
    /// メモリで保持する情報を格納するリポジトリのテスト
    /// </summary>
    [TestClass]
    public class VolatileSettingMemoryRepositoryTests
    {
        /// <summary>
        /// 取得できるインスタンスが単一になっていることを確認する
        /// </summary>
        [TestMethod]
        public void GetVolatileSettingTest()
        {
            // リポジトリからメモリ情報を取得し、任意の値を設定する
            VolatileSettingMemoryRepository setRepository = new VolatileSettingMemoryRepository();
            VolatileSetting expected = setRepository.GetVolatileSetting();
            expected.AccessToken = "abcd123456";
            expected.CheckedStartupAt = DateTime.Parse("2020/12/04 13:51");
            expected.CompletedDownload = true;
            expected.IsCheckedStartup = true;
            expected.IsConnected = true;
            expected.IsDownloading = true;
            expected.LastAccessAt = DateTime.Parse("2020/12/07 13:42");
            expected.IsNoticed = true;
            expected.InstallTargetFonts = new List<Font>()
            {
                new Font("aaa", "c:\file", true, true, "テストフォント", "1.0.0", "aaabbbccc123", new List<string>() { "12345aaaa", "9876bbbbb" }),
            };

            expected.NotificationFonts = new List<Font>()
            {
                new Font(string.Empty, "c:\file2", false, null, "テストフォント2", "1.0.1", string.Empty, new List<string>() { "99999aaa", "55555cccc" }),
            };

            // 別途にリポジトリを作成してメモリ情報を取得した際に、値が変更されていることを確認する
            VolatileSettingMemoryRepository getRepository = new VolatileSettingMemoryRepository();
            VolatileSetting actual = setRepository.GetVolatileSetting();
            Assert.AreEqual(expected.AccessToken, actual.AccessToken);
            Assert.IsTrue(((DateTime)actual.CheckedStartupAt).CompareTo(expected.CheckedStartupAt) == 0);
            Assert.AreEqual(expected.CompletedDownload, actual.CompletedDownload);
            Assert.AreEqual(expected.IsCheckedStartup, actual.IsCheckedStartup);
            Assert.AreEqual(expected.IsConnected, actual.IsConnected);
            Assert.AreEqual(expected.IsDownloading, actual.IsDownloading);
            Assert.IsTrue(((DateTime)actual.LastAccessAt).CompareTo(expected.LastAccessAt) == 0);
            Assert.AreEqual(expected.IsNoticed, actual.IsNoticed);
            Assert.AreEqual(1, actual.InstallTargetFonts.Count);
            Assert.IsTrue(expected.InstallTargetFonts[0].Equals(actual.InstallTargetFonts[0]));
            Assert.AreEqual(1, actual.NotificationFonts.Count);
            Assert.IsTrue(expected.NotificationFonts[0].Equals(actual.NotificationFonts[0]));
        }
    }
}
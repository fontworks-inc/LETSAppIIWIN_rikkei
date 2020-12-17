using System.IO;

namespace InfrastructureTests.TestData
{
    /// <summary>
    /// テストデータの情報を扱うクラス
    /// </summary>
    public static class TestDataService
    {
        /// <summary>
        /// テストデータのディレクトリ。
        /// </summary>
        private static readonly string Directory = "./TestData";

        /// <summary>
        /// 対象のテストデータディレクトリのパスを取得する。
        /// </summary>
        /// <param name="relative">ルートディレクトリからの相対パス。</param>
        /// <returns>テストデータディレクトリのパス。</returns>
        public static string GetTestDataDirectory(string relative)
        {
            return Path.Combine(Directory, relative);
        }
    }
}

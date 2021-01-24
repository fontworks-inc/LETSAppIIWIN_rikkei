using System.IO;

namespace Infrastructure.File
{
    /// <summary>
    /// ファイルリポジトリ基底クラス
    /// </summary>
    public abstract class TextFileRepositoryBase
    {
        /// <summary>
        /// ロック用オブジェクト
        /// </summary>
        private readonly object fileLock = new object();

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        protected TextFileRepositoryBase(string filePath)
        {
            this.FilePath = filePath;
        }

        /// <summary>
        /// ファイルパス
        /// </summary>
        protected string FilePath { get; }

        /// <summary>
        /// 削除する
        /// </summary>
        public void Delete()
        {
            if (System.IO.File.Exists(this.FilePath))
            {
                lock (this.fileLock)
                {
                    FileInfo file = new FileInfo(this.FilePath);
                    if ((file.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    {
                        file.Attributes = FileAttributes.Normal;
                    }

                    file.Delete();
                }
            }
        }

        /// <summary>
        /// ファイルを読みとり、内容を文字列で返す
        /// </summary>
        /// <returns>テキストファイルの内容</returns>
        protected virtual string ReadAll()
        {
            lock (this.fileLock)
            {
                return System.IO.File.ReadAllText(this.FilePath);
            }
        }

        /// <summary>
        /// テキストをファイルに書き込む
        /// </summary>
        /// <param name="text">テキスト</param>
        protected virtual void WriteAll(string text)
        {
            lock (this.fileLock)
            {
                // なければフォルダを作成する
                string directory = Path.GetDirectoryName(this.FilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                System.IO.File.WriteAllText(this.FilePath, text);
            }
        }
    }
}

using System;
using System.IO;
using System.Text;

namespace LETSDECODE
{
    /// <summary>
    /// ファイル出力を行う
    /// </summary>
    class OutputFile
    {
        /// <summary>
        /// エンコーディング
        /// </summary>
        private readonly Encoding encoding = null;

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="encodeName">エンコーディング名（デフォルトではShift-JIS）</param>
        public OutputFile(string encodeName = "shift_jis")
        {
            this.encoding = Encoding.GetEncoding(encodeName);
        }

        /// <summary>
        /// 指定のファイルを出力する
        /// </summary>
        /// <param name="filePath">出力先ファイルパス</param>
        /// <param name="filePath">出力する内容</param>
        public string Output(string filePath, string text)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, false, this.encoding))
                {
                    writer.WriteLine(text);
                }

                return string.Empty;
            }
            catch (Exception e)
            {
                // エラーメッセージを返却する
                return e.Message;
            }
        }
    }
}

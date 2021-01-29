using System;
using System.IO;
using System.Text;

namespace LETSDECODE
{
    /// <summary>
    /// 引数で渡された暗号化ファイルを復号化し、異なるファイルで出力する
    /// </summary>
    class DecodeMainClass
    {
        /// <summary>
        /// 出力するファイルの拡張子
        /// </summary>
        private static readonly string extensions = ".decode";

        static void Main(string[] args)
        {
            // 復号化したいファイルパスを入力
            string inputFilePath = string.Empty;
            if (args.Length > 0)
            {
                // アプリケーション引数からファイルパスを取得
                inputFilePath = args[0];
            }

            // 引数未入力の場合はコンソールから入力させる
            while (string.IsNullOrEmpty(inputFilePath))
            {
                Console.Write("復号化したいファイルのパスを入力してください：　");
                inputFilePath = Console.ReadLine();
            }

            // 処理実施
            string message = Decode(inputFilePath);

            // 結果表示
            Console.WriteLine(message);
            Console.WriteLine("終了するには適当なキーを入力してください。");
            Console.ReadKey();
        }

        /// <summary>
        /// 処理本体
        /// </summary>
        private static string Decode(string inputFilePath)
        {
            try
            {
                if (!File.Exists(inputFilePath))
                {
                    // ファイルが存在しない場合、エラーとする
                    return "【エラー】入力された暗号化ファイルが存在しません。";
                }

                // 内容を複合化する
                DecryptFile decryptFile = new DecryptFile();
                string decryptText = string.Empty;
                try
                {
                    decryptText = decryptFile.ReadAll(inputFilePath);
                }
                catch (Exception)
                {
                    // 復号化に失敗
                    return "【エラー】復号化に失敗しました。入力ファイルが暗号化されているか確認してください。";
                }

                // 取得した内容から複合化ファイルを作成する           
                string dirPath = Path.GetDirectoryName(inputFilePath);
                string fileName = Path.GetFileName(inputFilePath) + extensions;
                string decryptFilePath = Path.Combine(dirPath, fileName);

                OutputFile outputFile = new OutputFile();
                string ret = outputFile.Output(decryptFilePath, decryptText);
                if (!string.IsNullOrEmpty(ret))
                {
                    // ファイル出力時にエラー
                    return "【エラー】ファイル出力時にエラーが発生しました。：" + ret;
                }

                return "復号化ファイルは正常に出力されました。";
            }
            catch (Exception e)
            {
                return "【エラー】予期せぬエラーが発生しました。：" + e.Message;
            }
        }
    }
}

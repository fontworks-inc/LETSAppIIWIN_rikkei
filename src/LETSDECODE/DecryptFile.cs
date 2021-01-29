using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LETSDECODE
{
    /// <summary>
    /// 複合化を行う
    /// </summary>
    public class DecryptFile
    {
        /// <summary>
        /// AESで使用するIV
        /// </summary>
        private static readonly string AesIV = "nnTB3aksVMV2xnFP";

        /// <summary>
        /// AESで使用するキー
        /// </summary>
        private static readonly string AesKey = "aSgrTshePfRkhgTDW8DRU74hm3ErMHzD";

        /// <summary>
        /// キーサイズ
        /// </summary>
        private static readonly int KeySize = 256;

        /// <summary>
        /// ブロックサイズ
        /// </summary>
        private static readonly int BlockSize = 128;

        /// <summary>
        /// AESオブジェクト
        /// </summary>
        private AesManaged aes;

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        public DecryptFile()
        {
            this.aes = this.GetAesManaged();
        }

        /// <summary>
        /// AESオブジェクトを取得します
        /// </summary>
        /// <returns>AESオブジェクト</returns>
        private AesManaged GetAesManaged()
        {
            // AESオブジェクトを生成し、パラメータを設定します。
            var aes = new AesManaged();
            aes.KeySize = KeySize;
            aes.BlockSize = BlockSize;
            aes.Mode = CipherMode.CBC;
            aes.IV = Encoding.UTF8.GetBytes(AesIV);
            aes.Key = Encoding.UTF8.GetBytes(AesKey);
            aes.Padding = PaddingMode.PKCS7;
            return aes;
        }

        /// <summary>
        /// ファイルを読みとり、内容を文字列で返す
        /// </summary>
        /// <param name="filePath">復号化対象ファイルのパス</param>
        /// <returns>テキストファイルの内容</returns>
        public string ReadAll(string filePath)
        {
            lock (filePath)
            {
                string encryptValue = File.ReadAllText(filePath);
                return this.Decrypt(encryptValue);
            }
        }

        /// <summary>
        /// 暗号化されたBase64文字列を復号化します
        /// </summary>
        /// <param name="encryptValue">暗号化されたBase64文字列</param>
        /// <returns>復号化された文字列</returns>
        public string Decrypt(string encryptValue)
        {
            // 暗号化されたBase64文字列をバイトデータに変換します。
            var byteValue = Convert.FromBase64String(encryptValue);

            // バイトデータの長さを取得します。
            var byteLength = byteValue.Length;

            // 復号化オブジェクトを取得します。
            var decryptor = this.aes.CreateDecryptor();

            // 復号化します。
            var decryptValue = decryptor.TransformFinalBlock(byteValue, 0, byteLength);

            // 復号化されたバイトデータを文字列に変換します。
            var stringValue = Encoding.UTF8.GetString(decryptValue);
            return stringValue;
        }
    }
}

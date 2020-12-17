using System;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.File
{
    /// <summary>
    /// 暗号化ファイルリポジトリ基底クラス
    /// </summary>
    public abstract class EncryptFileBase : FileRepositoryBase
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
        /// <param name="filePath">ファイルパス</param>
        protected EncryptFileBase(string filePath)
            : base(filePath)
        {
            this.aes = this.GetAesManaged();
        }

        /// <summary>
        /// ファイルを読みとり、内容を文字列で返す
        /// </summary>
        /// <returns>テキストファイルの内容</returns>
        protected override string ReadAll()
        {
            return this.Decrypt(base.ReadAll());
        }

        /// <summary>
        /// テキストをファイルに書き込む
        /// </summary>
        /// <param name="text">テキスト</param>
        protected override void WriteAll(string text)
        {
            base.WriteAll(this.Encrypt(text));
        }

        /// <summary>
        /// 文字列の暗号化を行う
        /// </summary>
        /// <param name="value">変換前文字列</param>
        /// <returns>Base64文字列</returns>
        private string Encrypt(string value)
        {
            // 対象の文字列をバイトデータに変換します。
            var byteValue = Encoding.UTF8.GetBytes(value);

            // バイトデータの長さを取得します。
            var byteLength = byteValue.Length;

            // 暗号化オブジェクトを取得します。
            var encryptor = this.aes.CreateEncryptor();

            // 暗号化します。
            var encryptValue = encryptor.TransformFinalBlock(byteValue, 0, byteLength);

            // 暗号化されたバイトデータをBase64文字列に変換します。
            var base64Value = Convert.ToBase64String(encryptValue);
            return base64Value;
        }

        /// <summary>
        /// 暗号化されたBase64文字列を復号化します
        /// </summary>
        /// <param name="encryptValue">暗号化されたBase64文字列</param>
        /// <returns>復号化された文字列</returns>
        private string Decrypt(string encryptValue)
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
    }
}

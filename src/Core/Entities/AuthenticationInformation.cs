using System;

namespace Core.Entities
{
    /// <summary>
    /// 認証情報を表すクラス
    /// </summary>
    public class AuthenticationInformation
    {
        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        public AuthenticationInformation()
        {
        }

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="accessToken">アクセストークン</param>
        /// <param name="refreshToken">リフレッシュトークン</param>
        public AuthenticationInformation(string accessToken, string refreshToken)
        {
            this.AccessToken = accessToken;
            this.RefreshToken = refreshToken;
        }

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="groupType">アクセストークン</param>
        /// <param name="offlineDeviceId">リフレッシュトークン</param>
        /// <param name="licenseDecryptionKey">リフレッシュトークン</param>
        /// <param name="indefiniteAccessToken">リフレッシュトークン</param>
        public AuthenticationInformation(int groupType, string offlineDeviceId, string licenseDecryptionKey, string indefiniteAccessToken)
        {
            this.GroupType = groupType;
            this.OfflineDeviceId = offlineDeviceId;
            this.LicenseDecryptionKey = licenseDecryptionKey;
            this.IndefiniteAccessToken = indefiniteAccessToken;
        }

        /// <summary>
        /// アクセストークン
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// リフレッシュトークン
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// リフレッシュトークン
        /// </summary>
        public int GroupType { get; set; } = 0;

        /// <summary>
        /// リフレッシュトークン
        /// </summary>
        public string OfflineDeviceId { get; set; } = string.Empty;

        /// <summary>
        /// リフレッシュトークン
        /// </summary>
        public string LicenseDecryptionKey { get; set; } = string.Empty;

        /// <summary>
        /// リフレッシュトークン
        /// </summary>
        public string IndefiniteAccessToken { get; set; } = string.Empty;

        /// <summary>
        /// このオブジェクトが指定されたオブジェクトと等しいかどうかを判定する
        /// </summary>
        /// <param name="obj">比較対象のオブジェクト</param>
        /// <returns>このオブジェクトが指定されたオブジェクトと等しい場合はtrue、それ以外はfalse</returns>
        public override bool Equals(object obj)
        {
            return obj is AuthenticationInformation info &&
                   this.AccessToken == info.AccessToken;
        }

        /// <summary>
        /// このオブジェクトのハッシュコードを取得する
        /// </summary>
        /// <returns>このオブジェクトのハッシュコード</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.AccessToken);
        }
    }
}

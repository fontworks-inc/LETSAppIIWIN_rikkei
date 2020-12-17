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
        /// アクセストークン
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// リフレッシュトークン
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

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

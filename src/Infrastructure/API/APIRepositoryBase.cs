namespace Infrastructure.API
{
    /// <summary>
    /// リポジトリの基底クラス
    /// </summary>
    public abstract class APIRepositoryBase
    {
        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="apiConfiguration">設定情報</param>
        protected APIRepositoryBase(APIConfiguration apiConfiguration)
        {
        }

        /// <summary>
        /// アクセストークンをヘッダーに設定する
        /// </summary>
        /// <param name="accessToken">アクセストークン</param>
        protected void SetAuthenticationBearer(string accessToken)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 認証情報をヘッダーに設定する
        /// </summary>
        /// <param name="accessToken">アクセストークン</param>
        /// <param name="deviceId">デバイスID</param>
        protected void SetAuthentication(string accessToken, string deviceId)
        {
            throw new System.NotImplementedException();
        }
    }
}

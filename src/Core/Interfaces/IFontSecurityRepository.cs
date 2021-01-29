using Core.Entities;

namespace Core.Interfaces
{
    /// <summary>
    /// フォントセキュリティのリポジトリインターフェイス
    /// </summary>
    public interface IFontSecurityRepository
    {
        /// <summary>
        /// ユーザIDを取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>ユーザID</returns>
        UserId GetUserId(string deviceId, string accessToken);

        /// <summary>
        /// 他端末のフォントがコピーされた時にFW運用者に通知する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <param name="fontId">フォントID</param>
        /// <param name="originalUserId">当該フォントファイルをインストールした端末のユーザID</param>
        /// <param name="originalDeviceId">当該フォントファイルをインストールした端末のデバイスID</param>
        /// <param name="detected">検知日時</param>
        void PostFontFileCopyDetection(
            string deviceId,
            string accessToken,
            string fontId,
            string originalUserId,
            string originalDeviceId,
            string detected);
    }
}

using Core.Entities;

namespace Core.Interfaces
{
    /// <summary>
    ///  URLアドレスを格納するリポジトリのインターフェイス
    /// </summary>
    public interface IUrlRepository
    {
        /// <summary>
        /// パスワード再設定ページのURLを取得する
        /// </summary>
        /// <returns>パスワード再設定ページのURL</returns>
        Url GetResetPasswordPageUrl();

        /// <summary>
        /// 会員登録ページのURLを取得する
        /// </summary>
        /// <returns>会員登録ページのURL</returns>
        Url GetUserRegistrationPageUrl();

        /// <summary>
        /// ホーム画面URLを取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>ホーム画面URL</returns>
        Url GetUserPageUrl(string deviceId, string accessToken);

        /// <summary>
        /// お知らせ画面URLを取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>お知らせ画面URL</returns>
        Url GetAnnouncePageUrl(string deviceId, string accessToken);

        /// <summary>
        /// フォント一覧画面URLを取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>フォント一覧画面URL</returns>
        Url GetFontListPageUrl(string deviceId, string accessToken);

        /// <summary>
        /// ヘルプURLを取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>ヘルプURL</returns>
        Url GetHelpUrl(string deviceId, string accessToken);
    }
}

using System;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.API
{
    /// <summary>
    ///  URLアドレスを格納するリポジトリのインターフェイスのモック
    ///  TODO 削除
    /// </summary>
    public class UrlAPIRepositoryMock : APIRepositoryBase, IUrlRepository
    {
        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="apiConfiguration">設定情報</param>
        public UrlAPIRepositoryMock(APIConfiguration apiConfiguration)
            : base(apiConfiguration)
        {
        }

        /// <summary>
        /// パスワード再設定ページのURLを取得する
        /// </summary>
        /// <returns>パスワード再設定ページのURL</returns>
        public Url GetResetPasswordPageUrl()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 会員登録ページのURLを取得する
        /// </summary>
        /// <returns>会員登録ページのURL</returns>
        public Url GetUserRegistrationPageUrl()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ホーム画面URLを取得する
        /// </summary>
        /// <param name="uuid">UUID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>ホーム画面URL</returns>
        public Url GetUserPageUrl(string uuid, string accessToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// お知らせ画面URLを取得する
        /// </summary>
        /// <param name="uuid">UUID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>お知らせ画面URL</returns>
        public Url GetAnnouncePageUrl(string uuid, string accessToken)
        {
            return new Url("http://google.com");
        }

        /// <summary>
        /// フォント一覧画面URLを取得する
        /// </summary>
        /// <param name="uuid">UUID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>フォント一覧画面URL</returns>
        public Url GetFontListPageUrl(string uuid, string accessToken)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.API
{
    /// <summary>
    ///  URLアドレスを格納するリポジトリのインターフェイス
    /// </summary>
    public class UrlAPIRepository : APIRepositoryBase, IUrlRepository
    {
        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="apiConfiguration">設定情報</param>
        public UrlAPIRepository(APIConfiguration apiConfiguration)
            : base(apiConfiguration)
        {
        }

        /// <summary>
        /// パスワード再設定ページのURLを取得する
        /// </summary>
        /// <returns>パスワード再設定ページのURL</returns>
        /// <remarks>FUNCTION_08_01_10(パスワードを忘れた方_再設定画面URLの取得API)</remarks>
        public Url GetResetPasswordPageUrl()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 会員登録ページのURLを取得する
        /// </summary>
        /// <returns>会員登録ページのURL</returns>
        /// <remarks>FUNCTION_08_01_11(会員登録画面URLの取得API)</remarks>
        public Url GetUserRegistrationPageUrl()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ホーム画面URLを取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>ホーム画面URL</returns>
        /// <remarks>FUNCTION_08_03_01(ホーム画面URLの取得API)</remarks>
        public Url GetUserPageUrl(string deviceId, string accessToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// お知らせ画面URLを取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>お知らせ画面URL</returns>
        /// <remarks>FUNCTION_08_04_01(お知らせ画面URLの取得API)</remarks>
        public Url GetAnnouncePageUrl(string deviceId, string accessToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// フォント一覧画面URLを取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>フォント一覧画面URL</returns>
        /// <remarks>FUNCTION_08_06_01(フォント一覧画面URLの取得API)</remarks>
        public Url GetFontListPageUrl(string deviceId, string accessToken)
        {
            throw new NotImplementedException();
        }
    }
}

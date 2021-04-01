using System;
using Core.Entities;
using Core.Interfaces;
using NLog;
using Org.OpenAPITools.Api;
using Org.OpenAPITools.Client;
using Org.OpenAPITools.Model;

namespace Infrastructure.API
{
    /// <summary>
    /// 未読お知らせ情報を格納するリポジトリ
    /// </summary>
    public class UnreadNoticeRepository : APIRepositoryBase, IUnreadNoticeRepository
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private static readonly Logger Logger = LogManager.GetLogger("nlog.config");

        /// <summary>
        /// インスタンスの初期化を行う
        /// </summary>
        /// <param name="apiConfiguration">設定情報</param>
        public UnreadNoticeRepository(APIConfiguration apiConfiguration)
            : base(apiConfiguration)
        {
        }

        /// <summary>
        /// 未読お知らせ情報を取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>ユーザに紐づく未読お知らせ情報</returns>
        /// <remarks>FUNCTION_08_04_02(お知らせ情報取得API)</remarks>
        public UnreadNotice GetUnreadNotice(string deviceId, string accessToken)
        {
            this.ApiParam[APIParam.DeviceId] = deviceId;
            this.ApiParam[APIParam.AccessToken] = accessToken;

            UnreadNotice response = null;

            // API通信を行う(リトライ込み)を行う（共通処理）
            try
            {
                this.Invoke(this.CallUnreadNoticeAPI);

                // 戻り値のセット（個別処理）
                var ret = new NoticeResponse(this.ApiResponse);
                if (ret.Code == (int)ResponseCode.Succeeded)
                {
                    response = new UnreadNotice()
                    {
                        Total = ret.Data.Total,
                        ExistsLatestNotice = ret.Data.ExistsLatestNotice,
                    };
                }
                else
                {
                    throw new ApiException(ret.Code, ret.Message);
                }
            }
            catch (ApiException)
            {
                // 通信に失敗or通信しなかった
                throw;
            }

            return response;
        }

        /// <summary>
        /// ユーザの未読お知らせの有無・件数の取得呼び出し
        /// </summary>
        private void CallUnreadNoticeAPI()
        {
            Configuration config = new Configuration();
            config.BasePath = this.BasePath;
            config.UserAgent = (string)this.ApiParam[APIParam.UserAgent];
            config.WebProxy = this.APIConfiguration.GetWebProxy(this.BasePath);
            config.AccessToken = (string)this.ApiParam[APIParam.AccessToken];
            NoticeApi apiInstance = new NoticeApi(config);
            this.ApiResponse = apiInstance.GetNotice((string)this.ApiParam[APIParam.DeviceId], config.UserAgent);
        }
    }
}

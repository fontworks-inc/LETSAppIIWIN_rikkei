using Core.Entities;

namespace Core.Interfaces
{
    /// <summary>
    /// 未読お知らせ情報を格納するリポジトリのインターフェイス
    /// </summary>
    public interface IUnreadNoticeRepository
    {
        /// <summary>
        /// 未読お知らせ情報を取得する
        /// </summary>
        /// <param name="deviceId">デバイスID</param>
        /// <param name="accessToken">アクセストークン</param>
        /// <returns>ユーザに紐づく未読お知らせ情報</returns>
        /// <remarks>FUNCTION_08_04_02(お知らせ情報取得API)</remarks>
        UnreadNotice GetUnreadNotice(string deviceId, string accessToken);
    }
}

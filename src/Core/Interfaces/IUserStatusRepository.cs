using Core.Entities;

namespace Core.Interfaces
{
    /// <summary>
    /// ユーザ別ステータス情報を格納するリポジトリのインターフェイス
    /// </summary>
    public interface IUserStatusRepository
    {
        /// <summary>
        /// ユーザ別ステータス情報を取得する
        /// </summary>
        /// <returns>ユーザ別ステータス情報</returns>
        UserStatus GetStatus();

        /// <summary>
        /// ユーザ別ステータス情報を保存する
        /// </summary>
        /// <param name="status">ユーザ別ステータス情報</param>
        void SaveStatus(UserStatus status);
    }
}

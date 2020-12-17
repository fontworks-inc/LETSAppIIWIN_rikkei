using Core.Entities;

namespace Core.Interfaces
{
    /// <summary>
    /// ユーザ別フォント情報を格納するリポジトリのインターフェイス
    /// </summary>
    public interface IUserFontsSettingRepository
    {
        /// <summary>
        /// ユーザ別フォント情報を取得する
        /// </summary>
        /// <returns>ユーザ別フォント情報</returns>
        UserFontsSetting GetUserFontsSetting();

        /// <summary>
        /// ユーザ別フォント情報を保存する
        /// </summary>
        /// <param name="userFontsSetting">ユーザ別フォント情報</param>
        void SaveUserFontsSetting(UserFontsSetting userFontsSetting);
    }
}

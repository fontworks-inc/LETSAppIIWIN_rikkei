using System.Text.Json;
using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.File
{
    /// <summary>
    /// ユーザ別フォント情報を格納するファイルリポジトリ
    /// </summary>
    public class UserFontsSettingFileRepository : FileRepositoryBase, IUserFontsSettingRepository
    {
        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        public UserFontsSettingFileRepository(string filePath)
            : base(filePath)
        {
        }

        /// <summary>
        /// ユーザ別フォント情報を取得する
        /// </summary>
        /// <returns>ユーザ別フォント情報</returns>
        public UserFontsSetting GetUserFontsSetting()
        {
            if (System.IO.File.Exists(this.FilePath))
            {
                // ファイルが存在する場合、内容を返す
                string jsonString = this.ReadAll();
                return JsonSerializer.Deserialize<UserFontsSetting>(jsonString);
            }
            else
            {
                // ファイルが存在しない場合、新規のオブジェクトを返す
                return new UserFontsSetting();
            }
        }

        /// <summary>
        /// ユーザ別フォント情報を保存する
        /// </summary>
        /// <param name="userFontsSetting">ユーザ別フォント情報</param>
        public void SaveUserFontsSetting(UserFontsSetting userFontsSetting)
        {
            this.WriteAll(JsonSerializer.Serialize(userFontsSetting));
        }
    }
}

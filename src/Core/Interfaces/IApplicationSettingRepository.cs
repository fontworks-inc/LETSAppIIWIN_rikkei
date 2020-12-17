using Core.Entities;

namespace Core.Interfaces
{
    /// <summary>
    /// アプリケーション設定情報を格納するリポジトリのインターフェイス
    /// </summary>
    public interface IApplicationSettingRepository
    {
        /// <summary>
        /// アプリケーション設定情報を取得する
        /// </summary>
        /// <returns>アプリケーション設定情報</returns>
        ApplicationSetting GetSetting();
    }
}

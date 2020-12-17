using Core.Entities;

namespace Core.Interfaces
{
    /// <summary>
    /// メモリで保持する情報を格納するリポジトリのインターフェイス
    /// </summary>
    public interface IVolatileSettingRepository
    {
        /// <summary>
        /// メモリで保持する情報を取得する
        /// </summary>
        /// <returns>アプリケーション終了時に保持しない情報</returns>
        VolatileSetting GetVolatileSetting();
    }
}

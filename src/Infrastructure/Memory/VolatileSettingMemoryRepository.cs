using Core.Entities;
using Core.Interfaces;

namespace Infrastructure.Memory
{
    /// <summary>
    /// メモリで保持する情報を格納するリポジトリ
    /// </summary>
    public class VolatileSettingMemoryRepository : IVolatileSettingRepository
    {
        /// <summary>
        /// メモリで保持する情報を取得する
        /// </summary>
        /// <returns>アプリケーション終了時に保持しない情報</returns>
        public VolatileSetting GetVolatileSetting()
        {
            return SingletonVolatileSetting.GetInstance();
        }
    }
}

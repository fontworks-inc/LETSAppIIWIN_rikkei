using Core.Entities;

namespace Infrastructure.Memory
{
    /// <summary>
    /// メモリで保持する情報のインスタンスを管理するクラス
    /// </summary>
    public class SingletonVolatileSetting
    {
        /// <summary>
        /// メモリで保持する情報
        /// </summary>
        private static VolatileSetting singletonVolatileSetting = null;

        /// <summary>
        /// インスタンスを初期化する
        /// </summary>
        private SingletonVolatileSetting()
        {
        }

        /// <summary>
        /// インスタンスを取得する
        /// </summary>
        /// <returns>インスタンス</returns>
        public static VolatileSetting GetInstance()
        {
            if (singletonVolatileSetting == null)
            {
                singletonVolatileSetting = new VolatileSetting();
            }

            return singletonVolatileSetting;
        }
    }
}

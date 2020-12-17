using Prism.Ioc;
using Prism.Modularity;

namespace OS
{
    /// <summary>
    /// OSプロジェクトに関する設定を行う
    /// </summary>
    public class OSModule : IModule
    {
        /// <summary>
        /// 初期化時に実行されるイベント
        /// </summary>
        /// <param name="containerProvider">コンテナプロバイダ</param>
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        /// <summary>
        /// DIコンテナにインスタンスを登録する
        /// </summary>
        /// <param name="containerRegistry">コンテナレジストリ</param>
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}
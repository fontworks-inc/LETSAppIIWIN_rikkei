using Core.Entities;

namespace Core.Interfaces
{
    /// <summary>
    /// アプリケーション共通保存情報を格納するリポジトリのインターフェイス
    /// </summary>
    public interface IApplicationRuntimeRepository
    {
        /// <summary>
        /// アプリケーション共通保存情報を取得する
        /// </summary>
        /// <returns>アプリケーション共通保存情報</returns>
        ApplicationRuntime GetApplicationRuntime();

        /// <summary>
        /// アプリケーション共通保存情報を保存する
        /// </summary>
        /// <param name="applicationRuntime">アプリケーション共通保存情報</param>
        void SaveApplicationRuntime(ApplicationRuntime applicationRuntime);
    }
}

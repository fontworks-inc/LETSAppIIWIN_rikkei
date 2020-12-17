using Core.Entities;

namespace ApplicationService.Interfaces
{
    /// <summary>
    /// フォント管理に関する処理を行うサービスのインターフェイス
    /// </summary>
    public interface IFontManagerService
    {
        /// <summary>
        /// フォントの同期処理を実施する
        /// </summary>
        /// <param name="font">アクティベート対象フォント</param>
        /// <remarks>アクティベート通知からの同期処理</remarks>
        void Synchronize(ActivateFont font);

        /// <summary>
        /// フォントの同期処理を実施する
        /// </summary>
        /// <param name="startUp">起動時かどうか</param>
        /// <remarks>アクティベート通知以外からの同期処理</remarks>
        void Synchronize(bool startUp);
    }
}
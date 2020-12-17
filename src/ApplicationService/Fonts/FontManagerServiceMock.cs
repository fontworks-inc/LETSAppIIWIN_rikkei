using ApplicationService.Interfaces;
using Core.Entities;

namespace ApplicationService.Fonts
{
    /// <summary>
    /// フォント管理に関する処理を行うサービスのクラスのモック
    /// </summary>
    public class FontManagerServiceMock : IFontManagerService
    {
        /// <summary>
        /// フォントの同期処理を実施する
        /// </summary>
        /// <param name="font">アクティベート対象フォント</param>
        /// <remarks>アクティベート通知からの同期処理</remarks>
        public void Synchronize(ActivateFont font)
        {
            // 実際はモックでない本物の方で実装する
        }

        /// <summary>
        /// フォントの同期処理を実施する
        /// </summary>
        /// <param name="startUp">起動時かどうか</param>
        /// <remarks>アクティベート通知以外からの同期処理</remarks>
        public void Synchronize(bool startUp)
        {
        }
    }
}
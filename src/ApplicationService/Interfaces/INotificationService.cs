using System.Collections.Generic;
using Core.Entities;

namespace ApplicationService.Interfaces
{
    /// <summary>
    /// 通知に関する処理を行うサービスのインターフェイス
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// フォントのダウンロードを通知する
        /// </summary>
        /// <param name="fonts">通知対象のフォントリスト</param>
        void Notice(IList<Font> fonts);
    }
}

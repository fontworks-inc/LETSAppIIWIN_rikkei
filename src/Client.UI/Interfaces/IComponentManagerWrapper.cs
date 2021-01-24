using Client.UI.Components;

namespace Client.UI.Interfaces
{
    /// <summary>
    /// ComponentManagerWrapper のインタフェース
    /// </summary>
    public interface IComponentManagerWrapper
    {
        /// <summary>
        /// アプリケーションのコンポーネントを管理するクラス
        /// </summary>
        ComponentManager Manager { get; set; }
    }
}

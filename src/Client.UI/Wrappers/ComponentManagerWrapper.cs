using Client.UI.Components;
using Client.UI.Interfaces;

namespace Client.UI.Wrappers
{
    /// <summary>
    /// ComponentManagerWrapper クラス
    /// </summary>
    public class ComponentManagerWrapper : IComponentManagerWrapper
    {
        /// <summary>
        /// アプリケーションのコンポーネントを管理するクラス
        /// </summary>
        public ComponentManager Manager { get; set; }
    }
}

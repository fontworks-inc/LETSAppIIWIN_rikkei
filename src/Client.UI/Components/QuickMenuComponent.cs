using System.ComponentModel;
using System.Windows.Forms;

namespace Client.UI.Components
{
    /// <summary>
    /// クイックメニュークラス
    /// </summary>
    public class QuickMenuComponent : Component
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="manager">ComponentManager</param>
        public QuickMenuComponent(ComponentManager manager)
        {
            this.Manager = manager;
            this.Manager.Container.Add(this);

            this.InitializeComponent();

            this.ContextMenu.VisibleChanged += (s, e) =>
            {
                if (this.ContextMenu.Visible)
                {
                    this.Manager.ApplicationIcon.SetSelectedMode();
                }
                else
                {
                    this.Manager.ApplicationIcon.SetNormalMode();
                }
            };
        }

        /// <summary>
        /// ComponentManager
        /// </summary>
        public ComponentManager Manager
        {
            get;
            private set;
        }

        /// <summary>
        /// ContextMenuStrip
        /// </summary>
        public ContextMenuStrip ContextMenu
        {
            get;
            private set;
        }

        /// <summary>
        /// SuspendLayoutメソッドを呼び出す
        /// </summary>
        public void SuspendLayout()
        {
            this.ContextMenu.SuspendLayout();
        }

        /// <summary>
        /// ResumeLayoutメソッドを呼び出す
        /// </summary>
        public void ResumeLayout()
        {
            this.ContextMenu.ResumeLayout(false);
        }

        /// <summary>
        /// PerformLayoutメソッドを呼び出す
        /// </summary>
        public void PerformLayout()
        {
            this.ContextMenu.PerformLayout();
        }

        /// <summary>
        /// クイックメニュー初期化処理
        /// </summary>
        protected void InitializeComponent()
        {
            this.ContextMenu = new ContextMenuStrip(this.Manager.Container);
            this.ContextMenu.AutoSize = true;
        }
    }
}

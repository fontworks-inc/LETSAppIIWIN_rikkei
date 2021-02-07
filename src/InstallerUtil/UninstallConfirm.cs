using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InstallerUtil
{
    public partial class UninstallConfirm : Form
    {
        public UninstallConfirm()
        {
            Load += UninstallConfirm_Load;
            InitializeComponent();
            TopMost = true;
        }

        private void UninstallConfirm_Load(object sender, EventArgs e)
        {
            Screen s = Screen.FromControl(this);
            int halfW = s.Bounds.Width / 2;
            int halhH = s.Bounds.Height / 2;
            int formCenterX = s.Bounds.X + halfW;
            int formCenterY = s.Bounds.Y + halhH;

            this.Top = formCenterY - this.Height / 2 + 140;
            this.Left = formCenterX - this.Width / 2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

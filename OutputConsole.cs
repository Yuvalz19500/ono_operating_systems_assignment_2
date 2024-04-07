using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Scheduling
{
    public partial class OutputConsole : Form
    {
        public delegate void WriteDelegate(string s);

        public OutputConsole()
        {
            InitializeComponent();
            //Label.CheckForIllegalCrossThreadCalls = false;
            //Form.CheckForIllegalCrossThreadCalls = false;
        }

        private void OnResize(object sender, EventArgs e)
        {
            lblOutput.SetBounds(0, 0, this.Width, this.Height);
        }

        public void Write(string s)
        {
            s = s.Replace("\\n", "\n");
            lblOutput.Text += s;    
        }

        private void OutputConsole_Load(object sender, EventArgs e)
        {

        }
    }
}

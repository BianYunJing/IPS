using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DebugForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Common.AESClass.AESClass aESClass = new Common.AESClass.AESClass();
           // String password = aESClass.Encrypt("ahsgdhahdahshakjsdha", "catcher_tw");
            String passwordii = aESClass.Encrypt("catcher_tw", "ahsgdhahdahshakjsdha");
        }
    }
}

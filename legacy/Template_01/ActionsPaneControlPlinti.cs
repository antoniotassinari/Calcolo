using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Office = Microsoft.Office.Core;

namespace Template_01
{
    partial class ActionsPaneControlPlinti : UserControl
    {
        public ActionsPaneControlPlinti()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ofdReazioni.ShowDialog();
            ucProgettoPlintoNew1.apriFileRelazione(ofdReazioni.FileName);
        }
    }
}

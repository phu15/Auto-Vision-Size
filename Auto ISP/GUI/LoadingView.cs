using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Auto_Attach.GUI
{
    public partial class LoadingView : Form
    {

        public ProgressBar PrgBar = new ProgressBar();
        public LoadingView()
        {
            InitializeComponent();
        }

        private void frmLoadingView_Load(object sender, EventArgs e)
        {
           

           // PrgBar.Minimum = 1;
            PrgBar.Maximum = 300;
           // PrgBar.Value = 1;
            PrgBar.Step = 1;
            timer1.Interval = 1000;
            timer1.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            PrgBar.PerformStep();
           // PrgBar.Value++;
            if(PrgBar.Value == PrgBar.Maximum)
            {
                timer1.Enabled = false;
                this.Close();
            }
                
        }
    }
}

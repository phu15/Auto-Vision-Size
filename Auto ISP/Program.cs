using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Auto_Attach.Library;
using System.IO;
using System.IO.Ports;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.Blob;
using System.Diagnostics;

namespace Auto_Attach
{
    
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
      
        static void Main()
        {
            string mchSettingsFilePath;
            String exePath = System.AppDomain.CurrentDomain.BaseDirectory;
           

            string path = exePath + "Auto ANI.exe";
            string fileName = Path.GetFileName(path);
            Process[] processName = Process.GetProcessesByName(fileName.Substring(0, fileName.LastIndexOf('.')));
            if (processName.Length > 0)
            {
                // if (MessageBox.Show("Program open already, do you want to close?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                // {
                //  Application.Exit();
                // }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}

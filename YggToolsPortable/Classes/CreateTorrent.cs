
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace YggToolsPortable.Classes
{
    class CreateTorrent
    {
        private string contentUrl;
        MainWindow mainWindow;

        public CreateTorrent(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        public void OpenFile()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog();
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.Multiselect = false;
            bool? userClickedOK = openFileDialog1.ShowDialog();
            if (userClickedOK == true)
            {
                contentUrl = openFileDialog1.FileName;
                mainWindow.lbl_fileName.Text = contentUrl;
            }
        }

        public void OpenFolder()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    contentUrl = fbd.SelectedPath;
                    mainWindow.lbl_fileName.Text = contentUrl;
                }
            }
        }

        public void ValidateTorrent()
        {
        }
    }
}

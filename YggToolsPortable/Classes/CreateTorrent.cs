
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
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
            
        }
    }
}

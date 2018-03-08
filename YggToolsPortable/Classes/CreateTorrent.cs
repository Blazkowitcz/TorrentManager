
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using MonoTorrent.Common;
using System.Windows.Documents;

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
                mainWindow.txt_fileName.Text = contentUrl;
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
                    mainWindow.txt_fileName.Text = contentUrl;
                }
            }
        }

        public void ValidateTorrent()
        {
            if (mainWindow.txt_fileName.Text != "" && mainWindow.txt_tracker.Text != "" && mainWindow.txt_site.Text != "")
            {
                TorrentCreator torrent = new TorrentCreator();
                ITorrentFileSource fileSource = new TorrentFileSource(contentUrl);
                torrent.Private = true;
                List<string> urls = new List<string>();
                urls.Add(mainWindow.txt_tracker.Text);
                if (urls.Count != 0)
                {
                    torrent.Announces.Add(urls);
                }
                torrent.PublisherUrl = mainWindow.txt_site.Text;
                torrent.Create(fileSource, AppDomain.CurrentDomain.BaseDirectory + "toto.torrent");
            }
            else
            {

            }
        }
    }
}

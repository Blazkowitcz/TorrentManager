using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MonoTorrent.Client;
using MonoTorrent.Client.Encryption;
using MonoTorrent.Common;
using System.IO;
using System.Net;
using System.Threading;
using YggToolsPortable.Classes;
using Microsoft.Win32;

namespace YggToolsPortable
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BanList banList;
        ClientEngine engine;
        List<TorrentManager> managers = new List<TorrentManager>();
        List<TorrentInformation> items = new List<TorrentInformation>();
        TorrentEngineManager engineManager;
        TorrentManager manager;

        public MainWindow()
        {
            InitializeComponent();
            engineManager = new TorrentEngineManager();
            //items.Add(new TorrentInformation() { Title = "Complete this WPF tutorial", Completion = 0, DownSpeed =  "kb/s", UpSpeed = "kb/s"});
            lbTodoList.ItemsSource = items;

            //while (manager.State != TorrentState.Stopped && manager.State != TorrentState.Paused)
            //{
            //    Console.WriteLine(manager.Monitor.UploadSpeed);
            //    System.Threading.Thread.Sleep(1000);
            //}
        }

        private void btn_addTorrent_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Torrent (.torrent)|*.torrent|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.Multiselect = false;
            bool? userClickedOK = openFileDialog1.ShowDialog();
            if (userClickedOK == true)
            {
                foreach (string file in openFileDialog1.FileNames)
                {
                    Console.WriteLine(file);
                    engineManager.AddTorrent(file);
                }
                UpdateList();
            }

            while (engineManager.manager.State != TorrentState.Stopped && engineManager.manager.State != TorrentState.Paused)
            {
                Console.WriteLine(engineManager.manager.Monitor.DownloadSpeed);
                System.Threading.Thread.Sleep(1000);
            }
        }

        void UpdateList()
        {
            items.Clear();
            foreach (TorrentManager torrent in engineManager.managers)
            {
                items.Add(new TorrentInformation() { Title = torrent.Torrent.Name, Completion = torrent.Progress, DownSpeed = "kb/s", UpSpeed = "kb/s" });
            }
            lbTodoList.ItemsSource = items;
        }
    }


}

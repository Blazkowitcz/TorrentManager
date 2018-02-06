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
using System.ComponentModel;

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
        int i = 0;

        public MainWindow()
        {
            InitializeComponent();
            engineManager = new TorrentEngineManager();
            lbTodoList.ItemsSource = items;
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

            }
            UpdateList();
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(timer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            i++;
            UpdateList();
        }

        void UpdateList()
        {
            items.Clear();
            foreach (TorrentManager torrent in engineManager.managers)
            {
                Console.WriteLine(i);
                items.Add(new TorrentInformation() { Title = torrent.Torrent.Name, Completion = torrent.Progress, DownSpeed = torrent.Monitor.DownloadSpeed + " kb/s", UpSpeed = torrent.Monitor.UploadSpeed + "kb/s" });
            }
            lbTodoList.ItemsSource = items;
            lbTodoList.Items.Refresh();
        }

    }


}

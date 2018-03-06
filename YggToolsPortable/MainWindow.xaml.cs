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
using System.Collections.ObjectModel;
using MahApps.Metro.Controls;

namespace YggToolsPortable
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        List<TorrentManager> managers = new List<TorrentManager>();
        List<TorrentInformation> items = new List<TorrentInformation>();

        TorrentEngineManager engineManager;
        TorrentManager manager;
        ManageInformations manageInformations;
        CreateTorrent createTorrent;

        public MainWindow()
        {
            InitializeComponent();
            engineManager = new TorrentEngineManager(this);
            manageInformations = new ManageInformations(this);
            createTorrent = new CreateTorrent(this);
            engineManager.AutoStartTorrent();
            
            DgOrderCount.ItemsSource = items;
            //lbTodoList.ItemsSource = items;
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
            StartTimer();
        }

        public void StartTimer()
        {
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(timer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            UpdateList();
        }

        public void UpdateList()
        {
            items.Clear();
            foreach (TorrentManager torrent in engineManager.managers)
            {
                items.Add(new TorrentInformation()
                {
                    Title = torrent.Torrent.Name, Completion = Convert.ToInt32(Math.Floor(torrent.Progress)) ,
                    DownSpeed = torrent.Monitor.DownloadSpeed + " kb/s",
                    UpSpeed = torrent.Monitor.UploadSpeed + "kb/s"
                });
            }
            DgOrderCount.Items.Refresh();
            //lbTodoList.ItemsSource = items;
            //lbTodoList.Items.Refresh();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DgOrderCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgOrderCount.SelectedItem != null)
            {
                DataGrid dataGrid = DgOrderCount;
                DataGridRow Row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                DataGridCell RowAndColumn = (DataGridCell)dataGrid.Columns[0].GetCellContent(Row).Parent;
                Console.WriteLine(managers.Count);
                string CellValue = ((TextBlock)RowAndColumn.Content).Text;
                foreach (TorrentManager torrent in engineManager.managers)
                {
                    Console.WriteLine(torrent.Torrent.Name);
                    if(torrent.Torrent.Name == CellValue)
                    {
                        Console.WriteLine("toto");
                        lbl_name.Content = "Name : " + torrent.Torrent.Name;
                        lbl_size.Content = "Size : " + torrent.Torrent.Size;
                        lbl_download.Content = "Download : " + torrent.Monitor.DownloadSpeed;
                        lbl_upload.Content = "Upload : " + torrent.Monitor.UploadSpeed;
                        lbl_peers.Content = "Seeder : " + torrent.Peers.Seeds;
                        lbl_passkey.Content = "Passkey : " + torrent.Torrent.PublisherUrl;
                    }
                }

            }

        }

        private void btn_OpenFile_Click(object sender, RoutedEventArgs e)
        {
            createTorrent.OpenFile();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            createTorrent.OpenFolder();
        }
    }


}

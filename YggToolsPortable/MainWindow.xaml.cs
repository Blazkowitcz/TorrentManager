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
        string torrentSelected;

        public MainWindow()
        {
            InitializeComponent();
            engineManager = new TorrentEngineManager(this);
            manageInformations = new ManageInformations(this);
            createTorrent = new CreateTorrent(this);
            engineManager.AutoStartTorrent();
            
            DgOrderCount.ItemsSource = items;
            (wfhSample.Child as System.Windows.Forms.WebBrowser).ScriptErrorsSuppressed = true;
            (wfhSample.Child as System.Windows.Forms.WebBrowser).Navigate("http://ftorrent.tk/");
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
                Console.WriteLine("ISRUNNING : " + torrent.State);
                string state = torrent.State.ToString();
                string txtDownload = torrent.Monitor.DownloadSpeed > 1000000 ? torrent.Monitor.DownloadSpeed / 1000000 + " Mb/s" : torrent.Monitor.DownloadSpeed /1000 + " kb/s";
                string txtUpload = torrent.Monitor.UploadSpeed > 1000000 ? torrent.Monitor.UploadSpeed / 1000000 + " Mb/s" : torrent.Monitor.DownloadSpeed / 1000 + " kb/s";
                items.Add(new TorrentInformation()
                {
                    Title = torrent.Torrent.Name, Completion = Convert.ToInt32(Math.Floor(torrent.Progress)),
                    DownSpeed = txtDownload,
                    UpSpeed = txtUpload,
                    Status = state
                });
            }
            DgOrderCount.Items.Refresh();
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
                torrentSelected = CellValue;
                foreach (TorrentManager torrent in engineManager.managers)
                {
                    if(torrent.Torrent.Name == CellValue)
                    {
                        List<string> listName = new List<string>();
                        foreach (TorrentFile file in torrent.Torrent.Files)
                        {
                            listName.Add(file.Path);
                        }
                        ObservableCollection<string> oList;
                        oList = new ObservableCollection<string>(listName);
                        listBoxFiles.DataContext = oList;
                        Binding binding = new Binding();
                        listBoxFiles.SetBinding(ItemsControl.ItemsSourceProperty, binding);
                        lbl_name.Content = "Name : " + torrent.Torrent.Name;
                        lbl_size.Content = "Size : " + torrent.Torrent.Size;
                        lbl_download.Content = "Download : " + torrent.Monitor.DownloadSpeed;
                        lbl_upload.Content = "Upload : " + torrent.Monitor.UploadSpeed;
                        lbl_peers.Content = "Seeder : " + torrent.Peers.Seeds;
                        lbl_status.Content = "Status : " + torrent.SavePath;
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

        private void btn_Validate_Click(object sender, RoutedEventArgs e)
        {
            createTorrent.ValidateTorrent();
        }

        private void btn_Start_Click(object sender, RoutedEventArgs e)
        {
            engineManager.StartTorrent(torrentSelected);
        }

        private void btn_Pause_Click(object sender, RoutedEventArgs e)
        {
            engineManager.StopTorrent(torrentSelected);
        }

        private void btn_Move_Click(object sender, RoutedEventArgs e)
        {
            MyPopup.IsOpen = true;
        }

        private void btn_PopupCancel_Click(object sender, RoutedEventArgs e)
        {
            engineManager.Test(torrentSelected);
            MyPopup.IsOpen = false;
        }

        private void btn_PopupValidate_Click(object sender, RoutedEventArgs e)
        {
            engineManager.MoveTorrent(torrentSelected, txt_Move.Text);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            popupDelete.IsOpen = true;
        }

        private void btn_DeleteTorrent_Click(object sender, RoutedEventArgs e)
        {
            popupDelete.IsOpen = false;
            engineManager.DeleteTorrent(torrentSelected);
        }

        private void btn_DeleteData_Click(object sender, RoutedEventArgs e)
        {
            popupDelete.IsOpen = false;
            engineManager.DeleteData(torrentSelected);
        }

        private void btn_DeleteCancel_Click(object sender, RoutedEventArgs e)
        {
            popupDelete.IsOpen = false;
        }


    }
}

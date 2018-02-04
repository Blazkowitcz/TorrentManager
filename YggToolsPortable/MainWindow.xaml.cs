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
            items.Add(new TorrentInformation() { Title = "Complete this WPF tutorial", Completion = 45 });
            lbTodoList.ItemsSource = items;

            //while (manager.State != TorrentState.Stopped && manager.State != TorrentState.Paused)
            //{
            //    Console.WriteLine(manager.Monitor.UploadSpeed);
            //    System.Threading.Thread.Sleep(1000);
            //}
        }

    }


}

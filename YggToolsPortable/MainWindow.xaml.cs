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
        TorrentManager manager;

        public MainWindow()
        {
            InitializeComponent();
            SetupEngine();
            LoadTorrent();
            StartTorrents();
            List<TorrentInformation> items = new List<TorrentInformation>();
            items.Add(new TorrentInformation() { Title = "Complete this WPF tutorial", Completion = 45 });
            lbTodoList.ItemsSource = items;

            //while (manager.State != TorrentState.Stopped && manager.State != TorrentState.Paused)
            //{
            //    Console.WriteLine(manager.Monitor.UploadSpeed);
            //    System.Threading.Thread.Sleep(1000);
            //}

        }

        void SetupEngine()
        {
            EngineSettings settings = new EngineSettings();
            settings.AllowedEncryption = ChooseEncryption();
            settings.PreferEncryption = true;
            settings.SavePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Torrents");
            Console.WriteLine(settings.SavePath);
            settings.GlobalMaxUploadSpeed = 200 * 1024;
            engine = new ClientEngine(settings);
            engine.ChangeListenEndpoint(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6969));
        }

        EncryptionTypes ChooseEncryption()
        {
            EncryptionTypes encryption;
            // This completely disables connections - encrypted connections are not allowed
            // and unencrypted connections are not allowed
            encryption = EncryptionTypes.None;

            // Only unencrypted connections are allowed
            encryption = EncryptionTypes.PlainText;

            // Allow only encrypted connections
            encryption = EncryptionTypes.RC4Full | EncryptionTypes.RC4Header;

            // Allow unencrypted and encrypted connections
            encryption = EncryptionTypes.All;
            encryption = EncryptionTypes.PlainText | EncryptionTypes.RC4Full | EncryptionTypes.RC4Header;

            return encryption;
        }

        void LoadTorrent()
        {
            Torrent torrent = Torrent.Load("C:\\Users\\Jacky-Marley\\Downloads\\American.torrent");
            foreach (TorrentFile file in torrent.Files)
            {
                file.Priority = Priority.DoNotDownload;
            }
            torrent.Files[0].Priority = Priority.Highest;
            try
            {
                torrent.Files[1].Priority = Priority.Normal;
            }
            catch { }
            manager = new TorrentManager(torrent, "DownloadFolder", new TorrentSettings());
            managers.Add(manager);
            engine.Register(manager);
            PiecePicker picker = new StandardPicker();
            picker = new PriorityPicker(picker);
            manager.ChangePicker(picker);
        }

        void StartTorrents()
        {
            engine.StartAll();
        }
    }


}

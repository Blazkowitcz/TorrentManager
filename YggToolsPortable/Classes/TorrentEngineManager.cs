using MonoTorrent.Client;
using MonoTorrent.Client.Encryption;
using MonoTorrent.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace YggToolsPortable.Classes
{
    class TorrentEngineManager
    {
        ClientEngine engine;
        public List<TorrentManager> managers = new List<TorrentManager>();
        public TorrentManager manager;
        MainWindow mainWindow;

        public TorrentEngineManager(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            SetupConfig();
            SetupEngine();
            StartTorrents();
        }

        void SetupEngine()
        {
            EngineSettings settings = new EngineSettings();
            settings.GlobalMaxDownloadSpeed = 999999999;
            settings.GlobalMaxUploadSpeed = 999999999;
            settings.AllowedEncryption = ChooseEncryption();
            settings.PreferEncryption = true;
            settings.SavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Torrents");
            Console.WriteLine(settings.SavePath);
            engine = new ClientEngine(settings);
            engine.ChangeListenEndpoint(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6969));
            
        }

        void SetupConfig()
        {
            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TorrentManager"))
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TorrentManager");
                File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TorrentManager\\Torrents.json", "");
            }
        }

        EncryptionTypes ChooseEncryption()
        {
            EncryptionTypes encryption;
            encryption = EncryptionTypes.None;
            encryption = EncryptionTypes.PlainText;
            encryption = EncryptionTypes.RC4Full | EncryptionTypes.RC4Header;
            encryption = EncryptionTypes.All;
            encryption = EncryptionTypes.PlainText | EncryptionTypes.RC4Full | EncryptionTypes.RC4Header;

            return encryption;
        }

        public void SaveTorrent()
        {
            File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TorrentManager\\Torrents.json", String.Empty);
            List<TorrentScheme> listTorrentScheme = new List<TorrentScheme>();
            foreach (TorrentManager torrent in managers)
            {
                TorrentScheme tor = new TorrentScheme { Name = torrent.Torrent.Name, Path = torrent.SavePath };
                listTorrentScheme.Add(tor);
            }
            string torrentsSerialized = JsonConvert.SerializeObject(listTorrentScheme);
            File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TorrentManager\\Torrents.json", torrentsSerialized);
        }

        public void AutoStartTorrent()
        {
            var results = JsonConvert.DeserializeObject<List<TorrentScheme>>(File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TorrentManager\\Torrents.json"));
            DirectoryInfo directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "\\TorrentStorage\\");
            foreach (TorrentScheme file in results)
            { 
                LaunchTorrent(directory + "\\" + file.Name + ".torrent");
            }
            mainWindow.UpdateList();
            mainWindow.StartTimer();
        }

        void LaunchTorrent(string path)
        {
            Torrent torrent = Torrent.Load(path);
            foreach (TorrentFile file in torrent.Files)
            {
                file.Priority = Priority.Normal;
            }
            torrent.Files[0].Priority = Priority.Highest;
            manager = new TorrentManager(torrent, "DownloadFolder", new TorrentSettings());
            managers.Add(manager);
            engine.Register(manager);
            PiecePicker picker = new StandardPicker();
            picker = new PriorityPicker(picker);
            manager.ChangePicker(picker);
            engine.StartAll();
        }

        public void AddTorrent(string path)
        {
            LaunchTorrent(path);
            string fileName = Path.GetFileName(path);
            SaveTorrent();
            File.Copy(path, AppDomain.CurrentDomain.BaseDirectory + "\\TorrentStorage\\" + fileName, true);
        }

        public void StartTorrents()
        {
            engine.StartAll();
        }

        public ClientEngine getEngine()
        {
            return engine;
        }

        public TorrentManager getTorrentManager()
        {
            return manager;
        }

        public void StartTorrent(string torrentName)
        {
            foreach (TorrentManager torrent in managers)
            {
                if (torrent.Torrent.Name == torrentName)
                {
                    torrent.Start();
                }
            }
        }

        public void StopTorrent(string torrentName)
        {
            foreach (TorrentManager torrent in managers)
            {
                if (torrent.Torrent.Name == torrentName)
                {
                    torrent.Pause();
                }
            }
        }

        public void MoveTorrent(string torrentName, string path)
        {
            foreach (TorrentManager torrent in managers)
            {
                
                if (torrent.Torrent.Name == torrentName)
                {
                    try
                    {
                        torrent.MoveFiles(path, true);
                        ChangeTorrent(torrent, path);
                    }
                    catch
                    {
                        torrent.Stop();
                        MessageBox.Show("Torrent Stopping, move only when the state of the torrent is stopped");
                    }

                }
            }
        }

        private void ChangeTorrent(TorrentManager torrent, string path)
        {
            Torrent torrents = Torrent.Load(path);
            foreach (TorrentFile file in torrents.Files)
            {
                file.Priority = Priority.Normal;
            }
            torrent = new TorrentManager(torrents, path, new TorrentSettings());
        }

        public void Test(string torrentName)
        {
            foreach (TorrentManager torrent in managers)
            {
                if (torrent.Torrent.Name == torrentName)
                {
                    torrent.Stop();
                }
            }
        }

        public void DeleteTorrent(string torrentName)
        {
            foreach (TorrentManager torrent in managers)
            {
                if (torrent.Torrent.Name == torrentName)
                {
                    File.Delete(torrent.Torrent.TorrentPath);
                    managers.Remove(torrent);
                    break;
                }
            }
        }

        public void DeleteData(string torrentName)
        {
            foreach (TorrentManager torrent in managers)
            {
                if (torrent.Torrent.Name == torrentName)
                {
                    torrent.Stop();
                    string torrentFilePath = torrent.Torrent.TorrentPath;
                    string torrentDataPath = AppDomain.CurrentDomain.BaseDirectory + "\\DownloadFolder\\" + torrent.Torrent.Name;
                    managers.Remove(torrent);
                    File.Delete(torrentFilePath);
                    Task.Run(() => DeleteFile(torrentDataPath));
                    managers.Remove(torrent);
                    break;
                }
            }
        }

        private void DeleteFile(string path)
        {
            bool fileDeleted = false;
            if (File.Exists(path))
            {
                while (!fileDeleted)
                {
                    try
                    {
                        File.Delete(path);
                        fileDeleted = true;
                    }
                    catch{}
                }
            }
            else if (Directory.Exists(path))
            {
                while (!fileDeleted)
                {

                    try
                    {
                        Directory.Delete(path, true);
                        fileDeleted = true;
                    }
                    catch{}
                }
                
            }
        }


    }
}

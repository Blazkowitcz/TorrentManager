using MonoTorrent.Client;
using MonoTorrent.Client.Encryption;
using MonoTorrent.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace YggToolsPortable.Classes
{
    class TorrentEngineManager
    {
        ClientEngine engine;
        public List<TorrentManager> managers = new List<TorrentManager>();
        public TorrentManager manager;

        public TorrentEngineManager()
        {
            SetupEngine();
            StartTorrents();
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

        public void AddTorrent(String path)
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


    }
}

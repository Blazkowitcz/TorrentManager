using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace YggToolsPortable.Classes
{
    class ManageInformations
    {
        public MainWindow mainWindows;
        public UserInformations userInformations;
        string path = AppDomain.CurrentDomain.BaseDirectory + "\\conf.xml";
        string directoryPath = AppDomain.CurrentDomain.BaseDirectory + "\\TorrentStorage";
        public ManageInformations(MainWindow mainWindows)
        {
            this.mainWindows = mainWindows;
            bool exist = File.Exists(path);
            if (exist)
            {
                XmlSerializer xs = new XmlSerializer(typeof(UserInformations));
                using (StreamReader rd = new StreamReader(path))
                {
                    UserInformations p = xs.Deserialize(rd) as UserInformations;
                    mainWindows.txt_nick.Text = p.Nickname;
                }

            }
            else
            {
                Directory.CreateDirectory(directoryPath);
                UserInformations userInfo = new UserInformations();
                userInfo.Nickname = "Bobby-Ryan";
                userInfo.Ratio = "13.15";
                XmlSerializer xs = new XmlSerializer(typeof(UserInformations));
                using (StreamWriter wr = new StreamWriter(path))
                {
                    xs.Serialize(wr, userInfo);
                }
            }
        }
    }
}

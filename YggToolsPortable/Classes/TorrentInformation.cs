using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YggToolsPortable.Classes
{
    class TorrentInformation
    {
        public string Title { get; set; }
        public int Completion { get; set; }
        public int DownSpeed { get; set; }
        public int UpSpeed { get; set; }
    }
}

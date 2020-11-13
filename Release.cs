using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JX3SyncAssistant
{
    class Release
    {
        public Dictionary<string, string>[] assets { get; set; }
        public string body { get; set; }
        public DateTime created_at { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string tag_name { get; set; }
        public string target_commitish { get; set; }
        public bool prerelease { get; set; }
    }
}

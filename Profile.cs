using System;
using System.Collections.Generic;
namespace JX3SyncAssistant
{
    class Profile
    {
        public string version { get; set; }
        public string[] contains { get; set; }
        public Dictionary<string, string[]> files { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kalkul.Models
{
    public class DllFile
    {
        public string opValue { get; set; }
        public string assemblyFullName { get; set; }
        public string typeName { get; set; }
        public string dllFileName { get; set; }
        public string path { get; set; }
        public long size { get; set; }
        public DateTime created { get; set; }
    }
}

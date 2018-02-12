using CalcOperator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Kalkul.Models
{
    public class AssemblyItem : IDisposable
    {
        public Assembly assembly { get; internal set; }
        public string assemblyTypeName { get; internal set; }
        public string assemblyName { get; internal set; }
        public string assemblyFullName { get; internal set; }
        public string assemblyFilePath { get; internal set; }
        public IGeneralOperator iOperator { get; internal set; }
        public AssemblyItem() { }

        public void Dispose()
        {
            this.iOperator = null;
            this.Dispose();
        }
    }
}

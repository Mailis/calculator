using CalcOperator;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;

namespace Kalkul.Managers
{
    public class TypeObserverManager
    {
        string dllDirName = "Dynamic_dll";
        string restoreDllDirName = "Dynamic_dll_backup";
        public string dllDirPath;
        public string restoreDllDirPath;
        public DLLmanager dllMan;
        public Dictionary<string, IGeneralOperator> typesList;

        //public TypeObserverManager()
        //{
        //    this.dllDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dllDirName);
        //    if (!Directory.Exists(dllDirPath))
        //    {
        //        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dllDirName);
        //        //create dierctory
        //        Directory.CreateDirectory(dllDirPath);
        //    }
        //    this.dllMan = new DLLmanager(dllDirPath);
        //}


        public TypeObserverManager(IHostingEnvironment _hostingEnvironment)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;//.ContentRootPath;//
            this.dllDirPath = Path.Combine(webRootPath, dllDirName);
            this.restoreDllDirPath = Path.Combine(webRootPath, restoreDllDirName);
            //create dierctories, if these dont exist
            createDir(dllDirPath);
            createDir(restoreDllDirPath);
            this.dllMan = new DLLmanager(dllDirPath);
        }


        //create dierctory
        private void createDir(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }


        public void registerOperators()
        {
            //amongst other things, registers uploaded operator types
            this.dllMan.registerAllOperatorAssemblies();
            typesList = TypeObserver.getInstance().getAllTypes();
        }
    }
}

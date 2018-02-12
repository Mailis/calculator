using CalcOperator;
using Kalkul.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Kalkul.Managers
{
    public class DLLmanager
    {
        //path to folder for DLLs that can be added/removed
        string dllDirPath;
        public List<AssemblyItem> assemblyItems;

        List<string> allowedBaseTypes = new List<string>() { "IGeneralOperator", "IOperator" };

        public DLLmanager(string _dllDirPath)
        {
            this.dllDirPath = _dllDirPath;// Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dllDirName);
            assemblyItems = new List<AssemblyItem>();

        }



        public AssemblyItem getIOpertorAssemblyInfo(string dllFilePath)
        {
            AssemblyItem _assemblyItem = null;
            try
            {
                Assembly assembly = Assembly.Load(File.ReadAllBytes(dllFilePath));
                foreach (Type type in assembly.GetTypes())
                {
                    //check if object in an assembly implements IGeneralOperator abstract class
                    if (extendsIOperator(type))
                    {
                        _assemblyItem = new AssemblyItem();
                        _assemblyItem.assembly = assembly;
                        _assemblyItem.assemblyFilePath = dllFilePath;
                        string objName = (assembly.FullName).Split(',')[0];
                        _assemblyItem.assemblyName = objName;
                        _assemblyItem.assemblyTypeName = type.Name;
                        //dll name + class name
                        _assemblyItem.assemblyFullName = type.FullName;
                        _assemblyItem.iOperator = (IGeneralOperator)assembly.CreateInstance(type.FullName);
                    }
                }
                assembly = null;
            }
            catch
            {
                return null;
            }

            return _assemblyItem;
        }



        /**
         * populates dictionary 
         * Dictionary<string, IGeneralOperator>
         * that holds IGeneralOperator class and 
         * is accessible by class value (string operator sign)
         **/
        public void registerAllOperatorAssemblies()
        {
            if (Directory.Exists(dllDirPath))
            {
                IEnumerable<string> dllFiles = Directory.EnumerateFiles(dllDirPath, "*.dll");
                Parallel.ForEach(dllFiles, dllFilePath =>
                {
                    AssemblyItem assemblyItem = getIOpertorAssemblyInfo(dllFilePath);
                    if (assemblyItem != null)
                    {
                        this.assemblyItems.Add(assemblyItem);
                    }
                });
            }
        }

        /**
         * checks if class in an assembly extends IGeneralOperator abstract class
         **/
        public bool extendsIOperator(Type type)
        {
            bool isIOPtype = false;
            if (this.allowedBaseTypes.Contains(type.UnderlyingSystemType.BaseType.Name))
            {
                isIOPtype = true;
            }
            return isIOPtype;
        }


        public AssemblyItem getAssemblyItemByOperatorValue(string opValue)
        {
            AssemblyItem item = null;
            if (this.assemblyItems.Count() > 0)
            {
                foreach (AssemblyItem ai in this.assemblyItems)
                {
                    if (ai.iOperator.value.Equals(opValue))
                    {
                        item = ai;
                        break;
                    }
                }
            }
            return item;
        }


        public IGeneralOperator getIOperatorFromDllFilePath(string dllFilePath)
        {
            IGeneralOperator iopExtender = null;
            AssemblyItem assemblyItem = getIOpertorAssemblyInfo(dllFilePath);
            if (assemblyItem != null)
            {
                //while instatiation, _operator registers its value into CalcOperator.TypeObserver
                iopExtender = oneWaycreatingIOperator(assemblyItem);
            }
            return iopExtender;
        }


        public IGeneralOperator oneWaycreatingIOperator(AssemblyItem assemblyItem)
        {
            IGeneralOperator iopExtender = null;
            if (assemblyItem != null)
            {
                Assembly ass = assemblyItem.assembly;
                string assName = assemblyItem.assemblyFullName;
                iopExtender = (IGeneralOperator)ass.CreateInstance(assName);
            }
            return iopExtender;
        }

    }
}

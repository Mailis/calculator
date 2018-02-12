using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Kalkul.Managers;
using Kalkul.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Reflection;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Kalkul.Controllers
{
    [Route("api/[controller]")]
    public class FilesController : Controller
    {
        //allowed file types
        string[] allowedExtensions = new[] { ".dll", ".DLL" };
        TypeObserverManager typeMan;
        public IHostingEnvironment environment;

        public FilesController(IHostingEnvironment _environment)
        {
            this.environment = _environment;
            this.typeMan = new TypeObserverManager(environment);
        }

        #region Get
        // GET: api/values
        [HttpGet]
        public IEnumerable<DllFile> Get()
        {
            List<DllFile> uploadedFiles = new List<DllFile>();
            // Process the list of files found in the directory.
            this.typeMan.registerOperators();
            List<AssemblyItem> assemblyItems = typeMan.dllMan.assemblyItems;
            if (assemblyItems.Count() > 0)
            {
                uploadedFiles = (from ai in assemblyItems
                                 orderby ai.iOperator.orderAtGui
                                 select new DllFile
                                 {
                                     opValue = ai.iOperator.value,
                                     assemblyFullName = ai.assemblyFullName,
                                     typeName = ai.assemblyTypeName,
                                     path = ai.assemblyFilePath,
                                     dllFileName = ai.assemblyName + "." + ai.assemblyFilePath.Split('.').Last()
                                 }).ToList();
            }


                return uploadedFiles;
        }
        #endregion

        #region Restore
        /*
         * restoring files:
         * copys or overwrites all files from backup folder to 'Dynamic_dll' folder.
         * User cannot access to backup folder
         */
        // GET api/values/5
        [HttpPost]
        [Route("restore")]
        public ActionResult Restore()
        {
            var dllDirPath = this.typeMan.dllDirPath;
            var restoreDllDirPath = this.typeMan.restoreDllDirPath;
            string[] fileEntries = Directory.GetFiles(restoreDllDirPath);
            List<string> copySuccess = new List<string>();
            foreach (string filePath in fileEntries)
            {
                try {
                     string fName = Path.GetFileName(filePath);
                    // Will overwrite if the destination file already exists.
                    System.IO.File.Copy(Path.Combine(restoreDllDirPath, fName), Path.Combine(dllDirPath, fName), true);
                    copySuccess.Add(fName);
                }
                catch
                {
                    continue;
                }
            }
            return Json(copySuccess);
        }
        #endregion

        #region Post
        [HttpPost]
        public async Task<IActionResult> Post()// void Post()//
        {
            var dllDirPath = this.typeMan.dllDirPath;
            Dictionary<string, bool> fileUploaded = new Dictionary<string, bool>();
            var httpRequest = HttpContext.Request;
            var files = httpRequest.Form.Files;
            foreach (var file in files)
            {
                string fileName = file.FileName;
                var filepath = Path.Combine(dllDirPath, fileName);
                if (isDllFile(file))
                {
                    using (var fileStream = new FileStream(filepath, FileMode.Create, FileAccess.ReadWrite, FileShare.Delete))
                    {
                        await file.CopyToAsync(fileStream);
                        fileUploaded.Add(fileName, true);
                    }
                    //the file must extend IGeneralOperator or IOperator
                    fileExtendsRequiredAbstractClasses(fileName);
                }
                else
                {
                    fileUploaded.Add(fileName, false);
                    continue;
                }
            }
            return Json(fileUploaded);
        }
        #endregion



        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        

        #region Delete
        // DELETE api/values/fName
        [HttpDelete("{fName}")]
        public IActionResult Delete(string fName)
        {
            string result = "";
            if (fName != "undefined")
            {
                string filePath = Path.Combine(this.typeMan.dllDirPath, fName);
                //string webRootPath = this.environment.WebRootPath;//.ContentRootPath;//
                //string dllDirPath = Path.Combine(webRootPath, "dynamic");
                //DirectoryInfo di = new DirectoryInfo(dllDirPath);
                typeMan.registerOperators();
                DirectoryInfo di = new DirectoryInfo(typeMan.dllDirPath);
                FileInfo fileEntry = di.GetFiles(fName).First();
                bool isLockedFile = IsFileLocked(fileEntry);
                if (!isLockedFile)
                {
                    fileEntry.Delete();
                    result = "Successfully deleted a file " + fName + ".";
                }
                else
                {
                    result = fName + " is in use.";
                }
                
            }
            return Json(result);
        }
        #endregion


        #region Helpers

        private bool  fileExtendsRequiredAbstractClasses(string fName)
        {
            bool fileIsCorrect = false;
            string dllFilePath = Path.Combine(this.typeMan.dllDirPath, fName);
            string dllRestorFilePath = Path.Combine(this.typeMan.restoreDllDirPath, fName);
            //the file must extend IGeneralOperator or IOperator
            Assembly assem_temp = Assembly.Load(System.IO.File.ReadAllBytes(dllFilePath));
            foreach (Type type in assem_temp.GetTypes())
            {
                try
                {
                    if (this.typeMan.dllMan.extendsIOperator(type))
                    {
                        //file extends required abstract classes
                        //copy it to backup folder also, but dont override
                        System.IO.File.Copy(dllFilePath, dllRestorFilePath, false);
                        fileIsCorrect = true;
                        break;
                    }
                    else
                    {
                        //delete incorrect file
                        System.IO.File.Delete(dllFilePath);
                        fileIsCorrect = false;
                    }
                }
                catch { fileIsCorrect = false; }
            }
            return fileIsCorrect;
        }


        private bool isDllFile(IFormFile file)
        {
            bool isFile = (file != null && file.Length > 0) ? true : false;
            if (isFile)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!this.allowedExtensions.Contains(extension))
                {
                    isFile = false;
                }
            }
            return isFile;
        }

        protected virtual bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.Delete);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }
#endregion
    }
}

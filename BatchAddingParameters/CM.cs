using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.IO;
using ZetaLongPaths;

namespace BatchAddingParameters
{
    public static class CM
    {
        static List<string> PathToFamilyList;
        public static List<string> CreatePathArray(string path, bool withSubFolders)
        {
            PathToFamilyList = new List<string>();
            FileAttributes attributes = File.GetAttributes(path);

            //if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            //{
            //    Console.WriteLine("read-only file");
            //}
            //else
            //{
            //    Console.WriteLine("not read-only file");
            //}
            if (path.Contains(".rfa"))
            {
                PathToFamilyList.Add(path);
                return PathToFamilyList;
            }
            else
            {
                if (attributes == FileAttributes.Directory)
                {
                    if (withSubFolders)
                    {
                        AddPathsAndSubpathsToPathToFamilyList(path);
                    }
                    else
                    {
                        AddPathsToPathToFamilyList(path);
                    }
                }
                
            }
                
            return PathToFamilyList;
        }
        private static void AddPathsToPathToFamilyList(string targetDirectory)
        {
            //var folderPath = new ZlpDirectoryInfo(targetDirectory);
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (var filePath in fileEntries)
            {
                if (filePath.ToString().Contains(".rfa"))
                {
                    PathToFamilyList.Add(filePath);
                }
            }


        }
        private static void AddPathsAndSubpathsToPathToFamilyList(string targetDirectory)
        {
            //var folderPath = new ZlpDirectoryInfo(targetDirectory);
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (var filePath in fileEntries)
            {
                if (filePath.ToString().Contains(".rfa"))
                {
                    PathToFamilyList.Add(filePath);
                }
            }
            //foreach (var filePath in folderPath.GetFiles())
            //{
            //    if (filePath.ToString().Contains(".rfa"))
            //    {
            //        PathToFamilyList.Add(filePath.GetFullPath().ToString().Replace(@"\\?\UNC\", @"\\"));
            //    }
            //}

            var subfolderPaths = Directory.GetDirectories(targetDirectory);                         //folderPath.GetDirectories();
            foreach (var subfolderPath in subfolderPaths)
            {
                AddPathsAndSubpathsToPathToFamilyList(subfolderPath);                                 //.GetFullPath().ToString().Replace(@"\\?\UNC\", @"\\"));
            }


        }
        public static string DeleteBackupFilesOf(ZlpDirectoryInfo dirPath, string docName)
        {
            string output;
            var fileEntries = dirPath.GetFiles();
            try
            {
                foreach (var fileName in fileEntries)
                {
                    if ( (fileName.FullName.Contains($"{docName}")) && (fileName.FullName.Contains(".00")) )
                        fileName.Delete();
                }
                output = "";
            }
            catch (Exception e)
            {
                output = " - копию файла невозможно удалить по причине: " + e.ToString() + "\n";
            }
            return output;
        }
        public static string SaveAndCloseDoc(Document doc)
        {
            string output;
            try
            {
                var docPath = new ZlpFileInfo(doc.PathName);
                var dirPath = docPath.Directory;

                string docIsClosed = "";
                string docName = doc.Title + ".rfa";
                if (doc.Close(true))
                    docIsClosed = $" (файл {docName} закрыт) ";
                else
                    docIsClosed = $" (файл {docName} остался открытым) ";
                output = docIsClosed;


                DeleteBackupFilesOf(dirPath, docName.Replace(".rfa", ""));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return output;
        }
        public static string SaveAndCloseDocSimple(Document doc)
        {
            string output;
            try
            {
                var docPath = new ZlpFileInfo(doc.PathName);
                var dirPath = docPath.Directory;

                string docIsClosed = "";
                string docName = doc.Title + ".rfa";
                if (doc.Close(true))
                    docIsClosed = "";
                else
                    docIsClosed = "<-";
                output = docIsClosed;

                DeleteBackupFilesOf(dirPath, docName.Replace(".rfa", ""));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return output;
        }
        public static string CloseDoc(Document doc)
        {
            string output;
            try
            {
                var docPath = new ZlpFileInfo(doc.PathName);
                var dirPath = docPath.Directory;

                string docIsClosed = "";
                string docName = doc.Title + ".rfa";
                if (doc.Close())
                    docIsClosed = $" (файл {docName} закрыт) ";
                else
                    docIsClosed = $" (файл {docName} остался открытым) ";
                output = docIsClosed;

                DeleteBackupFilesOf(dirPath, docName.Replace(".rfa", ""));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return output;
        }
        public static string CloseDocSimple(Document doc)
        {
            string output;
            try
            {
                var docPath = new ZlpFileInfo(doc.PathName);
                var dirPath = docPath.Directory;

                string docIsClosed = "";
                string docName = doc.Title + ".rfa";
                if (doc.Close())
                    docIsClosed = "";
                else
                    docIsClosed = "<-";
                output = docIsClosed;

                DeleteBackupFilesOf(dirPath, docName.Replace(".rfa", ""));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return output;
        }
    }

}

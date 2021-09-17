using Autodesk.Revit.DB;
using System;
//using System.IO;
using ZetaLongPaths;

namespace BatchAddingParameters
{
    public static class CM
    {
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

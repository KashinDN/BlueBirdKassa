using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;


namespace BlueBirdKassa
{
    public class FileToLoad
    {
        private string errorMessage;
        public string ErrMessage { get { return errorMessage; } }
        public string FileName { get; set; }

        public XDocument checkLentaDocument { get; set; }
        /// <summary>
        /// получим все записи с указанной датой продаж
        /// </summary>
        /// <param name="saleDate">Дата продаж</param>
        /// <returns></returns>
        public IEnumerable<XElement> GetCheckItems(string saleDate)
        {
            IEnumerable<XElement> checkItems = null;
            if (checkLentaDocument != null)
            {
                try
                {
                    checkItems = from z in checkLentaDocument.Root.Elements("checkItem").Where(z => z.Element("saledate").Value.ToUpper().Equals(saleDate.ToUpper()))
                            select z;
                }
                catch (Exception e)
                { }
            }
            return checkItems;
        }
        /// <summary>
        /// получим файл с чековой лентой
        /// </summary>
        /// <param name="fileName">полное имя файла</param>
        /// <returns>true если все получилось. Иначе - false</returns>
        public bool GetFileData(string fileName)
        {
            bool result = false;

            try
            {
                //читаем данные из файла
                checkLentaDocument = XDocument.Load(fileName);
                result = true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                result = false;
            }

            return result;
        }
        ////////////////////////////////////////////////////////////////////////////
        //
        //Загрузим XML файл с продажами за смену
        //
        private bool GetFileData()
        {
            bool result = false;

            try
            {
                //читаем данные из файла
                checkLentaDocument = XDocument.Load(Path.Combine(WorkWithCheckLenta.CloseDirName, FileName));
                result = true;
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                result = false;
            }
            
            return result;
        }
        
        public bool MoveToProcessed()
        {
            bool result = false;
            if (File.Exists(Path.Combine(WorkWithCheckLenta.CloseDirName, FileName)))
            {
                try
                {
                    DirectoryInfo d = new DirectoryInfo(WorkWithCheckLenta.ArchDirName);
                    if (!d.Exists)
                    {
                        try
                        {
                            d.Create();
                            result = true;
                        }
                        catch (Exception e)
                        {
                            errorMessage = e.Message;
                            result = false;
                        }
                    }
                    else
                    {
                        result = true;
                    }
                    if (result)
                    {
                        File.Move(Path.Combine(WorkWithCheckLenta.CloseDirName, FileName), Path.Combine(WorkWithCheckLenta.ArchDirName, FileName));
                    }
                }
                catch (Exception e)
                {
                    errorMessage = e.Message;
                    result = false;
                }
            }
            else
            {
                errorMessage = "файл " + Path.Combine(WorkWithCheckLenta.CloseDirName, FileName) + " не найден";
            }
            return result;            
        }
    }
    public class WorkWithCheckLenta
    {
        private string errorMessage;
        public static string DirName { get { return "c:\\bluebirdkassa\\checklenta"; } }
        public static string CloseDirName { get { return "c:\\bluebirdkassa\\checklenta\\close"; } }
        public static string ArchDirName { get { return "c:\\bluebirdkassa\\checklenta\\arch"; } }
        public string ErrMessage { get { return errorMessage; } }
        private StreamWriter streamWriter;
        
        public bool CreateDirectory()
        {
            bool result = false;
            DirectoryInfo d = new DirectoryInfo(DirName);
            if (!d.Exists)
            {
                try
                {
                    d.Create();
                    result = true;
                }
                catch(Exception e)
                {
                    errorMessage = e.Message;
                }
            }
            else
            {
                result = true;
            }
            return result;
        }
        public bool CreateCloseDirectory()
        {
            bool result = false;
            DirectoryInfo d = new DirectoryInfo(CloseDirName);
            if (!d.Exists)
            {
                try
                {
                    d.Create();
                    result = true;
                }
                catch (Exception e)
                {
                    errorMessage = e.Message;
                }
            }
            else
            {
                result = true;
            }
            return result;
        }
        public bool OpenCheckLenta(string checkLentaFile)
        {
            bool result = false;
            bool existsFile = false;
            if (CreateDirectory())
            {
                FileInfo f = new FileInfo(Path.Combine(DirName, checkLentaFile));
                existsFile = f.Exists;
                
                    try
                    {

                        FileStream fs = f.Open(FileMode.Append, FileAccess.Write, FileShare.Read);

                        streamWriter = new StreamWriter(fs, Encoding.GetEncoding("windows-1251"));
                        if (!existsFile)
                        {
                            result = AddCheckItem("<?xml version=\"1.0\" encoding=\"windows-1251\"?><checkItems>");
                        }
                        else
                        {
                            //проверим открыта смена или нет
                            
                            result = true;
                        }
                    }
                    catch (Exception e)
                    {
                        errorMessage = e.Message;
                        result = false;
                    }
                
            }
            return result;
        }

        public bool AddCheckItem(string item)
        {
            bool result = false;
            if (streamWriter != null)
            {
                //Encoding dstEncodingFormat = Encoding.GetEncoding("windows-1251");
                //Encoding srcEncodingFormat = Encoding.UTF8;
                //byte[] originalByteString = srcEncodingFormat.GetBytes(item);
                //byte[] convertedByteString = Encoding.Convert(srcEncodingFormat, dstEncodingFormat, originalByteString);
                //string finalString = dstEncodingFormat.GetString(convertedByteString);
                streamWriter.WriteLine(item);
                streamWriter.Flush();
                result = true;
            }
            else
            {
                errorMessage = "Не удалось открыть файл";
            }
            return result;
        }

        public bool CloseCheckLenta(string checkLentaFile)
        {
            bool result = false;
            if (CreateCloseDirectory())
            {
                if (streamWriter != null)
                {
                    try
                    {
                        streamWriter.Close();
                        File.Move(Path.Combine(WorkWithCheckLenta.DirName, checkLentaFile), Path.Combine(WorkWithCheckLenta.CloseDirName, checkLentaFile));
                        result = true;
                    }
                    catch (Exception e)
                    {
                        errorMessage = e.Message;
                        result = false;
                    }
                }
            }
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace BlueBirdKassa
{
    /// <summary>
    /// Класс для работы с файлом остатков товара на складе
    /// </summary>
    public partial class LastBalance
    {
        /// <summary>
        /// файл остатков
        /// </summary>
        public static string LastBalanceFileName { get { return "c:\\bluebirdkassa\\balance\\lastbalance.xml"; } }
        /// <summary>
        /// файл текущих остатков
        /// </summary>
        public static string CurrBalanceFileName { get { return "c:\\bluebirdkassa\\balance\\currbalance.xml"; } }
        /// <summary>
        /// Документ с текущими данными по остаткам
        /// </summary>
        public XDocument CurrBalanceDocument { get; set; }
        
        /// <summary>
        /// загрузка XML файла
        /// </summary>
        /// <returns></returns>
        public string LoadBalance()
        {
            string result = "";
            if (CurrBalanceFileName != null)
            {
                try
                {
                    //читаем данные из файла
                    CurrBalanceDocument = XDocument.Load(CurrBalanceFileName);
                    result = "OK";
                }
                catch (Exception e)
                {
                    result = "Не удалось загрузить файл остатков товаров: " + e.Message;
                }
            }
            return result;
        }
        /// <summary>
        /// Применяем данные о продажах после формирования файла с остатками
        /// </summary>
        private string ApplyLastSales()
        {
            string result = "";
            //надо узнать дату формирования остатков
            string currDate = "";
            //теперь получаем все файлы продаж после этой даты
            foreach (string file in Directory.GetFiles(WorkWithCheckLenta.CloseDirName, "*.xml")) // извлекаем все файлы и кидаем их в список
            {
                try
                {
                    currDate = GetLastSaleDate();
                    FileToLoad checkLenta = new FileToLoad();
                    //Загрузим файл продаж
                    if (checkLenta.GetFileData(file))
                    {
                        do
                        {
                            foreach (XElement item in checkLenta.GetCheckItems(currDate))
                            {
                                foreach (XElement good in GetGoodById(item.Element("goodid").Value))
                                {
                                    //применяем продажи к остаткам
                                    if (!SaveGoodCount(good, Int16.Parse(item.Element("count").Value)))
                                    {
                                        result = "Не удалось обновить остатки товара: " + item.Element("goodid").Value;
                                        break;
                                    }                                    
                                }
                                if (result != "")
                                {
                                    break;
                                }
                            }
                            if (result != "")
                            {
                                break;
                            }
                            currDate = ((DateTime.Parse(currDate, new System.Globalization.CultureInfo("ru-RU", true))).AddDays(1)).ToString("dd.MM.yyyy");
                        } while (DateTime.Parse(currDate, new System.Globalization.CultureInfo("ru-RU", true)) < DateTime.Now);
                    }
                }
                catch (Exception e)
                {
                    result = "Не удалось загрузить файл текущих остатков товаров: " + e.Message;
                }
                if (result != "")
                {
                    break;
                }
            }
            //если все прошло нормально - сохраняем данные
            if (result == "")
            {
                CurrBalanceDocument.Save(CurrBalanceFileName);
                result = "OK";
            }
            return result;
        }
        /// <summary>
        /// загрузка XML файла
        /// </summary>
        /// <returns></returns>
        public string LoadCurrBalance(string depotID)
        {
            string result = "";
            
            if (CurrBalanceFileName != null)
            {
                try
                {
                    if (!File.Exists(CurrBalanceFileName))//Если файла текущих остатков нет, то его надо создать копированием последних актуальных остатков
                    {
                        File.Copy(LastBalanceFileName, CurrBalanceFileName);
                        //читаем данные из файла
                        result = LoadBalance();
                        if (result == "OK")
                        {
                            //теперь надо применить продажи
                            result = ApplyLastSales();
                        }
                    }
                    else
                    {
                        //читаем данные из файла
                        result = LoadBalance();
                    }
                }
                catch (Exception e)
                {
                    result = "Не удалось загрузить файл текущих остатков товаров: " + e.Message;
                }
            }

            if (result == "OK" && depotID != GetDepot())//файл с остатками загрузили, но он то другого склада
            {
                result = "Остатки другой точки продаж.";
            }
            return result;
        } 
        /// <summary>
        /// получим номер склада для которого сформированы остатки
        /// </summary>
        /// <returns>строка с идентификатором склада</returns>
        public string GetDepot()
        {
            string depot = "10";//По умолчанию пусть будет склад 10 - Домашние запасы
            if (CurrBalanceDocument != null)
            {
                try
                {
                    depot = CurrBalanceDocument.Root.Attribute("depot").Value;
                }
                catch (Exception e)
                {
                    depot = "10";
                }
            }
            return depot;
        }
        /// <summary>
        /// получим дату последней продажи. Это дата продаж, которые учтены при расчете остатков
        /// </summary>
        /// <returns>строка с датой</returns>
        public string GetLastSaleDate()
        {
            string lastSaleDate = "";//По умолчанию пусть будет Вчера
            if (CurrBalanceDocument != null)
            {
                try
                {
                    lastSaleDate = CurrBalanceDocument.Root.Attribute("lastSaleDate").Value;
                }
                catch (Exception e)
                {
                    lastSaleDate = DateTime.Now.AddDays(-1).ToString("dd.MM.yyyy"); 
                }
            }
            return lastSaleDate;
        }
        /// <summary>
        /// получим товар по идентификатору
        /// </summary>
        /// <param name="good_id"> идинтификатор товара</param>
        /// <returns></returns>
        public IEnumerable<XElement> GetGoodById(string good_id)
        {
            IEnumerable<XElement> goods = null;
            if (CurrBalanceDocument != null)
            {
                try
                {
                    goods = from z in CurrBalanceDocument.Root.Elements("good").Where(z => z.Element("g_id").Value.ToUpper().Equals(good_id.ToUpper()))
                            select z;
                }
                catch (Exception e)
                { }
            }
            return goods;
        }
        /// <summary>
        /// Сохраним количество товара. Получим текущее значение и отнимим проданное количество
        /// </summary>
        /// <param name="good">товар</param>
        /// <param name="cnt">проданное количество</param>
        /// <returns></returns>
        public bool SaveGoodCount(XElement good, int cnt)
        {
            bool result = false;
            
            try 
            {
                int curr_cnt = 0;
                try
                {
                    curr_cnt = Int16.Parse(good.Element("goodCnt").Value) - cnt;
                    curr_cnt = curr_cnt < 0 ? 0 : curr_cnt;
                }
                catch (Exception e)
                {
                    curr_cnt = 0;
                }
                good.SetElementValue("goodCnt", curr_cnt);
                result = true;
            }
            catch (Exception e)
            {
                result = false;
            }
            
            return result;
        }
        /// <summary>
        /// найдем товары по параметрам
        /// </summary>
        /// <param name="articul"></param>
        /// <param name="g_name"></param>
        /// <param name="gg_name"></param>
        /// <param name="docNumber"></param>
        /// <param name="docDate"></param>
        /// <returns></returns>
        public IEnumerable<XElement> GetGood(string articul, string g_name, string gg_name)
        {
            IEnumerable<XElement> goods = null;
            if (CurrBalanceDocument != null)
            {
                try
                {
                    goods = from z in CurrBalanceDocument.Root.Elements("good").Where(z => z.Element("articul").Value.ToUpper().Contains(articul.ToUpper()) && z.Element("gg_name").Value.ToUpper().Contains(gg_name.ToUpper()) && z.Element("g_name").Value.ToUpper().Contains(g_name.ToUpper()))
                            select z;
                }
                catch (Exception e)
                { }
            }
            return goods;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace BlueBirdKassa
{
    public partial class ConsignmentList
    {
        public string ConsignmentListFileName { get; set; }
        public XDocument ConsignmentListDocument { get; set; }

        ////////////////////////////////////////////////////////////////////////////
        //
        //Загрузим XML файл со списком накладных
        //
        public string LoadConsignmentList()
        {
            string result = "Не удалось найти файл со списком накладных";
            if (ConsignmentListFileName != null)
            {
                try
                {
                    //читаем данные из файла
                    ConsignmentListDocument = XDocument.Load(ConsignmentListFileName);
                    result = "OK";
                }
                catch (Exception e)
                {
                    result = "Не удалось загрузить список накладных: " + e.Message;
                }
            }
            return result;
        }
        /// <summary>
        /// получить номер склада для которого сформирован список накладных
        /// </summary>
        /// <param name="articul"></param>
        /// <param name="g_name"></param>
        /// <param name="gg_name"></param>
        /// <param name="customer"></param>
        /// <returns></returns>
        public string GetDepot()
        {
            string depot = "10";//По умолчанию пусть будет склад 1 - Домашние запасы
            if (ConsignmentListDocument != null)
            {
                try
                {
                    depot = ConsignmentListDocument.Root.Attribute("depot").Value;
                }
                catch (Exception e)
                {
                    depot = "10";
                }
            }
            return depot;
        }
        ////////////////////////////////////////////////////////////////////////////
        //
        //найдем товары по артикулу, наименованию или наименованию группы товара
        //
        public IEnumerable<XElement> GetGood(string articul, string g_name, string gg_name, string docNumber, string docDate)
        {
            IEnumerable<XElement> goods = null;
            if (ConsignmentListDocument != null)
            {
                try
                {
                    goods = from z in ConsignmentListDocument.Root.Elements("good").Where(z => z.Element("articul").Value.ToUpper().Contains(articul.ToUpper()) && z.Element("gg_name").Value.ToUpper().Contains(gg_name.ToUpper()) && z.Element("g_name").Value.ToUpper().Contains(g_name.ToUpper()) && z.Element("docDate").Value.ToUpper().Contains(docDate.ToUpper()) && z.Element("docDate").Value.ToUpper().Contains(docDate.ToUpper()))
                            select z;
                }
                catch (Exception e)
                { }
            }
            return goods;
        }
        ////////////////////////////////////////////////////////////////////////////
        //
        //найдем товары по ID
        //
        public IEnumerable<XElement> GetGoodById(string good_id)
        {
            IEnumerable<XElement> goods = null;
            if (ConsignmentListDocument != null)
            {
                try
                {
                    goods = from z in ConsignmentListDocument.Root.Elements("good").Where(z => z.Element("g_id").Value.ToUpper().Equals(good_id.ToUpper()))
                            select z;
                }
                catch (Exception e)
                { }
            }
            return goods;
        }

    }
}

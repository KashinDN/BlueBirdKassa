using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace BlueBirdKassa
{
    public partial class PriceList
    {
        public string PriceListFileName { get; set; }
        public XDocument PriceListDocument { get; set; }

        ////////////////////////////////////////////////////////////////////////////
        //
        //Загрузим XML файл с прайслистом
        //
        public string LoadPricelist()
        {
            string result = "";
            if (PriceListFileName != null)
            {
                try
                {
                    //читаем данные из файла
                    PriceListDocument = XDocument.Load(PriceListFileName);
                    result = "OK";
                }
                catch (Exception e)
                {
                    result = "Не удалось загрузить прайс лист: "+e.Message;
                }
            }
            return result;
        }
        /// <summary>
        /// получить номер склада для которого сформирован прайс лист
        /// </summary>
        /// <param name="articul"></param>
        /// <param name="g_name"></param>
        /// <param name="gg_name"></param>
        /// <param name="customer"></param>
        /// <returns></returns>
        public string GetDepot()
        {
            string depot = "10";//По умолчанию пусть будет склад 1 - Домашние запасы
            if (PriceListDocument != null)
            {
                try
                {
                    depot = PriceListDocument.Root.Attribute("depot").Value;                    
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
        public IEnumerable<XElement> GetGood(string articul, string g_name, string gg_name, string customer)
        {
            IEnumerable<XElement> goods = null;
            if (PriceListDocument != null)
            {
                try
                {
                    goods = from z in PriceListDocument.Root.Elements("good").Where(z => z.Element("articul").Value.ToUpper().Contains(articul.ToUpper()) && z.Element("gg_name").Value.ToUpper().Contains(gg_name.ToUpper()) && z.Element("g_name").Value.ToUpper().Contains(g_name.ToUpper()) && z.Element("customer").Value.ToUpper().Contains(customer.ToUpper()))
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
            if (PriceListDocument != null)
            {
                try
                {
                    goods = from z in PriceListDocument.Root.Elements("good").Where(z => z.Element("g_id").Value.ToUpper().Equals(good_id.ToUpper()))
                            select z;
                }
                catch (Exception e)
                { }
            }
            return goods;
        }
    }
}

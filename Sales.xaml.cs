using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.IO;

namespace BlueBirdKassa
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Sales : Window
    {
        /// <summary>
        /// файл с продажами. получаем его из базы
        /// </summary>
        public static string AllSalesFileName { get { return "c:\\bluebirdkassa\\checklenta\\sales.xml"; } }
        /// <summary>
        /// открытый документ с продажами
        /// </summary>
        public XDocument SalesDocument { get; set; }
        public IEnumerable<XElement> goodsSales { get; set; }
        public Sales()
        {
            InitializeComponent();
            GetSales();
        }
        public void PopulateList()
        {
            if (goodsSales != null)
            {
                dataGrid1.DataContext = goodsSales;
                dataGrid1.Focus();               
            }
        }
        private void ShowAllert(string allertText)
        {
            WindowAlerts wndOK = new WindowAlerts();
            wndOK.textBlock1.Text = allertText;
            wndOK.ShowDialog();
        }   
        private void textBox1_GotFocus(object sender, RoutedEventArgs e)
        {
            //выделить весь текст
            textBox1.SelectAll();
        }

        private void textBox2_GotFocus(object sender, RoutedEventArgs e)
        {
            //выделить весь текст
            textBox2.SelectAll();
        }

        private void textBox3_GotFocus(object sender, RoutedEventArgs e)
        {
            //выделить весь текст
            textBox3.SelectAll();
        }

        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            textBox1.Focus();
        }
        /// <summary>
        /// Нажали кнопку Поиск
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SalesDocument != null)
                {
                    goodsSales = GetSalesByParams(textBox1.Text, textBox2.Text, textBox3.Text);
                    PopulateList();
                }
            }
            catch (Exception err)
            {
                WindowAlerts wndOK = new WindowAlerts();
                wndOK.textBlock1.Text = err.Message;
                wndOK.ShowDialog();
            }
        }

        private string GetSales()
        {
            string result = "";
            //зарузим файл с продажами
            SalesDocument = XDocument.Load(AllSalesFileName);             

            //теперь получаем все файлы продаж
            foreach (string file in Directory.GetFiles(WorkWithCheckLenta.CloseDirName, "*.xml")) 
            {
                try
                {
                    FileToLoad checkLenta = new FileToLoad();
                    //Загрузим файл продаж
                    if (checkLenta.GetFileData(file))
                    {
                        //добавим его данные к общим данным о продажах
                        SalesDocument.Root.Add(checkLenta.checkLentaDocument.Root.Elements("checkItem"));
                    }
                }
                catch (Exception e)
                {
                    result = "Не удалось загрузить файл продаж "+file+": " + e.Message;
                }
                if (result != "")
                {
                    break;
                }
            }
            //если все прошло нормально - сохраняем данные
            if (result == "")
            {
                result = "OK";
                SalesDocument.Save("c:\\test.xml");
            }
            return result;
        }
        /// <summary>
        /// Получаем продажи товара по указанным аттрибутам
        /// </summary>
        /// <param name="articul"></param>
        /// <param name="g_name"></param>
        /// <param name="gg_name"></param>
        /// <returns></returns>
        public IEnumerable<XElement> GetSalesByParams(string articul, string g_name, string gg_name)
        {
            IEnumerable<XElement> sales = null;
            if (SalesDocument != null)
            {
                try
                {
                    sales = from z in SalesDocument.Root.Elements("checkItem").Where(z => z.Element("good").Value.ToUpper().Contains(articul.ToUpper()) && z.Element("good").Value.ToUpper().Contains(gg_name.ToUpper()) && z.Element("good").Value.ToUpper().Contains(g_name.ToUpper()))
                            orderby z.Element("checkid").Value descending
                            select z ;
                }
                catch (Exception e)
                { }
            }
            return sales;
        }
    }
}

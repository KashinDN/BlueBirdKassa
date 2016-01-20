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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace BlueBirdKassa
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected int npp;
        public DepotList comboBoxData { get; set; }
        public PriceList LoadedPriceList;
        public LastBalance LoadedBalance;
        public ConsignmentList LoadedConsignmentList;
        public WorkWithCheckLenta CheckLenta;
        private string checkId;
        public ObservableCollection<check_good> CurrentCheck { get; set; }
        public ObservableCollection<check_good> AllChecks { get; set; }
        private bool IsDayOpen;
        private string smenaFileName;
        private decimal checkTotal;
        private decimal smenaTotal;
        public MainWindow()
        {
            IsDayOpen = false;
            comboBoxData = new DepotList();
            //Загрузим прайс-лист
            LoadedPriceList = new PriceList();
            LoadedPriceList.PriceListFileName = "c:\\pricelist.xml";
            String result = LoadedPriceList.LoadPricelist();
            string depot = "10";
            if (result == "OK")
            {
                depot = LoadedPriceList.GetDepot();
                comboBoxData.PopulateList(Int16.Parse(depot));
            }
            //Загрузим остатки
            LoadedBalance = new LastBalance();
            string resultLoadedBalance = LoadedBalance.LoadCurrBalance(depot);
            
            LoadedConsignmentList = new ConsignmentList();
            LoadedConsignmentList.ConsignmentListFileName = "c:\\consignmentlist.xml";
            String result1 = LoadedConsignmentList.LoadConsignmentList();
            CurrentCheck = new ObservableCollection<check_good>();
            AllChecks = new ObservableCollection<check_good>();
            DataContext = this;

            InitializeComponent();
            dtpSaleDate.SelectedDate = DateTime.Now;
            textBoxID.Focus();

            checkTotal = 0;
            smenaTotal = 0;
            comboBox1.SelectedIndex = 0;
            npp = 0;
                
            
            if (result != "OK")
            {
                textBoxGoodDescription.Text = result;
            }
            if (result1 != "OK")
            {
                textBoxGoodDescription.Text = textBoxGoodDescription.Text + " " + result1;
            }
            if (resultLoadedBalance != "OK")
            {
                textBoxGoodDescription.Text = textBoxGoodDescription.Text + " " + resultLoadedBalance;
            }

        }
        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string strDepot = dtpSaleDate.SelectedDate.ToString();//comboBox1.SelectedValue.ToString();
        }

        private void buttonAddToCheck_Click(object sender, RoutedEventArgs e)
        {
            //сливаем чек в файл
            if (!IsDayOpen)
            {
                OpenSmena();
            }
            if (IsDayOpen)
            {
                check_good checkedGood = new check_good();
                long id = 0;
                if (!long.TryParse(textBoxID.Text.Trim(), out id))
                {
                    ShowAllert("Не правильное значение в поле ID. Должно быть число!");
                    return;
                }
                else
                {
                    checkedGood.ID = id;
                }
                decimal price = 0;
                if (!decimal.TryParse(textBox4.Text.Trim(), out price))
                {
                    ShowAllert("Не правильное значение в поле Цена. Должно быть число!");
                    return;
                }
                else
                {
                    checkedGood.Price = price;
                }
                int count = 0;
                if (!int.TryParse(textBox5.Text.Trim(), out count))
                {
                    ShowAllert("Не правильное значение в поле Кол-во. Должно быть целое число!");
                    return;
                }
                else
                {
                    checkedGood.Count = count;
                }
                decimal discountCost = 0;
                if (!decimal.TryParse(textBox1.Text.Trim(), out discountCost))
                {
                    ShowAllert("Не правильное значение в поле Стоимость со скидкой. Должно быть число!");
                    return;
                }
                else
                {
                    checkedGood.DiscountCost = discountCost;
                }
                checkedGood.Good = textBoxGoodDescription.Text;
                checkedGood.Cost = checkedGood.Price * checkedGood.Count;
                int discount = 0;
                if (!Int32.TryParse(textBox6.Text.Trim(), out discount))
                {
                    ShowAllert("Не правильное значение в поле Скидка. Должно быть число!");
                    return;
                }

                checkedGood.Discount = discount;

                checkedGood.CheckID = checkId + "_" + npp;

                textBox5.Text = "1";
                CurrentCheck.Add(checkedGood);
                buttonFinish.Focus();
                checkTotal += discountCost;
                npp++;
                labelItogoCheck.Content = "итого по чеку " + checkTotal.ToString();
            }
        }

        private void textBox4_GotFocus(object sender, RoutedEventArgs e)
        {
            //выделить весь текст
            textBox4.SelectAll();
        }

        private void textBox5_GotFocus(object sender, RoutedEventArgs e)
        {
            //выделить весь текст
            textBox5.SelectAll();
        }

        private void textBox6_GotFocus(object sender, RoutedEventArgs e)
        {
            //выделить весь текст
            textBox6.SelectAll();            
        }

        private void buttonFindGood_Click(object sender, RoutedEventArgs e)
        {
            //Найдем товар
            textBox4.Focus();
            FindGood findGoodWindow = new FindGood();
            findGoodWindow.Owner = this;
            findGoodWindow.Show();
            
        }

        private void buttonFinish_Click(object sender, RoutedEventArgs e)
        {
            //сливаем чек в файл
            if (!IsDayOpen)
            {
                OpenSmena();
            }
            if (IsDayOpen)
            {
                
                if (comboBox1.SelectedIndex < 0)
                {
                    ShowAllert("не выбрана точка продаж!");
                    return;
                }

                var check = from z in CurrentCheck select z;
                
                foreach (check_good checkItem in check)
                {
                    string discount = textBox6.Text.Length > 0 ? "0" : textBox6.Text;
                    if (!CheckLenta.AddCheckItem("<checkItem><checkid>" + checkItem.CheckID + "</checkid>" +
                                                 "<saledate>" + dtpSaleDate.SelectedDate.Value.ToString("dd.MM.yyyy") + "</saledate>" +
                                                 "<depotid>" + comboBox1.SelectedValue + "</depotid>" +
                                                 "<goodid>" + checkItem.ID.ToString() + "</goodid>" +
                                                 "<price>" + checkItem.Price.ToString() + "</price>" +
                                                 "<count>" + checkItem.Count.ToString() + "</count>" +
                                                 "<cost>" + checkItem.Cost.ToString() + "</cost>" +
                                                 "<checkdiscount>" + checkItem.Discount.ToString() + "</checkdiscount>" +
                                                 "<discountcost>" + checkItem.DiscountCost.ToString() + "</discountcost>" +
                                                 "<good>" + checkItem.Good + "</good></checkItem>"
                                                )
                       )
                    {
                        ShowAllert("Проблема с записью чека в файл с продажами: " + CheckLenta.ErrMessage);
                        return;
                    }
                    else
                    {
                        AllChecks.Add(checkItem);
                    }
                }
                //Запись с итогами по чеку
                check_good checkTotalData = new check_good();
                checkTotalData.Good = " итого по чеку ";
                checkTotalData.DiscountCost = checkTotal;
                AllChecks.Add(checkTotalData);

                CurrentCheck.Clear();
                buttonFindGood.Focus();
                textBox6.Text = "0";
                checkId = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + comboBox1.SelectedValue;
                labelItogoCheck.Content = "итого по чеку";
                smenaTotal += checkTotal;
                checkTotal = 0;
                textBoxID.Text = "";
                textBox4.Text = "0";
                textBox5.Text = "1";
                textBox1.Text = "0";
                textBoxGoodDescription.Text = "";
                if (expanderLenta.IsExpanded)
                {
                    label8Lenta.Content = "Итого за смену " + smenaTotal.ToString();
                }
            }
            else
            {
                ShowAllert("Не открыта смена. Нажмите кнопку 'Открыть смену'");
            }
        }
        private void OpenSmena()
        {

            smenaTotal = 0;
            if (comboBox1.SelectedIndex < 0)
            {
                ShowAllert("не выбрана точка продаж!");
                return;
            }
            CheckLenta = new WorkWithCheckLenta();
            if (CheckLenta.CreateDirectory())
            {
                smenaFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml";
                if (!CheckLenta.OpenCheckLenta(smenaFileName))
                {
                    ShowAllert("Проблема с открытием файла с продажами: " + CheckLenta.ErrMessage);
                    IsDayOpen = false;
                }
                else
                {
                    checkId = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + comboBox1.SelectedValue;
                    IsDayOpen = true;
                    //ShowAllert("Новая смена успешно открыта!");
                }
            }
            else
            {
                ShowAllert("Проблема с проверкой папки для файла с продажами: " + CheckLenta.ErrMessage);
                IsDayOpen = false;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //открыть смену
            OpenSmena();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
            smenaTotal = 0;
            AllChecks.Clear();
            label8Lenta.Content = "Итого за смену ";
            //Закрыть смену
            if (IsDayOpen)
            {
                if (CheckLenta.AddCheckItem("</checkItems>"))
                {
                    if (!CheckLenta.CloseCheckLenta(smenaFileName))
                    {
                        ShowAllert("Не удалось закрыть смену: " + CheckLenta.ErrMessage);
                    }
                    else
                    {
                        IsDayOpen = false;
                        ShowAllert("Смена успешно закрыта!");
                    }
                }
                else
                {
                    ShowAllert("Не удалось закрыть смену: " + CheckLenta.ErrMessage);
                }
            }
            else
            {
                ShowAllert("Не открыта смена.");
            }
        }
        private void ShowAllert(string allertText)
        {
            WindowAlerts wndOK = new WindowAlerts();
            wndOK.textBlock1.Text = allertText;
            wndOK.ShowDialog();
        }            

        
       
        private void CalcDiscount()
        {
            
            decimal price = 0;
            if (textBox4 != null)
            {
                if (!decimal.TryParse(textBox4.Text.Trim(), out price))
                {
                    ShowAllert("Не правильное значение в поле Цена. Должно быть число!");
                    return;
                }
            }
            int count = 0;
            if (textBox5 != null)
            {
                if (!int.TryParse(textBox5.Text.Trim(), out count))
                {
                    ShowAllert("Не правильное значение в поле Кол-во. Должно быть целое число!");
                    return;
                }
            }
            if (textBox6 != null)
            {
                if (textBox1 != null)
                {
                    int discount = 0;
                    if (!int.TryParse(textBox6.Text, out discount))
                    {
                        discount = 0;
                    }
                    textBox1.Text = Convert.ToString(Math.Floor(price * count - price * count / 100 * discount));
                }
            }
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            label8Lenta.Content = "Итого за смену " + smenaTotal.ToString();
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            label8Lenta.Content = "Итого за смену";
        }

        private void checkLenta_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            //
            if (((check_good)e.Row.Item).Good == " итого по чеку ")
            {
                e.Row.Background = (Brush)new BrushConverter().ConvertFromString("DimGray");
                e.Row.Foreground = (Brush)new BrushConverter().ConvertFromString("White");
                e.Row.FontWeight = FontWeights.Bold;
            }
            else
            {
                e.Row.Background = (Brush)new BrushConverter().ConvertFromString("White");
                e.Row.Foreground = (Brush)new BrushConverter().ConvertFromString("Black");
                e.Row.FontWeight = FontWeights.Normal;
            }
        }

       
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (IsDayOpen)
            {
                if (CheckLenta.AddCheckItem("</checkItems>"))
                {
                    if (!CheckLenta.CloseCheckLenta(smenaFileName))
                    {
                        ShowAllert("Не удалось закрыть смену: " + CheckLenta.ErrMessage);
                    }
                    else
                    {
                        IsDayOpen = false;
                        ShowAllert("Смена успешно закрыта!");
                    }
                }
                else
                {
                    ShowAllert("Не удалось закрыть смену: " + CheckLenta.ErrMessage);
                }
            }
        }

        private void textBox1_GotFocus(object sender, RoutedEventArgs e)
        {
            //выделить весь текст
            textBox1.SelectAll();
        }

        private void buttonFinish_LostFocus(object sender, RoutedEventArgs e)
        {
            buttonFindGood.Focus();
        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                buttonAddToCheck.Focus();
            }
        }

        private void textBox5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                buttonAddToCheck.Focus();
            }
        }

        private void textBox6_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                buttonAddToCheck.Focus();
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                buttonAddToCheck.Focus();
            }
        }        

        private void textBoxID_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                textBoxID.Text = textBoxID.Text.Trim();
                FindGood findGoodWindow = new FindGood();
                findGoodWindow.Owner = this;
                findGoodWindow.GetGoodById(textBoxID.Text);
                findGoodWindow.Show();
                findGoodWindow.button1.Focus();
            }
        }

        private void textBox4_LostFocus(object sender, RoutedEventArgs e)
        {
            CalcDiscount();
        }

        private void textBox5_LostFocus(object sender, RoutedEventArgs e)
        {
            CalcDiscount();
        }

        private void textBox6_LostFocus(object sender, RoutedEventArgs e)
        {
            CalcDiscount();
        }

        private void textBoxID_GotFocus(object sender, RoutedEventArgs e)
        {
            //выделить весь текст
            textBoxID.SelectAll();
        }

        private void textBoxGoodDescription_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                buttonAddToCheck.Focus();
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //отменим чек
            CurrentCheck.Clear();
            labelItogoCheck.Content = "итого по чеку";
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Consignment сonsignmentWindow = new Consignment();
            сonsignmentWindow.Owner = this;
            сonsignmentWindow.Show();
        }

        /// <summary>
        /// Открываем окно Остатки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Balance balanceWindow = new Balance();
            balanceWindow.Owner = this;
            balanceWindow.Show();
        }
        /// <summary>
        /// Открываем окно Продажи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            Sales salesWindow = new Sales();
            salesWindow.Owner = this;
            salesWindow.Show();
        }     
       
    }
}

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

namespace BlueBirdKassa
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class FindGood : Window
    {
        public IEnumerable<XElement> goods {get; set;}
        public FindGood()
        {
            InitializeComponent();
        }
        public void GetGoodById(string g_id)
        {
            MainWindow mainForm = this.Owner as MainWindow;
            goods = mainForm.LoadedPriceList.GetGoodById(g_id);
            PopulateList();
            if (mainForm.comboBox1.Items.Count == 1)
            {
                dataGrid1.Columns[6].Visibility = Visibility.Hidden;
            }
            button1.Focus();

        }
        public void PopulateList()
        {
            if (goods != null)
            {
                dataGrid1.DataContext = goods;
                dataGrid1.Focus();
            }
        }
        private void ShowAllert(string allertText)
        {
            WindowAlerts wndOK = new WindowAlerts();
            wndOK.textBlock1.Text = allertText;
            wndOK.ShowDialog();
        }   
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Finish(dataGrid1.SelectedIndex);
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

        private void textBox4_GotFocus(object sender, RoutedEventArgs e)
        {
            //выделить весь текст
            textBox4.SelectAll();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            textBox1.Focus();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainForm = this.Owner as MainWindow;
            goods = mainForm.LoadedPriceList.GetGood(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text);
            PopulateList();
            if (mainForm.comboBox1.Items.Count == 1)
            {
                dataGrid1.Columns[6].Visibility = Visibility.Hidden;
            }
            button1.Focus();
        }

        private void dataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            button1.Focus();
        }

        private void dataGrid1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Finish(dataGrid1.SelectedIndex);
        }
        private void Finish(int index)
        {
            try
            {
                XElement row = (XElement)dataGrid1.Items[index];
                MainWindow mainForm = this.Owner as MainWindow;

                if (mainForm != null && row != null)
                {
                    string decimalSep = System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
                    mainForm.textBox4.Text = row.Element("price_rub").Value + decimalSep + row.Element("price_kop").Value;
                    mainForm.textBoxID.Text = row.Element("g_id").Value;
                    mainForm.textBoxGoodDescription.Text = "Артикул: " + row.Element("articul").Value + "; " + row.Element("gg_name").Value + "; " + row.Element("g_name").Value + "; ";
                    //mainForm.buttonAddToCheck.Focus();
                    mainForm.textBox5.Focus();
                    mainForm.textBox5.SelectAll();
                }

                this.Close();
            }
            catch (Exception eee)
            {
                ShowAllert(eee.Message);
                return;
            }
        }
        private void dataGrid1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                int index = dataGrid1.SelectedIndex;
                if (index > 0)
                {
                    if (index != dataGrid1.Items.Count - 1)
                    {
                        index--;
                    }
                }
                Finish(index);
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                MainWindow mainForm = this.Owner as MainWindow;
                goods = mainForm.LoadedPriceList.GetGood(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text);
                PopulateList();
                button1.Focus();
            }
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                MainWindow mainForm = this.Owner as MainWindow;
                goods = mainForm.LoadedPriceList.GetGood(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text);
                PopulateList();
                button1.Focus();
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                MainWindow mainForm = this.Owner as MainWindow;
                goods = mainForm.LoadedPriceList.GetGood(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text);
                PopulateList();
                button1.Focus();
            }
        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                MainWindow mainForm = this.Owner as MainWindow;
                goods = mainForm.LoadedPriceList.GetGood(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text);
                PopulateList();
                button1.Focus();
            }
        }             
    }
}

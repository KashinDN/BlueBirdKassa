﻿using System;
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
    public partial class Balance : Window
    {
        public IEnumerable<XElement> goodsBalance { get; set; }
        public Balance()
        {
            InitializeComponent();
        }
        public void PopulateList()
        {
            if (goodsBalance != null)
            {
                dataGrid1.DataContext = goodsBalance;
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

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainForm = this.Owner as MainWindow;
            goodsBalance = mainForm.LoadedBalance.GetGood(textBox1.Text, textBox2.Text, textBox3.Text);
            PopulateList();            
        }
    }
}

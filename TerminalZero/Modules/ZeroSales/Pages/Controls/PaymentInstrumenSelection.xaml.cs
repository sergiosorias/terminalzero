﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZeroGUI;

namespace ZeroSales.Pages.Controls
{
    /// <summary>
    /// Interaction logic for PaymentInstrumentList.xaml
    /// </summary>
    public partial class PaymentInstrumentSelection : NavigationBasePage
    {
        public PaymentInstrumentSelection()
        {
            InitializeComponent();
        }

        private void multiOptions_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.D0:
                case Key.NumPad0:
                    paymentInstrumentsList.SelectItemByData("0");
                    e.Handled = true;
                    break;
                case Key.D1:
                case Key.NumPad1:
                    paymentInstrumentsList.SelectItemByData("1");
                    e.Handled = true;
                    break;
                case Key.D2:
                case Key.NumPad2:
                    paymentInstrumentsList.SelectItemByData("2");
                    e.Handled = true;
                    break;
                case Key.D3:
                case Key.NumPad3:
                    paymentInstrumentsList.SelectItemByData("3");
                    e.Handled = true;
                    break;
                case Key.D4:
                case Key.NumPad4:
                    paymentInstrumentsList.SelectItemByData("4");
                    e.Handled = true;
                    break;
                case Key.D5:
                case Key.NumPad5:
                    paymentInstrumentsList.SelectItemByData("5");
                    e.Handled = true;
                    break;
                case Key.D6:
                case Key.NumPad6:
                    paymentInstrumentsList.SelectItemByData("6");
                    e.Handled = true;
                    break;
                case Key.D7:
                case Key.NumPad7:
                    paymentInstrumentsList.SelectItemByData("7");
                    e.Handled = true;
                    break;
                case Key.D8:
                case Key.NumPad8:
                    paymentInstrumentsList.SelectItemByData("8");
                    e.Handled = true;
                    break;
                case Key.D9:
                case Key.NumPad9:
                    paymentInstrumentsList.SelectItemByData("9");
                    e.Handled = true;
                    break;
                case Key.Back:
                    break;
                case Key.Tab:
                    if (paymentInstrumentsList.SelectedItem == null)
                        e.Handled = true;
                    break;
            }
            if(!e.Handled) ((TextBox) sender).SelectAll();
            
        }

        private void ListNavigationControl_Loaded(object sender, RoutedEventArgs e)
        {
            multiOptions.Focus();
        }

        private void quantitySelected_GotFocus(object sender, RoutedEventArgs e)
        {
            ((TextBox) sender).SelectAll();
        }

        private void paymentInstrumentsList_ItemsLoaded(object sender, EventArgs e)
        {
            paymentInstrumentsList.SelectedIndex = 0;
        }
    }
}

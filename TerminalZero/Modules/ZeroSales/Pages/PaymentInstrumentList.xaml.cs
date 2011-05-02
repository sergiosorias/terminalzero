using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
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
using ZeroCommonClasses.Interfaces;

namespace ZeroSales.Pages
{
    /// <summary>
    /// Interaction logic for PaymentInstrumentList.xaml
    /// </summary>
    public partial class PaymentInstrumentList : ZeroGUI.ListNavigationControl
    {
        private ZeroBusiness.Entities.Data.DataModelManager _manager;
        public PaymentInstrumentList()
        {
            InitializeComponent();
            _manager = new ZeroBusiness.Entities.Data.DataModelManager();
            InitializeList(paymentInstrumentsDataGrid, _manager.PaymentInstruments);
        }

        private void multiOptions_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    if(paymentInstrumentsDataGrid.SelectedIndex>0)
                        paymentInstrumentsDataGrid.SelectedIndex = paymentInstrumentsDataGrid.SelectedIndex - 1;
                    break;
                case Key.Down:
                    if (paymentInstrumentsDataGrid.SelectedIndex < paymentInstrumentsDataGrid.Items.Count)
                        paymentInstrumentsDataGrid.SelectedIndex = paymentInstrumentsDataGrid.SelectedIndex + 1;
                    break;
                case Key.Execute:
                    int aux;
                    if(int.TryParse(multiOptions.Text,out aux))
                    paymentInstrumentsDataGrid.SelectedItem =
                        _manager.PaymentInstruments.FirstOrDefault(pi => pi.Code == aux);
                    break;
                case Key.NumPad0:
                case Key.D0:
                    paymentInstrumentsDataGrid.SelectedItem =
                        _manager.PaymentInstruments.FirstOrDefault(pi => pi.Code == 0);
                    break;
                case Key.NumPad1:
                case Key.D1:
                    paymentInstrumentsDataGrid.SelectedItem =
                        _manager.PaymentInstruments.FirstOrDefault(pi => pi.Code == 1);
                    break;
                case Key.NumPad2:
                case Key.D2:
                    paymentInstrumentsDataGrid.SelectedItem =
                        _manager.PaymentInstruments.FirstOrDefault(pi => pi.Code == 2);
                    break;
                case Key.NumPad3:
                case Key.D3:
                    paymentInstrumentsDataGrid.SelectedItem =
                        _manager.PaymentInstruments.FirstOrDefault(pi => pi.Code == 3);
                    break;
                case Key.NumPad4:
                case Key.D4:
                    paymentInstrumentsDataGrid.SelectedItem =
                        _manager.PaymentInstruments.FirstOrDefault(pi => pi.Code == 4);
                    break;
                case Key.NumPad5:
                case Key.D5:
                    paymentInstrumentsDataGrid.SelectedItem =
                        _manager.PaymentInstruments.FirstOrDefault(pi => pi.Code == 5);
                    break;
                case Key.NumPad6:
                case Key.D6:
                    paymentInstrumentsDataGrid.SelectedItem =
                        _manager.PaymentInstruments.FirstOrDefault(pi => pi.Code == 6);
                    break;
                case Key.NumPad7:
                case Key.D7:
                    paymentInstrumentsDataGrid.SelectedItem =
                        _manager.PaymentInstruments.FirstOrDefault(pi => pi.Code == 7);
                    break;
                case Key.NumPad8:
                case Key.D8:
                    paymentInstrumentsDataGrid.SelectedItem =
                        _manager.PaymentInstruments.FirstOrDefault(pi => pi.Code == 8);
                    break;
                case Key.NumPad9:
                case Key.D9:
                    paymentInstrumentsDataGrid.SelectedItem =
                        _manager.PaymentInstruments.FirstOrDefault(pi => pi.Code == 9);
                    break;
                case Key.Back:
                    break;
                default:
                    e.Handled = true;
                    break;
            }
        }

        private void paymentInstrumentsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count>0)
                SelectedItem = e.AddedItems[0] as EntityObject;
        }

        private void ListNavigationControl_Loaded(object sender, RoutedEventArgs e)
        {
            multiOptions.Focus();
        }

        
        
    }
}

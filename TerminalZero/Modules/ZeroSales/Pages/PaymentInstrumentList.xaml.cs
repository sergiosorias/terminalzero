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
using ZeroCommonClasses.Interfaces;

namespace ZeroSales.Pages
{
    /// <summary>
    /// Interaction logic for PaymentInstrumentList.xaml
    /// </summary>
    public partial class PaymentInstrumentList : UserControl, IZeroPage
    {
        public PaymentInstrumentList()
        {
            InitializeComponent();
        }

        #region Implementation of IZeroPage

        public Mode Mode { get;set;}

        public bool CanAccept(object parameter)
        {
            return true;
        }

        public bool CanCancel(object parameter)
        {
            return true;
        }

        #endregion
    }
}

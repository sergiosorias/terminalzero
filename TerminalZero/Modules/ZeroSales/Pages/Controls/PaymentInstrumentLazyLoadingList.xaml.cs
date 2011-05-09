using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using ZeroBusiness.Manager.Sale;

namespace ZeroSales.Pages.Controls
{
    /// <summary>
    /// Interaction logic for PaymentInstrumentLazyLoadingList.xaml
    /// </summary>
    public partial class PaymentInstrumentLazyLoadingList : ZeroGUI.LazyLoadingListControl
    {
        public PaymentInstrumentLazyLoadingList()
        {
            InitializeComponent();
            Loaded += (sender, e) => StartListLoad(Context.Instance.Manager.PaymentInstruments);
        }
    }
}

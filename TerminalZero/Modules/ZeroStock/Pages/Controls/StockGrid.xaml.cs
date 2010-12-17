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
using System.ComponentModel;
using ZeroCommonClasses.Interfaces;

namespace ZeroStock.Pages.Controls
{
    /// <summary>
    /// Interaction logic for StockGrid.xaml
    /// </summary>
    public partial class StockGrid : UserControl, IZeroPage
    {
        public StockGrid()
        {
            InitializeComponent();
        }

        public class Barcode
        {
            public string Code { get; set; }
            public string Product { get; set; }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                
            }
        }

        public void Add(string code)
        {
            barcodeList.Items.Add(new Barcode{Code = code, Product = code.Substring(4,3)});
        }


        #region IZeroPage Members

        public Mode Mode
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool CanAccept()
        {
            throw new NotImplementedException();
        }

        public bool CanCancel()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

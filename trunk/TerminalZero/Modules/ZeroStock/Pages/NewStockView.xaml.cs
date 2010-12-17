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

namespace ZeroStock.Pages
{
    /// <summary>
    /// Interaction logic for NewStockView.xaml
    /// </summary>
    public partial class NewStockView : UserControl, IZeroPage
    {
        public NewStockView()
        {
            InitializeComponent();
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
            return true;
        }

        public bool CanCancel()
        {
            return true;
        }

        #endregion

        private void BarCodeTextBox_BarcodeValidating(object sender, ZeroGUI.Classes.BarCodeValidationEventArgs e)
        {

        }

        private void BarCodeTextBox_BarcodeReceived(object sender, ZeroGUI.Classes.BarCodeEventArgs e)
        {
            stockGrid.Add(e.Code);
        }
    }
}

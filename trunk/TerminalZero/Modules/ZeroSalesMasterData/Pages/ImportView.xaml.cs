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

namespace ZeroMasterData.Pages
{
    /// <summary>
    /// Interaction logic for ImportView.xaml
    /// </summary>
    public partial class ImportView : UserControl, IZeroPage
    {
        public ImportView()
        {
            InitializeComponent();
        }

        #region IZeroPage Members

        ZeroCommonClasses.Interfaces.Mode _Mode = ZeroCommonClasses.Interfaces.Mode.New;
        public ZeroCommonClasses.Interfaces.Mode Mode
        {
            get
            {
                return _Mode;
            }
            set
            {
                _Mode = value;
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
    }
}

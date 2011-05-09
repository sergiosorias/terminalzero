using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZeroBusiness.Entities.Data;

namespace ZeroStock.Pages.Controls
{
    /// <summary>
    /// Interaction logic for DeliveryDocumentGrid.xaml
    /// </summary>
    public partial class DeliveryDocumentLazyLoadingList : ZeroGUI.LazyLoadingListControl
    {
        public bool NotUsedOnly { get; set; }

        public DeliveryDocumentLazyLoadingList()
        {
            InitializeComponent();
            NotUsedOnly = true;
        }
    }
}

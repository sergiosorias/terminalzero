using System;
using System.Windows;
using System.Windows.Controls;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;

namespace ZeroStock.Pages
{
    /// <summary>
    /// Interaction logic for DeliveryDocumentView.xaml
    /// </summary>
    public partial class DeliveryDocumentView : NavigationBasePage
    {
        public DeliveryDocumentView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsInDesignMode)
            {
             	dateFilter.SelectedDateFormat = DatePickerFormat.Short;
                dateFilter.DisplayDateEnd = DateTime.Now.AddDays(1);
                dateFilter.SelectedDateChanged+=dateFilter_SelectedDateChanged;
            }
        }

        private void SearchBox_Search(object sender, SearchCriteriaEventArgs e)
        {
            e.Matches = DeliveryGrid.ApplyFilter(e.Criteria, dateFilter.SelectedDate);
        }
        
        private void dateFilter_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DeliveryGrid.ApplyFilter(string.Empty, dateFilter.SelectedDate);
        }

        protected override void OnControlModeChanged(ControlMode newMode)
        {
            base.OnControlModeChanged(newMode);
            if (ControlMode == ControlMode.Selection)
            {
                DeliveryGrid.ApplyFilter(string.Empty, DateTime.Now.Date);
            }
        }
    }
}

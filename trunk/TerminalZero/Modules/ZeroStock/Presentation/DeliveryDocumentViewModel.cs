using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using ZeroBusiness.Entities.Data;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ZeroStock.Pages;
using ZeroStock.Pages.Controls;

namespace ZeroStock.Presentation
{
    public class DeliveryDocumentViewModel : ViewModelGui
    {
        #region Properties
        public DeliveryDocumentHeader SelectedDeliveryDocumentHeader { get; set; }

        private ObservableCollection<DeliveryDocumentHeader> deliveryDocumentCollection;

        public ObservableCollection<DeliveryDocumentHeader> DeliveryDocumentCollection
        {
            get { return deliveryDocumentCollection ?? (deliveryDocumentCollection = new ObservableCollection<DeliveryDocumentHeader>(ZeroBusiness.Manager.Data.BusinessContext.Instance.Model.DeliveryDocumentHeaders.Where(d => d.Used == null || d.Used.Value == false))); }
            set
            {
                if (deliveryDocumentCollection != value)
                {
                    deliveryDocumentCollection = value;
                    OnPropertyChanged("DeliveryDocumentCollection");
                }
            }
        }
        #endregion

        #region Commands
        private ICommand openNewDocumentCommand;
        public ICommand OpenNewDocumentCommand
        {
            get { return openNewDocumentCommand ?? (openNewDocumentCommand = new ZeroActionDelegate(OpenNewDocument)); }
        }

        private void OpenNewDocument(object parameter)
        {
            var det = new DocumentDeliveryDetail();
            bool? res = ZeroMessageBox.Show(det, Properties.Resources.NewDeliveryNote);
            if (res.HasValue && res.Value)
            {
                DeliveryDocumentCollection.Add(det.CurrentDocumentDelivery);
            }
        }
        #endregion

        public DeliveryDocumentViewModel(NavigationBasePage view)
            : base(view)
        {
            
        }

        #region Overrides
        public override bool CanAccept(object parameter)
        {
            bool ret = base.CanAccept(parameter);
            if (ret && View.ControlMode == ControlMode.Selection)
            {
                ret = (SelectedDeliveryDocumentHeader != null);
                if (!ret)
                {
                    MessageBox.Show("¡Por favor seleccione un documento!", "Atención", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }

            }

            return ret;
        }
        #endregion
    }
}

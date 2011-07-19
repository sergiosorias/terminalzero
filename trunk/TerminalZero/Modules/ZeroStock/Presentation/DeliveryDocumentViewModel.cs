using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ZeroStock.Pages.Controls;
using ZeroStock.Properties;

namespace ZeroStock.Presentation
{
    public class DeliveryDocumentViewModel : ViewModelGui
    {
        #region Properties
        public DeliveryDocumentHeader SelectedDeliveryDocumentHeader { get; set; }

        private ObservableCollection<DeliveryDocumentHeader> deliveryDocumentCollection;

        public ObservableCollection<DeliveryDocumentHeader> DeliveryDocumentCollection
        {
            get { return deliveryDocumentCollection ?? (deliveryDocumentCollection = LoadCollection()); }
            set
            {
                if (deliveryDocumentCollection != value)
                {
                    deliveryDocumentCollection = value;
                    OnPropertyChanged("DeliveryDocumentCollection");
                }
            }
        }

        private ObservableCollection<DeliveryDocumentHeader> LoadCollection()
        {
            if(View.ControlMode== ControlMode.Selection)
                return new ObservableCollection<DeliveryDocumentHeader>(BusinessContext.Instance.Model.DeliveryDocumentHeaders.Where(d => d.Used == null || d.Used.Value == false));
            
            return new ObservableCollection<DeliveryDocumentHeader>(BusinessContext.Instance.Model.DeliveryDocumentHeaders);
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
            bool? res = ZeroMessageBox.Show(det, Resources.NewDeliveryNote);
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
                    Terminal.Instance.CurrentClient.ShowDialog("¡Por favor seleccione un documento!",(o)=> { }, ZeroCommonClasses.GlobalObjects.MessageBoxButtonEnum.OK);
                }
            }
            return ret;
        }
        #endregion
    }
}

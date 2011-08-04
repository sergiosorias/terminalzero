using System;
using System.Collections.Generic;
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
using ZeroStock.Pages;
using ZeroStock.Pages.Controls;
using ZeroStock.Properties;

namespace ZeroStock.Presentation
{
    public class DeliveryDocumentViewModel : ViewModelGui
    {
        public class DeliveryDocumentHeaderExtended
        {
            public DeliveryDocumentHeader Header { get; set; }
            public TerminalTo TerminalDestination { get; set; }
        }
        #region Properties
        public DeliveryDocumentHeaderExtended SelectedDeliveryDocumentHeader { get; set; }

        private ObservableCollection<DeliveryDocumentHeaderExtended> deliveryDocumentCollection;

        public ObservableCollection<DeliveryDocumentHeaderExtended> DeliveryDocumentCollection
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

        private List<TerminalTo> terminals;

        private ObservableCollection<DeliveryDocumentHeaderExtended> LoadCollection()
        {
            terminals = new List<TerminalTo>(BusinessContext.Instance.Model.TerminalToes);
            IEnumerable<DeliveryDocumentHeader> quer;
            if (View.ControlMode == ControlMode.Selection)
                quer = BusinessContext.Instance.Model.DeliveryDocumentHeaders.Where(d => d.Used == null || d.Used.Value == false);
            else
                quer = BusinessContext.Instance.Model.DeliveryDocumentHeaders;

            return new ObservableCollection<DeliveryDocumentHeaderExtended>(quer.Select(BuildItem));
        }

        private DeliveryDocumentHeaderExtended BuildItem(DeliveryDocumentHeader h)
        {
            return new DeliveryDocumentHeaderExtended { Header = h, TerminalDestination = terminals.FirstOrDefault(t => t.Code == h.TerminalToCode) };
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
                DeliveryDocumentCollection.Add(BuildItem(det.CurrentDocumentDelivery));
            }
        }
        #endregion

        public DeliveryDocumentViewModel(ControlMode mode)
            : base(new DeliveryDocumentView(){ControlMode = mode})
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
                    Terminal.Instance.Client.ShowDialog("¡Por favor seleccione un documento!","Atención",(o)=> { }, ZeroCommonClasses.GlobalObjects.MessageBoxButtonEnum.OK);
                }
            }
            return ret;
        }
        #endregion
    }
}

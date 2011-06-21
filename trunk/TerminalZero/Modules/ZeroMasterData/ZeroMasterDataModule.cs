using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using ZeroBusiness;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Pack;
using ZeroGUI;
using ZeroMasterData.Pages;
using ZeroMasterData.Presentation;
using ZeroMasterData.Properties;

namespace ZeroMasterData
{
    public class ZeroMasterDataModule : ZeroModule
    {
        public ZeroMasterDataModule()
            : base(3, Resources.MasterDataModuleDescription)
        {
            BuildPosibleActions();
        }

        private void BuildPosibleActions()
        {
            Terminal.Instance.Session.Actions.Add(new ZeroAction(Actions.OpenProductsView, OpenProductView));
            Terminal.Instance.Session.Actions.Add(new ZeroAction(Actions.OpenProductMessage, OpenProductMessage));
            Terminal.Instance.Session.Actions.Add(new ZeroAction(Actions.OpenNewProductsMessage, OpenNewProductMessage, Rules.IsTerminalZero, false));
            Terminal.Instance.Session.Actions.Add(new ZeroAction(Actions.OpenSupplierView, OpenSupplierView, Rules.IsTerminalZero));
            Terminal.Instance.Session.Actions.Add(new ZeroAction(Actions.OpenCustomersView, OpenCustomerView));
            Terminal.Instance.Session.Actions.Add(new ZeroBackgroundAction(Actions.OpenCustomersSelectionView, OpenCustomerSelectionView,null,false));
            Terminal.Instance.Session.Actions.Add(new ZeroAction(Actions.ExecExportMasterData, ExportMasterDataPack, Rules.IsTerminalZero));
            //Terminal.Instance.Session.Actions.Add(new ZeroAction(Actions.ExecTestImportMasterData, TestImportDataPack));
        }

        public override string[] GetFilesToSend()
        {
            return PackManager.GetPacks(ModuleCode, WorkingDirectory);
        }

        public override void Init()
        {
        }

        public override void NewPackReceived(string path)
        {
            base.NewPackReceived(path);
            var packReceived = new MasterDataPackManager(Terminal.Instance);
            packReceived.Imported += (o, e) =>{try{File.Delete(path);}catch{}};
            Terminal.Instance.CurrentClient.Notifier.Log(TraceLevel.Verbose, "Starting Master Data pack import process");
            packReceived.Error += PackReceived_Error;
            if (packReceived.Import(path))
            {
                Terminal.Instance.CurrentClient.Notifier.SetUserMessage(false,"Importacion de master data completada con éxito!");
            }
            else
            {
                Terminal.Instance.CurrentClient.Notifier.SendNotification(
                    "Ocurrio un error durante el proceso de importacion de master data!");
            }
        }

        private void PackReceived_Error(object sender, ErrorEventArgs e)
        {
            Terminal.Instance.CurrentClient.Notifier.Log(TraceLevel.Error, e.GetException().ToString());
        }

        #region Actions Handle

        private void OpenProductView(object parameter)
        {
            BusinessContext.Instance.BeginOperation();
            var view = new ProductsViewModel();
            if (!Terminal.Instance.Session.Rules.IsValid(Rules.IsTerminalZero))
                view.View.ControlMode = ControlMode.ReadOnly;
            Terminal.Instance.CurrentClient.ShowView(view.View);
        }

        private void OpenProductMessage(object parameter)
        {
            BusinessContext.Instance.BeginOperation();
            var view = new ProductsView { ControlMode = ControlMode.ReadOnly };
            var mb = new ZeroMessageBox
                         {
                             Content = view,
                             SizeToContent = SizeToContent.WidthAndHeight,
                             ShowActivated = true,
                             Topmost = true,
                             MaxWidth = 600
                         };
            mb.Show();
        }

        private void OpenNewProductMessage(object parameter)
        {
            var detail = new ProductDetailViewModel();
            if (detail.View.ShowInModalWindow() && detail.View.ControlMode == ControlMode.New)
            {
                Terminal.Instance.Session[typeof (Product)] = new ActionParameter<Product>(false, detail.Product,true);
            }
        }

        private void OpenSupplierView(object parameter)
        {
            BusinessContext.Instance.BeginOperation();
            var view = new SupplierView();
            Terminal.Instance.CurrentClient.ShowView(view);
        }

        private void OpenCustomerView(object parameter)
        {
            BusinessContext.Instance.BeginOperation();
            var view = new CustomerViewModel();
            if (Terminal.Instance.Session.Rules.IsValid(Rules.IsTerminalZero))
            {
                view.View.ControlMode = ControlMode.Update;
            }
            Terminal.Instance.CurrentClient.ShowView(view.View);
        }

        private void OpenCustomerSelectionView(object parameter)
        {
            var view = new CustomerViewModel();
            view.View.ControlMode = ControlMode.Selection;
            if (Terminal.Instance.Session.Rules.IsValid(Rules.IsTerminalZero))
            {
                view.View.ControlMode |= ControlMode.Update;
            }
            bool? ret = ZeroMessageBox.Show(view.View, Resources.CustomerSelection);
            if (ret.HasValue && ret.Value)
            {
                Terminal.Instance.Session[typeof(Customer)] = new ActionParameter<Customer>(false, view.SelectedItem.Customer, true);
            }
        }

        private void ExportMasterDataPack(object parameter)
        {
            var masterDataPackManager = new MasterDataPackManager(Terminal.Instance);
            using (var modelManager = BusinessContext.CreateTemporaryModelManager(masterDataPackManager))
            {
                Terminal.Instance.CurrentClient.Notifier.SetProcess("Armando paquete");
                Terminal.Instance.CurrentClient.Notifier.SetProgress(10);
                var info = new ExportEntitiesPackInfo(ModuleCode, WorkingDirectory);
                info.AddTable(modelManager.Prices);
                info.AddTable(modelManager.Weights);
                info.AddTable(modelManager.PaymentInstruments);
                info.AddTable(modelManager.ProductGroups);
                info.AddTable(modelManager.Taxes);
                info.AddTable(modelManager.TaxPositions);
                info.AddTable(modelManager.Suppliers);
                info.AddTable(modelManager.Products);
                info.AddTable(modelManager.Customers);

                using (masterDataPackManager)
                {
                    masterDataPackManager.Exported += (sender, e) =>
                            {
                                Terminal.Instance.CurrentClient.Notifier.SetProgress(60);
                                Terminal.Instance.CurrentClient.Notifier.SetProcess("Datos Exportados");
                                Terminal.Instance.CurrentClient.Notifier.SetUserMessage(false,"Datos Exportados al directorio: " + WorkingDirectory);
                                Terminal.Instance.CurrentClient.Notifier.SetProgress(80);

                            };
                    try
                    {
                        Terminal.Instance.CurrentClient.Notifier.SetProcess("Creando paquete");
                        masterDataPackManager.Export(info);
                    }
                    catch (Exception ex)
                    {
                        Terminal.Instance.CurrentClient.Notifier.SetUserMessage(true, ex.ToString());
                    }

                }
                Terminal.Instance.CurrentClient.Notifier.SetProcess("Listo");
                Terminal.Instance.CurrentClient.Notifier.SetUserMessage(true, "Terminado");
                Terminal.Instance.CurrentClient.Notifier.SetProgress(100);

            }
        }

        private void TestImportDataPack(object parameter)
        {
            foreach (string s in GetFilesToSend())
            {
                NewPackReceived(s);
            }
        }

        #endregion
    }
}
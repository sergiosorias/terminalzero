using System;
using System.Diagnostics;
using System.IO;
using ZeroBusiness;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Pack;
using ZeroMasterData.Pages;
using ZeroMasterData.Presentation;
using ZeroMasterData.Properties;

namespace ZeroMasterData
{
    public class ZeroMasterDataModule : ZeroModule
    {
        public const int Code = 3;

        public ZeroMasterDataModule()
            : base(Code, Resources.MasterDataModuleDescription)
        {
            BuildPosibleActions();
        }

        private void BuildPosibleActions()
        {
            Terminal.Instance.Session.Actions.Add(new ZeroAction(Actions.OpenProductsView, OpenProductView));
            Terminal.Instance.Session.Actions.Add(new ZeroAction(Actions.OpenProductMessage, OpenProductMessage));
            Terminal.Instance.Session.Actions.Add(new ZeroAction(Actions.OpenSupplierView, OpenSupplierView, Rules.IsTerminalZero));
            Terminal.Instance.Session.Actions.Add(new ZeroAction(Actions.OpenCustomersView, OpenCustomerView));
            Terminal.Instance.Session.Actions.Add(new ZeroBackgroundAction(Actions.OpenCustomersSelectionView, OpenCustomerSelectionView,null,false));
            Terminal.Instance.Session.Actions.Add(new ZeroAction(Actions.ExecExportMasterData, TryExportMasterDataPack, Rules.IsTerminalZero));
            //Terminal.Instance.Session.Actions.Add(new ZeroAction(Actions.ExecTestImportMasterData, TestImportDataPack));
        }

        public override string[] GetFilesToSend()
        {
            TryExportMasterDataPack(null);
            return PackManager.GetPacks(ModuleCode, WorkingDirectory);
        }

        public override void Initialize()
        {
        }

        public override void NewPackReceived(string path)
        {
            base.NewPackReceived(path);
            var packReceived = new MasterDataPackManager();
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
            var view = new ProductsViewModel();
            view.View.ControlMode = ControlMode.ReadOnly;
            Terminal.Instance.CurrentClient.ShowWindow(view.View);
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
            Terminal.Instance.CurrentClient.ShowDialog(view.View, (res) =>
            {
                if (res)
                {
                    Terminal.Instance.Session[typeof(Customer)] = new ActionParameter<Customer>(false, view.SelectedItem.Customer, true);
                }
                Terminal.Instance.Session.Actions[Actions.OpenCustomersSelectionView].RaiseExecuted();
            });
            
        }

        private void TryExportMasterDataPack(object parameter)
        {
            try
            {
                new MasterDataPackManager().Export(WorkingDirectory);
            }
            catch (Exception ex)
            {
                Terminal.Instance.CurrentClient.Notifier.SetUserMessage(true, ex.ToString());
            }
        }

        private void TestImportDataPack(object parameter)
        {
            foreach (string s in PackManager.GetPacks(ModuleCode, WorkingDirectoryIn))
            {
                NewPackReceived(s);
            }
        }

        #endregion
    }
}
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using ZeroBusiness;
using ZeroBusiness.Entities.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Pack;
using ZeroGUI;
using ZeroMasterData.Pages;
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
            var openProducList = new ZeroAction( ActionType.MenuItem, Actions.OpenProductsView, OpenProductView);
            Terminal.Instance.Session.AddAction(openProducList);
            Terminal.Instance.Session.AddAction(new ZeroAction( ActionType.MenuItem, Actions.OpenProductMessage,OpenProductMessage));
            Terminal.Instance.Session.AddAction(new ZeroAction( ActionType.MenuItem, Actions.OpenSupplierView, OpenSupplierView, Rules.IsTerminalZero));
            Terminal.Instance.Session.AddAction(new ZeroAction( ActionType.MenuItem, Actions.OpenCustomersView,OpenCustomerView));
            Terminal.Instance.Session.AddAction(new ZeroAction( ActionType.MenuItem, Actions.ExecExportMasterData, ExportMasterDataPack, Rules.IsTerminalZero));
            Terminal.Instance.Session.AddAction(new ZeroAction(ActionType.MenuItem, "Test@Import master data", TestImportDataPack));
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
                Terminal.Instance.CurrentClient.Notifier.SendNotification("Importacion de master data completada con éxito!");
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

        private void OpenProductView()
        {
            var P = new ProductsView();
            if (!Terminal.Instance.Manager.ValidateRule(Rules.IsTerminalZero)) 
                P.ControlMode = ControlMode.ReadOnly;
            OnModuleNotifing(new ModuleNotificationEventArgs {ControlToShow = P});
        }

        private void OpenProductMessage()
        {
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

        private void OpenSupplierView()
        {
            var P = new SupplierView();
            OnModuleNotifing(new ModuleNotificationEventArgs {ControlToShow = P});
        }

        private void OpenCustomerView()
        {
            var P = new CustomerView();
            if (Terminal.Instance.Manager.ValidateRule(Rules.IsTerminalZero))
            {
                P.ControlMode = ControlMode.Update;
            }

            OnModuleNotifing(new ModuleNotificationEventArgs {ControlToShow = P});
        }

        private void ExportMasterDataPack()
        {
            var th = new Thread(
                ExportPackEntryPoint);

            th.Start();
        }

        private void TestImportDataPack()
        {
            foreach (string s in GetFilesToSend())
            {
                NewPackReceived(s);
            }
        }

        private void ExportPackEntryPoint(object o)
        {
            using (var ent = new DataModelManager())
            {
                Terminal.Instance.CurrentClient.Notifier.SetProcess("Armando paquete");
                Terminal.Instance.CurrentClient.Notifier.SetProgress(10);
                //TODO:
                //Fijarse si se puede hacer dinamica la carga del paquete.
                var info = new ExportEntitiesPackInfo(ModuleCode, WorkingDirectory);
                info.AddTable(ent.Prices);
                info.AddTable(ent.Weights);
                info.AddTable(ent.PaymentInstruments);
                info.AddTable(ent.ProductGroups);
                info.AddTable(ent.Taxes);
                info.AddTable(ent.TaxPositions);
                info.AddTable(ent.Suppliers);
                info.AddTable(ent.Products);
                info.AddTable(ent.Customers);

                using (var pack = new MasterDataPackManager(Terminal.Instance))
                {
                    pack.Exported += pack_Exported;
                    try
                    {
                        Terminal.Instance.CurrentClient.Notifier.SetProcess("Creando paquete");
                        pack.Export(info);
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

        private void pack_Exported(object sender, PackProcessingEventArgs e)
        {
            Terminal.Instance.CurrentClient.Notifier.SetProgress(60);
            Terminal.Instance.CurrentClient.Notifier.SetProcess("Datos Exportados");
            Terminal.Instance.CurrentClient.Notifier.SetUserMessage(false, "Datos Exportados al directorio: " + WorkingDirectory);
            Terminal.Instance.CurrentClient.Notifier.SetProgress(80);
        }

        #endregion
    }
}
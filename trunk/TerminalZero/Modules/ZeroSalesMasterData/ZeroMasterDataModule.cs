using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using ZeroBusiness;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Pack;
using ZeroGUI;
using ZeroMasterData.Entities;
using ZeroMasterData.Pages;
using ZeroMasterData.Pages.Controls;

namespace ZeroMasterData
{
    public class ZeroMasterDataModule : ZeroModule
    {
        public ZeroMasterDataModule(ITerminal iCurrentTerminal)
            : base(iCurrentTerminal, 3, "ABM de las estructuras necesarias para realizar operaciones diarias")
        {
            BuildPosibleActions();
        }

        private void BuildPosibleActions()
        {
            var openProducList = new ZeroAction( ActionType.MenuItem, Actions.OpenProductsView, OpenProductView);
            OwnerTerminal.Session.AddAction(openProducList);
            OwnerTerminal.Session.AddAction(new ZeroAction( ActionType.MenuItem, Actions.OpenProductMessage,OpenProductMessage));
            OwnerTerminal.Session.AddAction(new ZeroAction( ActionType.MenuItem, Actions.OpenSupplierView, OpenSupplierView, Rules.IsTerminalZero));
            OwnerTerminal.Session.AddAction(new ZeroAction( ActionType.MenuItem, Actions.OpenCustomersView,OpenCustomerView));
            OwnerTerminal.Session.AddAction(new ZeroAction( ActionType.MenuItem, Actions.ExecExportMasterData, ExportMasterDataPack, Rules.IsTerminalZero));
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
            var packReceived = new MasterDataPackManager(OwnerTerminal);
            packReceived.Imported += (o, e) =>{try{File.Delete(path);}catch{}};
            OwnerTerminal.Session.Notifier.Log(TraceLevel.Verbose, "Starting Master Data pack import process");
            packReceived.Error += PackReceived_Error;
            if (packReceived.Import(path))
            {
                OwnerTerminal.Session.Notifier.SendNotification("Importacion de master data completada con éxito!");
            }
            else
            {
                OwnerTerminal.Session.Notifier.SendNotification(
                    "Ocurrio un error durante el proceso de importacion de master data!");
            }
        }

        private void PackReceived_Error(object sender, ErrorEventArgs e)
        {
            OwnerTerminal.Session.Notifier.Log(TraceLevel.Error, e.GetException().ToString());
        }

        #region Actions Handle

        private void OpenProductView()
        {
            var P = new ProductsView();
            if (!OwnerTerminal.Manager.ValidateRule(Rules.IsTerminalZero)) 
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
                             Topmost = true
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
            if (OwnerTerminal.Manager.ValidateRule(Rules.IsTerminalZero))
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

        private void ExportPackEntryPoint(object o)
        {
            using (var ent = new MasterDataEntities())
            {
                OwnerTerminal.Session.Notifier.SetProcess("Armando paquete");
                OwnerTerminal.Session.Notifier.SetProgress(10);
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

                using (var pack = new MasterDataPackManager(OwnerTerminal))
                {
                    pack.Exported += pack_Exported;
                    try
                    {
                        OwnerTerminal.Session.Notifier.SetProcess("Creando paquete");
                        pack.Export(info);
                    }
                    catch (Exception ex)
                    {
                        OwnerTerminal.Session.Notifier.SetUserMessage(true, ex.ToString());
                    }
                }
                OwnerTerminal.Session.Notifier.SetProcess("Listo");
                OwnerTerminal.Session.Notifier.SetUserMessage(true, "Terminado");
                OwnerTerminal.Session.Notifier.SetProgress(100);
            }
        }

        private void pack_Exported(object sender, PackEventArgs e)
        {
            OwnerTerminal.Session.Notifier.SetProgress(60);
            OwnerTerminal.Session.Notifier.SetProcess("Datos Exportados");
            OwnerTerminal.Session.Notifier.SetUserMessage(false, "Datos Exportados al directorio: " + WorkingDirectory);
            OwnerTerminal.Session.Notifier.SetProgress(80);
        }

        private void NotifyEntityCreation(string entity, int rowCount)
        {
            OwnerTerminal.Session.Notifier.SetUserMessage(false, "Creando archivo de " + entity);
            OwnerTerminal.Session.Notifier.SetUserMessage(false, "Cantidad: " + rowCount);
        }

        #endregion
    }
}
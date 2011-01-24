using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
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
            var openProducList = new ZeroAction(null, ActionType.MenuItem,
                                                "Tablas Maestras@Productos@Lista de Productos",
                                                OpenProductView);
            Terminal.Session.AddAction(openProducList);

            Terminal.Session.AddAction(new ZeroAction(null, ActionType.MenuItem, "Tablas Maestras@Productos@Consulta",
                                                      OpenProductMessage));

            Terminal.Session.AddAction(new ZeroAction(null, ActionType.MenuItem, "Tablas Maestras@Proveedores",
                                                      OpenSupplierView, "ValidateTerminalZero"));

            Terminal.Session.AddAction(new ZeroAction(null, ActionType.MenuItem, "Tablas Maestras@Clientes",
                                                      OpenCustomerView));

            //IFileTransfer
            var ac = new ZeroAction(Terminal.Session, ActionType.MenuItem, "Tablas Maestras@Exportar Datos",
                                    ExportMasterDataPack, "ValidateTerminalZero");
            Terminal.Session.AddAction(ac);
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
            var packReceived = new MasterDataPackManager(Terminal);
            packReceived.Imported += (o, e) =>{try{File.Delete(path);}catch{}};
            Terminal.Session.Notifier.Log(TraceLevel.Verbose, "Starting Master Data pack import process");
            packReceived.Error += PackReceived_Error;
            if (packReceived.Import(path))
            {
                Terminal.Session.Notifier.SendNotification("Importacion de master data completada con éxito!");
            }
            else
            {
                Terminal.Session.Notifier.SendNotification(
                    "Ocurrio un error durante el proceso de importacion de master data!");
            }
        }

        private void PackReceived_Error(object sender, ErrorEventArgs e)
        {
            Terminal.Session.Notifier.Log(TraceLevel.Error, e.GetException().ToString());
        }

        #region Actions Handle

        private void OpenProductView()
        {
            var P = new ProductsView();
            if (Terminal.TerminalCode != 0)
                P.Mode = Mode.ReadOnly;
            OnModuleNotifing(new ModuleNotificationEventArgs {ControlToShow = P});
        }

        private void OpenProductMessage()
        {
            var mb = new ZeroMessageBox();
            var view = new ProductGrid();
            view.Mode = Mode.ReadOnly;
            mb.Content = view;
            mb.SizeToContent = SizeToContent.WidthAndHeight;
            mb.ShowActivated = true;
            mb.Topmost = true;
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
            if (Terminal.Manager.ValidateRule("ValidateTerminalZero"))
            {
                P.Mode = Mode.Update;
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
                Terminal.Session.Notifier.SetProcess("Armando paquete");
                Terminal.Session.Notifier.SetProgress(10);
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

                using (var pack = new MasterDataPackManager(Terminal))
                {
                    pack.Exported += pack_Exported;
                    try
                    {
                        Terminal.Session.Notifier.SetProcess("Creando paquete");
                        pack.Export(info);
                    }
                    catch (Exception ex)
                    {
                        Terminal.Session.Notifier.SetUserMessage(true, ex.ToString());
                    }
                }
                Terminal.Session.Notifier.SetProcess("Listo");
                Terminal.Session.Notifier.SetUserMessage(true, "Terminado");
                Terminal.Session.Notifier.SetProgress(100);
            }
        }

        private void pack_Exported(object sender, PackEventArgs e)
        {
            Terminal.Session.Notifier.SetProgress(60);
            Terminal.Session.Notifier.SetProcess("Datos Exportados");
            Terminal.Session.Notifier.SetUserMessage(false, "Datos Exportados al directorio: " + WorkingDirectory);
            Terminal.Session.Notifier.SetProgress(80);
        }

        private void NotifyEntityCreation(string entity, int rowCount)
        {
            Terminal.Session.Notifier.SetUserMessage(false, "Creando archivo de " + entity);
            Terminal.Session.Notifier.SetUserMessage(false, "Cantidad: " + rowCount);
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses;
using ZeroCommonClasses.Helpers;
using ZeroCommonClasses.Interfaces.Services;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.PackClasses;
using ZeroGUI;

namespace ZeroMasterData
{
    public class ZeroMasterDataModule : ZeroModule
    {
        public ZeroMasterDataModule(ITerminal iCurrentTerminal)
            :base(iCurrentTerminal, 3,"ABM de las estructuras necesarias para realizar operaciones diarias")
        {
            BuildPosibleActions();
        }

        private void BuildPosibleActions()
        {
            ZeroAction openProducList = new ZeroAction(ActionType.MenuItem, "Tablas Maestras@Productos@Lista de Productos",
                OpenProductView);
            Terminal.Session.AddAction( openProducList);

            Terminal.Session.AddAction(new ZeroAction(ActionType.MenuItem, "Tablas Maestras@Productos@Consulta",
                OpenProductMessage));

            Terminal.Session.AddAction( new ZeroAction(ActionType.MenuItem, "Tablas Maestras@Proveedores",
                OpenSupplierView, "ValidateTerminalZero"));

            Terminal.Session.AddAction( new ZeroAction(ActionType.MenuItem, "Tablas Maestras@Clientes",
                OpenCustomerView));

            //IFileTransfer
            ZeroAction ac = new ZeroAction(ActionType.MenuItem, "Tablas Maestras@Exportar Datos", ExportMasterDataPack, "ValidateTerminalZero");
            Terminal.Session.AddAction( ac);
                        
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
            MasterDataPackManager PackReceived = new MasterDataPackManager(path);
            PackReceived.Imported += (o, e) => { try { System.IO.File.Delete(path); } catch { } };
            Terminal.Session.Notifier.Log(System.Diagnostics.TraceLevel.Verbose,"Starting Master Data pack import process");
            PackReceived.Error += new System.IO.ErrorEventHandler(PackReceived_Error);
            if (PackReceived.Process())
            {
                Terminal.Session.Notifier.SendNotification("Importacion de master data completada con éxito!");
            }
            else
            {
                Terminal.Session.Notifier.SendNotification("Ocurrio un error durante el proceso de importacion de master data!");
            }
            
        }

        private void PackReceived_Error(object sender, System.IO.ErrorEventArgs e)
        {
            Terminal.Session.Notifier.Log(System.Diagnostics.TraceLevel.Error, e.GetException().ToString());
        }

        #region Actions Handle

        private void OpenProductView(ZeroRule rule)
        {
            ZeroMasterData.Pages.ProductsView P = new ZeroMasterData.Pages.ProductsView();
            if (Terminal.TerminalCode != 0)
                P.Mode = Mode.ReadOnly;
            OnModuleNotifing(new ModuleNotificationEventArgs { ControlToShow = P });
            
        }

        private void OpenProductMessage(ZeroRule rule)
        {
            ZeroMessageBox mb = new ZeroMessageBox();
            ZeroMasterData.Pages.Controls.ProductGrid view = new ZeroMasterData.Pages.Controls.ProductGrid();
            view.Mode = Mode.ReadOnly;
            mb.Content = view;
            mb.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
            mb.ShowActivated = true;
            mb.Topmost = true;
            mb.Show();
        }

        private void OpenSupplierView(ZeroRule rule)
        {
            ZeroMasterData.Pages.SupplierView P = new ZeroMasterData.Pages.SupplierView();
            OnModuleNotifing(new ModuleNotificationEventArgs { ControlToShow = P });
            
        }

        private void OpenCustomerView(ZeroRule rule)
        {
            ZeroMasterData.Pages.CustomerView P = new ZeroMasterData.Pages.CustomerView();
            if (Terminal.Manager.ValidateRule("ValidateTerminalZero"))
                P.Mode = Mode.Update;

            OnModuleNotifing(new ModuleNotificationEventArgs { ControlToShow = P });
        }

        private void ExportMasterDataPack(ZeroRule rule)
        {
            System.Threading.Thread th = new System.Threading.Thread(
                new System.Threading.ParameterizedThreadStart(ExportPackEntryPoint));

            th.Start();
        }

        private void ExportPackEntryPoint(object o)
        {
            using (ZeroMasterData.Entities.MasterDataEntities ent = new ZeroMasterData.Entities.MasterDataEntities())
            {
                Terminal.Session.Notifier.SetProcess("Armando paquete");
                Terminal.Session.Notifier.SetProgress(10);
                //TODO:
                //Fijarse si se puede hacer dinamica la carga del paquete.
                ExportEntitiesPackInfo info = new ExportEntitiesPackInfo(this.ModuleCode, this.WorkingDirectory);
                info.AddTable(ent.Prices);
                info.AddTable(ent.Weights);
                info.AddTable(ent.PaymentInstruments);
                info.AddTable(ent.ProductGroups);
                info.AddTable(ent.Taxes);
                info.AddTable(ent.TaxPositions);
                info.AddTable(ent.Suppliers);
                info.AddTable(ent.Products);
                info.AddTable(ent.Customers);
                
                using (MasterDataPackManager pack = new MasterDataPackManager(info))
                {
                    pack.Exported += new EventHandler<PackEventArgs>(pack_Exported);
                    try
                    {
                        Terminal.Session.Notifier.SetProcess("Creando paquete");
                        pack.Process();
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
            Terminal.Session.Notifier.SetUserMessage(false, "Creando archivo de "+entity);
            Terminal.Session.Notifier.SetUserMessage(false, "Cantidad: "+rowCount.ToString());
        }

        #endregion
    }
}

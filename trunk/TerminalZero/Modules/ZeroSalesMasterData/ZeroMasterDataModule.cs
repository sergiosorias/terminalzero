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

        }

        public override void BuildPosibleActions(List<ZeroAction> actions)
        {
            ZeroAction openProducList = new ZeroAction(ActionType.MenuItem, "Tablas Maestras@Productos@Lista de Productos",
                OpenProductView);
            actions.Add(openProducList);

            actions.Add(new ZeroAction(ActionType.MenuItem, "Tablas Maestras@Productos@Consulta",
                OpenProductMessage));

            actions.Add(new ZeroAction(ActionType.MenuItem, "Tablas Maestras@Proveedores",
                OpenSupplierView, "ValidateTerminalZero"));

            actions.Add(new ZeroAction(ActionType.MenuItem, "Tablas Maestras@Clientes",
                OpenCustomerView));

            //IFileTransfer
            ZeroAction ac = new ZeroAction(ActionType.MenuItem, "Tablas Maestras@Exportar Datos", ExportMasterDataPack, "ValidateTerminalZero");
            actions.Add(ac);

            //ac = new ZeroAction(ActionType.MenuItem, "Tablas Maestras@Importar Datos", ImportMasterDataPack, "ValidateUser");
            //actions.Add(ac);
        }

        public override void BuildRulesActions(List<ZeroRule> rules)
        {
            
        }

        public override string[] GetFilesToSend()
        {
            return PackManager.GetPacks(WorkingDirectory);
        }

        public override void Init()
        {
            
        }

        public override void NewPackReceived(string path)
        {
            base.NewPackReceived(path);
            MasterDataPackManager mdpm = new MasterDataPackManager(WorkingDirectoryIn);
            mdpm.Imported += (o, e) => { try { System.IO.File.Delete(path); } catch { } };
            mdpm.Process();

            
        }

        #region Actions Handle

        private void OpenProductView(ZeroRule rule)
        {
            ZeroMasterData.Pages.ProductsView P = new ZeroMasterData.Pages.ProductsView();
            if (ICurrentTerminal.TerminalCode != 0)
                P.Mode = Mode.ReadOnly;
            OnNotifing(new ModuleNotificationEventArgs { ControlToShow = P });
            
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
            OnNotifing(new ModuleNotificationEventArgs { ControlToShow = P });
            
        }

        private void OpenCustomerView(ZeroRule rule)
        {
            ZeroMasterData.Pages.CustomerView P = new ZeroMasterData.Pages.CustomerView();
            if (Session.ValidateRule("ValidateTerminalZero"))
                P.Mode = Mode.Update;

            OnNotifing(new ModuleNotificationEventArgs { ControlToShow = P });
        }

        private void ExportMasterDataPack(ZeroRule rule)
        {
            System.Threading.Thread th = new System.Threading.Thread(
                new System.Threading.ParameterizedThreadStart(ExportPackEntryPoint));

            th.Start();
        }

        private void ImportMasterDataPack(ZeroRule rule)
        {
            ZeroMasterData.Pages.ImportView o = new Pages.ImportView();
            OnNotifing(new ModuleNotificationEventArgs { ControlToShow = o });
            //System.Threading.Thread th = new System.Threading.Thread(
            //    new System.Threading.ParameterizedThreadStart(ImportPackEntryPoint));

            //th.Start();
        }

        private void ExportPackEntryPoint(object o)
        {
            using (ZeroMasterData.Entities.MasterDataEntities ent = new ZeroMasterData.Entities.MasterDataEntities())
            {
                Session.Notifier.SetProcess("Armando paquete");
                Session.Notifier.SetProgress(10);
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
                        Session.Notifier.SetProcess("Creando paquete");
                        pack.Process();
                    }
                    catch (Exception ex)
                    {
                        Session.Notifier.SetUserMessage(true, ex.ToString());
                    }

                }
                Session.Notifier.SetProcess("Listo");
                Session.Notifier.SetUserMessage(true, "Terminado");
                Session.Notifier.SetProgress(100);
            }

            
        }

        private void pack_Exported(object sender, PackEventArgs e)
        {
            Session.Notifier.SetProgress(60);
            Session.Notifier.SetProcess("Datos Exportados");
            Session.Notifier.SetUserMessage(false, "Datos Exportados al directorio: " + WorkingDirectory);
            Session.Notifier.SetProgress(80);
        }

        private void NotifyEntityCreation(string entity, int rowCount)
        {
            Session.Notifier.SetUserMessage(false, "Creando archivo de "+entity);
            Session.Notifier.SetUserMessage(false, "Cantidad: "+rowCount.ToString());
        }

        private void ImportPackEntryPoint(object o)
        {
            Session.Notifier.SetProcess("Importando Datos");

            using (MasterDataPackManager manager = new MasterDataPackManager(WorkingDirectory))
            {
                manager.Imported += new EventHandler<PackEventArgs>(manager_Imported);
                manager.Process();
            }
        }

        private void manager_Imported(object sender, PackEventArgs e)
        {
            Session.Notifier.SetProcess("Listo");
            Session.Notifier.SetProgress(100);
        }

        #endregion
    }
}

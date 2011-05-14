﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using ZeroBusiness;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects.Actions;
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
            Terminal.Instance.Session.AddAction(new ZeroAction( ActionType.MenuItem, Actions.OpenProductsView, OpenProductView));
            Terminal.Instance.Session.AddAction(new ZeroAction( ActionType.MenuItem, Actions.OpenProductMessage, OpenProductMessage));
            Terminal.Instance.Session.AddAction(new ZeroAction( ActionType.MenuItem, Actions.OpenSupplierView, OpenSupplierView, Rules.IsTerminalZero));
            Terminal.Instance.Session.AddAction(new ZeroAction( ActionType.MenuItem, Actions.OpenCustomersView,OpenCustomerView));
            Terminal.Instance.Session.AddAction(new ZeroAction( ActionType.MenuItem, Actions.ExecExportMasterData, ExportMasterDataPack, Rules.IsTerminalZero));
            
            //Terminal.Instance.Session.AddAction(new ZeroAction( ActionType.MenuItem, Actions.ExecTestImportMasterData, TestImportDataPack));
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
            var view = new ProductsView();
            if (!Terminal.Instance.Manager.IsRuleValid(Rules.IsTerminalZero)) 
                view.ControlMode = ControlMode.ReadOnly;
            BusinessContext.Instance.BeginOperation();
            Terminal.Instance.CurrentClient.ShowView(view);
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
            BusinessContext.Instance.BeginOperation();
            mb.Show();
        }

        private void OpenSupplierView()
        {
            var view = new SupplierView();
            BusinessContext.Instance.BeginOperation();
            Terminal.Instance.CurrentClient.ShowView(view);
        }

        private void OpenCustomerView()
        {
            var view = new CustomerView();
            if (Terminal.Instance.Manager.IsRuleValid(Rules.IsTerminalZero))
            {
                view.ControlMode = ControlMode.Update;
            }
            BusinessContext.Instance.BeginOperation();
            Terminal.Instance.CurrentClient.ShowView(view);
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
            var masterDataPackManager = new MasterDataPackManager(Terminal.Instance);
            using (var modelManager = BusinessContext.CreateTemporaryModelManager(masterDataPackManager))
            {
                Terminal.Instance.CurrentClient.Notifier.SetProcess("Armando paquete");
                Terminal.Instance.CurrentClient.Notifier.SetProgress(10);
                //TODO:
                //Fijarse si se puede hacer dinamica la carga del paquete.
                var info = new ExportEntitiesPackInfo(ModuleCode, WorkingDirectory);
                info.AddEntities(modelManager.Prices);
                info.AddEntities(modelManager.Weights);
                info.AddEntities(modelManager.PaymentInstruments);
                info.AddEntities(modelManager.ProductGroups);
                info.AddEntities(modelManager.Taxes);
                info.AddEntities(modelManager.TaxPositions);
                info.AddEntities(modelManager.Suppliers);
                info.AddEntities(modelManager.Products);
                info.AddEntities(modelManager.Customers);

                using (masterDataPackManager)
                {
                    masterDataPackManager.Exported += pack_Exported;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroCommonClasses.Interfaces;
using System.IO;
using System.Windows.Controls;
using TerminalZeroClient.Extras;
using ZeroCommonClasses.Interfaces.Services;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects;
using System.Diagnostics;


namespace TerminalZeroClient.Business
{
    internal partial class ZeroClient
    {
        public bool IsAllOK { get; private set; }
        private ILogBuilder Logger = null;
        private ITerminalManager Manager { get; set; }
        
        internal ZeroClient()
        {
            IsAllOK = true;
        }

        public void InitializeAppAsync()
        {
            bool canContinue = false;
            App.Instance.Session.Notifier.SetProcess("Buscando Módulos");
            try
            {
                App.Instance.Session.Notifier.SetProgress(5);
                string[] Modules = Directory.GetFiles(App.Directories.ModulesFolder, "*.dll");
                App.Instance.Session.Notifier.SetProgress(10);
                canContinue = Modules.Length != 0;
                if (!canContinue)
                {
                    App.Instance.Session.Notifier.SetProgress(20);
                    App.Instance.Session.Notifier.SetUserMessage(false, "No se encontraron modulos para ejecutar!");
                    App.Instance.Session.Notifier.SetUserMessage(false, "Finalizando proceso...");
                }
                else
                {
                    if (!GetModules(Modules))
                    {
                        canContinue = false;
                        App.Instance.Session.Notifier.SetUserMessage(true, "No se encontró inicializador, el sistema no puede ser utilizado sin el mismo.");
                    }
                    else
                    {

                        App.Instance.Session.Notifier.SetProgress(50);
                        canContinue = InitializeTerminal();

                        App.Instance.Session.Notifier.SetProgress(60);
                        if (!canContinue)
                            App.Instance.Session.Notifier.SetUserMessage(false, "Se ha finalizado la carga de la aplicación con algunos problemas encontrados.");
                        else
                            App.Instance.Session.Notifier.SetUserMessage(false, "Se ha finalizado la carga de la aplicación correctamente!.");



                    }
                }
            }
            catch (Exception ex)
            {
                App.Instance.Session.Notifier.SetProcess("Error!");
                App.Instance.Session.Notifier.SetUserMessage(true, ex.ToString());
                canContinue = false;
            }

            if (!canContinue)
            {
                IsAllOK = false;
                App.Instance.Session.Notifier.SendNotification("Ocurrio algun error en el momento de iniciar el programa, por favor lea el detalle del proceso!");
                App.Instance.Session.Notifier.SetUserMessage(true, "Error");
                App.Instance.Session.Notifier.SetProcess("Error!");
            }
            else
            {
                App.Instance.Session.Notifier.SetProcess("Listo");
            }

            App.Instance.Session.Notifier.SetProgress(100);
        }

        private bool InitializeTerminal()
        {
            App.Instance.Session.Notifier.SetProcess("Validando Módulos");
            bool ret = true;
            try
            {
                Manager.InitializeTerminal();
                App.Instance.Session.ModuleList.ForEach(c => c.TerminalStatus = Manager.GetModuleStatus(c));
                if (App.Instance.Session.ModuleList.Exists(c => c.TerminalStatus == ModuleStatus.NeedsSync))
                {
                    IsAllOK = false;
                    App.Instance.Session.Notifier.SetUserMessage(true, "Algunas configuraciones pueden no estar sincronizadas con el servidor,\n"
                                                    + "por favor conectese con la central lo antes posible!");
                }
            }
            catch (Exception ex)
            {
                App.Instance.Session.Notifier.SetUserMessage(false, "ERROR: " + ex.ToString());
                ret = false;
            }


            return ret;
        }

        private bool GetModules(string[] Modules)
        {
            App.Instance.Session.Notifier.SetProcess("Cargando Módulos");
            string aux = "";

            foreach (var item in Modules)
            {
                try
                {
                    aux = Path.GetFileNameWithoutExtension(item);
                    App.Instance.Session.Notifier.SetUserMessage(false, "Inicializando " + aux);
                    System.Reflection.Assembly ass = System.Reflection.Assembly.LoadFrom(item);
                    Type ty = ass.GetExportedTypes().FirstOrDefault(t => t.BaseType == typeof(ZeroCommonClasses.ZeroModule));
                    
                    object obj = ass.CreateInstance(ty.ToString(), false, System.Reflection.BindingFlags.CreateInstance,
                        null, new object[] { App.Instance as ITerminal }, System.Globalization.CultureInfo.InstalledUICulture, null);

                    TryAddModule(obj, item);

                }
                catch (Exception ex)
                {
                    App.Instance.Session.Notifier.SetUserMessage(false, "Assembly '" + item + "' no es un modulo válido, error: " + ex.ToString());
                }
            }
            App.Instance.Session.Notifier.SetProgress(30);
            if (Manager == null)
            {
                return false;
            }
            App.Instance.SetManager(Manager);
            return true;
        }

        private void TryAddModule(object obj, string path)
        {

            if (obj is ZeroModule)
            {
                ZeroModule mod = obj as ZeroModule;
                mod.TerminalStatus = ModuleStatus.Unknown;
                mod.UserStatus = ModuleStatus.Unknown;
                mod.WorkingDirectory = path + App.Directories.WorkingDirSubfix;
                App.Instance.Session.AddModule(mod);
                App.Instance.Session.Notifier.SetUserMessage(false, "Módulo ensamblado --> ''" + mod.Description + "''");

                if (Manager == null && obj is ITerminalManager)
                    Manager = obj as ITerminalManager;
                
                if (Logger == null && obj is ILogBuilder)
                    Logger = obj as ILogBuilder;

            }
            else
            {
                App.Instance.Session.Notifier.SetUserMessage(false, "Módulo invalido --> ''''");
            }
        }

        public ZeroMenu BuildMenu()
        {
            List<ZeroAction> validActions = App.Instance.Manager.BuilSessionActions();
            App.Instance.Session.ModuleList.ForEach(m => { m.Init(); });
            ZeroMenu menu = new ZeroMenu();
            string aux = "", current = "";
            int pos = 0;
            ZeroMenu currentlevel;
            
            #region build menu bar
            foreach (var item in validActions.Where(a => a.ActionType == ActionType.MenuItem || a.ActionType == ActionType.MainViewButton))
            {
                currentlevel = null;
                aux = item.Name;
                pos = aux.IndexOf('@');

                while (pos > 0)
                {
                    current = aux.Substring(0, pos);
                    aux = aux.Remove(0, pos + 1);

                    if (currentlevel == null)
                    {
                        if (!menu.ContainsKey(current))
                            menu.Add(current, new ZeroMenu());

                        currentlevel = menu[current];
                    }
                    else
                    {
                        if (!currentlevel.ContainsKey(current))
                            currentlevel.Add(current, new ZeroMenu());

                        currentlevel = currentlevel[current];
                    }

                    pos = aux.IndexOf('@');
                }

                if (currentlevel == null)
                {
                    if (!menu.ContainsKey(aux))
                        menu.Add(aux, new ZeroMenu());

                    currentlevel = menu[aux];
                }
                else
                {
                    if (!currentlevel.ContainsKey(aux))
                        currentlevel.Add(aux, new ZeroMenu());

                    currentlevel = currentlevel[aux];
                }

                currentlevel.MenuAction = item;
            }
            #endregion build menu bar

            return menu;
        }
    }
}

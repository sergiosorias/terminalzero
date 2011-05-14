using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using ZeroCommonClasses;
using ZeroCommonClasses.Context;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Interfaces.Services;
using ZeroGUI;

namespace TerminalZeroClient.Business
{
    internal class WpfClient : IZeroClient
    {
        public WpfClient()
        {
            ModuleList = new List<ZeroModule>();
            var param1 = new ActionParameter<ISyncService>(false, ConfigurationContext.CreateSyncConnection(), false);
            var param2 = new ActionParameter<IFileTransfer>(false,ConfigurationContext.CreateFileTranferConnection(),false);
            Terminal.Instance.Session[param1.Name] = param1;
            Terminal.Instance.Session[param2.Name] = param2;
        }

        public bool Initialized { get; private set; }

        public IProgressNotifier Notifier { get; set; }
        
        public List<ZeroModule> ModuleList { get; private set; }
        
        public ZeroMenu MainMenu
        {
            get
            {
                List<ZeroAction> validActions = ZeroCommonClasses.Terminal.Instance.Manager.BuilSessionActions();
                var menu = new ZeroMenu();
                string aux = "", current = "";
                int pos = 0;
                ZeroMenu currentlevel;

                #region build menu bar

                foreach (
                    var item in
                        validActions.Where(
                            a => a.ActionType == ActionType.MenuItem || a.ActionType == ActionType.MainViewButton))
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

        public void ShowView(object view)
        {
            if (view is ZeroMessageBox)
            {
                ((ZeroMessageBox)view).Owner = Application.Current.MainWindow;
                ((ZeroMessageBox)view).Top = Application.Current.MainWindow.Top + 1;
                ((ZeroMessageBox)view).ShowDialog();
            }
            else if (((MainWindow)Application.Current.MainWindow).PrimaryWindow.Content is NavigationBasePage)
            {
                if (((NavigationBasePage)((MainWindow)Application.Current.MainWindow).PrimaryWindow.Content).CanAccept(null))
                {
                    ((MainWindow)Application.Current.MainWindow).PrimaryWindow.Content = view;
                }
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).PrimaryWindow.Content = view;
            }
        }

        public void Initialize()
        {
            bool canContinue;
            Notifier.SetProcess("Buscando Módulos");
            try
            {
                Notifier.SetProgress(5);
                string[] Modules = Directory.GetFiles(ConfigurationContext.Directories.ModulesFolder, "*.dll");
                Notifier.SetProgress(10);
                canContinue = Modules.Length != 0;
                if (!canContinue)
                {
                    Notifier.SetProgress(20);
                    Notifier.SetUserMessage(false, "No se encontraron modulos para ejecutar!");
                    Notifier.SetUserMessage(false, "Finalizando proceso...");
                }
                else
                {
                    if (!GetModules(Modules))
                    {
                        canContinue = false;
                        Notifier.SetUserMessage(true, "No se encontró inicializador, el sistema no puede ser utilizado sin el mismo.");
                    }
                    else
                    {

                        Notifier.SetProgress(50);
                        canContinue = InitializeTerminal();

                        Notifier.SetProgress(60);
                        if (!canContinue)
                            Notifier.SetUserMessage(false, "Se ha finalizado la carga de la aplicación con algunos problemas encontrados.");
                        else
                            Notifier.SetUserMessage(false, "Se ha finalizado la carga de la aplicación correctamente!.");



                    }
                }
            }
            catch (Exception ex)
            {
                Notifier.SetProcess("Error!");
                Notifier.SetUserMessage(true, ex.ToString());
                canContinue = false;
            }

            if (!canContinue)
            {
                Initialized = false;
                Notifier.SendNotification("Ocurrio algun error en el momento de iniciar el programa, por favor lea el detalle del proceso!");
                Notifier.SetUserMessage(true, "Error");
                Notifier.SetProcess("Error!");
            }
            else
            {
                Initialized = true;
                Notifier.SetProcess("Listo");
            }

            Notifier.SetProgress(100);
        }

        private bool InitializeTerminal()
        {
            Notifier.SetProcess("Validando Módulos");
            bool ret = true;
            try
            {
                ZeroCommonClasses.Terminal.Instance.Manager.InitializeTerminal();
                ModuleList.ForEach(c => c.TerminalStatus = ZeroCommonClasses.Terminal.Instance.Manager.GetModuleStatus(c));
                if (ModuleList.Exists(c => c.TerminalStatus == ModuleStatus.NeedsSync))
                {
                    Initialized = false;
                    Notifier.SetUserMessage(true, "Algunas configuraciones pueden no estar sincronizadas con el servidor,\n"
                                                    + "por favor conectese con la central lo antes posible!");
                }
            }
            catch (Exception ex)
            {
                Notifier.SetUserMessage(false, "ERROR: " + ex);
                ret = false;
            }


            return ret;
        }

        private bool GetModules(string[] Modules)
        {
            Notifier.SetProcess("Cargando Módulos");
            string aux = "";

            foreach (var item in Modules)
            {
                try
                {
                    aux = Path.GetFileNameWithoutExtension(item);
                    Notifier.SetUserMessage(false, "Inicializando " + aux);
                    Assembly ass = Assembly.LoadFrom(item);
                    Type ty = ass.GetExportedTypes().FirstOrDefault(t => t.BaseType == typeof(ZeroModule));
                    
                    object obj = ass.CreateInstance(ty.ToString(), false, BindingFlags.CreateInstance,
                        null, null, CultureInfo.CurrentCulture, null);

                    TryAddModule(obj, item);

                }
                catch (Exception ex)
                {
                    Notifier.SetUserMessage(false, "Assembly '" + item + "' no es un modulo válido, error: " + ex);
                }
            }
            Notifier.SetProgress(30);
            if (ZeroCommonClasses.Terminal.Instance.Manager == null)
            {
                return false;
            }
            
            return true;
        }

        private void TryAddModule(object obj, string path)
        {

            if (obj is ZeroModule)
            {
                var mod = obj as ZeroModule;
                mod.TerminalStatus = ModuleStatus.Unknown;
                mod.UserStatus = ModuleStatus.Unknown;
                mod.WorkingDirectory = path + ConfigurationContext.Directories.WorkingDirSubfix;
                ModuleList.Add(mod);
                Notifier.SetUserMessage(false, "Módulo ensamblado --> ''" + mod.Description + "''");

            }
            else
            {
                Notifier.SetUserMessage(false, "Módulo invalido --> ''''");
            }
        }

        
    }
}

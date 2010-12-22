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
using TerminalZeroClient.Helpers;
using ZeroCommonClasses.GlobalObjects;
using System.Diagnostics;


namespace TerminalZeroClient.Business
{
    internal partial class ZeroClientManager
    {
        public bool IsAllOK { get; private set; }
        public ZeroSession Session {get; private set;}
        private ILogBuilder Logger = null;
        internal ITerminalClientManager Manager { get; private set; }
        public TraceSwitch LogLevel { get; private set; }

        internal ZeroClientManager()
        {
            IsAllOK = true;
            LogLevel = new TraceSwitch("ZeroLogLevelSwitch", "Zero Log Level Switch", "Error");
            Session = new ZeroSession();
            Session.AddNavigationParameter(new ZeroActionParameter<ISyncService>(false, App.Instance.ClientSyncServiceReference,false));
            Session.AddNavigationParameter(new ZeroActionParameter<IFileTransfer>(false, ZeroCommonClasses.Context.ContextBuilder.CreateFileTranferConnection(), false));
        }

        public void InitializeAppAsync()
        {
            bool canContinue = false;
            Session.Notifier.SetProcess("Buscando Módulos");
            try
            {
                Session.Notifier.SetProgress(5);


                string[] Modules = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, App.K_ModulesFolder), "*.dll");
                Session.Notifier.SetProgress(10);
                canContinue = Modules.Length != 0;
                if (!canContinue)
                {
                    Session.Notifier.SetProgress(20);
                    Session.Notifier.SetUserMessage(false, "No se encontraron modulos para ejecutar!");
                    Session.Notifier.SetUserMessage(false, "Finalizando proceso...");
                }
                else
                {
                    if (!GetModules(Modules))
                    {
                        canContinue = false;
                        Session.Notifier.SetUserMessage(true, "No se encontró inicializador, el sistema no puede ser utilizado sin el mismo.");
                    }
                    else
                    {

                        Session.Notifier.SetProgress(50);
                        canContinue = InitializeTerminal();

                        Session.Notifier.SetProgress(60);
                        if (!canContinue)
                            Session.Notifier.SetUserMessage(false, "Se ha finalizado la carga de la aplicación con algunos problemas encontrados.");
                        else
                            Session.Notifier.SetUserMessage(false, "Se ha finalizado la carga de la aplicación correctamente!.");



                    }
                }
            }
            catch (Exception ex)
            {
                Session.Notifier.SetProcess("Error!");
                Session.Notifier.SetUserMessage(true, ex.ToString());
                canContinue = false;
            }

            if (!canContinue)
            {
                IsAllOK = false;
                Session.Notifier.SendNotification("Ocurrio algun error en el momento de iniciar el programa, por favor lea el detalle del proceso!");
                Session.Notifier.SetUserMessage(true, "Error");
                Session.Notifier.SetProcess("Error!");
            }
            else
            {
                Session.Notifier.SetProcess("Listo");
            }

            Session.Notifier.SetProgress(100);
        }

        private bool InitializeTerminal()
        {
            Session.Notifier.SetProcess("Validando Módulos");
            bool ret = true;
            try
            {
                Manager.InitializeTerminal();
                Session.ModuleList.ForEach(c => c.TerminalStatus = Manager.GetModuleStatus(c));
                if (Session.ModuleList.Exists(c => c.TerminalStatus == ModuleStatus.NeedsSync))
                {
                    IsAllOK = false;
                    Session.Notifier.SetUserMessage(true, "Algunas configuraciones pueden no estar sincronizadas con el servidor,\n"
                                                    + "por favor conectese con la central lo antes posible!");
                }
            }
            catch (Exception ex)
            {
                Session.Notifier.SetUserMessage(false, "ERROR: " + ex.ToString());
                ret = false;
            }


            return ret;
        }

        private bool GetModules(string[] Modules)
        {
            Session.Notifier.SetProcess("Cargando Módulos");
            string aux = "";

            foreach (var item in Modules)
            {
                try
                {
                    aux = Path.GetFileNameWithoutExtension(item);
                    Session.Notifier.SetUserMessage(false, "Inicializando " + aux);
                    System.Reflection.Assembly ass = System.Reflection.Assembly.LoadFrom(item);
                    Type ty = ass.GetExportedTypes().FirstOrDefault(t => t.BaseType == typeof(ZeroCommonClasses.ZeroModule));
                    
                    object obj = ass.CreateInstance(ty.ToString(), false, System.Reflection.BindingFlags.CreateInstance,
                        null, new object[] { App.Instance as ITerminal }, System.Globalization.CultureInfo.InstalledUICulture, null);

                    TryAddModule(obj, item);

                }
                catch (Exception ex)
                {
                    Session.Notifier.SetUserMessage(false, "Assembly '" + item + "' no es un modulo válido, error: " + ex.ToString());
                }
            }
            Session.Notifier.SetProgress(30);
            return Manager != null;
        }

        private void TryAddModule(object obj, string path)
        {

            if (obj is ZeroModule)
            {
                ZeroModule mod = obj as ZeroModule;
                mod.TerminalStatus = ModuleStatus.Unknown;
                mod.UserStatus = ModuleStatus.Unknown;
                mod.WorkingDirectory = path + AppDirectories.WorkingDirSubfix;
                Session.AddModule(mod);
                Session.Notifier.SetUserMessage(false, "Módulo ensamblado --> ''" + mod.Description + "''");

                if (Manager == null && obj is ITerminalClientManager)
                    Manager = obj as ITerminalClientManager;

                if (Logger == null && obj is ILogBuilder)
                    Logger = obj as ILogBuilder;

            }
            else
            {
                Session.Notifier.SetUserMessage(false, "Módulo invalido --> ''''");
            }
        }

        public ZeroMenu BuildMenu()
        {
            List<ZeroAction> validActions = Session.BuilSessionActions();

            Session.ModuleList.ForEach(m => m.Init());

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

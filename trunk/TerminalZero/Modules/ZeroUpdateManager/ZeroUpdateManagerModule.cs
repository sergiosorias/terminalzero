using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.GlobalObjects;
using System.ComponentModel;

namespace ZeroUpdateManager
{
    public class ZeroUpdateManagerModule : ZeroCommonClasses.ZeroModule
    {
        public ZeroUpdateManagerModule(ITerminal terminal)
            : base(terminal, 5, "Maneja las actualizaciones del sistema")
        {

        }

        public override void BuildPosibleActions(List<ZeroCommonClasses.GlobalObjects.ZeroAction> actions)
        {
            actions.Add(new ZeroAction(ActionType.MenuItem, "Configuración@Actualizaciones@Base de datos", ImportScriptFile));
        }

        public override void BuildRulesActions(List<ZeroCommonClasses.GlobalObjects.ZeroRule> rules)
        {

        }

        public override string[] GetFilesToSend()
        {
            return new string[] { };
        }

        public override void Init()
        {

        }

        private void ImportScriptFile(ZeroRule rule)
        {
            ZeroUpdateManager.Database.DeployFile file = Database.DeployFile.LoadFrom(System.IO.Path.Combine(WorkingDirectory , "file.sql"));
            foreach (var item in file.GetStatements())
            {
                var a = ZeroCommonClasses.Context.ContextBuilder.ClientConnectionString;
                
            }
        }



    }
}

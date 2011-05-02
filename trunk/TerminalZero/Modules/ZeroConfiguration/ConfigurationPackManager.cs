using System.Collections.Generic;
using System.Linq;
using ZeroBusiness.Entities.Configuration;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Pack;

namespace ZeroConfiguration
{
    public class ConfigurationPackManager : PackManager
    {
        public ConfigurationPackManager(ITerminal terminal)
            : base(terminal)
        {
            
        }

        protected override void ExportProcess(PackProcessingEventArgs args)
        {
            base.ExportProcess(args);
            ((ExportEntitiesPackInfo) args.PackInfo).ExportTables();
        }

        protected override void ImportProcess(PackProcessingEventArgs args)
        {
            base.ImportProcess(args);
            var packInfo = (ExportEntitiesPackInfo)args.PackInfo;
            using (var ent = new ConfigurationModelManager())
            {
                if (packInfo.ContainsTable<Terminal>())
                {
                    ImportTerminal(ent, packInfo.GetTable<Terminal>());
                }

                if (packInfo.ContainsTable<TerminalProperty>())
                {
                    ImportTerminalProperties(ent, packInfo.GetTable<TerminalProperty>());
                }

                if (packInfo.ContainsTable<Module>())
                {
                    ImportModules(ent, packInfo.GetTable<Module>());
                }

            }
        }


        private void ImportModules(ConfigurationModelManager ent, IEnumerable<Module> items)
        {
            
        }

        private void ImportTerminalProperties(ConfigurationModelManager ent, IEnumerable<TerminalProperty> items)
        {
            
        }

        private void ImportTerminal(ConfigurationModelManager ent, IEnumerable<Terminal> items)
        {
            foreach (var item in items)
            {
                if (ent.Terminals.FirstOrDefault(t => t.Code == item.Code) == null)
                {
                    ent.Terminals.AddObject(item);
                }
                else
                    ent.Terminals.ApplyCurrentValues(item);
            }
            
        }

      


    }
}

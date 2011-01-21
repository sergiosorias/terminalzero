using System;
using System.IO;
using System.Linq;
using System.Xml;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.PackClasses;
using ZeroConfiguration.Entities;

namespace ZeroConfiguration
{
    public class ConfigurationPackManager : PackManager
    {
        public ConfigurationPackManager(ITerminal terminal)
            : base(terminal)
        {
            Exporting += new EventHandler<PackEventArgs>(ConfigurationPackManager_Exporting);
        }

        private void ConfigurationPackManager_Exporting(object sender, PackEventArgs e)
        {
            foreach (PackTableInfo item in ((ExportEntitiesPackInfo)e.PackInfo).Tables)
            {
                using (XmlWriter xmlwriter = XmlWriter.Create(Path.Combine(e.WorkingDirectory, item.RowType.ToString())))
                {
                    item.SerializeRows(xmlwriter);
                    xmlwriter.Close();
                }
            }
        }
        

        private void ImportConfigurations(object sender, PackEventArgs e)
        {
            ExportEntitiesPackInfo packInfo = (ExportEntitiesPackInfo)e.PackInfo;

            using (ConfigurationEntities ent = new ConfigurationEntities())
            {
                foreach (var item in packInfo.Tables)
                {
                    if (typeof(Terminal).ToString() == item.RowTypeName)
                    {
                        ImportTerminal(packInfo.Path, ent, item);
                    }
                    else if (typeof(TerminalProperty).ToString() == item.RowTypeName)
                    {
                        ImportTerminalProperties(packInfo.Path, ent, item);
                    }
                    else if (typeof(Module).ToString() == item.RowTypeName)
                    {
                        ImportModules(packInfo.Path, ent, item);
                    }
                }
            }
        }

        private void ImportModules(string p, ConfigurationEntities ent, PackTableInfo item)
        {
            
        }

        private void ImportTerminalProperties(string p, ConfigurationEntities ent, PackTableInfo item)
        {
            
        }

        private void ImportTerminal(string p, ConfigurationEntities ent, PackTableInfo inf)
        {
            foreach (var item in inf.DeserializeRows<Terminal>(p))
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

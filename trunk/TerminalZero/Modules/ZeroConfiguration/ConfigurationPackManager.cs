using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroCommonClasses.PackClasses;
using System.Xml;
using System.IO;
using ZeroConfiguration.Entities;
using ZeroCommonClasses.Helpers;

namespace ZeroConfiguration
{
    public class ConfigurationPackManager : PackManager
    {
        private ExportEntitiesPackInfo MyInfo;
        
        public ConfigurationPackManager(ExportEntitiesPackInfo info)
            : base(info)
        {
            MyInfo = info;
            Exporting += new EventHandler<PackEventArgs>(ConfigurationPackManager_Exporting);
        }

        private void ConfigurationPackManager_Exporting(object sender, PackEventArgs e)
        {
            foreach (PackTableInfo item in MyInfo.Tables)
            {
                using (XmlWriter xmlwriter = XmlWriter.Create(Path.Combine(e.WorkingDirectory, item.RowType.ToString())))
                {
                    item.SerializeRows(xmlwriter);
                    xmlwriter.Close();
                }
            }
        }

        public ConfigurationPackManager(string path)
            : base(path)
        {
            Importing += new EventHandler<PackEventArgs>(ImportConfigurations);
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

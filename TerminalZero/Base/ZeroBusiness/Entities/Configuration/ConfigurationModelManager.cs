using System.Linq;
using ZeroCommonClasses;

namespace ZeroBusiness.Entities.Configuration
{
    public class ConfigurationModelManager : Entities
    {
        public ConfigurationModelManager()
            : base(ZeroCommonClasses.Environment.Config.GetConnectionForCurrentEnvironment("Configuration.ConfigurationModel"))
        {
            
        }

        public static bool IsTerminalZero(ConfigurationModelManager configurationModelManager, int tCode)
        {
            bool ret = false;
            
            Terminal T = configurationModelManager.Terminals.First(t => t.Code == tCode);
            ret = T.IsTerminalZero;
            
            return ret;
        }

        public static ModuleStatus GetTerminalModuleStatus(ConfigurationModelManager configurationModelManager, int terminalCode, ZeroModule c)
        {
            ModuleStatus ret;
            Module M;
            M = configurationModelManager.Modules.FirstOrDefault(m => m.Code == c.ModuleCode);
            if (M == null)
            {
                ret = ModuleStatus.NeedsSync;
                Module.AddNewModule(configurationModelManager, terminalCode, c.ModuleCode, "", c.Description);
            }
            else
            {
                ret = M.Active.HasValue ? (M.Active.Value ? ModuleStatus.Valid : ModuleStatus.Invalid) : ModuleStatus.Unknown;
            }
            return ret;
        }

        public static void CreateTerminalProperties(ConfigurationModelManager configurationModelManager, int terminalCode)
        {
            TerminalProperty value = configurationModelManager.TerminalProperties.FirstOrDefault(t => t.Code == SystemProperty.SyncRecurrence.Code && t.TerminalCode == terminalCode);
            if (value == null)
            {
                value = new TerminalProperty
                {
                    TerminalCode = terminalCode,
                    Code = SystemProperty.SyncRecurrence.Code,
                    Value = SystemProperty.SyncRecurrence.DefaultValue,
                    Description = SystemProperty.SyncRecurrence.Description
                };
                configurationModelManager.TerminalProperties.AddObject(value);
                configurationModelManager.SaveChanges();
            }
            value = configurationModelManager.TerminalProperties.FirstOrDefault(t => t.Code == SystemProperty.HomeShortcut.Code && t.TerminalCode == terminalCode);
            if (value == null)
            {
                value = new TerminalProperty
                            {
                                TerminalCode = terminalCode,
                                Code = SystemProperty.HomeShortcut.Code,
                                Value = SystemProperty.HomeShortcut.DefaultValue,
                                LargeValue = "",
                                Description = SystemProperty.HomeShortcut.Description
                };
                configurationModelManager.TerminalProperties.AddObject(value);
                configurationModelManager.SaveChanges();
            }
        }
        

    }
}

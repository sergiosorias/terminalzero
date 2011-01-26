using System.Linq;
using ZeroCommonClasses;

namespace ZeroConfiguration.Entities
{
    public class ConfigurationEntities : Entities
    {
        public ConfigurationEntities()
            : base(ZeroCommonClasses.Context.ContextBuilder.GetConnectionForCurrentEnvironment("Configuration"))
        {
            
        }

        public static bool IsTerminalZero(ConfigurationEntities configurationEntities, int tCode)
        {
            bool ret = false;
            
            Terminal T = configurationEntities.Terminals.First(t => t.Code == tCode);
            ret = T.IsTerminalZero;
            
            return ret;
        }

        internal static ModuleStatus GetTerminalModuleStatus(ConfigurationEntities configurationEntities, int terminalCode, ZeroModule c)
        {
            ModuleStatus ret;
            Module M;
            M = configurationEntities.Modules.FirstOrDefault(m => m.Code == c.ModuleCode);
            if (M == null)
            {
                ret = ModuleStatus.NeedsSync;
                Module.AddNewModule(configurationEntities, terminalCode, c.ModuleCode, "", c.Description);
            }
            else
            {
                ret = M.Active.HasValue ? (M.Active.Value ? ModuleStatus.Valid : ModuleStatus.Invalid) : ModuleStatus.Unknown;
            }
            return ret;
        }

        internal static void CreateTerminalProperties(ConfigurationEntities configurationEntities, int terminalCode)
        {
            TerminalProperty value = configurationEntities.TerminalProperties.FirstOrDefault(t => t.Code == Namespace.Properties.SyncRecurrence && t.TerminalCode == terminalCode);
            if (value == null)
            {
                value = new TerminalProperty
                {
                    TerminalCode = terminalCode,
                    Code = Namespace.Properties.SyncRecurrence,
                    Value = "10",
                    Description = "Valor que determina cada cuanto (en minutos) la sucursal sincronizara automaticamente"
                };
                configurationEntities.TerminalProperties.AddObject(value);
                configurationEntities.SaveChanges();
            }
            value = configurationEntities.TerminalProperties.FirstOrDefault(t => t.Code == Namespace.Properties.HomeShorcuts && t.TerminalCode == terminalCode);
            if (value == null)
            {
                value = new TerminalProperty
                {
                    TerminalCode = terminalCode,
                    Code = Namespace.Properties.HomeShorcuts,
                    Value = "No se usa",
                    LargeValue = "",
                    Description = "Accesos directos de la página de inicio"
                };
                configurationEntities.TerminalProperties.AddObject(value);
                configurationEntities.SaveChanges();
            }
        }

        


    }
}

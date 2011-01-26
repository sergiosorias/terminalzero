using System.Linq;

namespace ZeroConfiguration.Entities
{
    public partial class Module
    {
        internal static Module AddNewModule(ConfigurationEntities configurationEntities, int terminalCode, int moduleCode, string moduleName, string moduleDescription)
        {
            Module T = CreateModule(moduleCode, moduleName);
            T.Description = moduleDescription;
            T.Terminals.Add(configurationEntities.Terminals.First(t => t.Code == terminalCode));
            configurationEntities.AddToModules(T);
            configurationEntities.SaveChanges();
            return T;
        }
    }
}
using System.Linq;

namespace ZeroBusiness.Entities.Configuration
{
    public partial class Module
    {
        public static Module AddNewModule(ConfigurationModelManager configurationModelManager, int terminalCode, int moduleCode, string moduleName, string moduleDescription)
        {
            Module T = CreateModule(moduleCode, moduleName);
            T.Description = moduleDescription;
            T.Terminals.Add(configurationModelManager.Terminals.First(t => t.Code == terminalCode));
            configurationModelManager.AddToModules(T);
            configurationModelManager.SaveChanges();
            return T;
        }
    }
}
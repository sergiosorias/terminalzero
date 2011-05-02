namespace ZeroBusiness.Entities.Configuration
{
    public partial class Terminal
    {
        public static Terminal AddNewTerminal(ConfigurationModelManager configurationModelManager, int terminalCode, string terminalName)
        {
            Terminal T = CreateTerminal(terminalCode, terminalName, true, terminalCode == 0);
            T.IsSyncronized = false;
            configurationModelManager.AddToTerminals(T);
            configurationModelManager.SaveChanges();

            return T;
        }
    }
}
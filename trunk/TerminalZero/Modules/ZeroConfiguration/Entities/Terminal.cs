namespace ZeroConfiguration.Entities
{
    public partial class Terminal
    {
        internal static Terminal AddNewTerminal(ConfigurationEntities configurationEntities, int terminalCode, string terminalName)
        {
            Terminal T = CreateTerminal(terminalCode, terminalName, true, terminalCode == 0);
            T.IsSyncronized = false;
            configurationEntities.AddToTerminals(T);
            configurationEntities.SaveChanges();

            return T;
        }
    }
}
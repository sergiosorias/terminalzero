
namespace ZeroCommonClasses.Interfaces
{
    public interface ITerminal
    {
        int Code { get; }
        string TerminalName { get; }
        ZeroSession Session { get; }
        ITerminalManager Manager { get; set; }
        IZeroClient Client { get; set; }
    }
}

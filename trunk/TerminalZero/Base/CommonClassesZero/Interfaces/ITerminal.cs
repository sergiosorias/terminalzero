
namespace ZeroCommonClasses.Interfaces
{
    public interface ITerminal
    {
        int TerminalCode { get; }
        string TerminalName { get; }
        ZeroSession Session { get; }
        ITerminalManager Manager { get; set; }
        IZeroClient CurrentClient { get; set; }
    }
}

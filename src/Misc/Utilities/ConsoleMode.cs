namespace Quizzical.Misc.Utilities;

public static class ConsoleMode
{
    public static bool IsInteractive => !Console.IsInputRedirected && !Console.IsOutputRedirected;
}
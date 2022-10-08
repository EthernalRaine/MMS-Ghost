namespace GhostIM.Utility;

public static class Logger
{
    public static void write(String szPrefix, String szData)
    {
        Console.Write("[");
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write(szPrefix);
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("] ");
        Console.WriteLine(szData);
    }
}
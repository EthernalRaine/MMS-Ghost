using GhostIM.MSIM;
using GhostIM.Utility;

namespace GhostIM 
{
    static class Entry
    {
        internal static void Main()
        {
            Logger.write("Entry", "Loading GhostIM Server");
            new Thread(() =>
            {
                Logger.write("MySpaceIM Server", "Initializing Server");
                Server.msimServer();
            }).Start();
        }
            
    }
}
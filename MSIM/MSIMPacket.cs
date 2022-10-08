using System.Text;
using GhostIM.Utility;

namespace GhostIM.MSIM;

public static class MSIMPacket
{
    public static void SendPacket(TcpServer srv, string packet)
    {
        byte[] packetBytes = Encoding.ASCII.GetBytes(packet);
        
        srv.WriteTraffic(packetBytes, 0, packetBytes.Length);
        if (Server.debugmode)
            Logger.write("MSIM Debug", "SendPacket | " + packet);
    }

    public static void RecvPacket(MSIMClient client, TcpServer srv)
    {
        var traffic = srv.ReadTraffic();
        string decodedPacket = Encoding.ASCII.GetString(traffic.Item1, 0, traffic.Item2);

        if (decodedPacket != "")
        {
            if (Server.debugmode)
               Logger.write("MSIM Debug", "RecvPacket | " + decodedPacket);
            
            /*
             * respond to packet
             */
           
            MSIMResponse.RespondToPacket(client, srv, decodedPacket);
        }
    }

    public static bool Logout(MSIMClient client)
    {
        try
        {
            if (client.logout)
                return true;
            return false;
        }
        catch
        {
            return true;
        }
    }
}
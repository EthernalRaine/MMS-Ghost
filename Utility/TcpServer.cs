using System.Net;
using System.Net.Sockets;

namespace GhostIM.Utility;

public class TcpServer {
    private static TcpListener listener;
    private static TcpClient client;

    public TcpServer(IPAddress addr, int port) {
        listener = new TcpListener(addr, port);
        listener.Start();
        Logger.write("TcpServer", "Opened listener on " + addr + ":" + port);
    }

    public TcpClient GetClient() {
        return client = listener.AcceptTcpClient();
    }

    public void WriteTraffic(byte[] bytes, int offset, int len) {
        client.GetStream().Write(bytes, offset, len);
    }
    
    public Tuple<byte[], int> ReadTraffic() {
        byte[] bytes = new byte[4096];
        if (client != null)
        {
            int ix = client.GetStream().Read(bytes, 0, bytes.Length);
            return new (bytes, ix);
        }
        return null;
    }
}
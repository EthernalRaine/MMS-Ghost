using System.Net.Sockets;

namespace GhostIM.MSIM;

public class MSIMClient {
    public MSIMClient(byte[] nonce, int sessionkey, int userid, TcpClient client) {
        sessKey = sessionkey;
        userId = userid;
        tcpClient = client;
        clientNonce = nonce;
        logout = false;
    }

    public byte[] clientNonce;
    public int sessKey;
    public int userId;
    public TcpClient tcpClient;
    public bool logout;
}
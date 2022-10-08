using System.Net.Sockets;

namespace GhostIM.MSIM;

public class MSIMClient {
    public MSIMClient(byte[] nonce, int sessionkey, int userid, string screenname, int clientversion, TcpClient client) {
        sessKey = sessionkey;
        userId = userid;
        tcpClient = client;
        clientNonce = nonce;
        logout = false;
        screenName = screenname;
        clientVer = clientversion;
    }

    public byte[] clientNonce;
    public int sessKey;
    public int userId;
    public TcpClient tcpClient;
    public string screenName;
    public int clientVer;
    public bool logout;
}
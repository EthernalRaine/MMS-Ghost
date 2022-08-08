using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Text;
using GhostIM.Utility;
using MySql.Data.MySqlClient;

namespace GhostIM.MSIM;

public class Server
{
    public static List<MSIMClient> clients = new List<MSIMClient>();
    public static readonly bool debugmode = false;
    
    static public void msimServer()
    {
        const string connectCtx = @"server=localhost;userid=ghost;password=oe_FDC[]L0sYJ5eN;database=im";
        MySqlConnection conn = new MySqlConnection(connectCtx);
        conn.Open();
        
        TcpServer srv = new TcpServer(IPAddress.Any, 1863); // alt port

        while (true) 
        {
            TcpClient client = srv.GetClient();
            new Thread(() =>
            {
                if (client.Client.RemoteEndPoint != null)
                    Logger.write("MySpaceIM Server", "Client awaiting Authentication from " + client.Client.RemoteEndPoint);
                string response = "";
                
                /* MSIM Server
                 * Coded by AzureDiamond
                 * Copyright 2022 | ghost.im
                 */

                /*
                 * Handshake Client
                 */

                byte[] nonce = MSIMStructure.GenerateNonce();
                response = "\\lc\\1\\nc\\" + Encoding.ASCII.GetString(nonce) + "\\id\\1\\final\\";
                MSIMPacket.SendPacket(srv, response);
                
                /*
                 * Client Structure
                 */

                var login2 = srv.ReadTraffic();
                string decodedPacket = Encoding.ASCII.GetString(login2.Item1, 0, login2.Item2);
                
                if (Server.debugmode) 
                    Logger.write("MSIM Debug", "RecvPacket | " + decodedPacket);
                
                int sessionkey = MSIMStructure.GenerateSessionKey();

                string username = MSIMBuilder.Decode("username", login2.Item1);
                var sql = new MySqlCommand("SELECT * FROM `users` WHERE `username` LIKE '" + username + "'", conn).ExecuteReader();
                sql.Read();
                int userid = sql.GetInt32(0);
                string screenName = sql.GetString(3);

                /*
                 * Authenticate Client
                 */
                
                response = "\\lc\\2\\sesskey\\" + sessionkey + "\\proof\\" + sessionkey +
                           "\\userid\\" + userid + "\\profileid\\" + userid +
                           "\\uniquenick\\" + screenName + "\\id\\1\\final\\";
                
                MSIMClient ctx = new MSIMClient(nonce, sessionkey, userid, client);
                clients.Add(ctx);
                
                MSIMPacket.SendPacket(srv, response);

                string version = MSIMBuilder.Decode("clientver", login2.Item1);
                Logger.write("MySpaceIM Server","Client authenticated! | Screenname: " + screenName + " | Client Version: 1.0." + version + ".0");
                
                while (true)
                {
                    try
                    {
                        //  srv.WriteTraffic(new byte[] { 0 }, 0, 0);
                        Tuple<byte[], int> traffic = srv.ReadTraffic();
                        if (traffic != null) 
                            MSIMPacket.RecvPacket(ctx, srv);
                        // Item1 = Byte Packets, Item2 = Packet Length

                        if (!client.Connected)
                            break;
                        
                        if (MSIMPacket.Logout(ctx))
                            break;
                    }
                    catch
                    {
                        // ignored
                        break;
                    }
                }


                // Dispose
                Logger.write("MySpaceIM Server", "Client disconnected! | Screenname: " + screenName + " | Client Version: 1.0." + version + ".0");
                client.Dispose();
                clients.Remove(ctx);
                sql.Dispose();
                sql.Close();
                
            }).Start();
        }
    }
}
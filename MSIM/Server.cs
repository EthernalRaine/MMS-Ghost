using System.Net;
using System.Net.Sockets;
using System.Timers;
using GhostIM.Utility;
using Org.BouncyCastle.Asn1.X509;
using Timer = System.Timers.Timer;

namespace GhostIM.MSIM;

public static class Server
{
    public static List<MSIMClient> clients = new List<MSIMClient>();
    public const bool debugmode = true;

    private static async void KeepaliveFireCallback(Object source, ElapsedEventArgs e, TcpServer srv, Tuple<MSIMClient, bool> ctx)
    {
        if (!ctx.Item2)
            return;

        if (ctx.Item1.logout)
            return;

        try
        {
            MSIMPacket.SendPacket(srv, "\\ka\\\\\\final\\");
        }
        catch
        {
            // ignored
            return;
        }
    } 
    
    static public void msimServer()
    {
        TcpServer srv = new TcpServer(IPAddress.Any, 1863); // alt port

        while (true) 
        {
            TcpClient client = srv.GetClient();
            new Thread(() =>
            {
                if (client.Client.RemoteEndPoint != null)
                    Logger.write("MySpaceIM Server", "Client awaiting Authentication from " + client.Client.RemoteEndPoint);

                /* MSIM Server
                 * Coded by EthernalRaine/lnkexploit
                 * Copyright 2022 | ghost.im
                 */

                /*
                 * Handshake Client
                 */

                var ctx = MSIMResponse.HandshakeClient(srv, client);
                
                Thread.Sleep(250);
                
                /*
                 * Launch Keepalive Timer
                 */
                Timer t = new Timer(180000);
                t.AutoReset = true;
                t.Elapsed += (sender, e) => KeepaliveFireCallback(sender, e, srv, ctx);
                t.Start();

                Thread.Sleep(2500);
                
                //MSIMResponse.SendBuddyMessage(MSIMBuddyMessageType.InstantMessage, ctx.Item1.userId, "<f f='Times' h='16'><c v='black'><b v='white'>hello</1b></1c></1f></1p>", ctx.Item1, srv);

                while (true)
                {
                    try
                    {
                        srv.WriteTraffic(new byte[] { 69 }, 0, 0);
                        Tuple<byte[], int> traffic = srv.ReadTraffic();
                        if (traffic != null) 
                            MSIMPacket.RecvPacket(ctx.Item1, srv);
                        // Item1 = Byte Packets, Item2 = Packet Length

                        if (MSIMPacket.Logout(ctx.Item1))
                            break;
                    }
                    catch
                    {
                        // ignored
                        break;
                    }
                }


                // Dispose
                if (ctx.Item2)
                {
                    Logger.write("MySpaceIM Server", "Client disconnected! | Screenname: " + ctx!.Item1.screenName);
                    clients.Remove(ctx.Item1);
                }
                else
                {
                    Logger.write("MySpaceIM Server", "Client disconnected! | Screenname: Unknown");
                }
                client.Dispose();
                t.Dispose();
                t.Close();

            }).Start();
        }
    }
}
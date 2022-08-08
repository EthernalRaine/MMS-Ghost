using System.Net;
using System.Net.Sockets;
using GhostIM.Utility;

namespace GhostIM.OSCAR;

static class AuthServer
{
    static void plainAuth()
    {
        TcpServer srv = new TcpServer(IPAddress.Any, 5190);

        while (true)
        {
            TcpClient client = srv.GetClient();
            client.SendTimeout = 2000;
            new Thread(() =>
            {
                
                /* AIM Server
                 * Coded by AzureDiamond
                 * Copyright 2022 | ghost.im
                 */
                
                
                
            }).Start();
        }
    }

    static void sslAuth()
    {
        
    }
}
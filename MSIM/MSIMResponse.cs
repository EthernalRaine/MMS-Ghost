using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using GhostIM.Utility;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Modes;

namespace GhostIM.MSIM;

public enum MSIMBuddyMessageType
{
    InstantMessage = 1,
    StatusMessage = 100,
    ActionMessage = 121,
    MediaMessage = 122,
    ProfileMessage = 124,
    StatusMoodMessage = 126
}

public static class MSIMResponse
{
    public static void RespondToPacket(MSIMClient client, TcpServer srv, string packet)
    {
        switch (MSIMInterpret.getPacketType(packet))
        {
        //    case MSIMPackets.msim_login_challenge:
        //        LoginChallengeResponse(srv, client, packet);
        //       break;
            case MSIMPackets.msim_logout:
                LogoutClient(client);
                break;
            case MSIMPackets.msim_callback_request:
                break;
            
            /*fallthrough*/

            case MSIMPackets.msim_login_challenge:
            case MSIMPackets.msim_keepalive:
            case MSIMPackets.msim_callback_reply:
            case MSIMPackets.msim_login_initial:
            case MSIMPackets.msim_login_response:
            case MSIMPackets.msim_not_a_packet:
            case MSIMPackets.msim_unknown_packet:
            default:
                break;
        }
        
    }

    public static Tuple<MSIMClient, bool> HandshakeClient(TcpServer srv, TcpClient client)
    {
        byte[] nonce = MSIMStructure.GenerateNonce();
        string base64 = Convert.ToBase64String(nonce);
        string response = "\\lc\\1\\nc\\" + base64 + "\\id\\1\\final\\";
        MSIMPacket.SendPacket(srv, response);

        return LoginChallengeResponse(srv, client, nonce);
    }
    
    private static Tuple<MSIMClient, bool> LoginChallengeResponse(TcpServer srv, TcpClient client, byte[] nonce)
    {
        bool bAuthed = false;

        var packet = srv.ReadTraffic();
        
        string username = MSIMBuilder.Decode("username", packet.Item1);
        string version = MSIMBuilder.Decode("clientver", packet.Item1);

        int userid = UserData.GetUserId(username);
        int sessionkey = MSIMStructure.GenerateSessionKey();
        string screenname = UserData.GetScreenName(username);
        string password = UserData.GetPassword(username);

        string response = MSIMBuilder.Encode(MSIMPackets.msim_login_response, new[]
        {
            Tuple.Create("sesskey", Convert.ToString(sessionkey)),
            Tuple.Create("proof", Convert.ToString(sessionkey)),
            Tuple.Create("userid", Convert.ToString(userid)),
            Tuple.Create("profileid", Convert.ToString(userid)), 
            Tuple.Create("uniquenick", screenname),
            Tuple.Create("id", "1")
        });
        
        /*
         * nc1 = 0x20 first bytes
         * nc2 = 0x20 last bytes
         * 
         * Key:
         * SHA1(SHA1(userpw) + nc2)
         *
         * Data:
         * nc1+username+ip_list
         *
         * ip_list:
         * 00 00 00 00
         * 1 byte network interface count
         * connection address
         * host network interfaces
         */
        
        byte[] byte_nc2 = new byte[32];
        byte[] byte_rc4_key = new byte[16];
        
        byte[] byte_challenge = nonce;

        Encoding utf16;
        byte[] byte_password;
        
        try
        {
            for (int i = 0; i < 32; i++)
                byte_nc2[i] = byte_challenge[i + 32];
            
            utf16 = Encoding.GetEncoding("UTF-16LE");
            byte_password = utf16.GetBytes(password);
        }
        catch
        {
            return new Tuple<MSIMClient, bool>(null, false);
        }
        
        SHA1 sha = SHA1.Create();
        byte[] byte_hash_phase1 = sha.ComputeHash(byte_password);

        byte[] byte_hash_phase2 = byte_hash_phase1.Concat(byte_nc2).ToArray();
        byte[] byte_hash_total = sha.ComputeHash(byte_hash_phase2);

        for (int i = 0; i < 16; i++)
            byte_rc4_key[i] = byte_hash_total[i];

        string packetrc4data = MSIMBuilder.Decode("response", packet.Item1);
        byte[] byte_rc4_data = Convert.FromBase64String(packetrc4data);
        
        byte[] rc4data = RC4.Decrypt(byte_rc4_key, byte_rc4_data);
       
        if (Encoding.ASCII.GetString(rc4data).Contains(username))
            bAuthed = true;
        
        if (bAuthed)
        {
            MSIMClient ctx = new MSIMClient(nonce, sessionkey, userid, screenname, Convert.ToInt32(version), client);
            
            Server.clients.Add(ctx);
            Logger.write("MySpaceIM Server","Client authenticated! | Screenname: " 
                                            + screenname + " | Client Version: 1.0." + version + ".0");
            MSIMPacket.SendPacket(srv, response);

            return new Tuple<MSIMClient, bool>(ctx, true);
        }
        else
        {
            string errResponse = MSIMBuilder.Encode(MSIMPackets.msim_error, new[]
            {
                Tuple.Create("\\errmsg", "The password provided is incorrect."),
                Tuple.Create("err", "260"),
                Tuple.Create("fatal", "\\")
            });
            
            MSIMPacket.SendPacket(srv, errResponse);
        }

        return new Tuple<MSIMClient, bool>(null, false);
    }
    
    private static void LogoutClient(MSIMClient client)
    {
        client.logout = true;
    }

    public static void SendBuddyMessage(MSIMBuddyMessageType buddyMessageType, int SendToUid, string msg, MSIMClient ctn, TcpServer srv)
    {
        switch (buddyMessageType)
        {
            case MSIMBuddyMessageType.InstantMessage:
                long unixtime = DateTimeOffset.Now.ToUnixTimeSeconds();
                string imdata = "\\bm\\1\\sesskey\\" + ctn.sessKey + "\\t\\" + SendToUid + "\\msg\\" + msg + "\\final\\";
                MSIMPacket.SendPacket(srv, imdata);
                break;
        }
    }
}
using System.Text;

namespace GhostIM.MSIM;

public enum MSIMPackets
{
    /*all*/
    msim_not_a_packet = -2,     // garbage
    msim_unknown_packet = -1,   // unknown packet
    msim_error,                 // error
    
    /*send*/
    msim_login_initial,         // lc 1
    msim_login_response,        // lc 2
    msim_keepalive,             // keepalive
    msim_callback_reply,        // persiststr
    
    /*recv*/
    msim_login_challenge,       // login2
    msim_logout,                // logout
    msim_callback_request,      // persist
}

public static class MSIMInterpret
{
    public static MSIMPackets getPacketType(string packet)
    {
        if (!packet.Contains("\\"))
            return MSIMPackets.msim_not_a_packet;
        else
        {
            if (MSIMBuilder.Decode("lc", Encoding.ASCII.GetBytes(packet)) == "1")
                return MSIMPackets.msim_login_initial;

            if (packet.Contains("\\login2"))
                return MSIMPackets.msim_login_challenge;
            
            if (MSIMBuilder.Decode("lc", Encoding.ASCII.GetBytes(packet)) == "2")
                return MSIMPackets.msim_login_response;

            if (packet.Contains("\\logout"))
                return MSIMPackets.msim_logout;

            if (packet.Contains("\\keepalive"))
                return MSIMPackets.msim_keepalive;

            if (packet.Contains("\\persist"))
                return MSIMPackets.msim_callback_request;

            if (packet.Contains("\\periststr"))
                return MSIMPackets.msim_callback_reply;

            if (packet.Contains("\\error"))
                return MSIMPackets.msim_error;
        }
        
        return MSIMPackets.msim_unknown_packet;
    }

    public static string getPacketHeader(MSIMPackets packet)
    {
        switch (packet)
        {
            /*unk*/
            
            case MSIMPackets.msim_error:
                return "\\error";
            case MSIMPackets.msim_not_a_packet:
            case MSIMPackets.msim_unknown_packet:
                return "**invalid**";
            
            /*send*/
            
            case MSIMPackets.msim_login_initial:
                return "\\lc\\1";
            case MSIMPackets.msim_login_response:
                return "\\lc\\2";
            case MSIMPackets.msim_keepalive:
                return "\\ka";
            case MSIMPackets.msim_callback_reply:
                return "\\persiststr";
            
            /*recv*/
            
            case MSIMPackets.msim_login_challenge:
                return "\\login2";
            case MSIMPackets.msim_logout:
                return "\\logout";
            case MSIMPackets.msim_callback_request:
                return "\\persist";

            default:
                return "**invalid**";
        }
    }
}
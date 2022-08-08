using System.Text;

namespace GhostIM.MSIM;

public class MSIMBuilder
{
    
    public static string Decode(string search, byte[] packet) {
        string decodedPacket = Encoding.ASCII.GetString(packet);
        string[] splits = decodedPacket.Split("\\");

        for (int ix = 0; ix < splits.Length; ix++)
        {
            if (splits[ix] == search)
                return splits[ix + 1];
        }

        return "**invalid**";
    }
}
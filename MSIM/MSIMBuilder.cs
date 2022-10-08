using System.Text;

namespace GhostIM.MSIM;

public static class MSIMBuilder
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
    
    public static string Decode(string search, string packet) {
        string[] splits = packet.Split("\\");

        for (int ix = 0; ix < splits.Length; ix++)
        {
            if (splits[ix] == search)
                return splits[ix + 1];
        }

        return "**invalid**";
    }

    public static string Encode(MSIMPackets packet, Tuple<string, string>[] datapairs)
    {
        string header = MSIMInterpret.getPacketHeader(packet);
        string final = "";

        final += header;
        foreach (var datanode in datapairs)
        {
            final += "\\" + datanode.Item1;
            final += "\\" + datanode.Item2;
        }

        final += "\\final\\";

        return final;
    }
}
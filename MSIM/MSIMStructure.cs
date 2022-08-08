using System.Text;
using GhostIM.Utility;

namespace GhostIM.MSIM;

public class MSIMStructure
{
    /*
     * This generates 64 random bytes as a nonce
     */
    
    public static byte[] GenerateNonce() {
        const string inclusive = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890+*-äöü|.,:;_#'~";
        byte[] nonce = Randomness.RandomBytesExP(0x40, new(Encoding.ASCII.GetBytes(inclusive)));
        return nonce;
    }

    /*
     * This generates a Random Int for a session key
     */
    
    public static int GenerateSessionKey()
    {
        return Randomness.RandomIntegers(100000);
    }
}
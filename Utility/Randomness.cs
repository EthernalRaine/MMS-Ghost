namespace GhostIM.Utility;

public class Randomness
{
    public static IEnumerable<byte> RandomBytes(int size) {
        Random random = new Random();
        byte[] buffer = new byte[size];

        while (true)
        {
            random.NextBytes(buffer);
            foreach (byte returnByte in buffer)
            {
                yield return returnByte;
            }
        }
    }

    public static byte[] RandomBytesEx(int size, HashSet<byte> exclusions) {
        return RandomBytes(size).Where(x => exclusions.Contains(x) == false).Take(size).ToArray();
    }

    public static byte[] RandomBytesExP(int size, HashSet<byte> exclusive) {
        return RandomBytes(size).Where(exclusive.Contains).Take(size).ToArray();
    }

    public static int RandomIntegers(int sizeLimit) {
        Random random = new Random();
        return random.Next(sizeLimit);
    }
}
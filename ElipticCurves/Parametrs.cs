using System.Globalization;
using System.Numerics;
namespace ElipticCurves
{
    public class Parametrs
    {
        public  BigInteger a = new BigInteger(-3);
        public BigInteger b = BigInteger.Parse("64210519e59c80e70fa7e9ab72243049feb8deecc146b9b1", NumberStyles.HexNumber);
        public BigInteger p  = BigInteger.Parse("6277101735386680763835789423207666416083908700390324961279");
        public BigInteger n = BigInteger.Parse("6277101735386680763835789423176059013767194773182842284081");
    }
}
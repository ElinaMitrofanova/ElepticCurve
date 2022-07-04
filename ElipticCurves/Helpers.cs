using System.Numerics;

namespace ElipticCurves
{
    internal class Helpers
    {
        public BigInteger Inverse(BigInteger num, BigInteger mod)
        {
            BigInteger q, r, t, u1 = BigInteger.One, u2 = BigInteger.Zero, v1 = BigInteger.Zero, v2 = BigInteger.One,
                        a = num, b = mod;
            while (b != BigInteger.Zero)
            {
                q = a / b;
                r = a % b;
                a = b; b = r;
                t = u2;
                u2 = u1 - q * u2;
                u1 = t;
                t = v2;
                v2 = v1 - q * v2;
                v1 = t;
            }
            if (u1 < BigInteger.Zero)
            {
                u1 += mod;
            }
            return u1;
        }

        public Boolean CompareAffine(AffinePoint a, AffinePoint b)
        {
            return a.GetX() == b.GetX() && a.GetY() == b.GetY();
        }

        public Boolean CompareProjective(ProjectivePoint a, ProjectivePoint b)
        {
            return a.GetX() == b.GetX() && a.GetY() == b.GetY() && a.GetZ() == b.GetZ();
        }

    }
}
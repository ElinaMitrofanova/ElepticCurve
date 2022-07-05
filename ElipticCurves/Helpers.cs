using System.Numerics;
using System.Security.Cryptography;

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

        public Tuple<BigInteger, BigInteger> Signature(string input, ProjectivePoint point, BigInteger secretKey, ElipticCurve _elipticCurve, BigInteger n)
        {
            var H = SHA256.Create();
            var bytes = System.Text.Encoding.ASCII.GetBytes(input);
            BigInteger r;
            BigInteger hashValueBigInt;
            BigInteger k;
            do
            {
                var hashValue = H.ComputeHash(bytes);
                hashValueBigInt = new BigInteger(hashValue);
                k = _elipticCurve.GenerateBigInteger(n);
                var kP = _elipticCurve.ScalarMultiplication(point, k);
                var affine = _elipticCurve.ToAffine(kP);
                r = affine.GetX() % n;

            } while (r == BigInteger.Zero);
            var s = (_elipticCurve.Inverse(k, n) * (hashValueBigInt + secretKey * r)) % n;
            return Tuple.Create(r, s);
        }

        public Boolean Verify(string input, Tuple<BigInteger, BigInteger> signature, ProjectivePoint publicKey, ElipticCurve _elipticCurve, BigInteger n, ProjectivePoint point)
        {
            var H = SHA256.Create();
            var bytes = System.Text.Encoding.ASCII.GetBytes(input);
            var hashValue = H.ComputeHash(bytes);
            var hashValueBigInt = new BigInteger(hashValue);
            var u1 = (_elipticCurve.Inverse(signature.Item2, n) * hashValueBigInt) % n;
            var u2 = (_elipticCurve.Inverse(signature.Item1, n) * hashValueBigInt) % n;
            var result = _elipticCurve.AddPoints(_elipticCurve.ScalarMultiplication(point, u1), _elipticCurve.ScalarMultiplication(publicKey, u2));
            var affine = _elipticCurve.ToAffine(result);
            var v = affine.GetX() % n;
            return v == signature.Item1;
        }
    }
}
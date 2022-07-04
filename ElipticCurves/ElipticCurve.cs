using System.Numerics;
using System.Collections;

namespace ElipticCurves
{
    internal class ElipticCurve
    {
        private BigInteger A;
        private BigInteger B;
        private BigInteger P;
        public ProjectivePoint PROJECTIVE_INF = new ProjectivePoint(BigInteger.Zero, BigInteger.One, BigInteger.Zero);
        public AffinePoint AFFINE_INF = new AffinePoint(BigInteger.Zero, BigInteger.Zero);

        static string HexToBin(string hexString)
        {
            string binString = "";

            for (int i = 0; i < hexString.Length; i++)
            {
                if (hexString[i] == '0') { binString += "0000"; }
                if (hexString[i] == '1') { binString += "0001"; }
                if (hexString[i] == '2') { binString += "0010"; }
                if (hexString[i] == '3') { binString += "0011"; }
                if (hexString[i] == '4') { binString += "0100"; }
                if (hexString[i] == '5') { binString += "0101"; }
                if (hexString[i] == '6') { binString += "0110"; }
                if (hexString[i] == '7') { binString += "0111"; }
                if (hexString[i] == '8') { binString += "1000"; }
                if (hexString[i] == '9') { binString += "1001"; }
                if (hexString[i] == 'A') { binString += "1010"; }
                if (hexString[i] == 'B') { binString += "1011"; }
                if (hexString[i] == 'C') { binString += "1100"; }
                if (hexString[i] == 'D') { binString += "1101"; }
                if (hexString[i] == 'E') { binString += "1110"; }
                if (hexString[i] == 'F') { binString += "1111"; }
            }

            return binString;
        }

        static BigInteger ParseHex(string hexStr)
        {
            string binaryStr = HexToBin(hexStr);
            var res = BigInteger.Zero;
            foreach (char c in binaryStr)
            {
                res <<= 1;
                res += c == '1' ? 1 : 0;
            }

            return res;
        }
        public ElipticCurve(BigInteger a, BigInteger b, BigInteger p)
        {
            this.A = a;
            this.B = b;
            this.P = p;
        }
        public  BigInteger GenerateBigInteger(BigInteger max)
        {
            Random rnd = new Random();
            byte[] maxBytes = max.ToByteArray(true, false);
            byte[] seedBytes = new byte[maxBytes.Length];

            rnd.NextBytes(seedBytes);
            seedBytes[seedBytes.Length - 1] &= (byte)0x7F;
            var seed = new BigInteger(seedBytes);

            while (seed > max || seed < new BigInteger(2))
            {
                rnd.NextBytes(seedBytes);
                seedBytes[seedBytes.Length - 1] &= (byte)0x7F;
                seed = new BigInteger(seedBytes);
            }

            return seed;
        }

        public AffinePoint createAffinePoint()
        {
            AffinePoint result = new AffinePoint(new BigInteger(-1), new BigInteger(-1));
            while (!IsOnCurveAffine(result))
            {
                BigInteger xPoint = GenerateBigInteger(this.P);
                var temp = BigInteger.Multiply(A, xPoint) + BigInteger.Pow(xPoint, 3) + B;
                BigInteger yPoint = SqrtMod(temp%this.P, this.P);
                result = new AffinePoint(xPoint, yPoint);
            }

            return result;
        }
        public Boolean CompareAffine(AffinePoint a, AffinePoint b)
        {
            return a.GetX() == b.GetX() && a.GetY() == b.GetY();
        }

        public Boolean CompareProjective(ProjectivePoint a, ProjectivePoint b)
        {
            return a.GetX() == b.GetX() && a.GetY() == b.GetY() && a.GetZ() == b.GetZ();
        }

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

        public Boolean IsOnCurveAffine(AffinePoint point) =>
        BigInteger.ModPow(point.GetCoordinates()[1], 2, P) 
        == (BigInteger.ModPow(point.GetCoordinates()[0], 3, P) + A * point.GetCoordinates()[0] + B) % P;


        public AffinePoint ToAffine(ProjectivePoint point)
        {
            if (CompareProjective(point, PROJECTIVE_INF)) return AFFINE_INF;
            var Z_inv = Inverse(point.GetZ(), P);
            var point1 = (point.GetX() * Z_inv) % P;
            var point2 = (point.GetY() * Z_inv) % P;
            if ( point1<0)
            {
                point1 = P + point1;
            }

            if (point2 < 0)
            {
                point2 = P + point2;
            }
            return new AffinePoint(point1, point2);
        }


        public ProjectivePoint ToProjective(AffinePoint point)
        {
            if (CompareAffine(point, AFFINE_INF)) return PROJECTIVE_INF;

            return new ProjectivePoint(point.GetX(), point.GetY(), BigInteger.One);
        }

        public BigInteger SqrtMod(BigInteger a, BigInteger p)
        {
            //4k+3
            //k = (p - 3)/4
            BigInteger k = BigInteger.Divide(p- (new BigInteger(3)), new BigInteger(4));
            var result = BigInteger.ModPow(a, BigInteger.Add(k, new BigInteger(1)), p);
            return result;
        }
        public ProjectivePoint DoublePoints( ProjectivePoint point)
        {
            if (CompareProjective(point, PROJECTIVE_INF)) return point;
           
            var W = (A * BigInteger.ModPow(point.GetZ(), 2, P) + 3 * BigInteger.ModPow(point.GetX(), 2, P))%P;
            var S = (point.GetZ() * point.GetY())%P;
            var B = (point.GetX() * point.GetY() * S)%P;
            var H = (BigInteger.ModPow(W, 2, P) - 8 * B)%P;
            var X = (2 * H * S)%P;
            var Y = (W * (4 * B - H) - 8 * BigInteger.ModPow(point.GetY(), 2, P) * BigInteger.ModPow(S, 2, P))%P;
            var Z = (8 * BigInteger.ModPow(S, 3, P))%P;

            return new ProjectivePoint(X, Y, Z);
        }


        public ProjectivePoint AddPoints(ProjectivePoint point1, ProjectivePoint point2)
        {
            if (CompareProjective(point1, PROJECTIVE_INF)) return point2;
            if (CompareProjective(point2, PROJECTIVE_INF)) return point1;

            var U1 = (point2.GetY() * point1.GetZ())%P;
            var U2 = (point1.GetY() * point2.GetZ())%P;
            var V1 = (point2.GetX() * point1.GetZ())%P;
            var V2 = (point1.GetX() * point2.GetZ())%P;
            if( V1 == V2)
            {
                if( U2 != U1)
                {
                    return PROJECTIVE_INF;
                }
                return DoublePoints(point1);
            }
            var U = (U1 - U2) %P;
            var V = (V1 - V2)%P;
            var W = (point1.GetZ() * point2.GetZ())%P;
            var A = (BigInteger.ModPow(U, 2, P) * W - BigInteger.ModPow(V, 3, P) - 2 * BigInteger.ModPow(V, 2, P) * V2 )% P;
            var X3 = (V * A)%P;
            var Y3 = (U * (BigInteger.ModPow(V, 2, P) * V2 - A) - BigInteger.ModPow(V, 3, P) * U2)%P;
            var Z3 = (BigInteger.ModPow(V, 3, P) * W)%P;

            return new ProjectivePoint(X3, Y3, Z3);
        }

        public ProjectivePoint ScalarMultiplication(ProjectivePoint point, BigInteger n)
        {
            ProjectivePoint R0 = PROJECTIVE_INF;
            ProjectivePoint R1 = point;

            while (!n.Equals(BigInteger.Zero))
            {
                if ((n & BigInteger.Zero).Equals(BigInteger.Zero))
                {
                    R1 = AddPoints(R0, R1);
                    R0 = DoublePoints(R0);
                }
                else
                {
                    R0 = AddPoints(R0, R1);
                    R1 = DoublePoints(R1);
                }
                n = n >> 1;
            }
            return R0;
        }

        public ProjectivePoint ScalarMultiplicationMontgomery(ProjectivePoint point, BigInteger k)
        {
            var bytes = k.ToByteArray();
            var bits = new BitArray(bytes);
            var r0 = new ProjectivePoint(PROJECTIVE_INF.GetX(), PROJECTIVE_INF.GetY(), PROJECTIVE_INF.GetZ());
            var r1 = new ProjectivePoint(point.GetX(), point.GetY(), point.GetZ());
            for(int i=bits.Length-1; i>=0; i--)
            {
                if (bits[i] == false)
                {
                    r1 = AddPoints(r0, r1);
                    r0 = DoublePoints(r0);
                } else
                {
                    r0 = AddPoints(r0, r1);
                    r1 = DoublePoints(r1);
                }
            }
            return r0;
        }
        public ProjectivePoint getSharedKey(ProjectivePoint point, BigInteger n)
        {
            return ScalarMultiplication(point, n);
        }
    }
}
using System.Numerics;

namespace ElipticCurves;

internal class AffinePoint
{
    private BigInteger X;
    private BigInteger Y;

    public AffinePoint(BigInteger x, BigInteger y)
    {
        this.X = x;
        this.Y = y;
    }

    public BigInteger[] GetCoordinates()
    {
        return new BigInteger[] { X, Y };
    }

    public BigInteger GetX()
    {
        return X;
    }

    public BigInteger GetY()
    {
        return Y;
    }
}
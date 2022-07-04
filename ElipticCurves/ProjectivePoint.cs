using System.Numerics;

namespace ElipticCurves;

internal class ProjectivePoint
{
    private BigInteger X;
    private BigInteger Y;
    private BigInteger Z;

    public ProjectivePoint(BigInteger x, BigInteger y, BigInteger z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    public BigInteger[] GetCoordinates()
    {
        return new BigInteger[] { X, Y, Z };
    }

    public BigInteger GetX()
    {
        return X;
    }

    public BigInteger GetY()
    {
        return Y;
    }

    public BigInteger GetZ()
    {
        return Z;
    }
}
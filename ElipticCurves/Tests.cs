using NUnit.Framework;
using System.Numerics;
using System.Security.Cryptography;

namespace ElipticCurves;
[TestFixture]
public class Tests
{
    private ElipticCurve _elipticCurve;
    private Parametrs _parametrs;
    private AES _aes;
    [SetUp]
    public void SetUP()
    {
        _parametrs = new Parametrs();
        _elipticCurve = new ElipticCurve(_parametrs.a, _parametrs.b, _parametrs.p);
        _aes = new AES();
    }

    [Test]
    public void ExistPoint()
    {
        _elipticCurve = new ElipticCurve(_parametrs.a, _parametrs.b, _parametrs.p);
        var point = _elipticCurve.createAffinePoint();
        Assert.AreEqual(true, _elipticCurve.IsOnCurveAffine(point));
    }

    [Test]
    public void AddPoint()
    {
        _elipticCurve = new ElipticCurve(new BigInteger(0), new BigInteger(0), new BigInteger(37));
        ProjectivePoint A = _elipticCurve.ToProjective(new AffinePoint(new BigInteger(6), new BigInteger(1)));
        ProjectivePoint B = _elipticCurve.ToProjective(new AffinePoint(new BigInteger(8), new BigInteger(1)));
        AffinePoint C = _elipticCurve.ToAffine(_elipticCurve.AddPoints(A, B));
        Assert.AreEqual(_elipticCurve.CompareAffine(C, new AffinePoint(new BigInteger(23), new BigInteger(36))), true);
    }

    [Test]
    public void SqrtMod()
    {
        // a = 4, p = 15 -> k = 3 -> y = 4^4 mod 15 == 1
        _elipticCurve = new ElipticCurve(new BigInteger(4), new BigInteger(1), new BigInteger(15));
        Assert.AreEqual(_elipticCurve.SqrtMod(new BigInteger(4), new BigInteger(15)), new BigInteger(1));
    }

    [Test]
    public void PointDouble()
    {

        // P(5,11) a = 2, p = 17 -> 2P(15,5)
        _elipticCurve = new ElipticCurve(new BigInteger(2), _parametrs.b, new BigInteger(17));
        ProjectivePoint A = _elipticCurve.ToProjective(new AffinePoint(new BigInteger(5), new BigInteger(11)));
        ProjectivePoint B = _elipticCurve.DoublePoints(A);
        AffinePoint C = _elipticCurve.ToAffine(B);
        Assert.AreEqual(_elipticCurve.CompareAffine(C, new AffinePoint(new BigInteger(15), new BigInteger(5))), true);
    }

    [Test]
    public void ScalarMultiplication_192()
    {
        AffinePoint point = _elipticCurve.createAffinePoint();
        ProjectivePoint point1 = _elipticCurve.ToProjective(point);
        var condition = _elipticCurve.CompareProjective(_elipticCurve.ScalarMultiplication(point1, _parametrs.n), _elipticCurve.PROJECTIVE_INF);
        Assert.AreEqual(true, condition);
    }

    [Test]
    public void ScalarMultiplicationM_192()
    {
        AffinePoint point = _elipticCurve.createAffinePoint();
        ProjectivePoint point1 = _elipticCurve.ToProjective(point);
        var condition = _elipticCurve.CompareProjective(_elipticCurve.ScalarMultiplicationMontgomery(point1, _parametrs.n), _elipticCurve.PROJECTIVE_INF);
        Assert.AreEqual(true, condition);
    }

    [Test]
    public void KeyGeneration()
    {
        AffinePoint point = _elipticCurve.createAffinePoint();
        ProjectivePoint point1 = _elipticCurve.ToProjective(point);

        BigInteger dA = _elipticCurve.GenerateBigInteger(_parametrs.p);
        BigInteger dB = _elipticCurve.GenerateBigInteger(_parametrs.p);

        ProjectivePoint QA = _elipticCurve.getSharedKey(point1, dA);
        ProjectivePoint QB = _elipticCurve.getSharedKey(point1, dB);

        ProjectivePoint SB = _elipticCurve.getSharedKey(QA, dB);
        ProjectivePoint SA = _elipticCurve.getSharedKey(QB, dA);

        AffinePoint SA_final = _elipticCurve.ToAffine(SA);
        AffinePoint SB_final = _elipticCurve.ToAffine(SB);
        var condition = _elipticCurve.CompareAffine(SA_final,SB_final);
        Assert.AreEqual(true, condition);
    }

    [Test]
    public void EncDec()
    {
        string input = "Mariupol is the best city on the planet";
        Aes myAes = Aes.Create();
        byte[] encrypted = _aes.EncryptStringToBytes_Aes(input, myAes.Key, myAes.IV);
        string roundtrip = _aes.DecryptStringFromBytes_Aes(encrypted, myAes.Key, myAes.IV);
        Assert.AreEqual(input, roundtrip);
    }

    [Test]
    public void Order()
    {
        _elipticCurve = new ElipticCurve(_parametrs.a, _parametrs.b, _parametrs.p);
        AffinePoint point = _elipticCurve.createAffinePoint();
        ProjectivePoint point1 = _elipticCurve.ToProjective(point);
        while (_elipticCurve.CompareProjective(point1, _elipticCurve.PROJECTIVE_INF))
        {
            point = _elipticCurve.createAffinePoint();
            point1 = _elipticCurve.ToProjective(point);

        }
        var result = _elipticCurve.ScalarMultiplication(point1, _parametrs.n);
        Assert.AreEqual(true, _elipticCurve.CompareProjective(result, _elipticCurve.PROJECTIVE_INF));
    }

    [Test]
    public void VerifySignature()
    {
        AffinePoint point = _elipticCurve.createAffinePoint();
        ProjectivePoint point1 = _elipticCurve.ToProjective(point);
        var secretKey = _elipticCurve.GenerateBigInteger(_parametrs.n);
        var publicKey = _elipticCurve.ScalarMultiplication(point1, secretKey);
        string input = "Mariupol is the best city on the planet";
        var helper = new Helpers();
        var signature = helper.Signature(input, point1, secretKey, _elipticCurve, _parametrs.n);
        var verify = helper.Verify(input, signature, publicKey, _elipticCurve, _parametrs.n, point1);
        Assert.AreEqual(true, verify);
    }
}
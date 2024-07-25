using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Security.Cryptography;

namespace KeyParse
{

    [TestFixture]
    public class KeyRingGeneratorTests
    {
        [Test]
        public void TestGenerateAESKey()
        {
            var keyData = KeyRingGenerator.GenerateAESKey(1, 256);

            Assert.AreEqual(3, keyData.keyType);
            Assert.AreEqual(1, keyData.keyFormat);
            Assert.AreEqual(256 / 8, keyData.keyLength);
            Assert.AreEqual(32, keyData.keyIntegrityHash.Length);
            Assert.IsNotNull(keyData.key);
        }

        [Test]
        public void TestGenerateECKey()
        {
            var keyData = KeyRingGenerator.GenerateECKey(1, ECCurve.NamedCurves.nistP384);

            Assert.AreEqual(1, keyData.keyType);
            Assert.AreEqual(1, keyData.keyFormat);
            Assert.AreEqual(96, keyData.keyLength); // P-384 Qx + Qy
            Assert.AreEqual(32, keyData.keyIntegrityHash.Length);
            Assert.IsNotNull(keyData.key);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyParse
{

    public class KeyRingGenerator
    {
        public static void GenerateTestFile(string filePath)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
            {
                // Key Valid (4 bytes)
                writer.Write((uint)1); // VALID

                // Key Type (4 bytes)
                writer.Write((uint)3); // AES

                // Key Format (4 bytes)
                writer.Write((uint)1); // RAW

                // Key Name (32 bytes)
                string keyName = "TestKey";
                byte[] keyNameBytes = new byte[32];
                Encoding.UTF8.GetBytes(keyName, 0, keyName.Length, keyNameBytes, 0);
                writer.Write(keyNameBytes);

                // Key EC Curve (4 bytes)
                writer.Write((uint)1); // P-384

                // Key AES Cipher Type (4 bytes)
                writer.Write((uint)3); // AES256

                // Key AES Cipher Mode (4 bytes)
                writer.Write((uint)5); // CTR

                // Key Integrity Hash Algorithm (4 bytes)
                writer.Write((uint)3); // SHA256

                // Key Integrity Hash (64 bytes)
                byte[] keyIntegrityHash = new byte[64];
                new Random().NextBytes(keyIntegrityHash);
                writer.Write(keyIntegrityHash);

                // Key Length (4 bytes)
                writer.Write((uint)32); // 32 bytes

                // Key (144 bytes)
                byte[] key = new byte[144];
                new Random().NextBytes(key);
                writer.Write(key);

                // Key Reserved (240 bytes)
                byte[] keyReserved = new byte[240];
                new Random().NextBytes(keyReserved);
                writer.Write(keyReserved);

                // End (remaining bytes to make total 512 bytes)
                long currentPosition = writer.BaseStream.Position;
                long remainingBytes = 512 - currentPosition;
                writer.Write(new byte[remainingBytes]);
            }
        }
    }
}

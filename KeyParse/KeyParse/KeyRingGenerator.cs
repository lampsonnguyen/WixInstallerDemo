using System.Collections;
using System.Text;


namespace KeyParse
{
    public class KeyRingGenerator
    {
        public static void GenerateTestFile(string filePath, byte[] key, uint keyValid, uint keyType, uint keyFormat, string keyName, uint keyECCurve, uint keyAESCipherType, uint keyAESCipherMode, uint keyIntegrityHashAlgorithm, uint keyLength)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
            {
                // Prepare a MemoryStream to hold the binary data
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (BinaryWriter memoryWriter = new BinaryWriter(memoryStream))
                    {
                        // Key Valid (4 bytes)
                        memoryWriter.Write(keyValid);

                        // Key Type (4 bytes)
                        memoryWriter.Write(keyType);

                        // Key Format (4 bytes)
                        memoryWriter.Write(keyFormat);

                        // Key Name (32 bytes)
                        byte[] keyNameBytes = new byte[32];
                        Encoding.UTF8.GetBytes(keyName, 0, keyName.Length, keyNameBytes, 0);
                        memoryWriter.Write(keyNameBytes);

                        // Key EC Curve (4 bytes)
                        memoryWriter.Write(keyECCurve);

                        // Key AES Cipher Type (4 bytes)
                        memoryWriter.Write(keyAESCipherType);

                        // Key AES Cipher Mode (4 bytes)
                        memoryWriter.Write(keyAESCipherMode);

                        // Key Integrity Hash Algorithm (4 bytes)
                        memoryWriter.Write(keyIntegrityHashAlgorithm);

                        // Key Integrity Hash (64 bytes)
                        byte[] keyIntegrityHash = new byte[64];
                        new Random().NextBytes(keyIntegrityHash);
                        memoryWriter.Write(keyIntegrityHash);

                        // Key Length (4 bytes)
                        memoryWriter.Write(keyLength);

                        // Key (144 bytes)
                        byte[] keyValue = new byte[144];
                        Buffer.BlockCopy(key, 0, keyValue, 0, key.Length);
                        
                        memoryWriter.Write(keyValue);

                        // Key Reserved (240 bytes)
                        byte[] keyReserved = new byte[240];
                        new Random().NextBytes(keyReserved);
                        memoryWriter.Write(keyReserved);

                        // End (remaining bytes to make total 512 bytes)
                        long currentPosition = memoryStream.Position;
                        long remainingBytes = 512 - currentPosition;
                        memoryWriter.Write(new byte[remainingBytes]);

                        // Print out the binary data
                        byte[] binaryData = memoryStream.ToArray();
                        Console.WriteLine("Binary Data for " + keyName + ":");
                        Console.WriteLine(BitConverter.ToString(binaryData));

                        // Write the binary data to the file
                        writer.Write(binaryData);
                    }
                }
            }
        }
    }

}

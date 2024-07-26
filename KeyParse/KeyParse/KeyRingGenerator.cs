using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class KeyRingGenerator
{
    public static void GenerateTestFile(string filePath, uint numberOfKeys, Func<uint, (uint keyType, uint keyFormat, string keyName, uint keyECCurve, uint keyAESCipherType, uint keyAESCipherMode, uint keyIntegrityHashAlgorithm, uint keyLength, byte[] key, byte[] keyIntegrityHash)>[] keyGenerators)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
        {
            // Prepare a MemoryStream to hold the binary data
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter memoryWriter = new BinaryWriter(memoryStream))
                {
                    // Write Header
                    WriteHeader(memoryWriter, numberOfKeys);

                    // Write Keys
                    for (uint i = 0; i < numberOfKeys; i++)
                    {
                        var keyData = keyGenerators[i](i);
                        WriteKey(memoryWriter, 1, keyData.keyType, keyData.keyFormat, keyData.keyName, keyData.keyECCurve, keyData.keyAESCipherType, keyData.keyAESCipherMode, keyData.keyIntegrityHashAlgorithm, keyData.keyLength, keyData.key, keyData.keyIntegrityHash);
                    }

                    // Write Footer
                    WriteFooter(memoryWriter);

                    // Print out the binary data
                    byte[] binaryData = memoryStream.ToArray();
                    Console.WriteLine("Binary Data:");
                    Console.WriteLine(BitConverter.ToString(binaryData));

                    // Write the binary data to the file
                    writer.Write(binaryData);
                }
            }
        }
    }

    private static void WriteHeader(BinaryWriter writer, uint numberOfKeys)
    {
        // Header Flag (4 bytes) 
        int flag = ConvertStringToHex("KHDR");
        writer.Write(flag); 

        // Header Version (4 bytes)
        writer.Write(2); // Version 2

        // Reserved (4 bytes)
        writer.Write(0); // Reserved

        // Number of Keys (4 bytes)
        writer.Write(numberOfKeys);

        // Reserved (48 bytes)
        writer.Write(new byte[48]);
    }
    private static int ConvertStringToHex(string asciiString)
    {
        int hexValue = 0;
        foreach (char c in asciiString)
        {
            hexValue = (hexValue << 8) + c;
        }
        return hexValue;
    }

    private static void WriteKey(BinaryWriter writer, uint keyValid, uint keyType, uint keyFormat, string keyName, uint keyECCurve, uint keyAESCipherType, uint keyAESCipherMode, uint keyIntegrityHashAlgorithm, uint keyLength, byte[] key, byte[] keyIntegrityHash)
    {
        // Key Valid (4 bytes)
        writer.Write(keyValid);

        // Key Type (4 bytes)
        writer.Write(keyType);

        // Key Format (4 bytes)
        writer.Write(keyFormat);

        // Key Name (32 bytes)
        byte[] keyNameBytes = new byte[32];
        Encoding.UTF8.GetBytes(keyName, 0, keyName.Length, keyNameBytes, 0);
        writer.Write(keyNameBytes);

        // Key EC Curve (4 bytes)
        writer.Write(keyECCurve);

        // Key AES Cipher Type (4 bytes)
        writer.Write(keyAESCipherType);

        // Key AES Cipher Mode (4 bytes)
        writer.Write(keyAESCipherMode);

        // Key Integrity Hash Algorithm (4 bytes)
        writer.Write(keyIntegrityHashAlgorithm);

        // Key Integrity Hash (64 bytes)
        writer.Write(keyIntegrityHash);

        // Key Length (4 bytes)
        writer.Write(keyLength);

        // Key (144 bytes)
        writer.Write(key);

        // Key Reserved (240 bytes)
        writer.Write(new byte[240]);
    }

    private static void WriteFooter(BinaryWriter writer)
    {
        // Reserved for future use (32 bytes)
        writer.Write(new byte[32]);
    }

    public static (uint keyType, uint keyFormat, string keyName, uint keyECCurve, uint keyAESCipherType, uint keyAESCipherMode, uint keyIntegrityHashAlgorithm, uint keyLength, byte[] key, byte[] keyIntegrityHash) GenerateAESKey(uint keyIndex, uint keyLength)
    {
        using (var aes = Aes.Create())
        {
            aes.KeySize = (int)keyLength;
            aes.GenerateKey();
            aes.GenerateIV();
            byte[] key = new byte[144];
            Array.Copy(aes.Key, key, aes.Key.Length);

            byte[] keyIntegrityHash = new byte[64];
            Array.Copy(SHA256.HashData(aes.Key), keyIntegrityHash, aes.Key.Length);

            return (3, 1, $"AESKey_{keyIndex}", 0, 3, 5, 3, keyLength / 8, key, keyIntegrityHash);
        }
    }

    public static (uint keyType, uint keyFormat, string keyName, uint keyECCurve, uint keyAESCipherType, uint keyAESCipherMode, uint keyIntegrityHashAlgorithm, uint keyLength, byte[] key, byte[] keyIntegrityHash) GenerateECKey(uint keyIndex, ECCurve curve)
    {
        using (var ecdsa = ECDsa.Create(curve))
        {
            byte[] key = new byte[144];
            var parameters = ecdsa.ExportParameters(false);

            Array.Copy(parameters.Q.X, key, parameters.Q.X.Length);
            Array.Copy(parameters.Q.Y, 0, key, parameters.Q.X.Length, parameters.Q.Y.Length);

            byte[] keyIntegrityHash = new byte[64];
            var hash = SHA256.HashData(key);
            Array.Copy(hash, keyIntegrityHash, hash.Length);

            uint keyLength = (uint)(parameters.Q.X.Length + parameters.Q.Y.Length);
            uint curveType = curve.Oid.Value switch
            {
                "1.3.132.0.34" => 1, // P-384
                "1.3.132.0.35" => 2, // P-521
                "1.2.840.10045.3.1.7" => 3, // P-256
                _ => 0
            };

            return (1, 1, $"ECKey_{keyIndex}", curveType, 0, 0, 3, keyLength, key, keyIntegrityHash);
        }
    }
}

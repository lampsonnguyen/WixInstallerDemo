using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class KeyRingGenerator
{
    public static void GenerateTestFile(string filePath, uint numberOfKeys, Func<uint, KeyData>[] keyGenerators)
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
                        WriteKey(memoryWriter, keyData);
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

    private static void WriteKey(BinaryWriter writer, KeyData keyEntry)
    {
        // Key Valid (4 bytes)
        writer.Write(keyEntry.KeyValid);

        // Key Type (4 bytes)
        writer.Write(keyEntry.KeyType);

        // Key Format (4 bytes)
        writer.Write(keyEntry.KeyFormat);

        // Key Name (32 bytes)
        byte[] keyNameBytes = new byte[32];
        Encoding.UTF8.GetBytes(keyEntry.KeyName, 0, keyEntry.KeyName.Length, keyNameBytes, 0);
        writer.Write(keyNameBytes);

        // Key EC Curve (4 bytes)
        writer.Write(keyEntry.KeyECCurve);

        // Key AES Cipher Type (4 bytes)
        writer.Write(keyEntry.KeyAESCipherType);

        // Key AES Cipher Mode (4 bytes)
        writer.Write(keyEntry.KeyAESCipherMode);

        // Key Integrity Hash Algorithm (4 bytes)
        writer.Write(keyEntry.KeyIntegrityHashAlgorithm);

        // Key Integrity Hash (64 bytes)
        writer.Write(keyEntry.KeyIntegrityHash);

        // Key Length (4 bytes)
        writer.Write(keyEntry.KeyLength);

        // Key (144 bytes)
        writer.Write(keyEntry.Key);

        // Key Reserved (240 bytes)
        writer.Write(new byte[240]);
    }

    private static void WriteFooter(BinaryWriter writer)
    {
        // Reserved for future use (32 bytes)
        writer.Write(new byte[32]);
    }

    public static KeyData GenerateAESKey(uint keyIndex, uint keyLength)
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

            KeyData keyData = new KeyData()
            {
                KeyType = 3,
                KeyFormat = 1,
                KeyName = $"AESKey_{keyIndex}",
                KeyECCurve = 0,
                KeyAESCipherType = 3,
                KeyAESCipherMode = 5,
                KeyIntegrityHashAlgorithm = 3,
                KeyLength = keyLength / 8,
                Key = key,
                KeyIntegrityHash = keyIntegrityHash
            };
             
            return keyData;
        }
    }

    public static KeyData GenerateECKey(uint keyIndex, ECCurve curve)
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
            KeyData keyData = new KeyData()
            {
                KeyType =1,
                KeyFormat = 1,
                KeyName = $"ECKey_{keyIndex}",
                KeyECCurve = curveType,
                KeyAESCipherType = 0,
                KeyAESCipherMode = 0,
                KeyIntegrityHashAlgorithm = 3,
                KeyLength = keyLength ,
                Key = key,
                KeyIntegrityHash = keyIntegrityHash
            };
            return keyData;
        }
    }
}

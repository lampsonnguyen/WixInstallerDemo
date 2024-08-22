using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class KeyRingGenerator
{
    private readonly string _filePath;
    private readonly uint _numberOfKeys;
    private readonly KeyData[] _keys;
    private readonly int _version;
    public KeyRingGenerator(string filePath, uint numberOfKeys, KeyData[] keys)
    {
        _filePath = filePath;
        _numberOfKeys = numberOfKeys;
        _keys = keys;
    }
    private KeyRingGenerator(Builder builder)
    {
        _filePath = builder.FilePath;
        _numberOfKeys = builder.NumberOfKeys;
        _keys = builder.Keys;
        _version = builder.Version;
    }
    public void Generate()
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(memoryStream))
            {
                new Header(_numberOfKeys).WriteTo(writer);

                foreach (var keyData in _keys)
                {
                    new KeySection(keyData).WriteTo(writer);
                }

                new Footer().WriteTo(writer);

                // Save the binary data to the file
                File.WriteAllBytes(_filePath, memoryStream.ToArray());

                // Optionally, print the binary data
                Console.WriteLine("Binary Data:");
                Console.WriteLine(BitConverter.ToString(memoryStream.ToArray()));
            }
        }
    }


    public class Builder
    {
        public string FilePath { get; private set; }
        public uint NumberOfKeys { get; private set; }
        public KeyData[] Keys { get; private set; }
        public int Version { get; private set; } = 2; // Default value

        public Builder(string filePath, uint numberOfKeys, KeyData[] keys)
        {
            FilePath = filePath;
            NumberOfKeys = numberOfKeys;
            Keys = keys;
        }

        public Builder SetVersion(int version)
        {
            Version = version;
            return this;
        }

        public KeyRingGenerator Build()
        {
            return new KeyRingGenerator(this);
        }
    }

}

public class Header
{
    private readonly uint _numberOfKeys;

    public Header(uint numberOfKeys)
    {
        _numberOfKeys = numberOfKeys;
    }

    public void WriteTo(BinaryWriter writer)
    {
        int flag = ConvertStringToHex("KHDR");
        writer.Write(flag);
        writer.Write(2); // Version
        writer.Write(0); // Reserved
        writer.Write(_numberOfKeys);
        writer.Write(new byte[48]); // Reserved
    }

    private int ConvertStringToHex(string asciiString)
    {
        int hexValue = 0;
        foreach (char c in asciiString)
        {
            hexValue = (hexValue << 8) + c;
        }
        return hexValue;
    }
}

public class KeySection
{
    private readonly KeyData _keyData;

    public KeySection(KeyData keyData)
    {
        _keyData = keyData;
    }

    public void WriteTo(BinaryWriter writer)
    {
        writer.Write(_keyData.KeyValid);
        writer.Write(_keyData.KeyType);
        writer.Write(_keyData.KeyFormat);

        byte[] keyNameBytes = new byte[32];
        Encoding.UTF8.GetBytes(_keyData.KeyName, 0, _keyData.KeyName.Length, keyNameBytes, 0);
        writer.Write(keyNameBytes);

        writer.Write(_keyData.KeyECCurve);
        writer.Write(_keyData.KeyAESCipherType);
        writer.Write(_keyData.KeyAESCipherMode);
        writer.Write(_keyData.KeyIntegrityHashAlgorithm);
        writer.Write(_keyData.KeyIntegrityHash);
        writer.Write(_keyData.KeyLength);
        writer.Write(_keyData.Key);
        writer.Write(new byte[240]); // Reserved
    }
}

public class Footer
{
    public void WriteTo(BinaryWriter writer)
    {
        writer.Write(new byte[32]); // Reserved
    }
}

public class KeyData
{
    public uint KeyValid { get; set; }
    public uint KeyType { get; set; }
    public uint KeyFormat { get; set; }
    public string KeyName { get; set; }
    public uint KeyECCurve { get; set; }
    public uint KeyAESCipherType { get; set; }
    public uint KeyAESCipherMode { get; set; }
    public uint KeyIntegrityHashAlgorithm { get; set; }
    public byte[] KeyIntegrityHash { get; set; }
    public uint KeyLength { get; set; }
    public byte[] Key { get; set; }
}

public static class KeyGenerators
{
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

            return new KeyData
            {
                KeyValid = 1,
                KeyType = 3,
                KeyFormat = 1,
                KeyName = $"AESKey_{keyIndex}",
                KeyECCurve = 0,
                KeyAESCipherType = 3,
                KeyAESCipherMode = 5,
                KeyIntegrityHashAlgorithm = 3,
                KeyIntegrityHash = keyIntegrityHash,
                KeyLength = keyLength / 8,
                Key = key
            };
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

            return new KeyData
            {
                KeyValid = 1,
                KeyType = 1,
                KeyFormat = 1,
                KeyName = $"ECKey_{keyIndex}",
                KeyECCurve = curveType,
                KeyAESCipherType = 0,
                KeyAESCipherMode = 0,
                KeyIntegrityHashAlgorithm = 3,
                KeyIntegrityHash = keyIntegrityHash,
                KeyLength = keyLength,
                Key = key
            };
        }
    }
}

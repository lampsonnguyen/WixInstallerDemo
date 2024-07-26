using System.Text;

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
    public byte[] KeyReserved { get; set; }

    public static KeyData Parse(byte[] data)
    {
        using (MemoryStream stream = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                KeyData keyData = new KeyData
                {
                    KeyValid = reader.ReadUInt32(),
                    KeyType = reader.ReadUInt32(),
                    KeyFormat = reader.ReadUInt32(),
                    KeyName = Encoding.UTF8.GetString(reader.ReadBytes(32)).TrimEnd('\0'),
                    KeyECCurve = reader.ReadUInt32(),
                    KeyAESCipherType = reader.ReadUInt32(),
                    KeyAESCipherMode = reader.ReadUInt32(),
                    KeyIntegrityHashAlgorithm = reader.ReadUInt32(),
                    KeyIntegrityHash = reader.ReadBytes(64),
                    KeyLength = reader.ReadUInt32(),
                    Key = reader.ReadBytes(144),
                    KeyReserved = reader.ReadBytes(240)
                };

                return keyData;
            }
        }
    }

    public string DecodeKeyValid() => KeyValid switch
    {
        0 => "NULL",
        1 => "VALID",
        _ => "UNKNOWN"
    };

    public string DecodeKeyType() => KeyType switch
    {
        0 => "NULL",
        1 => "EC Public",
        2 => "EC Private",
        3 => "AES",
        _ => "UNKNOWN"
    };

    public string DecodeKeyFormat() => KeyFormat switch
    {
        0 => "NULL",
        1 => "RAW",
        _ => "UNKNOWN"
    };

    public string DecodeKeyECCurve() => KeyECCurve switch
    {
        0 => "NULL",
        1 => "P-384",
        2 => "P-521",
        3 => "P-256",
        _ => "UNKNOWN"
    };

    public string DecodeKeyAESCipherType() => KeyAESCipherType switch
    {
        0 => "None",
        1 => "AES128",
        2 => "AES192",
        3 => "AES256",
        _ => "UNKNOWN"
    };

    public string DecodeKeyAESCipherMode() => KeyAESCipherMode switch
    {
        0 => "None",
        5 => "CTR",
        6 => "GCM",
        _ => "UNKNOWN"
    };

    public string DecodeKeyIntegrityHashAlgorithm() => KeyIntegrityHashAlgorithm switch
    {
        0 => "None",
        3 => "SHA256",
        4 => "SHA384",
        5 => "SHA512",
        101 => "SHA3_512",
        _ => "UNKNOWN"
    };
}

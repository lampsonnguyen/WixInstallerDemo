using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyParse
{
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

        public string DecodeKeyValid() => KeyValid == 1 ? "VALID" : "NULL";
        public string DecodeKeyType() => KeyType switch
        {
            1 => "EC Public",
            2 => "EC Private",
            3 => "AES",
            _ => "NULL"
        };
        public string DecodeKeyFormat() => KeyFormat == 1 ? "RAW" : "NULL";
        public string DecodeKeyECCurve() => KeyECCurve switch
        {
            1 => "P-384",
            2 => "P-521",
            3 => "P-256",
            _ => "NULL"
        };
        public string DecodeKeyAESCipherType() => KeyAESCipherType switch
        {
            1 => "AES128",
            2 => "AES192",
            3 => "AES256",
            _ => "None"
        };
        public string DecodeKeyAESCipherMode() => KeyAESCipherMode switch
        {
            5 => "CTR",
            6 => "GCM",
            _ => "None"
        };
        public string DecodeKeyIntegrityHashAlgorithm() => KeyIntegrityHashAlgorithm switch
        {
            3 => "SHA256",
            4 => "SHA384",
            5 => "SHA512",
            101 => "SHA3_512",
            _ => "None"
        };
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyParse
{
    public class BinaryParser
    {
        public static KeyData Parse(byte[] data)
        {
            if (data.Length != 512)
            {
                throw new ArgumentException("Invalid data length");
            }

            KeyData keyData = new()
            {
                KeyValid = BitConverter.ToUInt32(data, 0),
                KeyType = BitConverter.ToUInt32(data, 4),
                KeyFormat = BitConverter.ToUInt32(data, 8),
                KeyName = Encoding.UTF8.GetString(data, 12, 32).TrimEnd('\0'),
                KeyECCurve = BitConverter.ToUInt32(data, 44),
                KeyAESCipherType = BitConverter.ToUInt32(data, 48),
                KeyAESCipherMode = BitConverter.ToUInt32(data, 52),
                KeyIntegrityHashAlgorithm = BitConverter.ToUInt32(data, 56),
                KeyIntegrityHash = data[60..124],
                KeyLength = BitConverter.ToUInt32(data, 124),
                Key = data[128..272],
                KeyReserved = data[272..512]
            };

            return keyData;
        }


        public static HeaderData KeyRingHeaderParser(byte[] data)
        {
            if (data.Length != 64)
            {
                throw new ArgumentException("Invalid data length");
            }

            HeaderData headerData = new()
            {
                HeaderFlag = data[0..4],
                HeaderVersion = BitConverter.ToInt32(data, 4),
                NumberOfKeys = BitConverter.ToInt32(data, 12),
                Reserved = data[272..512]
            };
            return headerData;
        }
    }
}
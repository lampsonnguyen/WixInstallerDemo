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

            KeyData keyData = new KeyData();

            keyData.KeyValid = BitConverter.ToUInt32(data, 0);
            keyData.KeyType = BitConverter.ToUInt32(data, 4);
            keyData.KeyFormat = BitConverter.ToUInt32(data, 8);
            keyData.KeyName = Encoding.UTF8.GetString(data, 12, 32).TrimEnd('\0');
            keyData.KeyECCurve = BitConverter.ToUInt32(data, 44);
            keyData.KeyAESCipherType = BitConverter.ToUInt32(data, 48);
            keyData.KeyAESCipherMode = BitConverter.ToUInt32(data, 52);
            keyData.KeyIntegrityHashAlgorithm = BitConverter.ToUInt32(data, 56);
            keyData.KeyIntegrityHash = data[60..124];
            keyData.KeyLength = BitConverter.ToUInt32(data, 124);
            keyData.Key = data[128..272];
            keyData.KeyReserved = data[272..512];

            return keyData;
        }
    }
}

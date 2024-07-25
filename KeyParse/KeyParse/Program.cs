using KeyParse;
namespace KeyParse;
class Program
{
    static void Main()
    {
        string path = @"C:\Users\lamps\OneDrive\Documents\GitHub\WixInstallerDemo\KeyParse\KeyParse\";
        // Example for AES key with CTR mode
        GenerateAndPrint(path + "key_ring_aes_ctr.bin", 1, 3, 1, "AES_CTR_Key", 0, 3, 5, 3, 32);

        // Example for EC P-384 key
        GenerateAndPrint(path + "key_ring_ec_p384.bin", 1, 1, 1, "EC_P384_Key", 1, 0, 0, 3, 96);

        // Example for EC P-521 key
        GenerateAndPrint(path + "key_ring_ec_p521.bin", 1, 1, 1, "EC_P521_Key", 2, 0, 0, 4, 132);

        // Example for EC P-256 key
        GenerateAndPrint(path + "key_ring_ec_p256.bin", 1, 1, 1, "EC_P256_Key", 3, 0, 0, 5, 64);

        // Read and parse the files to demonstrate decoding
        string[] filePaths = { "key_ring_aes_ctr.bin", "key_ring_ec_p384.bin", "key_ring_ec_p521.bin", "key_ring_ec_p256.bin" };

        DisplayResult(path, filePaths);
    }

    private static void DisplayResult(string path, string[] filePaths)
    {
        foreach (var filePath in filePaths)
        {
            byte[] binaryData = File.ReadAllBytes(path + filePath);
            KeyData parsedData = BinaryParser.Parse(binaryData);
            Console.WriteLine("------------------------------");

            Console.WriteLine($"Parsed Data from {filePath}:");
            Console.WriteLine("Key Valid: " + parsedData.DecodeKeyValid());
            Console.WriteLine("Key Type: " + parsedData.DecodeKeyType());
            Console.WriteLine("Key Format: " + parsedData.DecodeKeyFormat());
            Console.WriteLine("Key Name: " + parsedData.KeyName);
            Console.WriteLine("Key EC Curve: " + parsedData.DecodeKeyECCurve());
            Console.WriteLine("Key AES Cipher Type: " + parsedData.DecodeKeyAESCipherType());
            Console.WriteLine("Key AES Cipher Mode: " + parsedData.DecodeKeyAESCipherMode());
            Console.WriteLine("Key Integrity Hash Algorithm: " + parsedData.DecodeKeyIntegrityHashAlgorithm());
            Console.WriteLine("Key Integrity Hash: " + BitConverter.ToString(parsedData.KeyIntegrityHash));
            Console.WriteLine("Key Length: " + parsedData.KeyLength);
            Console.WriteLine("Key: " + BitConverter.ToString(parsedData.Key));
            Console.WriteLine("Key Reserved: " + BitConverter.ToString(parsedData.KeyReserved));
            Console.WriteLine("------------------------------");
        }
    }

    static void GenerateAndPrint(string filePath, uint keyValid, uint keyType, uint keyFormat, string keyName, uint keyECCurve, uint keyAESCipherType, uint keyAESCipherMode, uint keyIntegrityHashAlgorithm, uint keyLength)
    {
        Console.WriteLine("------------------------------");
        KeyRingGenerator.GenerateTestFile(filePath, keyValid, keyType, keyFormat, keyName, keyECCurve, keyAESCipherType, keyAESCipherMode, keyIntegrityHashAlgorithm, keyLength);
        Console.WriteLine($"Test binary file created at: {filePath}");
    }
}
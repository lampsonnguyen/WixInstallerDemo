using KeyParse;
using System.Security.Cryptography;
namespace KeyParse;
class Program
{
    static void Main()
    {
        Aes aes = Aes.Create();
        
        string path = @"C:\Users\lamps\OneDrive\Documents\GitHub\WixInstallerDemo\KeyParse\";
        // Example for AES key with CTR mode 
        GenerateAndPrintAESKeys(path + "key_ring_aes_ctr.bin", 1, 256); // AES 256-bit key

        // Example for EC P-384 key
        GenerateAndPrintECKeys(path + "key_ring_ec_p384.bin", 1, ECCurve.NamedCurves.nistP384);

        // Example for EC P-521 key
        GenerateAndPrintECKeys(path + "key_ring_ec_p521.bin", 1, ECCurve.NamedCurves.nistP521);

        // Example for EC P-256 key
        GenerateAndPrintECKeys(path + "key_ring_ec_p256.bin", 1, ECCurve.NamedCurves.nistP256);




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


    static void GenerateAndPrintAESKeys(string filePath, uint numberOfKeys, uint keyLength)
    {
        KeyRingGenerator.GenerateTestFile(filePath, numberOfKeys, i => KeyRingGenerator.GenerateAESKey(i, keyLength));
        Console.WriteLine($"Test binary file created at: {filePath}");
    }

    static void GenerateAndPrintECKeys(string filePath, uint numberOfKeys, ECCurve curve)
    {
        KeyRingGenerator.GenerateTestFile(filePath, numberOfKeys, i => KeyRingGenerator.GenerateECKey(i, curve));
        Console.WriteLine($"Test binary file created at: {filePath}");
    } 
}
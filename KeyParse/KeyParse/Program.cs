using KeyParse;
namespace KeyParse;
class Program
{
    static void Main()
    {
        string filePath = @"C:\Users\lamps\OneDrive\Documents\GitHub\WixInstallerDemo\KeyParse\KeyParse\key_ring_test.bin";
        KeyRingGenerator.GenerateTestFile(filePath);
        Console.WriteLine("Test binary file created at: " + filePath);

        byte[] binaryData = File.ReadAllBytes(filePath);
        KeyData parsedData = BinaryParser.Parse(binaryData);

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
    }
}
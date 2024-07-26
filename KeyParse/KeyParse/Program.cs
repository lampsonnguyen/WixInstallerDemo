﻿using KeyParse;
using System;
using System.IO;
using System.Security.Cryptography;

class Program
{
    static void Main(string[] args)
    {
        // Example of generating a single file with all four types of keys
        GenerateAndPrintAllKeys("key_ring_all_keys.bin");

        // Read and parse the file to demonstrate decoding
        string filePath = "key_ring_all_keys.bin";

        byte[] binaryData = File.ReadAllBytes(filePath);
        var header = HeaderParser.Parse(binaryData);
        Console.WriteLine($"Parsed Header from {filePath}:");
        Console.WriteLine("Header Flag: " + header.HeaderFlag);
        Console.WriteLine("Header Version: " + header.HeaderVersion);
        Console.WriteLine("Number of Keys: " + header.NumberOfKeys);
        Console.WriteLine();

        for (uint i = 0; i < header.NumberOfKeys; i++)
        {
            byte[] keyData = new byte[512];
            Array.Copy(binaryData, 64 + i * 512, keyData, 0, 512);
            KeyData parsedData = KeyData.Parse(keyData);

            Console.WriteLine($"Parsed Data for Key {i + 1} from {filePath}:");
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
            Console.WriteLine();
        }
    }

    static void GenerateAndPrintAllKeys(string filePath)
    {
        var keyGenerators = new Func<uint, (uint keyType, uint keyFormat, string keyName, uint keyECCurve, uint keyAESCipherType, uint keyAESCipherMode, uint keyIntegrityHashAlgorithm, uint keyLength, byte[] key, byte[] keyIntegrityHash)>[]
        {
            i => KeyRingGenerator.GenerateAESKey(i, 256), // AES 256-bit key
            i => KeyRingGenerator.GenerateECKey(i, ECCurve.NamedCurves.nistP384),
            i => KeyRingGenerator.GenerateECKey(i, ECCurve.NamedCurves.nistP521),
            i => KeyRingGenerator.GenerateECKey(i, ECCurve.NamedCurves.nistP256)
        };

        KeyRingGenerator.GenerateTestFile(filePath, (uint)keyGenerators.Length, keyGenerators);
        Console.WriteLine($"Test binary file created at: {filePath}");
    }
}

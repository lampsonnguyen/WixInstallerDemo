using System;
using System.IO;
using System.Text;

public class BinaryFileGenerator
{
    public static void Main(string[] args)
    {
        string filePath = "path/to/your/testdatafile.bin";
        GenerateTestDataFile(filePath);
    }

    public static void GenerateTestDataFile(string filePath)
    {
        using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        using (BinaryWriter writer = new BinaryWriter(fs))
        {
            // Write header
            string header = "ExampleHeader123456";
            byte[] headerBytes = Encoding.UTF8.GetBytes(header);
            writer.Write(headerBytes);

            // Write six keys
            for (int i = 0; i < 6; i++)
            {
                byte[] keyBytes = new byte[256];
                for (int j = 0; j < keyBytes.Length; j++)
                {
                    keyBytes[j] = (byte)(i + 1); // Fill each key with a repeating pattern
                }
                writer.Write(keyBytes);
            }

            // Write footer
            string footer = "ExampleFooter";
            byte[] footerBytes = Encoding.UTF8.GetBytes(footer);
            writer.Write(footerBytes);
        }

        Console.WriteLine($"Test data file '{filePath}' generated successfully.");
    }
}

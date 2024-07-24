using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class BinaryFileParserExample
{
    public static void Main(string[] args)
    {
        string filePath = "path/to/your/binaryfile.bin";

        // Define the structure of the binary file
        List<Section> sections = new List<Section>
        {
            new Section("Header", 20, SectionType.String),
            new Section("Key1", 256, SectionType.ByteArray),
            new Section("Key2", 256, SectionType.ByteArray),
            new Section("Key3", 256, SectionType.ByteArray),
            new Section("Key4", 256, SectionType.ByteArray),
            new Section("Key5", 256, SectionType.ByteArray),
            new Section("Key6", 256, SectionType.ByteArray),
            new Section("Footer", -1, SectionType.String) // -1 indicates read till end of file
        };

        // Parse the binary file
        Dictionary<string, object> parsedData = BinaryFileParser.ParseBinaryFile(filePath, sections);
        
        // Process the parsed data
        ProcessParsedData(parsedData);
    }

    public static void ProcessParsedData(Dictionary<string, object> parsedData)
    {
        // Print the header
        Console.WriteLine($"Header: {parsedData["Header"]}");

        // Print the keys
        for (int i = 1; i <= 6; i++)
        {
            byte[] key = (byte[])parsedData[$"Key{i}"];
            Console.WriteLine($"Key{i}: {BitConverter.ToString(key)}");
        }

        // Print the footer
        Console.WriteLine($"Footer: {parsedData["Footer"]}");
    }
}

public class BinaryFileParser
{
    public static Dictionary<string, object> ParseBinaryFile(string filePath, List<Section> sections)
    {
        Dictionary<string, object> parsedData = new Dictionary<string, object>();

        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        using (BinaryReader reader = new BinaryReader(fs))
        {
            foreach (var section in sections)
            {
                object data = null;
                if (section.Length == -1)
                {
                    data = reader.ReadBytes((int)(fs.Length - fs.Position));
                }
                else
                {
                    data = ReadSection(reader, section.Length, section.Type);
                }
                
                parsedData.Add(section.Name, data);
            }
        }

        return parsedData;
    }

    private static object ReadSection(BinaryReader reader, int length, SectionType type)
    {
        switch (type)
        {
            case SectionType.String:
                return Encoding.UTF8.GetString(reader.ReadBytes(length));
            case SectionType.ByteArray:
                return reader.ReadBytes(length);
            default:
                throw new ArgumentException("Unsupported section type");
        }
    }
}

public class Section
{
    public string Name { get; set; }
    public int Length { get; set; }
    public SectionType Type { get; set; }

    public Section(string name, int length, SectionType type)
    {
        Name = name;
        Length = length;
        Type = type;
    }
}

public enum SectionType
{
    String,
    ByteArray
}

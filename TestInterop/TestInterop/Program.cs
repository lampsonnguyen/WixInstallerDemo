using System;
using System.Diagnostics;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        string pythonPath = @"C:\Users\lamps\AppData\Local\Microsoft\WindowsApps\python3.12.exe";
        string scriptPath = @"C:\Users\lamps\OneDrive\Documents\GitHub\WixInstallerDemo\TestInterop\TestInterop\script.py";

        ProcessStartInfo start = new ProcessStartInfo();
        start.FileName = pythonPath;
        start.Arguments = string.Format("\"{0}\"", scriptPath);
        start.UseShellExecute = false;
        start.RedirectStandardOutput = true;
        start.RedirectStandardError = true;
        start.CreateNoWindow = true;

        using (Process process = Process.Start(start))
        {
            using (StreamReader reader = process.StandardOutput)
            {
                string result = reader.ReadToEnd();
                Console.WriteLine("Result from Python script: ");
                Console.WriteLine(result);
            }
        }
    }
}

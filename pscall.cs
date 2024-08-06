using System;
using System.Diagnostics;

namespace PowerShellExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Path to the PowerShell script
            string scriptPath = @"C:\Users\lamps\source\repos\ConsoleApp1\ConsoleApp1\test.ps1";

            // Define the parameter value
            string paramValue = "John";

            // Create a new process start info
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"{scriptPath}\" -Name \"{paramValue}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // Start the process
            using (Process process = Process.Start(psi))
            {
                // Read the output
                using (System.IO.StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd();
                    Console.WriteLine(result);
                }
            }
        }
    }
}

using System;
using Renci.SshNet;

class Program
{
    static void Main(string[] args)
    {
        string host = "your_hsm_ip";
        string username = "your_hsm_username";
        string password = "your_hsm_password";

        using (var client = new SshClient(host, username, password))
        {
            try
            {
                client.Connect();
                var cmd = client.RunCommand("status sysstat show");
                var result = cmd.Result;

                Console.WriteLine(result);
                ParseSystemStatus(result);
                
                client.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    static void ParseSystemStatus(string statusOutput)
    {
        if (statusOutput.Contains("ISO"))
        {
            Console.WriteLine("System State: In Service Okay (ISO)");
        }
        else if (statusOutput.Contains("IST"))
        {
            Console.WriteLine("System State: In Service with Trouble (IST)");
        }
        else if (statusOutput.Contains("OFL"))
        {
            Console.WriteLine("System State: Off Line (OFL)");
        }
        else if (statusOutput.Contains("OOS"))
        {
            Console.WriteLine("System State: Out Of Service (OOS)");
        }
        else
        {
            Console.WriteLine("System State: Unknown");
        }
    }
}

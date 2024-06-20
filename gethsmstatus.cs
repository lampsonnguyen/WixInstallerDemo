using System;
using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;

class Program
{
    static void Main(string[] args)
    {
        string primaryHsmLibraryPath = "/path/to/primary/pkcs11/library"; // Path to primary HSM's PKCS#11 library
        string backupHsmLibraryPath = "/path/to/backup/pkcs11/library";   // Path to backup HSM's PKCS#11 library
        string primaryUserPin = "primary-user-pin";
        string backupUserPin = "backup-user-pin";

        Console.WriteLine("Checking status of Primary HSM:");
        CheckHsmStatus(primaryHsmLibraryPath, primaryUserPin);

        Console.WriteLine("Checking status of Backup HSM:");
        CheckHsmStatus(backupHsmLibraryPath, backupUserPin);
    }

    static void CheckHsmStatus(string pkcs11LibraryPath, string userPin)
    {
        try
        {
            using (Pkcs11 pkcs11 = new Pkcs11(pkcs11LibraryPath, AppType.SingleThreaded))
            {
                Slot slot = pkcs11.GetSlotList(SlotsType.WithTokenPresent)[0];

                using (Session session = slot.OpenSession(SessionType.ReadWrite))
                {
                    session.Login(CKU.CKU_USER, userPin);

                    // Perform some status check operation
                    // For example, get the token info
                    TokenInfo tokenInfo = slot.GetTokenInfo();
                    Console.WriteLine("Label: " + tokenInfo.Label);
                    Console.WriteLine("Model: " + tokenInfo.Model);
                    Console.WriteLine("Serial Number: " + tokenInfo.SerialNumber);
                    Console.WriteLine("Firmware Version: " + tokenInfo.FirmwareVersion);
                    Console.WriteLine("Free Private Memory: " + tokenInfo.FreePrivateMemory);
                    Console.WriteLine("Free Public Memory: " + tokenInfo.FreePublicMemory);

                    session.Logout();
                }
            }
        }
        catch (Pkcs11Exception ex)
        {
            Console.WriteLine($"PKCS#11 Exception: {ex.Message} (Error code: {ex.RV})");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General Exception: {ex.Message}");
        }
    }

     static async Task CheckHsmStatusWithRestApi(string hsmApiUrl, string adminPassword)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"admin:{adminPassword}")));
            HttpResponseMessage response = await client.GetAsync(hsmApiUrl);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine("HSM Status from REST API:");
                Console.WriteLine(responseBody);
            }
            else
            {
                Console.WriteLine($"Failed to get HSM status. Status code: {response.StatusCode}");
            }
        }
    }



    static bool PingHost(string nameOrAddress)
    {
        bool pingable = false;
        Ping pinger = new Ping();

        try
        {
            PingReply reply = pinger.Send(nameOrAddress);
            pingable = reply.Status == IPStatus.Success;
        }
        catch (PingException)
        {
            // Discard PingExceptions and return false
        }

        return pingable;
    }
}



class Program
{
static async Task Main(string[] args)
    {
        string hsmApiUrl = "https://your-hsm-address/api/status";
        string adminPassword = "your-admin-password";

        await CheckHsmStatusWithRestApi(hsmApiUrl, adminPassword);
    }

    static async Task CheckHsmStatusWithRestApi(string hsmApiUrl, string adminPassword)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"admin:{adminPassword}")));
            HttpResponseMessage response = await client.GetAsync(hsmApiUrl);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine("HSM Status from REST API:");
                HandleCustomStatusCode(responseBody);
            }
            else
            {
                Console.WriteLine($"Failed to get HSM status. HTTP Status code: {response.StatusCode}");
            }
        }
    }

    static void HandleCustomStatusCode(string statusCode)
    {
        switch (statusCode)
        {
            case "ISO":
                Console.WriteLine("System Status: In Service Okay - The appliance is online and the necessary subsystems are operational.");
                break;
            case "IST":
                Console.WriteLine("System Status: In Service with Trouble - The appliance is online and the necessary subsystems are operational with some troubles.");
                break;
            case "OFL":
                Console.WriteLine("System Status: Off Line - The appliance is not currently connected to the Ethernet network and cannot provide service.");
                break;
            case "OOS":
                Console.WriteLine("System Status: Out Of Service - The appliance is online but the necessary subsystems are NOT operational.");
                break;
            default:
                Console.WriteLine($"Unknown status code: {statusCode}");
                break;
        }
    }
    }
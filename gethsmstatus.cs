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
	//==============================================================================
	
	using System;
using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;
using Net.Pkcs11Interop.HighLevelAPI.Factories;

class Program
{
    static void Main(string[] args)
    {
        string libraryPath = "/usr/lib/libCryptoki2_64.so"; // Path to the PKCS#11 library
        ulong slotId = 0; // The slot ID of the partition you want to check

        try
        {
            // Initialize PKCS#11 library
            IPkcs11Library pkcs11Library = new Pkcs11LibraryFactory().LoadPkcs11Library(
                Pkcs11InteropFactories.Pkcs11InteropFactory, libraryPath, AppType.MultiThreaded);

            // Get the list of slots
            ISlot[] slots = pkcs11Library.GetSlotList(SlotsType.WithTokenPresent);

            foreach (ISlot slot in slots)
            {
                Console.WriteLine($"Slot ID: {slot.SlotId}");

                // Check if the token is present in the slot
                if (slot.TokenInfo != null)
                {
                    Console.WriteLine("Token present");
                    Console.WriteLine($"Token Label: {slot.TokenInfo.Label}");
                    Console.WriteLine($"Token Manufacturer: {slot.TokenInfo.ManufacturerId}");
                    Console.WriteLine($"Token Model: {slot.TokenInfo.Model}");
                    Console.WriteLine($"Token Serial Number: {slot.TokenInfo.SerialNumber}");

                    // Open a session with the token
                    using (ISession session = slot.OpenSession(SessionType.ReadWrite))
                    {
                        try
                        {
                            // Attempt to login as the user
                            session.Login(CKU.CKU_USER, "user-PIN");

                            // If login is successful, the partition is not locked out
                            Console.WriteLine("Partition is not locked out");

                            // Perform other operations if needed...

                            // Logout
                            session.Logout();
                        }
                        catch (Pkcs11Exception ex)
                        {
                            if (ex.RV == CKR.CKR_PIN_LOCKED)
                            {
                                // If CKR_PIN_LOCKED error is thrown, the partition is locked out
                                Console.WriteLine("Partition is locked out");
                            }
                            else
                            {
                                // Handle other exceptions
                                Console.WriteLine($"Error occurred: {ex.RV}");
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No token present");
                }

                Console.WriteLine();
            }

            // Finalize the PKCS#11 library
            pkcs11Library.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }
    }
}


using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;
using Net.Pkcs11Interop.HighLevelAPI.Factories;
using System;

class Program
{
    static void Main(string[] args)
    {
        string libraryPath = "path_to_your_pkcs11_library"; // Replace with actual path
        CKU userRole = CKU.CKU_USER; // User role (e.g., CKU_USER for Partition Security Officer)
        string pin = "your_partition_pin"; // Replace with actual PIN

        try
        {
            // Load PKCS#11 library
            using (IPkcs11Library pkcs11Library = new Pkcs11LibraryFactory().LoadPkcs11Library(libraryPath, AppType.MultiThreaded))
            {
                // Get list of available slots
                ISlotList slots = pkcs11Library.GetSlotList(SlotsType.WithTokenPresent);

                if (slots.Count > 0)
                {
                    // Open session with the first slot
                    using (ISession session = slots[0].OpenSession(SessionType.ReadWrite))
                    {
                        // Login to the partition
                        session.Login(userRole, pin);

                        // Example: List all objects (keys, certificates, etc.) in the token
                        var objects = session.FindAllObjects();
                        foreach (var obj in objects)
                        {
                            Console.WriteLine($"Object handle: {obj.ObjectId}");
                            // Retrieve and print object attributes if needed
                        }

                        // Logout from the session
                        session.Logout();
                    }
                }
                else
                {
                    Console.WriteLine("No slots with tokens found.");
                }
            }
        }
        catch (Pkcs11Exception ex)
        {
            Console.WriteLine($"PKCS#11 error: {ex.Message} (Error code: {ex.RV})");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}


//call CA_
using System;
using System.Runtime.InteropServices;

class Program
{
    // Define the CA_CAPABILITIES structure
    [StructLayout(LayoutKind.Sequential)]
    public struct CA_CAPABILITIES
    {
        public bool supportsRSA;
        public bool supportsECC;
        public bool supportsAES;
        public bool supportsSHA256;
        // Add other capabilities as needed
    }

    // Define the CA_STATUS enum (assuming 0 is success)
    public enum CA_STATUS
    {
        CA_SUCCESS = 0,
        // Add other status codes as needed
    }

    // Define the CA_TOKEN_HANDLE as an IntPtr (pointer)
    public struct CA_TOKEN_HANDLE
    {
        public IntPtr handle;
    }

    // P/Invoke declaration for CA_InitializeToken (if needed)
    [DllImport("thirdparty.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern CA_STATUS CA_InitializeToken(out CA_TOKEN_HANDLE tokenHandle);

    // P/Invoke declaration for CA_GetTokenCapabilities
    [DllImport("thirdparty.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern CA_STATUS CA_GetTokenCapabilities(CA_TOKEN_HANDLE tokenHandle, out CA_CAPABILITIES capabilities);

    // P/Invoke declaration for CA_FinalizeToken (if needed)
    [DllImport("thirdparty.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern CA_STATUS CA_FinalizeToken(CA_TOKEN_HANDLE tokenHandle);

    static void Main(string[] args)
    {
        CA_TOKEN_HANDLE tokenHandle;
        CA_CAPABILITIES capabilities;

        // Initialize the token
        CA_STATUS status = CA_InitializeToken(out tokenHandle);
        if (status != CA_STATUS.CA_SUCCESS)
        {
            Console.WriteLine("Failed to initialize token");
            return;
        }

        // Get the token capabilities
        status = CA_GetTokenCapabilities(tokenHandle, out capabilities);
        if (status != CA_STATUS.CA_SUCCESS)
        {
            Console.WriteLine("Failed to get token capabilities");
            return;
        }

        // Print the capabilities
        Console.WriteLine("Token Capabilities:");
        Console.WriteLine("Supports RSA: " + (capabilities.supportsRSA ? "Yes" : "No"));
        Console.WriteLine("Supports ECC: " + (capabilities.supportsECC ? "Yes" : "No"));
        Console.WriteLine("Supports AES: " + (capabilities.supportsAES ? "Yes" : "No"));
        Console.WriteLine("Supports SHA-256: " + (capabilities.supportsSHA256 ? "Yes" : "No"));

        // Finalize the token
        CA_FinalizeToken(tokenHandle);
    }
}


//ssh and see system status 
using System;
using Renci.SshNet;

class Program
{
    static void Main(string[] args)
    {
        string hsmIp = "HSM_IP";
        string username = "USERNAME";
        string password = "PASSWORD";
        string statusCommand = "STATUS_COMMAND";

        using (var client = new SshClient(hsmIp, username, password))
        {
            try
            {
                // Connect to the HSM
                client.Connect();

                if (client.IsConnected)
                {
                    // Execute the status command
                    var command = client.CreateCommand(statusCommand);
                    var result = command.Execute();

                    // Print the output of the status command
                    Console.WriteLine("System Status:\n" + result);
                }
                else
                {
                    Console.WriteLine("Failed to connect to the HSM.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            finally
            {
                // Disconnect the client
                if (client.IsConnected)
                {
                    client.Disconnect();
                }
            }
        }
    }
}


// view   key 
using System;
using System.Collections.Generic;
using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;
using Net.Pkcs11Interop.HighLevelAPI.Factories;

namespace HsmPublicKeyReader
{
    class Program
    {
        static void Main(string[] args)
        {
            // Path to the PKCS#11 library
            string pkcs11LibraryPath = @"C:\Program Files\SafeNet\LunaClient\cryptoki.dll";
            
            // Initialize PKCS#11 library
            Pkcs11InteropFactories factories = new Pkcs11InteropFactories();
            using (IPkcs11Library library = factories.Pkcs11LibraryFactory.LoadPkcs11Library(factories, pkcs11LibraryPath, AppType.MultiThreaded))
            {
                // Get list of available slots with tokens
                List<ISlot> slots = library.GetSlotList(SlotsType.WithTokenPresent);
                if (slots.Count == 0)
                {
                    Console.WriteLine("No slots found");
                    return;
                }

                // Assume the first slot is the one we want to use
                ISlot slot = slots[0];

                // Open a read-write session
                using (ISession session = slot.OpenSession(SessionType.ReadWrite))
                {
                    // Login as user
                    session.Login(CKU.CKU_USER, "your_partition_password");

                    // Find all public keys
                    List<IObjectAttribute> searchTemplate = new List<IObjectAttribute>();
                    searchTemplate.Add(session.Factories.ObjectAttributeFactory.Create(CKA.CKA_CLASS, CKO.CKO_PUBLIC_KEY));
                    List<IObjectHandle> foundObjects = session.FindAllObjects(searchTemplate);

                    // Display found keys
                    foreach (IObjectHandle objectHandle in foundObjects)
                    {
                        Console.WriteLine($"Object Handle: {objectHandle.ObjectId}");

                        // Retrieve public key value
                        List<CKA> attributes = new List<CKA> { CKA.CKA_LABEL, CKA.CKA_ID, CKA.CKA_PUBLIC_EXPONENT, CKA.CKA_MODULUS };
                        List<IObjectAttribute> objectAttributes = session.GetAttributeValue(objectHandle, attributes);

                        foreach (IObjectAttribute attr in objectAttributes)
                        {
                            if (attr.Type == CKA.CKA_LABEL)
                                Console.WriteLine($"Label: {attr.GetValueAsString()}");
                            else if (attr.Type == CKA.CKA_ID)
                                Console.WriteLine($"ID: {BitConverter.ToString(attr.GetValueAsByteArray())}");
                            else if (attr.Type == CKA.CKA_PUBLIC_EXPONENT)
                                Console.WriteLine($"Public Exponent: {BitConverter.ToString(attr.GetValueAsByteArray())}");
                            else if (attr.Type == CKA.CKA_MODULUS)
                                Console.WriteLine($"Modulus: {BitConverter.ToString(attr.GetValueAsByteArray())}");
                        }
                    }

                    // Logout
                    session.Logout();
                }
            }
        }
    }
}



// dispose 
  public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
                CloseSession();
                _tokenInfo?.Dispose();
                _pkcs11Library?.Dispose();
            }

            // Dispose unmanaged resources if any

            _disposed = true;
        }
    }
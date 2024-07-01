Here’s the implementation of a custom wrapper for the Luna extensions that retrieve various statuses from the HSM:

Define the P/Invoke Signatures:
csharp
Copy code
using System;
using System.Runtime.InteropServices;

public static class LunaStatusExtensions
{
    private const string DLL_NAME = "cryptoki.dll";

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int CA_GetFunctionStatus(uint hSession);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int CA_GetMofNStatus(uint hSession, out uint status);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int CA_GetRemotePEDVectorStatus(uint hSession, out uint status);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int CA_GetTokenStatus(uint hSession, out uint status);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int CA_GetTokenStorageInformation(uint hSession, out uint totalStorage, out uint usedStorage);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int CA_GetHAState(uint hSession, out uint state);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int CA_GetSessionInfo(uint hSession, out uint info);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int CA_GetTokenInsertionCount(uint hSession, out uint count);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int CA_GetHSMStats(uint hSession, out uint stats);

    // Add more P/Invoke signatures here for other status functions
}
Create a Wrapper Class:
csharp
Copy code
using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;
using System;

public class LunaStatusWrapper
{
    private readonly IPkcs11Library _pkcs11Library;

    public LunaStatusWrapper(IPkcs11Library pkcs11Library)
    {
        _pkcs11Library = pkcs11Library;
    }

    public void GetFunctionStatus(Session session)
    {
        var rv = LunaStatusExtensions.CA_GetFunctionStatus(session.Handle);
        if (rv != CKR.CKR_OK)
        {
            throw new Pkcs11Exception("CA_GetFunctionStatus failed", rv);
        }
    }

    public uint GetMofNStatus(Session session)
    {
        uint status;
        var rv = LunaStatusExtensions.CA_GetMofNStatus(session.Handle, out status);
        if (rv != CKR.CKR_OK)
        {
            throw new Pkcs11Exception("CA_GetMofNStatus failed", rv);
        }
        return status;
    }

    public uint GetRemotePEDVectorStatus(Session session)
    {
        uint status;
        var rv = LunaStatusExtensions.CA_GetRemotePEDVectorStatus(session.Handle, out status);
        if (rv != CKR.CKR_OK)
        {
            throw new Pkcs11Exception("CA_GetRemotePEDVectorStatus failed", rv);
        }
        return status;
    }

    public uint GetTokenStatus(Session session)
    {
        uint status;
        var rv = LunaStatusExtensions.CA_GetTokenStatus(session.Handle, out status);
        if (rv != CKR.CKR_OK)
        {
            throw new Pkcs11Exception("CA_GetTokenStatus failed", rv);
        }
        return status;
    }

    public (uint totalStorage, uint usedStorage) GetTokenStorageInformation(Session session)
    {
        uint totalStorage, usedStorage;
        var rv = LunaStatusExtensions.CA_GetTokenStorageInformation(session.Handle, out totalStorage, out usedStorage);
        if (rv != CKR.CKR_OK)
        {
            throw new Pkcs11Exception("CA_GetTokenStorageInformation failed", rv);
        }
        return (totalStorage, usedStorage);
    }

    public uint GetHAState(Session session)
    {
        uint state;
        var rv = LunaStatusExtensions.CA_GetHAState(session.Handle, out state);
        if (rv != CKR.CKR_OK)
        {
            throw new Pkcs11Exception("CA_GetHAState failed", rv);
        }
        return state;
    }

    public uint GetSessionInfo(Session session)
    {
        uint info;
        var rv = LunaStatusExtensions.CA_GetSessionInfo(session.Handle, out info);
        if (rv != CKR.CKR_OK)
        {
            throw new Pkcs11Exception("CA_GetSessionInfo failed", rv);
        }
        return info;
    }

    public uint GetTokenInsertionCount(Session session)
    {
        uint count;
        var rv = LunaStatusExtensions.CA_GetTokenInsertionCount(session.Handle, out count);
        if (rv != CKR.CKR_OK)
        {
            throw new Pkcs11Exception("CA_GetTokenInsertionCount failed", rv);
        }
        return count;
    }

    public uint GetHSMStats(Session session)
    {
        uint stats;
        var rv = LunaStatusExtensions.CA_GetHSMStats(session.Handle, out stats);
        if (rv != CKR.CKR_OK)
        {
            throw new Pkcs11Exception("CA_GetHSMStats failed", rv);
        }
        return stats;
    }

    // Add more wrapper methods here for other status functions
}
Usage in Application:
csharp
Copy code
using Net.Pkcs11Interop.HighLevelAPI;
using Net.Pkcs11Interop.HighLevelAPI.Factories;

public class Program
{
    public static void Main()
    {
        using (var pkcs11Library = new Pkcs11Library("cryptoki.dll"))
        {
            var slot = pkcs11Library.GetSlotList(true)[0];
            using (var session = slot.OpenSession(false))
            {
                var lunaStatusWrapper = new LunaStatusWrapper(pkcs11Library);

                // Example usage of the Luna status wrapper
                lunaStatusWrapper.GetFunctionStatus(session);
                uint mofNStatus = lunaStatusWrapper.GetMofNStatus(session);
                uint remotePEDStatus = lunaStatusWrapper.GetRemotePEDVectorStatus(session);
                uint tokenStatus = lunaStatusWrapper.GetTokenStatus(session);
                var (totalStorage, usedStorage) = lunaStatusWrapper.GetTokenStorageInformation(session);
                uint haState = lunaStatusWrapper.GetHAState(session);
                uint sessionInfo = lunaStatusWrapper.GetSessionInfo(session);
                uint insertionCount = lunaStatusWrapper.GetTokenInsertionCount(session);
                uint hsmStats = lunaStatusWrapper.GetHSMStats(session);

                // Add more function calls as needed
            }
        }
    }
}
This example demonstrates how to create a wrapper for the status retrieval functions of the Luna HSM extensions. You can extend the LunaStatusExtensions and LunaStatusWrapper classes to include additional status functions as needed.
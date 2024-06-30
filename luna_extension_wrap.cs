The Luna HSM extensions to PKCS#11 include a variety of functions designed for enhanced control and management of HSM operations. Here is a list of some of these extensions:

CA_CheckOperationState - Checks if a specific cryptographic operation is in progress.
CA_CloneAsSource/CA_CloneAsTarget/CA_CloneAsTargetInit/CA_CloneObject/CA_ClonePrivateKey - Functions related to the secure cloning of objects and private keys.
CA_CloseApplicationID/CA_CloseApplicationIDForContainer - Deactivates application identifiers.
CA_ConfigureRemotePED - Configures a slot to use remote PED information.
CA_CreateContainer/CA_CreateContainerLoginChallenge/CA_CreateLoginChallenge - Functions for creating and managing partitions and login challenges.
CA_Deactivate - Deactivates a partition.
CA_DeleteContainer/CA_DeleteContainerWithHandle/CA_DeleteRemotePEDVector - Deletes partitions or remote PED vectors.
CA_DeriveKeyAndWrap - Optimizes key derivation and wrapping.
CA_DestroyMultipleObjects - Deletes multiple objects simultaneously.
CA_DuplicateMofN - Creates duplicates of secret splits.
CA_EncodeECChar2Params/CA_EncodeECParamsFromFile/CA_EncodeECPrimeParams - Encodes EC curve parameters.
CA_Extract - Extracts a SIM3 blob.
CA_FactoryReset - Resets the HSM to factory settings.
CA_FindAdminSlotForSlot - Gets the admin slot for a given slot.
CA_FirmwareRollback - Rolls back the firmware to a previous version.
CA_GenerateCloneableMofN/CA_GenerateCloningKEV/CA_GenerateMofN - Functions for generating cloneable MofN vectors and key exchange values.
CA_Get - Retrieves HSM parameters such as serial numbers and certificates.
CA_GetApplicationID - Gets an application's access ID.
CA_GetConfigurationElementDescription - Retrieves descriptions and properties of capabilities and policies.
CA_GetContainerCapabilitySet/CA_GetContainerCapabilitySetting/CA_GetContainerList/CA_GetContainerName - Functions for managing and querying container capabilities and policies.
CA_GetCurrentHAState/CA_GetHAState - Gets the High Availability (HA) state of the HSM.
CA_GetHSMCapabilitySet/CA_GetHSMCapabilitySetting/CA_GetHSMPolicySet/CA_GetHSMPolicySetting/CA_GetHSMStats/CA_GetHSMStorageInformation - Functions for retrieving HSM capabilities, policies, statistics, and storage information.
CA_GetMofNStatus - Retrieves the MofN structure of a token.
CA_GetSessionInfo - Retrieves session information, including authentication state and container handle.
CA_GetSlotIdForContainer/CA_GetSlotIdForPhysicalSlot/CA_GetSlotListFromServerInstance - Functions for managing slot information.
CA_GetTime - Gets the HSM time.
CA_GetTokenCapabilities/CA_GetTokenCertificateInfo/CA_GetTokenCertificates/CA_GetTokenInsertionCount/CA_GetTokenObjectHandle/CA_GetTokenObjectUID/CA_GetTokenPolicies/CA_GetTokenStatus/CA_GetTokenStorageInformation - Functions for managing token capabilities, certificates, insertion counts, object handles, policies, and storage information.
CA_HAActivateMofN/CA_HAAnswerLoginChallenge/CA_HAAnswerMofNChallenge/CA_HAGetLoginChallenge/CA_HAGetMasterPublic/CA_HAInit/CA_HALogin - High Availability functions related to login and MofN challenges.
CA_InitAudit/CA_InitializeRemotePEDVector/CA_InitRolePIN/CA_InitSlotRolePIN/CA_InitToken - Functions for initializing various aspects of the HSM.
CA_Insert - Inserts a SIM3 blob.
CA_IsMofNEnabled/CA_IsMofNRequired - Queries MofN status.
CA_ListSecureTokenInit/CA_ListSecureTokenUpdate - Functions for retrieving information from secure tokens.
CA_LogExportSecret/CA_LogExternal/CA_LogGetConfig/CA_LogGetStatus/CA_LogImportSecret/CA_LogSetConfig/CA_LogVerify/CA_LogVerifyFile - Functions for managing and verifying audit logs.
CA_ManualKCV - Sets the key cloning vector.
CA_ModifyMofN - Modifies the MofN vector.
CA_ModifyUsageCount - Modifies key usage count.
CA_MTKGetState/CA_MTKResplit/CA_MTKRestore/CA_MTKSetStorage/CA_MTKZeroize - Functions related to the master tamper key (MTK).
CA_OpenApplicationID/CA_OpenApplicationIDForContainer/CA_OpenSecureToken/CA_OpenSession/CA_OpenSessionWithAppID - Functions for opening application identifiers and sessions.
CA_PerformSelfTest - Performs a self-test on the HSM.
CA_QueryLicense - Queries license information.
CA_ResetDevice/CA_ResetPIN - Resets the HSM or role PIN.



Creating a comprehensive wrapper for all the Luna extensions can be quite extensive. Below is an example of how you can start by defining the P/Invoke signatures and creating a wrapper class for the Luna HSM extensions. This example includes a few of the functions for brevity, but the same pattern can be extended to cover all the functions listed.

Define the P/Invoke Signatures:

using System;
using System.Runtime.InteropServices;

public static class LunaExtensions
{
    private const string DLL_NAME = "cryptoki.dll";

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int CA_CheckOperationState(uint hSession);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int CA_CloneAsSource(uint hSession);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int CA_CloneAsTarget(uint hSession);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int CA_CloneAsTargetInit(uint hSession);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int CA_CloneObject(uint hSession);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int CA_ClonePrivateKey(uint hSession);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int CA_CloseApplicationID(uint hSession, uint appID);

    [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    public static extern int CA_CloseApplicationIDForContainer(uint hSession, uint containerID, uint appID);

    // Add more P/Invoke signatures here for other functions
}
Create a Wrapper Class:

using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;
using System;

public class LunaExtensionsWrapper
{
    private readonly IPkcs11Library _pkcs11Library;

    public LunaExtensionsWrapper(IPkcs11Library pkcs11Library)
    {
        _pkcs11Library = pkcs11Library;
    }

    public void CheckOperationState(Session session)
    {
        var rv = LunaExtensions.CA_CheckOperationState(session.Handle);
        if (rv != CKR.CKR_OK)
        {
            throw new Pkcs11Exception("CA_CheckOperationState failed", rv);
        }
    }

    public void CloneAsSource(Session session)
    {
        var rv = LunaExtensions.CA_CloneAsSource(session.Handle);
        if (rv != CKR.CKR_OK)
        {
            throw new Pkcs11Exception("CA_CloneAsSource failed", rv);
        }
    }

    public void CloneAsTarget(Session session)
    {
        var rv = LunaExtensions.CA_CloneAsTarget(session.Handle);
        if (rv != CKR.CKR_OK)
        {
            throw new Pkcs11Exception("CA_CloneAsTarget failed", rv);
        }
    }

    public void CloneAsTargetInit(Session session)
    {
        var rv = LunaExtensions.CA_CloneAsTargetInit(session.Handle);
        if (rv != CKR.CKR_OK)
        {
            throw new Pkcs11Exception("CA_CloneAsTargetInit failed", rv);
        }
    }

    public void CloneObject(Session session)
    {
        var rv = LunaExtensions.CA_CloneObject(session.Handle);
        if (rv != CKR.CKR_OK)
        {
            throw new Pkcs11Exception("CA_CloneObject failed", rv);
        }
    }

    public void ClonePrivateKey(Session session)
    {
        var rv = LunaExtensions.CA_ClonePrivateKey(session.Handle);
        if (rv != CKR.CKR_OK)
        {
            throw new Pkcs11Exception("CA_ClonePrivateKey failed", rv);
        }
    }

    public void CloseApplicationID(Session session, uint appID)
    {
        var rv = LunaExtensions.CA_CloseApplicationID(session.Handle, appID);
        if (rv != CKR.CKR_OK)
        {
            throw new Pkcs11Exception("CA_CloseApplicationID failed", rv);
        }
    }

    public void CloseApplicationIDForContainer(Session session, uint containerID, uint appID)
    {
        var rv = LunaExtensions.CA_CloseApplicationIDForContainer(session.Handle, containerID, appID);
        if (rv != CKR.CKR_OK)
        {
            throw new Pkcs11Exception("CA_CloseApplicationIDForContainer failed", rv);
        }
    }

    // Add more wrapper methods here for other functions
}
Usage in Application:

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
                var lunaExtensionsWrapper = new LunaExtensionsWrapper(pkcs11Library);

                // Example usage of the Luna extensions wrapper
                lunaExtensionsWrapper.CheckOperationState(session);
                lunaExtensionsWrapper.CloneAsSource(session);
                lunaExtensionsWrapper.CloneAsTarget(session);
                lunaExtensionsWrapper.CloneAsTargetInit(session);
                lunaExtensionsWrapper.CloneObject(session);
                lunaExtensionsWrapper.ClonePrivateKey(session);
                lunaExtensionsWrapper.CloseApplicationID(session, 12345);
                lunaExtensionsWrapper.CloseApplicationIDForContainer(session, 54321, 12345);

                // Add more function calls as needed
            }
        }
    }
}
This example demonstrates how to start creating the wrapper. To complete the wrapper, you need to add the P/Invoke signatures and corresponding methods in the wrapper class for all the Luna extension functions listed in the documentation. This pattern can be followed for each function, ensuring that the return values are properly checked and appropriate exceptions are thrown for any errors.
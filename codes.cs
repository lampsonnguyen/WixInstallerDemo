using System;
using System.Collections.Generic;
using Net.Pkcs11Interop.Common;
using Net.Pkcs11Interop.HighLevelAPI;

class Program
{
    static void Main(string[] args)
    {
        string pkcs11LibraryPath = "/usr/safenet/lunaclient/lib/libCryptoki2_64.so"; // Path to your PKCS#11 library
        string userPin = "your-user-pin";

        using (Pkcs11 pkcs11 = new Pkcs11(pkcs11LibraryPath, AppType.SingleThreaded))
        {
            Slot slot = pkcs11.GetSlotList(SlotsType.WithTokenPresent)[0];

            using (Session session = slot.OpenSession(SessionType.ReadWrite))
            {
                session.Login(CKU.CKU_USER, userPin);

                // Template for a new AES key object
                List<ObjectAttribute> objectAttributes = new List<ObjectAttribute>
                {
                    new ObjectAttribute(CKA.CKA_CLASS, CKO.CKO_SECRET_KEY),
                    new ObjectAttribute(CKA.CKA_KEY_TYPE, CKK.CKK_AES),
                    new ObjectAttribute(CKA.CKA_TOKEN, true), // Persistent key
                    new ObjectAttribute(CKA.CKA_PRIVATE, true), // Private key
                    new ObjectAttribute(CKA.CKA_LABEL, "ExportableAESKey"),
                    new ObjectAttribute(CKA.CKA_VALUE_LEN, 32), // 256-bit key
                    new ObjectAttribute(CKA.CKA_ENCRYPT, true),
                    new ObjectAttribute(CKA.CKA_DECRYPT, true),
                    new ObjectAttribute(CKA.CKA_SENSITIVE, false), // Non-sensitive key
                    new ObjectAttribute(CKA.CKA_EXTRACTABLE, true) // Extractable key
                };

                // Generate a new AES key object
                ObjectHandle keyHandle = session.GenerateKey(new Mechanism(CKM.CKM_AES_KEY_GEN), objectAttributes);

                try
                {
                    // Export the key
                    byte[] keyValue = session.GetAttributeValue(keyHandle, new List<CKA> { CKA.CKA_VALUE })[0].GetValueAsByteArray();
                    Console.WriteLine("Key value exported successfully:");
                    Console.WriteLine(BitConverter.ToString(keyValue));
                }
                catch (Pkcs11Exception ex)
                {
                    Console.WriteLine($"PKCS#11 Exception: {ex.Message} (Error code: {ex.RV})");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"General Exception: {ex.Message}");
                }

                session.Logout();
            }
        }
    }
}




<?xml version="1.0" encoding="UTF-8"?>
<Wix xmlns="http://wixtoolset.org/schemas/v4/wxs" xmlns:util="http://wixtoolset.org/schemas/v4/wixUtilExtension" xmlns:iis="http://wixtoolset.org/schemas/v4/iis">
    <Package Name="MyWebService" Manufacturer="MyCompany" Version="1.0.0.0" UpgradeCode="PUT-GUID-HERE" Compressed="yes">
        <Media Id="1" Cabinet="product.cab" EmbedCab="yes" />

        <Property Id="INSTALLFOLDER" Value="C:\Program Files\MyCompany\MyWebService" />

        <Directory Id="TARGETDIR" Name="SourceDir">
            <Directory Id="ProgramFilesFolder">
                <Directory Id="INSTALLFOLDER" Name="MyWebService" />
            </Directory>
        </Directory>

        <DirectoryRef Id="INSTALLFOLDER">
            <Component Id="MainExecutable" Guid="PUT-GUID-HERE">
                <File Source="path\to\your\webservice\bin\yourwebservice.dll" />
                <File Source="path\to\your\webservice\bin\yourwebservice.pdb" />
                <File Id="AppSettingsFile" Name="appsettings.json" Source="path\to\your\config\appsettings.json" KeyPath="yes" />
            </Component>
        </DirectoryRef>

        <ComponentGroup Id="ProductComponents">
            <ComponentRef Id="MainExecutable" />
        </ComponentGroup>

        <DirectoryRef Id="INSTALLFOLDER">
            <Component Id="WebAppComponent" Guid="PUT-GUID-HERE">
                <iis:WebSite Id="MyWebSite" Description="My Web Service Site">
                    <iis:WebAddress Id="AllUnassigned" IP="*" Port="80" />
                    <iis:WebApplication Id="MyWebApp" Name="MyWebApp" WebAppPool="MyAppPool">
                        <iis:WebVirtualDir Id="MyVirtualDir" Alias="MyWebApp" Directory="INSTALLFOLDER">
                            <iis:WebDirProperties Id="MyWebDirProperties" AnonymousAccess="yes" DefaultDocuments="index.html"/>
                        </iis:WebVirtualDir>
                    </iis:WebApplication>
                </iis:WebSite>

                <iis:WebAppPool Id="MyAppPool" Name="MyAppPool" ManagedPipelineMode="Integrated" ManagedRuntimeVersion="v4.0" />
            </Component>
        </DirectoryRef>

        <Feature Id="ProductFeature" Title="MyWebService" Level="1">
            <ComponentGroupRef Id="ProductComponents" />
            <ComponentRef Id="WebAppComponent" />
        </Feature>

        <CustomAction Id="ModifyAppSettings" BinaryKey="ModifyAppSettings.ps1" Execute="deferred" Impersonate="no" Return="check">
            <![CDATA[
            param(
                [string]$installFolder
            )

            $appSettingsPath = Join-Path -Path $installFolder -ChildPath "appsettings.json"

            if (Test-Path $appSettingsPath) {
                $json = Get-Content -Raw -Path $appSettingsPath | ConvertFrom-Json
                
                # Modify the JSON content here, replace DLL_PATH and SLOT_NUM
                $json.hsm.crypto = "C:/temp/ck.dll"
                $json.hsm.slot = 0
                
                $json | ConvertTo-Json -Depth 32 | Set-Content -Path $appSettingsPath -Force
            }
            ]]>
        </CustomAction>

        <InstallExecuteSequence>
            <Custom Action="ModifyAppSettings" After="InstallFiles">NOT Installed</Custom>
        </InstallExecuteSequence>
    </Package>
</Wix>









<Wix xmlns="http://wixtoolset.org/schemas/v4/wxs">
    <Package Name="MyWebService" Manufacturer="MyCompany" Version="1.0.0.0" UpgradeCode="PUT-GUID-HERE" Compressed="yes">
        <Media Id="1" Cabinet="product.cab" EmbedCab="yes" />

        <Property Id="INSTALLFOLDER" Value="C:\Program Files\MyCompany\MyWebService" />

        <Directory Id="TARGETDIR" Name="SourceDir">
            <Directory Id="ProgramFilesFolder">
                <Directory Id="INSTALLFOLDER" Name="MyWebService" />
            </Directory>
        </Directory>

        <DirectoryRef Id="INSTALLFOLDER">
            <Component Id="MainExecutable" Guid="PUT-GUID-HERE">
                <File Source="path\to\your\webservice\bin\yourwebservice.dll" />
                <File Source="path\to\your\webservice\bin\yourwebservice.pdb" />
                <File Id="AppSettingsFile" Name="appsettings.json" Source="path\to\your\config\appsettings.json" KeyPath="yes" />
            </Component>
        </DirectoryRef>

        <ComponentGroup Id="ProductComponents">
            <ComponentRef Id="MainExecutable" />
        </ComponentGroup>

        <Binary Id="CustomActionsDll" SourceFile="path\to\your\CustomActions.dll" />

        <CustomAction Id="ModifyAppSettings" BinaryKey="CustomActionsDll" DllEntry="ModifyAppSettings" Execute="deferred" Return="check" Impersonate="no" />

        <InstallExecuteSequence>
            <Custom Action="ModifyAppSettings" After="InstallFinalize">NOT Installed</Custom>
        </InstallExecuteSequence>

        <Feature Id="ProductFeature" Title="MyWebService" Level="1">
            <ComponentGroupRef Id="ProductComponents" />
        </Feature>
    </Package>
</Wix>


 public class CustomActions
    {
        [CustomAction]
        public static ActionResult ModifyAppSettings(Session session)
        {
            session.Log("Begin ModifyAppSettings");

            try
            {
                string installFolder = session["INSTALLFOLDER"];
                string appSettingsPath = Path.Combine(installFolder, "appsettings.json");

                if (File.Exists(appSettingsPath))
                {
                    var json = JObject.Parse(File.ReadAllText(appSettingsPath));

                    // Modify the JSON content here, replace DLL_PATH and SLOT_NUM
                    json["hsm"]["crypto"] = "C:/temp/ck.dll";
                    json["hsm"]["slot"] = 0;

                    File.WriteAllText(appSettingsPath, json.ToString());
                }
                else
                {
                    session.Log("appsettings.json file not found.");
                }
            }
            catch (Exception ex)
            {
                session.Log($"Exception: {ex.Message}");
                return ActionResult.Failure;
            }

            session.Log("End ModifyAppSettings");
            return ActionResult.Success;
        }
    }
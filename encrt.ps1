# Load the PKCS11Interop DLL
Add-Type -Path "C:\path\to\PKCS11Interop.dll"

# Define the path to the SoftHSM2 PKCS#11 library (cryptoki.dll)
$softhsm2Path = "C:\path\to\softhsm2.dll"

# Initialize PKCS11Interop
$pkcs11 = New-Object Pkcs11Interop.Pkcs11($softhsm2Path, $false)

# Open a session on the first available slot
$slotList = $pkcs11.GetSlotList($false)
$slot = $slotList[0]  # Assuming slot 0 is being used
$session = $slot.OpenSession([Pkcs11Interop.Common.SessionType]::ReadWrite)

# Log in to the session with your user PIN (replace with your actual PIN)
$session.Login([Pkcs11Interop.Common.UserType]::User, "your_pin")

# Function to find object by label
function Get-ObjectByLabel {
    param (
        [string]$label
    )

    $template = New-Object Pkcs11Interop.HighLevelAPI.MechanismParams.AttributeCollection
    $template.Add([Pkcs11Interop.Common.CKA]::LABEL, $label)
    
    $session.FindObjectsInit($template)
    $objects = $session.FindObjects(1)  # Assuming only one object matches the label
    $session.FindObjectsFinal()
    
    return $objects[0]  # Return the first object found
}

# Lookup Encryption Key by Label ("Enc Key" for AES-256)
$encKeyObject = Get-ObjectByLabel "Enc Key"

# Encrypt data using AES-256
$encryptionMechanism = New-Object Pkcs11Interop.HighLevelAPI.Mechanism([Pkcs11Interop.Common.CKM]::AES_CBC_PAD, (New-Object byte[] 16))  # 16-byte IV for AES CBC

$dataToEncrypt = [System.Text.Encoding]::UTF8.GetBytes("Secret Message")
$encryptedData = $session.Encrypt($encryptionMechanism, $encKeyObject, $dataToEncrypt)
Write-Host "Encrypted Data: " -NoNewline
[BitConverter]::ToString($encryptedData)

# Clean up and logout
$session.Logout()
$session.CloseSession()
$pkcs11.Dispose()

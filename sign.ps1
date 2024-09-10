#.\signature_script.ps1 -filePath "C:\path\to\ps_output.ehdr"

param (
    [string]$filePath  # Accepts the path to the binary file
)

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

# Lookup ECDSA Private Key by Label ("Signing Private Key")
$signingPrivateKey = Get-ObjectByLabel "Signing Private Key"

# Read binary data from the file passed as a parameter
$dataToSign = Get-Content $filePath -AsByteStream

# Sign data using ECDSA SHA-384
$signatureMechanism = New-Object Pkcs11Interop.HighLevelAPI.Mechanism([Pkcs11Interop.Common.CKM]::ECDSA_SHA384)

$signature = $session.Sign($signatureMechanism, $signingPrivateKey, $dataToSign)
Write-Host "`nSignature: " -NoNewline
[BitConverter]::ToString($signature)

# Clean up and logout
$session.Logout()
$session.CloseSession()
$pkcs11.Dispose()

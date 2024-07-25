# Define the app pool name
$appPoolName = "Mram"

# Define the desired managed runtime version for .NET Core (No Managed Code)
$runtimeVersion = ""

# Import the WebAdministration module to manage IIS
Import-Module WebAdministration 

# Get the application pool
$appPool = Get-Item -Path "IIS:\AppPools\$appPoolName" -ErrorAction SilentlyContinue

# Check if the app pool exists
if ($appPool -eq $null) {
    Write-Host "Application pool '$appPoolName' does not exist. Creating it..."
    # Create the application pool
	$newAppPool = New-WebAppPool -Name $appPoolName
    Set-ItemProperty -Path IIS:\AppPools\$appPoolName managedRuntimeVersion $runtimeVersion
    Write-Host "Application pool '$appPoolName' created with managed runtime version 'No Managed Code'."
} else {
    Write-Host "Application pool '$appPoolName' already exists. Updating the managed runtime version..."
    # Update the managed runtime version
    $appPool | Set-ItemProperty -Name managedRuntimeVersion -Value $runtimeVersion
    Write-Host "Updated the managed runtime version for application pool '$appPoolName' to 'No Managed Code'."
}

# Output the current configuration for confirmation
Get-Item "IIS:\AppPools\$appPoolName" | Select-Object Name, managedRuntimeVersion

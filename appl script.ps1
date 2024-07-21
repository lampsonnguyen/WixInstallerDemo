Import-Module WebAdministration

# Define variables
$siteName = "MdsuService"
$appAlias = "mram"
$appPath = "C:\Program File\R10\MramService"
$appPoolName = "MramService"
$appPoolRuntime = "No Managed Code"

# Create the application pool if it does not exist
if (-not (Get-WebAppPoolState -Name $appPoolName -ErrorAction SilentlyContinue)) {
    New-WebAppPool -Name $appPoolName
    Set-ItemProperty "IIS:\AppPools\$appPoolName" -Name "managedRuntimeVersion" -Value $appPoolRuntime
}

# Add the application to the site
$app = "IIS:\Sites\$siteName\$appAlias"
New-WebApplication -Site $siteName -Name $appAlias -PhysicalPath $appPath -ApplicationPool $appPoolName

# Confirm the application was added
Write-Output "Application $appAlias added to site $siteName with application pool $appPoolName."

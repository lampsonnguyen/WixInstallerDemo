# Define the path to the file
$filePath = "C:/Program Files/R10/HsmService/Data/user.txt"

# Define the user and the access rule
$user = "DOMAIN\User" # Replace DOMAIN\User with the actual domain and username
$fileSystemRights = [System.Security.AccessControl.FileSystemRights]::Read, [System.Security.AccessControl.FileSystemRights]::Write
$accessControlType = [System.Security.AccessControl.AccessControlType]::Allow

# Get the current ACL
$acl = Get-Acl -Path $filePath

# Create a new file system access rule
$accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule($user, $fileSystemRights, $accessControlType)

# Add the new access rule to the ACL
$acl.AddAccessRule($accessRule)

# Set the new ACL on the file
Set-Acl -Path $filePath -AclObject $acl

# Output the updated ACL for verification
Get-Acl -Path $filePath | Format-List

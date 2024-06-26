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




# When working with ACLs (Access Control Lists) in PowerShell, particularly when dealing with file system ACLs, you can set various permissions. Here are the primary options you can use with ACLs in PowerShell, specifically for FileSystemRights:
# 
# FileSystemRights Enumeration
# These are some of the most commonly used permissions:
# 
# * FullControl: Full control over the file or directory.
# * Modify: Read, write, modify, and delete the file or directory.
# * ReadAndExecute: Read and run executable files, including script files.
# * ListDirectory: List the contents of a directory.
# * Read: Read the contents of the file or directory.
# * Write: Write to the file or directory.
# * Delete: Delete the file or directory.
# * ReadPermissions: Read the permissions of the file or directory.
# * WritePermissions: Change the permissions of the file or directory.
# * TakeOwnership: Take ownership of the file or directory.
# * Synchronize: Synchronize access and use the file or directory for synchronization.


# AccessControlType Enumeration
# These options specify whether to allow or deny the specified permissions:
# 
# * Allow: Allow the specified permissions.
# * Deny: Deny the specified permissions.


# InheritanceFlags Enumeration
# These flags specify how the permissions are inherited by subdirectories and files:
# 
# * None: The permission is not inherited.
# * ContainerInherit: The permission is inherited by subdirectories.
# * ObjectInherit: The permission is inherited by files.
# * InheritOnly: The permission applies only to child objects.


# PropagationFlags Enumeration
# These flags specify how inheritance of permissions is propagated to child objects:
# 
# * None: No inheritance flags.
# * NoPropagateInherit: The inheritance is not propagated to child objects.
# * InheritOnly: The permission applies only to child objects.
# Example: Combining Permissions and Options
# Here's an example PowerShell script that combines various options to set permissions:


# Define the path to the file
$filePath = "C:/Program Files/R10/HsmService/Data/user.txt"

# Define the user and the access rule
$user = "DOMAIN\User" # Replace DOMAIN\User with the actual domain and username
$fileSystemRights = [System.Security.AccessControl.FileSystemRights]::Read, [System.Security.AccessControl.FileSystemRights]::Write
$accessControlType = [System.Security.AccessControl.AccessControlType]::Allow
$inheritanceFlags = [System.Security.AccessControl.InheritanceFlags]::None
$propagationFlags = [System.Security.AccessControl.PropagationFlags]::None

# Get the current ACL
$acl = Get-Acl -Path $filePath

# Create a new file system access rule
$accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule($user, $fileSystemRights, $inheritanceFlags, $propagationFlags, $accessControlType)

# Add the new access rule to the ACL
$acl.AddAccessRule($accessRule)

# Set the new ACL on the file
Set-Acl -Path $filePath -AclObject $acl

# Output the updated ACL for verification
Get-Acl -Path $filePath | Format-List


# In this script, the InheritanceFlags and PropagationFlags are set to None, meaning the permissions apply only to the specified file and do not propagate to child objects. Adjust these options as needed to fit your specific use case.
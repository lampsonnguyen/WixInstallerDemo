param (
    [string]$IV,
    [string]$KeyFile,
    [string]$InputFile,
    [string]$OutputFile
)

$opensslPath = "C:\Path\To\OpenSSL\openssl.exe"  # Update this to the path of your OpenSSL executable

# Read the key from the key file and remove any whitespace or newline characters
$key = (Get-Content -Raw $KeyFile).Trim()

$arguments = "aes-256-ctr -d -in `"$InputFile`" -out `"$OutputFile`" -K `"$key`" -iv `"$IV`""

$process = Start-Process -FilePath $opensslPath -ArgumentList $arguments -NoNewWindow -Wait -PassThru

if ($process.ExitCode -eq 0) {
    Write-Output "Decryption successful. Output file: $OutputFile"
} else {
    Write-Error "Decryption failed. Exit code: $($process.ExitCode)"
}

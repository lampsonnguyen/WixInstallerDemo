   $jsonContent = [regex]::Replace($(Get-Content $jsonFilePath -Raw),"//.*|(?s)/\*.*?\*/","") | ConvertFrom-Json
   
   
   
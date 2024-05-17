# Write "Hello, world!" to log file
Add-Content -Path "C:\ExecutionLog.log" -Value "Hello, world!"



#Set the user permission 
Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned -Force


# Define paths
Write-Host "Before reading configuration
$scriptFolder = "E:\Mohammed Amaan's\Dotnet Projects\WebAPI_Projects\Ecom_api\Ecom_API_latest\DBScript"
$configurationFile = "$scriptFolder\Configuration.ini"
$logFile = "$scriptFolder\ExecutionLog.log"
# Write "Hello, world!" to log file
"Hello, world!" | Out-File -FilePath $logFile -Append

Write-Host "After reading configuration"
# Read configuration
"This will be written in file" | Out-File -FilePath $scriptFolder
$config = Get-Content -Path $configurationFile | ForEach-Object {
    $key, $value = $_ -split "="
    @{ $key = $value }
}
Write-Host "Configuration read successfully"

# Set SQL Server connection details
$serverInstance = $config["SQLServer"]
$database = $config["SQLDBName"]
$username = $config["Username"]
$password = $config["Password"]

# Initialize log variables
$successfulCount = 0
$errorCount = 0
$pendingCount = 0

# Function to write log
Write-Host "Before executing SQL scripts
function Write-Log {
    param(
        [string]$message
    )
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $logMessage = "$timestamp - $message"
    Add-Content -Path $logFile -Value $logMessage
}
Write-Host "After executing SQL scripts"
# Function to execute SQL script
function Execute-SqlScript {
    param(
        [string]$scriptPath
    )
    try {
        # Read SQL script content
        $scriptContent = Get-Content -Path $scriptPath -Raw
        
        # Replace placeholders with actual values
        $scriptContent = $scriptContent -replace "{{DBNAME}}", $database
        
        # Execute SQL script
        Invoke-Sqlcmd -ServerInstance $serverInstance -Database $database -Username $username -Password $password -Query $scriptContent -ErrorAction Stop
        
        Write-Log "Successfully executed: $scriptPath"
        $global:successfulCount++
    }
    catch {
        Write-Log "Error executing $scriptPath: $_"
        $global:errorCount++
    }
}

# Get list of SQL script files
$sqlScripts = Get-ChildItem -Path $scriptFolder -Filter "*.sql" | Sort-Object Name

# Execute SQL scripts
foreach ($script in $sqlScripts) {
    Write-Log "Executing script: $($script.Name)"
    Execute-SqlScript -scriptPath $script.FullName
}

# Write summary to log
Write-Log "Execution Summary:"
Write-Log "Successful scripts: $successfulCount"
Write-Log "Error scripts: $errorCount"
Write-Log "Pending scripts: $($sqlScripts.Count - $successfulCount - $errorCount)"

# Display log
Get-Content -Path $logFile

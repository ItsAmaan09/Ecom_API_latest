#Created by Ashik
#v1.1 - Modified by Jija Dahifale, on 6th Nov, 2019. To get release folders correctly sorted. Giving issue only when 0.0.0.1, 0.0.0.2, 0.0.0.10
#It was consiering 0.0.0.10 after 0.0.0.1 instead of 0.0.0.2. Added Sort-Naturally UDF

cls
ECHO "Please wait..."

#Set the user permission
# Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned -Force

#Adding SQL cmdlets if needed
    if ( (Get-PSSnapin -Name SqlServerCmdletSnapin100 -ErrorAction SilentlyContinue) -eq $null )
        {
            Add-PSSnapin SqlServerCmdletSnapin100
        }
    if ( (Get-PSSnapin -Name SqlServerProviderSnapin100 -ErrorAction SilentlyContinue) -eq $null )
        {
            Add-PSSnapin SqlServerProviderSnapin100
        }

#Find current date
$date = (Get-Date).ToString('dd-MMM-yyyy hh:mm:ss tt') #(Get-Date).ToString() #get-date -format g

#DO
#{
    #Declare Server

    #cls
    #Write-Warning "What SQL Server will be used for these queries?"

    #$server = Read-host `n"Press enter after typing the name of the server"

    #Declare Database

    #cls
    #Write-Warning "What database will be used for these queries?"

   # $database = Read-host `n"Press enter after typing the database name"

    #Declare Username
    #cls
    #Write-Warning "Username?"

    #$Username = Read-host `n"Press enter after typing the user name"

    #Declare Password

    #cls
   # Write-Warning "Password?"

    #$Password = Read-host `n"Press enter after typing the Password"

    #Confirm SQL server and database information

    #DO
    #    {
     #       cls
            #Replacement Word
    #        Write-Warning "These queries will be utilizing the server '$server', database '$database', username '$Username' and password '$Password'. Is this correct?"

     #       $sqlInfoConfirm = Read-host `n"Press 'y' for yes or 'n' for no " `n"Press enter after making your selection to continue"
     #   }

   # while ("y", "n" -notcontains $sqlInfoConfirm)
#}

#while ("y" -notcontains $sqlInfoConfirm)

$ScriptVariable = Split-Path $script:MyInvocation.MyCommand.Path
$ScriptVariable += "\Configuration.ini"

Get-Content "$ScriptVariable" | ForEach-Object -Begin {$settings=@{}} -Process {$store = [regex]::split($_,'='); if(($store[0].CompareTo("") -ne 0) -and ($store[0].StartsWith("[") -ne $True) -and ($store[0].StartsWith("#") -ne $True)) {$settings.Add($store[0], $store[1])}}

$server = $settings.Get_Item("SQLServer")
$database = $settings.Get_Item("SQLDBName")
$Username = $settings.Get_Item("Username")
$Password = $settings.Get_Item("Password")

$successCount = 0
$errorCount = 0



#Find current directory path
$CurrentDir = $(get-location).Path;

# Log file creation if not exists
$path = $CurrentDir  + "\ExecutionLog.log"

If(!(test-path $path))
{
      New-Item -Path . -Name "ExecutionLog.log" -ItemType "file"
}


#Get all of the folder into an object

function Sort-Naturally
{
    PARAM(
        [System.Collections.ArrayList]$Array,
        [switch]$Descending
    )

    Add-Type -TypeDefinition @'
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    namespace NaturalSort {
    public static class NaturalSort
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern int StrCmpLogicalW(string psz1, string psz2);
        public static System.Collections.ArrayList Sort(System.Collections.ArrayList foo)
        {
            foo.Sort(new NaturalStringComparer());
            return foo;
        }
    }
    public class NaturalStringComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            return NaturalSort.StrCmpLogicalW(x.ToString(), y.ToString());
        }
    }
}
'@
    $Array.Sort((New-Object NaturalSort.NaturalStringComparer))
    if($Descending)
    {
        $Array.Reverse()
    }
    return $Array
}


$folders = Get-ChildItem -Path $CurrentDir -Recurse  -Directory -Force -ErrorAction SilentlyContinue | Select-Object FullName

$folders = Sort-Naturally -Array $folders

#Clear the content of Log file
Clear-Content $path

"------------------------------" | out-file $path -Append
"SERVER DETAILS" | out-file $path -Append
"------------------------------" | out-file $path -Append

"Server 	 	- $server
Database 	- $database
Username 	- $Username" | out-file $path -Append

cls

#Run each folders

$TotalFiles = 0

#ForEach ($folder in $folders)
    #{
       #$files = Get-ChildItem $folder.FullName  *.sql
        $files = Get-ChildItem $CurrentDir.FullName  *.sql

       $TotalFiles = $TotalFiles + $files.Count
    #}


if( $TotalFiles -ge 1 )
{
    "------------------------------" | out-file $path -Append
    "EXECUTION DETAILS" | out-file $path -Append
    "------------------------------" | out-file $path -Append
}

$FileCounter = 0

#ForEach ($folder in $folders)
   # {
        If ( $errorCount -ge 1 )
           { break }


                #$files = Get-ChildItem $folder.FullName  *.sql
                $files = Get-ChildItem $CurrentDir.FullName  *.sql

                ForEach ($file in $files)
                    {
                        If ( $errorCount -ge 1 )
                            { break }

                        If ( $file -Like "*.sql")
                            {
                                try
                                    {

                                        $FileCounter = $FileCounter + 1
										$FilePath = Get-ChildItem $file.FullName

                                        Write-Host "Executing file ($FileCounter/$TotalFiles) - $FilePath"
                                        If ($FileCounter -eq 1)
                                        {
                                            $cc = Get-Content -Path $FilePath
                                            $cc.Replace('{{DBNAME}}',$database) | out-file $FilePath
                                            Invoke-SQLCMD -Inputfile $FilePath -serverinstance $server -database 'master' -Username $Username -Password $Password -QueryTimeout 3600 -ErrorAction Stop
                                            $cc.Replace($database,'{{DBNAME}}') | out-file $FilePath
                                        }
                                        else
                                        {

                                        Invoke-SQLCMD -Inputfile $FilePath -serverinstance $server -database $database -Username $Username -Password $Password -QueryTimeout 3600 -ErrorAction Stop
                                        }

                                        "File (" + $FileCounter + "/" + $TotalFiles + "): $FilePath" | out-file $path -Append
                                        "------------------------------------------------------------" | out-file $path -Append
                                        "-- Command executed successfully on $date " | out-file $path -Append
                                        " " | out-file $path -Append

                                        $successCount = $successCount + 1
                                    }
                                Catch
                                    {
                                        $ErrorMessage = $_.Exception.Message

                                        Write-Host "...ERROR in File: $ErrorMessage"

                                        "File : $FilePath" | out-file $path -Append
                                        "------------------------------------------------------------" | out-file $path -Append
                                        "-- Command executed with error on $date -$ErrorMessage " | out-file $path -Append
                                        " " | out-file $path -Append

                                        $errorCount = $errorCount + 1
                                        #throw "ERROR"
                                        break
                                    }
                            }
                    }

    #}

$PendingScripts = $TotalFiles - $successCount - $errorCount

$Summary = "------------------------------
SUMMARY
------------------------------
Total Scripts 		= " + $TotalFiles + "
Successful Count 	= " + $successCount + "
Failed Count      	= " + $errorCount + "
Pending Scripts 	= " + $PendingScripts

$Summary | out-file $path -Append

Write-Host $Summary
if($errorCount -gt 0)
{ throw "Failed"}
#$ProcessCompleted = Read-host "
#Press any key to exit"

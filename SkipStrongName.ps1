# variables
$Configuration = "Debug"

$SysDirectory = [System.IO.Directory]
$SysPath = [System.IO.Path]
$SysFile = [System.IO.File]

$PROGRAMFILESX86 = [Environment]::GetFolderPath("ProgramFilesX86")
$env:ENLISTMENT_ROOT = Split-Path -Parent $MyInvocation.MyCommand.Definition
$ENLISTMENT_ROOT = Split-Path -Parent $MyInvocation.MyCommand.Definition
$LOGDIR = $ENLISTMENT_ROOT + "\src\\bin"

$MainDll = $ENLISTMENT_ROOT + "\src\bin\" + $Configuration + "\Microsoft.OData.ConnectedService.dll"
$TestDll = $ENLISTMENT_ROOT + "\test\ODataConnectedService.Tests\bin\" + $Configuration + "\ODataConnectedService.Tests.dll"

# Figure out the directory and path for SN.exe
$SN = $null
$SNx64 = $null
$SNVersions = @()
ForEach ($directory in $SysDirectory::EnumerateDirectories($PROGRAMFILESX86 + "\Microsoft SDKs\Windows", "*A"))
{
    # remove the first char 'v'
    $directoryName = $SysPath::GetFileName($directory).substring(1)

    # remove the last char 'A'
    $directoryName = $directoryName.substring(0, $directoryName.LastIndexOf('A'))

    # parse to double "10.0"
    $versionNo = [System.Double]::Parse($directoryName)

    $fileobject = $null
    $fileobject = New-Object System.Object
    $fileobject | Add-Member -type NoteProperty -Name version -Value $versionNo
    $fileobject | Add-Member -type NoteProperty -Name directory -Value $directory

    $SNVersions += $fileobject
}

# Color
$Success = 'Green'
$Warning = 'Yellow'
$Err = 'Red'

# Helper methods for log
Function Error ($msg)
{
    Write-Host "[Error:]" $msg -ForegroundColor $Err
}

Function Warning ($msg)
{
    Write-Host "[Warning:]" $msg -ForegroundColor $Warning
}

Function Success ($msg)
{
    Write-Host "[Success:]" $msg -ForegroundColor $Success
    Write-Host
}

Function Info ($msg)
{
    Write-Host "[Info:]" $msg
}


# using the latest version
$SNVersions = $SNVersions | Sort-Object -Property version -Descending

ForEach ($ver in $SNVersions)
{
    # only care about the folder has "bin" subfolder
    $snBinDirectory = $ver.directory + "\bin"
    if(!$SysDirectory::Exists($snBinDirectory))
    {
        continue
    }

    if($SysFile::Exists($snBinDirectory + "\sn.exe") -and $SysFile::Exists($snBinDirectory + "\x64\sn.exe"))
    {
        $SN = $snBinDirectory + "\sn.exe"
        $SNx64 = $snBinDirectory + "\x64\sn.exe"
        break
    }
    else
    {
        ForEach ($netFxDirectory in $SysDirectory::EnumerateDirectories($snBinDirectory, "NETFX * Tools") | Sort -Descending)
        {
            # currently, sorting descending for the NETFX version looks good.
            if($SysFile::Exists($netFxDirectory + "\sn.exe") -and $SysFile::Exists($netFxDirectory + "\x64\sn.exe"))
            {
                $SN = $netFxDirectory + "\sn.exe"
                $SNx64 = $netFxDirectory + "\x64\sn.exe"
                break
            }
        }
    }
    
    if ($SN -ne $null -and $SNx64 -ne $null)
    {
        break
    }
}

Function GetDlls
{
    $dlls = @()
    
    # ASP.NET Classic/Core Product
    $dlls += $MainDll
    $dlls += $TestDll
    return $dlls
}

Function SkipStrongName
{
    $SnLog = $LOGDIR + "\SkipStrongName.log"
    Out-File $SnLog

    Info('Skip strong name validations for assemblies...')

    $dlls = GetDlls
    ForEach ($dll in $dlls)
    {
        & $SN /Vr $dll | Out-File $SnLog -Append
    }

    ForEach ($dll in $dlls)
    {
        & $SNx64 /Vr $dll | Out-File $SnLog -Append
    }

    Success("SkipStrongName Done")
}

Function DisableSkipStrongName
{
    $SnLog = $LOGDIR + "\DisableSkipStrongName.log"
    Out-File $SnLog

    Info("Disable skip strong name validations for assemblies...")

    $dlls = GetDlls
    ForEach ($dll in $dlls)
    {
        & $SN /Vu $dll | Out-File $SnLog -Append
    }

    ForEach ($dll in $dlls)
    {
        & $SNx64 /Vu $dll | Out-File $SnLog -Append
    }

    Success("DisableSkipStrongName Done")
}

SkipStrongName
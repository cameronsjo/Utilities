# Modified and adapted from http://stackoverflow.com/a/13478773/2607840

function Get-MSBuildPath {
    param([string]$dnv = $null)
    $regKey = "HKLM:\software\Microsoft\MSBuild\ToolsVersions\$dnv"
    $regProperty = "MSBuildToolsPath"
    return Join-Path -Path (Get-ItemProperty $regKey).$regProperty -ChildPath "msbuild.exe"
}
function Get-MSBuild {
    param([string]$dotNetVersion = $null)
    
    # Valid Versions are the following: 2.0, 3.5, 4.0, 12.0 (Visual Studio 2013), 14.0 (Visual Studio 2015)
    $validValues = @("14.0","12.0","4.0","3.5","2.0")
    
    if (![string]::IsNullOrEmpty($dotNetVersion))
    {
        try
        {
            return Get-MSBuildPath $dotNetVersion
        }
        catch
        {
            throw "Unable to find MSBuild with specified version"
        }
    }
    foreach($v in $validValues)
    {
        try
        {
            return Get-MSBuildPath $v
        }
        catch{}
    }
}
function Get-Solutions { Get-ChildItem -Recurse -Include *.sln | % { $_.FullName } }

$msbuild = Get-MSBuild
Get-Solutions | % { &$msbuild /nologo /v:q $_ }

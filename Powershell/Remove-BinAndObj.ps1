# Recursively deletes bin and obj files from the current directory
# This is great for cleaning and fresh-starting from a directory
# http://stackoverflow.com/a/5924807/2607840

if ((Get-Process devenv -ea SilentlyContinue) -ne $null)
{
 echo 'Visual Studio is currently running.'
 echo 'For best results, Visual Studio needs to be closed.'
 echo ''
 
 $response = Read-Host -Prompt 'Would you like to close Visual Studio? (Y/N)'
 
 if ($response.StartsWith('Y', 'CurrentCultureIgnoreCase'))
 {
    Stop-Process -ProcessName 'devenv'
 }
 else
 {
    $reponse = Read-Host -Prompt 'Would you like to continue? (Y/N)'
    if ($response.StartsWith('N', 'CurrentCultureIgnoreCase'))
    {
        return
    }
 } 
}

Get-ChildItem .\ -Include bin,obj -Recurse | foreach ($_) { Remove-Item $_.FullName -Force -Recurse }

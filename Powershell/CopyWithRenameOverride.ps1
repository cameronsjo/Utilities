function CopyWithRename {
    param([string] $Path,
          [string] $Destination)
          
    $Extension = [System.IO.Path]::GetExtension($Path)
    $FileName = [System.IO.Path]::GetFileNameWithoutExtension($Path)
     
    $DestinationFile = [System.IO.Path]::Combine($Destination, [String]::Concat($Filename, $Extension))
     
    If (Test-Path $DestinationFile) {
        $i = 0
        While (Test-Path $DestinationFile) {
            $i += 1
            $DestinationFile = [System.IO.Path]::Combine($Destination, [String]::Concat($Filename, $i, $Extension))
        }
    }
    
    Copy-Item -Path $Path -Destination $DestinationFile
}
$Edmx = Get-ChildItem -Recurse -Include *.edmx
$Edmx | % { CopyWithRename $_.FullName 'C:\Temp\Edmx' }

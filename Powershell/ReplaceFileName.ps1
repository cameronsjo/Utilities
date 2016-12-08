$token = "File"
$value = "Data"

#Long Hand
$files = Get-ChildItem
foreach($file in $files)
{
    $name = $file -replace $token, $value
    Rename-Item $file $name
}

#Short Hand
Get-ChildItem | % { Rename-Item $_ ($_ -Replace $token, $value) }

$find = 'CreateSMTPClient'
$replace = 'CreateSmtpClient'

$files = Get-ChildItem -Recurse -Include *.cs,*.vb | Where-Object {(Select-String -InputObject $_ -Pattern $find -Quiet) -eq $true }
files | % { ((Get-Content $_.FullName) -Replace $find, $replace) | Set-Content $_.FullName -Encoding UTF8 }

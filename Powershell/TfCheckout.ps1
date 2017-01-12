$tf = 'C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\tf.exe'
Get-ChildItem -Recurse -Include *.csproj | % { & $tf $_.FullName }

Get-ChildItem -r -Include app.config | % { Copy-Item $_.FullName ("C:\Temp\Configs\" + $_.Directory.Parent.Name + "-" + $_.Name) }

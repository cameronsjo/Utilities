# Recursively deletes bin and obj files from the current directory
# This is great for cleaning and fresh-starting from a directory
# http://stackoverflow.com/a/5924807/2607840
Get-ChildItem .\ -Include bin,obj -Recurse | foreach ($_) { Remove-Item $_.FullName -Force -Recurse }

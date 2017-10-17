# Recursively deletes all the suo files in the parent folder.
# Suo files are a binary file that's used by Visual Studio to contain preferences.
# Usually when Visual Studio gets janky or is jacked up the first step is to remove all the suo files.
#
# As always, you'll need to make sure that Visual Studio is closed before running this otherwise
# it'll probably have a lock on the file and make powershell blow out.

Get-ChildItem -Force -Recurse -Include *.suo | ForEach ($_) { Remove-Item $_.FullName -Force }

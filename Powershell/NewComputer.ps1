# First, install chocolatey in an escalated powershell

Set-ExecutionPolicy Bypass -Scope Process -Force; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))

# Probably need to make a new (escalated) powershell instance cause chocolatey gonna add some environment variables.

# Next, install stuff

# Mandatory stuff
choco install -y 7zip.install  googlechrome  dropbox 1password

# Media / Productivity
choco install -y itunes  vlc  malwarebyte  greenshot

# Dev stuff
choco install git openssl.light vscode linqpad6.anycpu.portable

# Gaming stuff
choco install steam battle.net origin discord

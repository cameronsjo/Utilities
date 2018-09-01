# First, install chocolatey in an escalated powershell

Set-ExecutionPolicy Bypass -Scope Process -Force; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))

# Probably need to make a new (escalated) powershell instance cause chocolatey gonna add some environment variables.

# Next, install stuff

# Mandatory stuff
choco install 7zip.install
choco install googlechrome
choco install dropbox
choco install 1password

# Media / Productivity
choco install itunes
choco install vlc
choco install malwarebytes
choco install greenshot

# Dev stuff
choco install nodejs.install git openssl.light
choco install vscode linqpad5.anycpu.portable
choco install visualstudio2017community

# Gaming stuff
choco install steam battle.net origin discord

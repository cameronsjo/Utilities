# Could use lowercase "d" in delete for a "safe delete"
# https://stackoverflow.com/a/57314194/2607840
git branch --format '%(refname:lstrip=2)' | Where-Object { $_ -ne 'master' } | % { git branch -D $_ }

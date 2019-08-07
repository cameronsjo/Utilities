# Could use lowercase "d" in delete for a "safe delete"
# https://stackoverflow.com/a/57314194/2607840

# Remove local branches
git branch --format '%(refname:lstrip=2)' `
  | Where-Object { $_ -ne 'master' } `
  | % { git branch -D $_ }


# Remove remote branches
git branch --format  '%(refname:lstrip=2)' -r `
  | Where-Object { $_.StartsWith('origin') -And !($_.Contains('master')) } `
  | % { git push origin --delete $_.Substring(7) }

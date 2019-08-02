$dir = $pwd

Get-ChildItem  -dir | % {
  cd $_.FullName
  
  if ((Test-Path .\.git) -eq $true)
  {
    $branch = git rev-parse --abbrev-ref HEAD
    git reset --hard origin/$branch
  } else {
    Write-Host "Directory is not a git repository"
  }
}

cd $dir

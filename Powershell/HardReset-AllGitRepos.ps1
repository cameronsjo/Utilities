# Save off current directory, because git is funky.
$dir = $pwd

# List off every child directory. (Depth = 1)
Get-ChildItem  -dir | % {

  # Navigate to directory
  cd $_.FullName
  
  # If the directory isn't a git repo, then we'll skip it.
  if ((Test-Path .\.git) -eq $true)
  {
    # Get current branch
    $branch = git rev-parse --abbrev-ref HEAD
    
    # Hard reset to origin of branch, because we broked it real good.
    git reset --hard origin/$branch
  } else {
    Write-Host "Directory is not a git repository"
  }
}

# Navigate back to original directory. 
cd $dir

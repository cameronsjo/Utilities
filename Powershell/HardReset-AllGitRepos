$dir  =  $pwd ; Get-ChildItem  -dir | % {
  cd $_.FullName
  $branch = git rev-parse --abbrev-ref HEAD
  git reset --hard origin/$branch
} ; cd $dir

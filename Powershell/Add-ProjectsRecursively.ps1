$scriptDirectory = (Get-Item $MyInvocation.MyCommand.Path).Directory.FullName
$dteObj = [System.Activator]::CreateInstance([System.Type]::GetTypeFromProgId("VisualStudio.DTE.12.0"))

$slnDir = ".\"
$slnName = "All"

$dteObj.Solution.Create($scriptDirectory, $slnName)
(ls . -Recurse *.csproj) | % { $dteObj.Solution.AddFromFile($_.FullName, $false) }

$dteObj.Solution.SaveAs( (Join-Path $scriptDirectory 'All.sln') ) 

$dteObj.Quit()

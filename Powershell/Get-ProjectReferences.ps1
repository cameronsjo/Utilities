# http://stackoverflow.com/a/10472358/2607840
Function Get-ProjectReferences ($rootFolder)
{
    $projectFiles = Get-ChildItem $rootFolder\* -Include *.csproj, *.vbproj -Recurse
    $ns = @{ defaultNamespace = "http://schemas.microsoft.com/developer/msbuild/2003" }

    $projectFiles | ForEach-Object {
        $projectFile = $_ | Select-Object -ExpandProperty FullName
        $projectName = $_ | Select-Object -ExpandProperty BaseName
        $projectXml = [xml](Get-Content $projectFile)

        $projectReferences = $projectXml | Select-Xml '//defaultNamespace:ProjectReference/defaultNamespace:Name' -Namespace $ns | Select-Object -ExpandProperty Node | Select-Object -ExpandProperty "#text"

        $projectReferences | ForEach-Object {
            "[" + $projectName + "] -> [" + $_ + "]"
        }
    }
}

Get-ProjectReferences $pwd | Out-File references.txt

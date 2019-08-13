function Mark {
    [CmdLetBinding()]
    param(
        [Parameter(Mandatory = $false)]
        [int]
        $Indicator
    )

    $path = $env:USERPROFILE + "\.mark"
    if ($Indicator -gt 0) {
        $path += $Indicator.ToString()
    }

    echo $pwd.Path >> $path
}
function Recall {
    [CmdLetBinding()]
    param(
        [Parameter(Mandatory = $false)]
        [int]
        $Indicator
    )

    $path = $env:USERPROFILE + "\.mark"
    if ($Indicator -gt 0) {
        $path += $Indicator.ToString()
    }

    if (-Not    ( Test-Path $path)) {
        echo "Mark doesn''t exist"
        return
    }

    try {
        Get-Content $path | cd 
        rm $path
    }
    catch { }
}

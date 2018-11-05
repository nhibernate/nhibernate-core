param (
    $TaskList = 'Default',
    [hashtable]$Properties = @{}
)

Install-Module psake -Force -Scope CurrentUser
Import-Module psake
Invoke-psake -buildFile .\psake.ps1 -nologo -taskList $TaskList -properties $Properties
exit ( [int] ( -not $psake.build_success ) )
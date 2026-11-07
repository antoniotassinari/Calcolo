[CmdletBinding()]
param([string]$Version = ((Get-Content "$PSScriptRoot/../version.json" -Raw | ConvertFrom-Json).version), [string]$Compiler)
$ErrorActionPreference='Stop'
if($Version -notmatch '^\d+\.\d+\.\d+$'){throw 'Versione non valida'}
$repo=(Resolve-Path "$PSScriptRoot/..").Path
Push-Location $repo
try {
    dotnet publish apps/STA.Calcolo.Desktop -c Release -r win-x64 --self-contained true "-p:Version=$Version" -p:DebugType=None -p:DebugSymbols=false -o artifacts/publish
    if($LASTEXITCODE -ne 0){throw 'Pubblicazione desktop fallita'}
    dotnet publish apps/STA.Calcolo.Updater -c Release -r win-x64 --self-contained true "-p:Version=$Version" -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -p:DebugType=None -p:DebugSymbols=false -o artifacts/updater
    if($LASTEXITCODE -ne 0){throw 'Pubblicazione aggiornatore fallita'}
    New-Item -ItemType Directory artifacts/publish/Updater -Force | Out-Null
    Copy-Item artifacts/updater/STA.Calcolo.Updater.exe artifacts/publish/Updater/STA.Calcolo.Updater.exe -Force
    Copy-Item deployment/INSTALLAZIONE.md artifacts/publish/INSTALLAZIONE.md -Force
    & "$PSScriptRoot/Build-UpdatePackage.ps1" -Version $Version
    if(!$Compiler){$Compiler='C:\Program Files (x86)\Inno Setup 6\ISCC.exe'}
    if(!(Test-Path -LiteralPath $Compiler)){throw 'Compilatore Inno Setup 6 non trovato; specificare -Compiler'}
    & $Compiler "/DAppVersion=$Version" installer.iss
    if($LASTEXITCODE -ne 0){throw 'Creazione installatore fallita'}
    Get-ChildItem artifacts/installer -File | Where-Object Name -Match ([regex]::Escape("-$Version-")+ 'win-x64\.(exe|zip)$') | ForEach-Object {
        $hash=(Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant()
        "$hash  $($_.Name)"
    } | Set-Content "artifacts/installer/SHA256SUMS-$Version.txt" -Encoding utf8
} finally {Pop-Location}

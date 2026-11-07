param([Parameter(Mandatory)][string]$Version)
$ErrorActionPreference='Stop'
$registry='HKCU:\Software\Microsoft\Windows\CurrentVersion\Uninstall\{9FEC2950-470C-4A70-ADCD-EE58E640A4DD}_is1'
if(Test-Path $registry){throw 'STA Calcolo è già installato: usare una macchina di test isolata'}
$destination=Join-Path ([IO.Path]::GetTempPath()) ('STA-Calcolo-InstallerCheck-'+[guid]::NewGuid().ToString('N'))
$setup=(Resolve-Path "artifacts/installer/STA-Calcolo-Setup-$Version-win-x64.exe").Path
$app=$null
function Install {
    $process=Start-Process -FilePath $setup -ArgumentList @('/VERYSILENT','/SUPPRESSMSGBOXES','/NORESTART','/SP-',('/DIR="'+$destination+'"')) -WindowStyle Hidden -Wait -PassThru
    if($process.ExitCode -ne 0){throw "Installazione fallita: $($process.ExitCode)"}
}
try {
    Install
    $exe=Join-Path $destination 'STA.Calcolo.Desktop.exe'
    if((Get-Item $exe).VersionInfo.FileVersion -ne "$Version.0"){throw 'Versione installata non corrispondente'}
    if(!(Test-Path (Join-Path $destination 'Updater/STA.Calcolo.Updater.exe'))){throw 'Aggiornatore mancante'}
    $personal=Join-Path $destination 'progetto-personale.json';Set-Content -LiteralPath $personal -Value 'documento di controllo'
    $before=(Get-FileHash -LiteralPath $personal).Hash
    Install
    if((Get-FileHash -LiteralPath $personal).Hash -ne $before){throw 'Documento personale modificato'}
    $start=[Diagnostics.ProcessStartInfo]::new($exe);$start.UseShellExecute=$false;$start.WindowStyle=[Diagnostics.ProcessWindowStyle]::Hidden
    $start.Environment['STA_CALCOLO_SKIP_UPDATE_ONCE']='1';$app=[Diagnostics.Process]::Start($start)
    $deadline=[DateTime]::UtcNow.AddSeconds(30)
    do {Start-Sleep -Milliseconds 300;$app.Refresh()} while(!$app.HasExited -and $app.MainWindowHandle -eq 0 -and [DateTime]::UtcNow -lt $deadline)
    if($app.HasExited -or $app.MainWindowHandle -eq 0){throw 'Applicazione installata non avviata'}
    [void]$app.CloseMainWindow();if(!$app.WaitForExit(15000)){throw 'Applicazione non chiusa normalmente'}
    # Exercise the real installed bootstrap and independent updater before uninstalling.
    $automatic=[Diagnostics.ProcessStartInfo]::new($exe);$automatic.UseShellExecute=$false;$automatic.WindowStyle=[Diagnostics.ProcessWindowStyle]::Hidden
    $launcher=[Diagnostics.Process]::Start($automatic)
    $deadline=[DateTime]::UtcNow.AddSeconds(60);$ready=$null
    do {
        Start-Sleep -Milliseconds 500
        $ready=Get-Process -Name 'STA.Calcolo.Desktop' -ErrorAction SilentlyContinue | Where-Object { $_.Id -ne $launcher.Id -and $_.Path -eq $exe -and $_.MainWindowHandle -ne 0 } | Select-Object -First 1
    } while(!$ready -and [DateTime]::UtcNow -lt $deadline)
    if(!$ready){throw 'Il controllo automatico non ha riavviato Calcolo'}
    $app=$ready;[void]$app.CloseMainWindow();if(!$app.WaitForExit(15000)){throw 'Applicazione aggiornata non chiusa'}
    # An interrupted update must be recoverable even if the main application cannot load.
    $cache=Join-Path $env:LOCALAPPDATA 'StudioTassinari/Calcolo/Updates';New-Item -ItemType Directory $cache -Force | Out-Null
    $journal=Join-Path $cache 'pending-update.json';if(Test-Path $journal){throw 'Ripristino già pendente: test interrotto'}
    $backup=Join-Path $cache ('backup-test-'+[guid]::NewGuid().ToString('N'));New-Item -ItemType Directory $backup | Out-Null
    $dll=Join-Path $destination 'STA.Calcolo.Desktop.dll';$dllHash=(Get-FileHash $dll).Hash
    Copy-Item -LiteralPath $dll -Destination (Join-Path $backup 'STA.Calcolo.Desktop.dll')
    @{Root=$destination;Backup=$backup;ServiceWasRunning=$false;Changes=@(@{Path='STA.Calcolo.Desktop.dll';Existed=$true})}|ConvertTo-Json -Depth 5|Set-Content $journal
    Set-Content -LiteralPath $dll -Value 'interrupted write simulation'
    $recovery=Join-Path $destination 'Recovery/STA.Calcolo.Recovery.exe'
    $repair=Start-Process -FilePath $recovery -ArgumentList @('--root',('"'+$destination+'"'),'--parent','0') -WindowStyle Hidden -PassThru
    $deadline=[DateTime]::UtcNow.AddSeconds(60);$ready=$null
    do {Start-Sleep -Milliseconds 500;$ready=Get-Process -Name 'STA.Calcolo.Desktop' -ErrorAction SilentlyContinue | Where-Object {$_.Path -eq $exe -and $_.MainWindowHandle -ne 0}|Select-Object -First 1} while(!$ready -and [DateTime]::UtcNow -lt $deadline)
    if(!$ready -or (Get-FileHash $dll).Hash -ne $dllHash -or (Test-Path $journal)){throw 'Ripristino indipendente fallito'}
    $app=$ready;[void]$app.CloseMainWindow();if(!$app.WaitForExit(15000)){throw 'Applicazione ripristinata non chiusa'}
    if(!$repair.WaitForExit(15000)){throw 'Il ripristino non ha terminato il processo'}
    $uninstall=Start-Process -FilePath (Join-Path $destination 'unins000.exe') -ArgumentList '/VERYSILENT','/SUPPRESSMSGBOXES','/NORESTART' -WindowStyle Hidden -Wait -PassThru
    if($uninstall.ExitCode -ne 0){throw 'Disinstallazione fallita'}
    if(Test-Path $registry){throw 'Registrazione Windows non rimossa'}
    if((Get-FileHash -LiteralPath $personal).Hash -ne $before){throw 'Documento personale rimosso alla disinstallazione'}
    Write-Output 'PASS: installazione, reinstallazione, avvio self-contained, conservazione dati e disinstallazione'
} finally {
    if($app -and !$app.HasExited){[void]$app.CloseMainWindow();[void]$app.WaitForExit(10000)}
    # Leave failure evidence intact. Successful tests remove only their own temporary directory.
    $resolved=[IO.Path]::GetFullPath($destination)
    if(!(Test-Path $registry) -and $resolved.StartsWith([IO.Path]::GetFullPath([IO.Path]::GetTempPath()),[StringComparison]::OrdinalIgnoreCase) -and [IO.Path]::GetFileName($resolved).StartsWith('STA-Calcolo-InstallerCheck-')){
        if(Test-Path -LiteralPath $resolved){Remove-Item -LiteralPath $resolved -Recurse -Force}
    }
}

param([Parameter(Mandatory)][string]$Version,[string]$Publish='artifacts/publish',[string]$Output='artifacts/installer')
$ErrorActionPreference='Stop'
Add-Type -AssemblyName System.IO.Compression
$base=(Resolve-Path -LiteralPath $Publish).Path
New-Item -ItemType Directory $Output -Force | Out-Null
$files=@(Get-ChildItem -LiteralPath $base -Recurse -File | Where-Object Extension -ne '.pdb' | Sort-Object FullName)
$specs=@(foreach($file in $files){@{Path=[IO.Path]::GetRelativePath($base,$file.FullName).Replace('\','/');Length=$file.Length;Sha256=(Get-FileHash -LiteralPath $file.FullName).Hash}})
$manifest=@{SchemaVersion=1;Component='Calcolo';Version=$Version;Files=$specs}|ConvertTo-Json -Depth 5
$destination=Join-Path $Output "STA-Calcolo-Update-$Version-win-x64.zip"
if(Test-Path -LiteralPath $destination){Remove-Item -LiteralPath $destination}
$zip=[IO.Compression.ZipFile]::Open($destination,[IO.Compression.ZipArchiveMode]::Create)
try {
    $entry=$zip.CreateEntry('update-manifest.json');$writer=[IO.StreamWriter]::new($entry.Open(),[Text.UTF8Encoding]::new($false))
    try{$writer.Write($manifest)}finally{$writer.Dispose()}
    foreach($file in $files){[void][IO.Compression.ZipFileExtensions]::CreateEntryFromFile($zip,$file.FullName,'payload/'+[IO.Path]::GetRelativePath($base,$file.FullName).Replace('\','/'),[IO.Compression.CompressionLevel]::Optimal)}
} finally {$zip.Dispose()}
Write-Output "Creato $destination"

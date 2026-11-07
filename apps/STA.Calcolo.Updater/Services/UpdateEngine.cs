using System.IO;
using System.Diagnostics;
using System.Net.Http;
using STA.Calcolo.Updates;
using STA.Calcolo.Updates.Windows;
namespace STA.Calcolo.Updater.Services;
public sealed class UpdateEngine(string root,int parentId,Action<string> progress)
{
    public async Task<bool> RunAsync()
    {
        if(!InstalledApplication.IsInstalled(root))throw new IOException("Installazione Calcolo non trovata o cartella diversa da quella registrata.");
        if(parentId>0)
        {
            try
            {
                using var parent=Process.GetProcessById(parentId);
                if(!string.Equals(parent.MainModule?.FileName,Path.Combine(root,InstalledApplication.ExeName),StringComparison.OrdinalIgnoreCase))throw new IOException("Processo di avvio non riconosciuto.");
                using var timeout=new CancellationTokenSource(TimeSpan.FromSeconds(20));await parent.WaitForExitAsync(timeout.Token);
            }
            catch(ArgumentException) { /* The launcher has already exited. */ }
        }
        using var instance=new Mutex(false,InstalledApplication.MutexName,out var exclusive);
        if(!exclusive)return false; // Never close an open project to update its application.
        var cache=InstalledApplication.Cache;Directory.CreateDirectory(cache);
        using var lease=new FileStream(Path.Combine(cache,"update.lock"),FileMode.OpenOrCreate,FileAccess.ReadWrite,FileShare.None);
        try
        {
            if(FileTransaction.Pending(cache)!=null){progress("Ripristino dell'aggiornamento interrotto…");FileTransaction.Recover(root,cache,()=>{});}
            using var http=new HttpClient {Timeout=TimeSpan.FromMinutes(10)};
            var github=new GitHubReleaseClient(http);
            progress("Controllo aggiornamenti su GitHub…");
            using var checkTimeout=new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var release=await github.LatestAsync(InstalledApplication.VersionAt(root),checkTimeout.Token);
            if(release==null){Status("Versione aggiornata.");return true;}
            var work=Path.Combine(cache,"download-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(work);
            var zip=Path.Combine(work,"update.zip");
            progress("Download della versione "+release.Version+"…");await github.DownloadAsync(release,zip);
            progress("Verifica del pacchetto…");var stage=Path.Combine(work,"stage");
            var manifest=UpdatePackage.Stage(zip,stage,"Calcolo",release.Version);
            foreach(var file in manifest.Files)UpdatePackage.SafePath(root,file.Path);
            var latest=Version.Parse(release.Version+".0");
            if(InstalledApplication.VersionAt(stage)!=latest)throw new InvalidDataException("Il pacchetto contiene una versione diversa da quella dichiarata.");
            progress("Aggiornamento dei file. I progetti personali restano invariati…");
            FileTransaction.Apply(root,stage,manifest,cache,false,()=>
            {
                if(InstalledApplication.VersionAt(root)!=latest)throw new IOException("Verifica della versione installata fallita.");
            },()=>{});
            try {InstalledApplication.UpdateVersion(release.Version);} catch(UnauthorizedAccessException) { /* File version remains authoritative. */ }
            Status("Aggiornato alla versione "+release.Version+".");
            return true;
        }
        catch(Exception ex)
        {
            Status("Aggiornamento non completato: "+ex.Message);
            if(FileTransaction.Pending(cache)!=null)throw new IOException("Ripristino non completato. Riaprire Calcolo per riprovare; i backup sono conservati.",ex);
            return true; // Offline or rejected download: launch the intact installed version.
        }
    }
    private static void Status(string text)=>File.WriteAllText(Path.Combine(InstalledApplication.Cache,"last-status.txt"),text);
}

using System.Net.Http;
using STA.Calcolo.Desktop.Infrastructure;
using STA.Calcolo.Updates;
using STA.Calcolo.Updates.Windows;
namespace STA.Calcolo.Desktop.ViewModels;
public sealed class UpdatesViewModel : ObservableObject
{
    public string VersionText=>"Versione "+typeof(UpdatesViewModel).Assembly.GetName().Version!.ToString(3);
    private string status="Gli aggiornamenti vengono applicati automaticamente all'avvio.";
    public string Status {get=>status;private set=>Set(ref status,value);}
    public AsyncCommand CheckCommand {get;}
    public UpdatesViewModel()
    {
        CheckCommand=new(CheckAsync,e=>Status="Controllo non disponibile: "+e.Message);
        var path=System.IO.Path.Combine(InstalledApplication.Cache,"last-status.txt");
        try {if(System.IO.File.Exists(path))Status=System.IO.File.ReadAllText(path);}catch(System.IO.IOException){}
    }
    private async Task CheckAsync()
    {
        Status="Controllo aggiornamenti…";using var http=new HttpClient{Timeout=TimeSpan.FromSeconds(10)};
        var package=await new GitHubReleaseClient(http).LatestAsync(typeof(UpdatesViewModel).Assembly.GetName().Version!);
        Status=package==null?"Nessun aggiornamento disponibile.":"Disponibile "+package.Version+". Salvare i progetti e riaprire Calcolo per aggiornare.";
    }
}

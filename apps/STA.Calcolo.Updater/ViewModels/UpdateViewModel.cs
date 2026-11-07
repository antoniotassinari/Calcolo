using STA.Calcolo.Desktop.Infrastructure;
using STA.Calcolo.Updater.Services;
namespace STA.Calcolo.Updater.ViewModels;
public sealed class UpdateViewModel : ObservableObject
{
    private string status="Preparazione aggiornamento…";
    public string Status {get=>status;private set=>Set(ref status,value);}
    public bool Working {get;private set;}=true;
    public event Action<bool>? Completed;
    public async Task StartAsync(string root,int parentId)
    {
        var progress=new Progress<string>(text=>Status=text);
        try {var launch=await Task.Run(()=>new UpdateEngine(root,parentId,text=>((IProgress<string>)progress).Report(text)).RunAsync());Working=false;Changed(nameof(Working));Completed?.Invoke(launch);}
        catch(Exception e){Status=e.Message+" Chiudere questa finestra e riprovare dall'applicazione.";}
        finally {Working=false;Changed(nameof(Working));}
    }
}

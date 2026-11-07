using System.IO;
using System.Diagnostics;
using STA.Calcolo.Updates;
using STA.Calcolo.Updates.Windows;
namespace STA.Calcolo.Desktop.Services;
public static class UpdateBootstrap
{
    public static bool TryStart()
    {
        if(Environment.GetEnvironmentVariable("STA_CALCOLO_SKIP_UPDATE_ONCE")=="1") {Environment.SetEnvironmentVariable("STA_CALCOLO_SKIP_UPDATE_ONCE",null);return false;}
        var root=AppContext.BaseDirectory;
        if(!InstalledApplication.IsInstalled(root))return false;
        var source=Path.Combine(root,"Updater",InstalledApplication.UpdaterName);
        if(!File.Exists(source)){if(FileTransaction.Pending(InstalledApplication.Cache)!=null)throw new IOException("Aggiornamento interrotto: reinstallare Calcolo dal setup per ripristinare i file.");return false;}
        try
        {
            var hash=UpdatePackage.Hash(source);
            var runner=Path.Combine(InstalledApplication.Cache,"runner-"+hash[..16]);Directory.CreateDirectory(runner);
            var exe=Path.Combine(runner,InstalledApplication.UpdaterName);
            if(!File.Exists(exe)||UpdatePackage.Hash(exe)!=hash)File.Copy(source,exe,true);
            var start=new ProcessStartInfo(exe){UseShellExecute=false,CreateNoWindow=true,WorkingDirectory=runner};
            start.ArgumentList.Add("--root");start.ArgumentList.Add(root);
            start.ArgumentList.Add("--parent");start.ArgumentList.Add(Environment.ProcessId.ToString());
            return Process.Start(start)!=null;
        }
        catch(Exception ex) when(ex is IOException or UnauthorizedAccessException or System.ComponentModel.Win32Exception) {if(FileTransaction.Pending(InstalledApplication.Cache)!=null)throw new IOException("Ripristino necessario prima di avviare Calcolo.",ex);return false;}
    }
}

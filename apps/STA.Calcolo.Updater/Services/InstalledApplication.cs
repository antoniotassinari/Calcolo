using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
namespace STA.Calcolo.Updates.Windows;
public static class InstalledApplication
{
    public const string AppId="{9FEC2950-470C-4A70-ADCD-EE58E640A4DD}";
    public const string MutexName=@"Local\STA.Calcolo.Desktop";
    public const string ExeName="STA.Calcolo.Desktop.exe";
    public const string UpdaterName="STA.Calcolo.Updater.exe";
    public static string Cache=>Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"StudioTassinari","Calcolo","Updates");
    private const string RegistryPath=@"Software\Microsoft\Windows\CurrentVersion\Uninstall\"+AppId+"_is1";
    public static bool IsInstalled(string root)
    {
        using var key=Registry.CurrentUser.OpenSubKey(RegistryPath);
        return key?.GetValue("InstallLocation") is string location && Path.GetFullPath(location).TrimEnd('\\').Equals(Path.GetFullPath(root).TrimEnd('\\'),StringComparison.OrdinalIgnoreCase)
            && File.Exists(Path.Combine(root,ExeName));
    }
    public static Version VersionAt(string root)=>Version.Parse(FileVersionInfo.GetVersionInfo(Path.Combine(root,ExeName)).FileVersion!);
    public static void UpdateVersion(string version)
    {
        using var key=Registry.CurrentUser.OpenSubKey(RegistryPath,true);
        key?.SetValue("DisplayVersion",version);
    }
    public static void Launch(string root)
    {
        var start=new ProcessStartInfo(Path.Combine(root,ExeName)){UseShellExecute=false,WorkingDirectory=root};
        start.Environment["STA_CALCOLO_SKIP_UPDATE_ONCE"]="1";Process.Start(start);
    }
}

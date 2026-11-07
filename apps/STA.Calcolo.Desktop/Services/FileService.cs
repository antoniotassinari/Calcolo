using Microsoft.Win32;
namespace STA.Calcolo.Desktop.Services;

public sealed class FileService : IFileService
{
    public string? Open(string filter) { var dialog = new OpenFileDialog { Filter = filter }; return dialog.ShowDialog() == true ? dialog.FileName : null; }
    public string? Save(string filter, string name) { var dialog = new SaveFileDialog { Filter = filter, FileName = name, InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) }; return dialog.ShowDialog() == true ? dialog.FileName : null; }
    public string Read(string path) => System.IO.File.ReadAllText(path);
    public void Write(string path, string text) => System.IO.File.WriteAllText(path, text);
}

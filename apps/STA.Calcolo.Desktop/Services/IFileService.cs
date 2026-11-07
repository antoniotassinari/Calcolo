namespace STA.Calcolo.Desktop.Services;

public interface IFileService
{
    string? Open(string filter);
    string? Save(string filter, string name);
    string Read(string path);
    void Write(string path, string text);
}

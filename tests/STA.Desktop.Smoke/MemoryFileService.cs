using STA.Calcolo.Desktop.Services;
internal sealed class MemoryFileService : IFileService
{
    public Dictionary<string, string> Content { get; } = [];
    public string? NextOpen { get; set; }
    public string? NextSave { get; set; }
    public string? Open(string filter) => NextOpen;
    public string? Save(string filter, string name) => NextSave;
    public string Read(string path) => Content.TryGetValue(path, out var text) ? text : System.IO.File.ReadAllText(path);
    public void Write(string path, string text) => Content[path] = text;
}

using System.Text.Json;
namespace STA.Calcolo.Application;

public static class ModuleFiles
{
    private static readonly JsonSerializerOptions Options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true, UnmappedMemberHandling = System.Text.Json.Serialization.JsonUnmappedMemberHandling.Disallow };
    public static T Read<T>(string json) => JsonSerializer.Deserialize<T>(json, Options) ?? throw new ArgumentException("Progetto vuoto.");
    public static string Write<T>(T value) => JsonSerializer.Serialize(value, Options);
}

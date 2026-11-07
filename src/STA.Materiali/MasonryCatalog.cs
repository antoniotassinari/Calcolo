using System.Text.Json;
namespace STA.Materiali;

public static class MasonryCatalog
{
    public static IReadOnlyList<MasonryMaterial> All { get; } = Load();
    private static MasonryMaterial[] Load()
    {
        using var stream = typeof(MasonryCatalog).Assembly.GetManifestResourceStream("STA.Materiali.murature.json");
        return JsonSerializer.Deserialize<MasonryMaterial[]>(stream!, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }
    public static double Confidence(string level) => level switch { "LC1" => 1.35, "LC2" => 1.2, "LC3" => 1, _ => throw new ArgumentException("Livello di conoscenza non valido.") };
}

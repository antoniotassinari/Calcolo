using System.Text.Json;
namespace STA.Profilati.Core;
public static class ProfileCatalog {
 public const string Version="oppo-2026-11-06";
 public static IReadOnlyList<SteelProfile> All {get;}=Load();
 private static SteelProfile[] Load() { using var s=typeof(ProfileCatalog).Assembly.GetManifestResourceStream("STA.Profilati.Core.profili.json"); using var d=JsonDocument.Parse(s!); return d.RootElement.GetProperty("profiles").Deserialize<SteelProfile[]>(new JsonSerializerOptions {PropertyNameCaseInsensitive=true})!; }
}
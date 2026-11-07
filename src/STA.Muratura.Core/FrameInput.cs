using System.Text.Json.Serialization;
namespace STA.Muratura.Core;
public sealed record FrameInput
{
 [JsonPropertyName("enabled")] public bool Enabled {get;set;} = false;
 [JsonPropertyName("profile")] public string Profile {get;set;} = "HEA 180";
 [JsonPropertyName("catalogVersion")] public string CatalogVersion {get;set;} = "oppo-2026-11-06";
 [JsonPropertyName("fy")] public double Fy {get;set;} = 275;
 [JsonPropertyName("gamma")] public double Gamma {get;set;} = 1.05;
 [JsonPropertyName("h")] public double H {get;set;} = 150;
 [JsonPropertyName("E")] public double E {get;set;} = 2100000;
 [JsonPropertyName("nProfiles")] public int NProfiles {get;set;} = 1;
 [JsonPropertyName("nColumns")] public int NColumns {get;set;} = 2;
 [JsonPropertyName("portal")] public string Portal {get;set;} = "closed";
}
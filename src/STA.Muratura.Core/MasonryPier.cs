using System.Text.Json.Serialization;
namespace STA.Muratura.Core;
public sealed record MasonryPier
{
 [JsonPropertyName("h")] public double H {get;set;} = 340;
 [JsonPropertyName("l")] public double L {get;set;} = 470;
 [JsonPropertyName("t")] public double T {get;set;} = 20;
 [JsonPropertyName("N")] public double N {get;set;} = 50;
 [JsonPropertyName("q")] public double Q {get;set;} = 0;
 [JsonPropertyName("loadMode")] public string LoadMode {get;set;} = "total";
 [JsonPropertyName("rho")] public double? Rho {get;set;} = 1800;
 [JsonPropertyName("fmBase")] public double FmBase {get;set;} = 260;
 [JsonPropertyName("tauBase")] public double TauBase {get;set;} = 5;
 [JsonPropertyName("E")] public double E {get;set;} = 1500;
 [JsonPropertyName("G")] public double G {get;set;} = 500;
 [JsonPropertyName("lc")] public string Lc {get;set;} = "LC1";
 [JsonPropertyName("materialId")] public string MaterialId {get;set;} = "mattoni-calce";
 [JsonPropertyName("materialVersion")] public int MaterialVersion {get;set;} = 2;
 [JsonPropertyName("type")] public string Type {get;set;} = "Mattoni pieni e malta di calce";
 [JsonPropertyName("cracked")] public bool Cracked {get;set;} = false;
 [JsonPropertyName("c")] public double[] C {get;set;} = [1,1,1,1,1,1,1];
}
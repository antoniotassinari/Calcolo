using System.Text.Json.Serialization;
namespace STA.Muratura.Core;
public sealed record MasonryProject
{
 [JsonPropertyName("version")] public int Version {get;set;} = 2;
 [JsonPropertyName("title")] public string Title {get;set;} = "Confronto maschi murari";
 [JsonPropertyName("before")] public MasonryPier[] Before {get;set;} = [new()];
 [JsonPropertyName("after")] public MasonryPier[] After {get;set;} = [new()];
 [JsonPropertyName("frame")] public FrameInput Frame {get;set;} = new();
}
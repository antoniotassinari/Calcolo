using System.Text.Json.Serialization;
namespace STA.Muratura.Core;
public sealed record MasonryProject
{
 [JsonPropertyName("equalHeights")] public Dictionary<string,bool> EqualHeights {get;set;} = new(){["before"]=false,["after"]=false};
 [JsonPropertyName("version")] public int Version {get;set;} = 2;
 [JsonPropertyName("title")] public string Title {get;set;} = "Confronto maschi murari";
 [JsonPropertyName("before")] public MasonryPier[] Before {get;set;} = [new()];
 [JsonPropertyName("after")] public MasonryPier[] After {get;set;} = [new()];
 [JsonPropertyName("frame")] public FrameInput Frame {get;set;} = new();
 [JsonPropertyName("additionalFrames")] public FrameInput[] AdditionalFrames {get;set;} = [];
 [JsonIgnore] public IEnumerable<FrameInput> AllFrames => new[]{Frame}.Concat(AdditionalFrames);
}

using System.Text.Json;
using System.Text.Json.Nodes;
using STA.Materiali;
namespace STA.Muratura.Core;
public static class MasonryFiles
{
    private static readonly JsonSerializerOptions Options=new(){WriteIndented=true};
    public static MasonryProject Read(string text)
    {
        var root=JsonNode.Parse(text)?.AsObject() ?? throw new ArgumentException("Progetto vuoto.");
        int version=root["version"]?.GetValue<int>()??1;
        if (version is not (1 or 2)) throw new ArgumentException("Versione progetto non riconosciuta.");
        foreach(var side in new[]{"before","after"})
        {
            var rows=root[side]?.AsArray() ?? throw new ArgumentException("Stato mancante: "+side);
            foreach(var row in rows)
            {
                var p=row?.AsObject() ?? throw new ArgumentException("Maschio nullo.");
                foreach(var key in new[]{"h","l","t","rho","E","G","lc","c"}) if(p[key] is null) throw new ArgumentException("Dato mancante: "+key);
                var load=p["loadMode"]?.GetValue<string>()??"total";
                if(p[load=="linear"?"q":"N"] is null) throw new ArgumentException("Carico mancante.");
                if(p["materialVersion"]?.GetValue<int>()!=2)
                {
                    double fc=MasonryCatalog.Confidence(p["lc"]!.GetValue<string>());
                    p["fmBase"]=(p["fm"]?.GetValue<double>()??throw new ArgumentException("fm mancante."))*fc;
                    p["tauBase"]=(p["tau"]?.GetValue<double>()??throw new ArgumentException("tau mancante."))*fc;
                    p["materialId"]="custom";p["materialVersion"]=2;p.Remove("fm");p.Remove("tau");
                }
                if(p["fmBase"] is null||p["tauBase"] is null)throw new ArgumentException("Resistenze mancanti.");
            }
        }
        if(root["frame"] is JsonObject frame && frame["enabled"]?.GetValue<bool>()==true)
            foreach(var key in new[]{"profile","catalogVersion","fy","gamma","h","E","nProfiles","nColumns","portal"})
                if(frame[key] is null)throw new ArgumentException("Dato cerchiatura mancante: "+key);
        root["version"]=2;
        var project=root.Deserialize<MasonryProject>(Options) ?? throw new ArgumentException("Progetto non valido.");
        Validate(project);return project;
    }
    public static void Validate(MasonryProject p)
    {
        if(p.Version!=2||p.Frame is null||string.IsNullOrWhiteSpace(p.Title))throw new ArgumentException("Progetto non valido.");
        var before=MasonryCalculator.Total(p.Before);var after=MasonryCalculator.Total(p.After);FrameCalculator.Calculate(p.Frame,before,after);
    }
    public static string Write(MasonryProject p){Validate(p);return JsonSerializer.Serialize(p,Options);}
    public static MasonryProject Example(){using var s=typeof(MasonryFiles).Assembly.GetManifestResourceStream("STA.Muratura.Core.esempio.json");using var reader=new StreamReader(s!);return Read(reader.ReadToEnd());}
}

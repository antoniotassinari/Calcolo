using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using STA.Calcolo.Contracts;
using STA.Plinti.Core;
namespace STA.Calcolo.IO;
public static class CalculationFiles
{
    public static JsonSerializerOptions InputOptions { get; } = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow };
    private static readonly JsonSerializerOptions OutputOptions = new(InputOptions) { WriteIndented = true, Converters = { new FiniteDoubleConverter() } };
    public static FootingRequest ReadRequest(string json)
    {
        var request = JsonSerializer.Deserialize<FootingRequest>(json, InputOptions) ?? throw new InvalidDataException("Progetto JSON vuoto");
        if (request.SchemaVersion != 1) throw new InvalidDataException("Versione del progetto non supportata");
        ArgumentNullException.ThrowIfNull(request.Parameters);
        FootingCalculator.Validate(request.Parameters); ReactionValidation.Validate(request.Reactions); return request;
    }
    public static string Write<T>(T value) => JsonSerializer.Serialize(value, OutputOptions);
    public static IReadOnlyList<NodeReaction> ReadCsv(string text)
    {
        var lines = text.Split('\n').Select(s => s.Trim()).Where(s => s.Length > 0).ToArray();
        if (lines.Length < 2) throw new InvalidDataException("CSV senza reazioni");
        char sep = lines[0].Contains(';') ? ';' : ',';
        if (string.Join(',', lines[0].Split(sep).Select(s => s.Trim())) != "node,case,FX,FY,FZ,MX,MY,MZ") throw new InvalidDataException("Intestazione richiesta: node,case,FX,FY,FZ,MX,MY,MZ");
        var rows = new List<NodeReaction>();
        foreach (var line in lines.Skip(1))
        {
            var c = line.Split(sep).Select(s => s.Trim()).ToArray();
            if (c.Length != 8 || c.Any(s => s.Contains('"'))) throw new InvalidDataException("Attese 8 colonne senza virgolette; usare il punto decimale");
            double N(int i) => double.Parse(c[i], NumberStyles.Float, CultureInfo.InvariantCulture);
            rows.Add(new() { Node = int.Parse(c[0], CultureInfo.InvariantCulture), Case = c[1], FX = N(2), FY = N(3), FZ = N(4), MX = N(5), MY = N(6), MZ = N(7) });
        }
        ReactionValidation.Validate(rows); return rows;
    }
    // Risultati non applicabili: null. Gli input non finiti sono rifiutati dal motore.
    private sealed class FiniteDoubleConverter : JsonConverter<double>
    {
        public override double Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options) => reader.GetDouble();
        public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
        { if (double.IsFinite(value)) writer.WriteNumberValue(value); else writer.WriteNullValue(); }
    }
}

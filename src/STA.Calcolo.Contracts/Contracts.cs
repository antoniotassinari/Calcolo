using System.Text.Json.Serialization;

namespace STA.Calcolo.Contracts;

/// <summary>Reazioni del vincolo sulla struttura: kN, kN·m; assi globali, Z verso l'alto.</summary>
public sealed record NodeReaction
{
    [JsonPropertyName("node"), JsonRequired] public int Node { get; init; }
    [JsonPropertyName("case"), JsonRequired] public string Case { get; init; } = "";
    [JsonPropertyName("FX"), JsonRequired] public double FX { get; init; }
    [JsonPropertyName("FY"), JsonRequired] public double FY { get; init; }
    [JsonPropertyName("FZ"), JsonRequired] public double FZ { get; init; }
    [JsonPropertyName("MX"), JsonRequired] public double MX { get; init; }
    [JsonPropertyName("MY"), JsonRequired] public double MY { get; init; }
    [JsonPropertyName("MZ"), JsonRequired] public double MZ { get; init; }
}

public sealed record ReactionSelection(IReadOnlyList<int> Nodes, IReadOnlyList<string> Cases);
public sealed record ReactionBatch(IReadOnlyList<NodeReaction> Rows, string Source, DateTimeOffset RetrievedAt,
    string ForceUnit = "kN", string LengthUnit = "m");

public interface IReactionSource
{
    Task<ReactionBatch> ReadAsync(ReactionSelection selection, CancellationToken cancellationToken = default);
}

public sealed record ModuleInfo(string Id, string Name, string Version, string Scope);
public interface ICalculationModule<in TInput, out TOutput>
{
    ModuleInfo Info { get; }
    TOutput Calculate(TInput input);
}

public static class ReactionValidation
{
    public static void ValidateCase(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || System.Text.RegularExpressions.Regex.IsMatch(name,
            @"(?:CB\s*:\s*(?:max|min|all)|envelope|invilupp|\b(?:max|min)\b|\(RS\)|\(MV\)|\(SM\)|\(TH\))",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            throw new ArgumentException($"Combinazione non concomitante o non supportata: {name}");
    }

    public static void Validate(IReadOnlyList<NodeReaction> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        if (rows.Count == 0) throw new ArgumentException("Nessuna reazione assegnata");
        var seen = new HashSet<(int, string)>();
        foreach (var row in rows)
        {
            if (row.Node <= 0) throw new ArgumentException("ID nodo non valido");
            ValidateCase(row.Case);
            if (!seen.Add((row.Node, row.Case))) throw new ArgumentException("Nodo/combinazione duplicati");
            if (!new[] { row.FX, row.FY, row.FZ, row.MX, row.MY, row.MZ }.All(double.IsFinite))
                throw new ArgumentException("Reazione non finita");
        }
    }
}

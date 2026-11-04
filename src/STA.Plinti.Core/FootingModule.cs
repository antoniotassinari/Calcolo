using System.Text.Json.Serialization;
using STA.Calcolo.Contracts;

namespace STA.Plinti.Core;

public sealed record FootingRequest
{
    [JsonRequired] public int SchemaVersion { get; init; }
    [JsonRequired] public FootingParameters Parameters { get; init; } = null!;
    [JsonRequired] public IReadOnlyList<NodeReaction> Reactions { get; init; } = [];
}

public sealed record FootingReport(int SchemaVersion, string ModuleVersion, string Scope,
    string Units, IReadOnlyList<FootingResult> Results);

public sealed class FootingModule : ICalculationModule<FootingRequest, FootingReport>
{
    public ModuleInfo Info { get; } = new("plinti", "Plinti da reazioni vincolari", "0.2.0",
        "Verifiche parziali recuperate dal prototipo; punzonamento preliminare, esclusi cedimenti e dettagli costruttivi.");

    public FootingReport Calculate(FootingRequest input)
    {
        ArgumentNullException.ThrowIfNull(input);
        if (input.SchemaVersion != 1) throw new ArgumentException("SchemaVersion supportata: 1");
        ArgumentNullException.ThrowIfNull(input.Parameters);
        return new(1, Info.Version, Info.Scope, "kN, m, kPa; materiali MPa; armature mm²/m",
            FootingCalculator.Calculate(input.Reactions, input.Parameters));
    }
}

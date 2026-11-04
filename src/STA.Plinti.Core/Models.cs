using System.Text.Json.Serialization;

namespace STA.Plinti.Core;

public sealed record FootingParameters
{
    [JsonPropertyName("B"), JsonRequired] public double B { get; init; }
    [JsonPropertyName("L"), JsonRequired] public double L { get; init; }
    [JsonPropertyName("h"), JsonRequired] public double H { get; init; }
    [JsonPropertyName("cx"), JsonRequired] public double Cx { get; init; }
    [JsonPropertyName("cy"), JsonRequired] public double Cy { get; init; }
    [JsonPropertyName("qRd"), JsonRequired] public double QRd { get; init; }
    [JsonPropertyName("fck"), JsonRequired] public double Fck { get; init; }
    [JsonPropertyName("fyk"), JsonRequired] public double Fyk { get; init; }
    [JsonPropertyName("gammaC"), JsonRequired] public double GammaC { get; init; }
    [JsonPropertyName("gammaS"), JsonRequired] public double GammaS { get; init; }
    [JsonPropertyName("gammaG"), JsonRequired] public double GammaG { get; init; }
    [JsonPropertyName("beta"), JsonRequired] public double Beta { get; init; }
    [JsonPropertyName("cover"), JsonRequired] public double Cover { get; init; }
    [JsonPropertyName("diameter"), JsonRequired] public double Diameter { get; init; }
    [JsonPropertyName("spacing"), JsonRequired] public double Spacing { get; init; }
    [JsonPropertyName("density"), JsonRequired] public double Density { get; init; }
    [JsonPropertyName("mu"), JsonRequired] public double Mu { get; init; }
    [JsonPropertyName("extraWeight"), JsonRequired] public double ExtraWeight { get; init; }
    [JsonPropertyName("nodeHeight"), JsonRequired] public double NodeHeight { get; init; }
    [JsonPropertyName("alphaCC"), JsonRequired] public double AlphaCC { get; init; }
    [JsonPropertyName("limitState"), JsonRequired] public string LimitState { get; init; } = "";
}

public sealed record FootingResult
{
    [JsonPropertyName("node")] public int Node { get; init; }
    [JsonPropertyName("case")] public string Case { get; init; } = "";
    [JsonPropertyName("B")] public double B { get; init; }
    [JsonPropertyName("L")] public double L { get; init; }
    [JsonPropertyName("h")] public double H { get; init; }
    [JsonPropertyName("N")] public double N { get; init; }
    [JsonPropertyName("H")] public double Horizontal { get; init; }
    [JsonPropertyName("Mx")] public double Mx { get; init; }
    [JsonPropertyName("My")] public double My { get; init; }
    [JsonPropertyName("weight")] public double Weight { get; init; }
    [JsonPropertyName("ex")] public double Ex { get; init; }
    [JsonPropertyName("ey")] public double Ey { get; init; }
    [JsonPropertyName("Be")] public double Be { get; init; }
    [JsonPropertyName("Le")] public double Le { get; init; }
    [JsonPropertyName("qEff")] public double QEff { get; init; }
    [JsonPropertyName("qMin")] public double QMin { get; init; }
    [JsonPropertyName("qMax")] public double QMax { get; init; }
    [JsonPropertyName("sliding")] public double Sliding { get; init; }
    [JsonPropertyName("d")] public double D { get; init; }
    [JsonPropertyName("AsProvided")] public double AsProvided { get; init; }
    [JsonPropertyName("AsMin")] public double AsMin { get; init; }
    [JsonPropertyName("AsX")] public double AsX { get; init; }
    [JsonPropertyName("AsY")] public double AsY { get; init; }
    [JsonPropertyName("mX")] public double MX { get; init; }
    [JsonPropertyName("mY")] public double MY { get; init; }
    [JsonPropertyName("vEdX")] public double VEdX { get; init; }
    [JsonPropertyName("vEdY")] public double VEdY { get; init; }
    [JsonPropertyName("vRdc")] public double VRdc { get; init; }
    [JsonPropertyName("vp")] public double Vp { get; init; }
    [JsonPropertyName("vFace")] public double VFace { get; init; }
    [JsonPropertyName("vMax")] public double VMax { get; init; }
    [JsonPropertyName("betaMin")] public double BetaMin { get; init; }
    [JsonPropertyName("soilOK")] public bool SoilOK { get; init; }
    [JsonPropertyName("concreteOK")] public bool? ConcreteOK { get; init; }
    [JsonPropertyName("ok")] public bool Ok { get; init; }
    [JsonPropertyName("issues")] public IReadOnlyList<string> Issues { get; init; } = [];
}

public sealed record DesignResult([property:JsonPropertyName("node")] int Node, [property:JsonPropertyName("found")] bool Found, [property:JsonPropertyName("results")] IReadOnlyList<FootingResult> Results);

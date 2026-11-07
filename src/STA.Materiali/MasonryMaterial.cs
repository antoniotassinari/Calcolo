using System.Text.Json.Serialization;
namespace STA.Materiali;

/// <summary>Tabella C8.5.I copiata dal modulo originale: MPa e kN/m³.</summary>
public sealed record MasonryMaterial(string Id, string Name, double[] F, double[] Tau, double[] E, double[] G,
    [property: JsonPropertyName("w")] double? Weight, string Note = "", bool Regular = false);

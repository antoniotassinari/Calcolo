using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using STA.Calcolo.Contracts;

namespace STA.Integrations.Midas;

/// <summary>Adattatore in lettura. La chiave rimane in memoria e non entra nei risultati.</summary>
public sealed class MidasReactionSource : IReactionSource, IDisposable
{
    private readonly HttpClient client;
    private readonly Uri baseUri;
    private readonly string key;

    public MidasReactionSource(string baseUrl, string apiKey, HttpMessageHandler? handler = null)
    {
        if (!Uri.TryCreate(baseUrl.TrimEnd('/') + "/", UriKind.Absolute, out var uri) ||
            uri.Scheme != "https" || uri.UserInfo.Length != 0 || uri.Query.Length != 0 || uri.Fragment.Length != 0)
            throw new ArgumentException("Base URL HTTPS non valido");
        if (string.IsNullOrWhiteSpace(apiKey) || apiKey.Contains('\r') || apiKey.Contains('\n'))
            throw new ArgumentException("MAPI-Key non valida");
        baseUri = uri; key = apiKey;
        client = new HttpClient(handler ?? new HttpClientHandler { AllowAutoRedirect = false }) { Timeout = TimeSpan.FromSeconds(60) };
    }

    private async Task<JsonDocument> Request(string path, object? body, CancellationToken ct)
    {
        using var request = new HttpRequestMessage(body is null ? HttpMethod.Get : HttpMethod.Post, new Uri(baseUri, path));
        request.Headers.Add("MAPI-Key", key);
        if (body is not null) request.Content = JsonContent.Create(body, options: new JsonSerializerOptions { PropertyNamingPolicy = null });
        using var response = await client.SendAsync(request, ct);
        if (!response.IsSuccessStatusCode) throw new HttpRequestException($"MIDAS HTTP {(int)response.StatusCode}");
        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
        if (doc.RootElement.ValueKind != JsonValueKind.Object || doc.RootElement.TryGetProperty("error", out _) || doc.RootElement.TryGetProperty("ERROR", out _))
        { doc.Dispose(); throw new InvalidDataException("Risposta MIDAS non valida o contenente un errore"); }
        return doc;
    }

    public async Task<ReactionBatch> ReadAsync(ReactionSelection selection, CancellationToken cancellationToken = default)
    {
        if (selection.Nodes.Count == 0 || selection.Cases.Count == 0 || selection.Nodes.Any(n => n <= 0))
            throw new ArgumentException("Assegnare nodi positivi e combinazioni");
        var nodes = selection.Nodes.Distinct().ToArray(); var cases = selection.Cases.Distinct().ToArray();
        foreach (var name in cases) ReactionValidation.ValidateCase(name);
        using var model = await Request("db/NODE", null, cancellationToken);
        if (!model.RootElement.TryGetProperty("NODE", out var nodeMap)) throw new InvalidDataException("Tabella NODE mancante");
        foreach (var n in nodes) if (!nodeMap.TryGetProperty(n.ToString(CultureInfo.InvariantCulture), out _))
            throw new InvalidDataException($"Nodo {n} assente nel modello");
        var body = new { Argument = new { TABLE_NAME = "STA_REACTIONS", TABLE_TYPE = "REACTIONG",
            UNIT = new { FORCE = "KN", DIST = "M" }, STYLES = new { FORMAT = "Scientific", PLACE = 12 },
            NODE_ELEMS = new { KEYS = nodes }, LOAD_CASE_NAMES = cases } };
        using var result = await Request("post/TABLE", body, cancellationToken);
        var rows = ParseTable(result.RootElement);
        if (rows.Any(r => !nodes.Contains(r.Node) || !cases.Contains(r.Case))) throw new InvalidDataException("Risultati fuori selezione");
        foreach (var n in nodes) foreach (var c in cases)
            if (!rows.Any(r => r.Node == n && r.Case == c)) throw new InvalidDataException($"Risultati mancanti: nodo {n}, caso {c}");
        return new(rows, "MIDAS GEN NX API", DateTimeOffset.UtcNow);
    }

    public static IReadOnlyList<NodeReaction> ParseTable(JsonElement root)
    {
        if (root.ValueKind != JsonValueKind.Object) throw new InvalidDataException("Atteso oggetto MIDAS");
        var tables = root.EnumerateObject().Select(p => p.Value).Where(v => v.ValueKind == JsonValueKind.Object &&
            v.TryGetProperty("HEAD", out _) && v.TryGetProperty("DATA", out _)).ToArray();
        if (tables.Length != 1) throw new InvalidDataException("Attesa una sola tabella HEAD/DATA");
        var table = tables[0];
        var headers = table.GetProperty("HEAD").EnumerateArray().Select(h => Regex.Replace(
            Regex.Replace(h.GetString()!.ToUpperInvariant(), @"\([^)]*\)|\[[^\]]*\]", ""), "[^A-Z0-9]", "")).ToArray();
        int Index(params string[] aliases)
        {
            var hits = Enumerable.Range(0, headers.Length).Where(i => aliases.Contains(headers[i])).ToArray();
            return hits.Length == 1 ? hits[0] : throw new InvalidDataException($"Intestazione {aliases[0]} assente o ambigua");
        }
        int node = Index("NODE", "NODENO", "NODEID"), load = Index("LOAD", "LOADCASE", "LOADCASENAME", "LOADCASECOMB", "LOADCOMB", "LOADCOMBINATION");
        var components = new[] { "FX", "FY", "FZ", "MX", "MY", "MZ" }.Select(c => Index(c)).ToArray();
        var rows = new List<NodeReaction>();
        foreach (var item in table.GetProperty("DATA").EnumerateArray())
        {
            var cells = item.EnumerateArray().ToArray();
            if (cells.Length != headers.Length) throw new InvalidDataException("Riga incoerente con HEAD");
            double Number(int i) => cells[i].ValueKind == JsonValueKind.Number ? cells[i].GetDouble() :
                double.Parse(cells[i].GetString()!, NumberStyles.Float, CultureInfo.InvariantCulture);
            var id = Number(node);
            if (!double.IsFinite(id) || id <= 0 || id > int.MaxValue || id != Math.Truncate(id)) throw new InvalidDataException("Nodo non intero positivo");
            rows.Add(new() { Node = (int)id, Case = cells[load].GetString()!, FX = Number(components[0]), FY = Number(components[1]),
                FZ = Number(components[2]), MX = Number(components[3]), MY = Number(components[4]), MZ = Number(components[5]) });
        }
        ReactionValidation.Validate(rows);
        return rows;
    }

    public void Dispose() => client.Dispose();
}

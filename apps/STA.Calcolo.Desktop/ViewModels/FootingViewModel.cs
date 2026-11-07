using System.Globalization;
using System.Text.Json;
using STA.Calcolo.Contracts;
using STA.Calcolo.IO;
using STA.Plinti.Core;
using STA.Integrations.Midas;
using STA.Calcolo.Desktop.Infrastructure;
using STA.Calcolo.Desktop.Services;
namespace STA.Calcolo.Desktop.ViewModels;

public sealed class FootingViewModel : ObservableObject
{
    private readonly IFileService files;
    private FootingRequest? lastInput;
    private FootingReport? report;
    private long revision;
    private string status = "Aprire un progetto oppure caricare l'esempio.";
    public string Status { get => status; private set => Set(ref status, value); }
    public IReadOnlyList<ParameterRow> Parameters { get; private set; } = [];
    public IReadOnlyList<NodeReaction> Reactions { get; private set; } = [];
    public object[] Results { get; private set; } = [];
    public string BaseUrl { get; set; } = "";
    public string ApiKey { get; set; } = "";
    public string NodeIds { get; set; } = "";
    public string Cases { get; set; } = "";
    public bool AnalysisReady { get; set; }
    public RelayCommand OpenCommand { get; }
    public RelayCommand SaveCommand { get; }
    public RelayCommand ImportCommand { get; }
    public RelayCommand ExampleCommand { get; }
    public RelayCommand CalculateCommand { get; }
    public RelayCommand ExportCommand { get; }
    public AsyncCommand ReadMidasCommand { get; }
    private static readonly Dictionary<string, string> Labels = new()
    {
        ["B"] = "Base X [m]",
        ["L"] = "Base Y [m]",
        ["h"] = "Spessore [m]",
        ["cx"] = "Pilastro X [m]",
        ["cy"] = "Pilastro Y [m]",
        ["qRd"] = "Pressione limite terreno [kPa]",
        ["fck"] = "fck [MPa]",
        ["fyk"] = "fyk [MPa]",
        ["gammaC"] = "γc",
        ["gammaS"] = "γs",
        ["gammaG"] = "γ pesi aggiuntivi",
        ["beta"] = "β punzonamento",
        ["cover"] = "Copriferro [mm]",
        ["diameter"] = "Diametro [mm]",
        ["spacing"] = "Passo [mm]",
        ["density"] = "Peso unitario cls [kN/m³]",
        ["mu"] = "Attrito di progetto μ",
        ["extraWeight"] = "Altri pesi [kN]",
        ["nodeHeight"] = "Quota nodo sopra base [m]",
        ["alphaCC"] = "αcc",
        ["limitState"] = "Stato limite ULS / SLS"
    };

    public FootingViewModel(IFileService files)
    {
        this.files = files;
        OpenCommand = new(() => Run(() => { var path = files.Open("Progetto JSON|*.json"); if (path != null) SetProject(CalculationFiles.ReadRequest(files.Read(path))); }));
        SaveCommand = new(() => Run(() => { var input = GetProject(); _ = new FootingModule().Calculate(input); var path = files.Save("Progetto JSON|*.json", "plinti.json"); if (path != null) files.Write(path, CalculationFiles.Write(input)); }));
        ImportCommand = new(() => Run(() => { var path = files.Open("Reazioni CSV|*.csv"); if (path != null) { var rows = CalculationFiles.ReadCsv(files.Read(path)); Invalidate(); Reactions = rows; Changed(nameof(Reactions)); Status = "Reazioni importate in kN/m."; } }));
        ExampleCommand = new(() => Run(() => SetProject(CalculationFiles.ReadRequest(files.Read(System.IO.Path.Combine(AppContext.BaseDirectory, "plinti-example.json"))))));
        CalculateCommand = new(() => Run(Calculate));
        ExportCommand = new(() => Run(() => { var path = files.Save("Risultati JSON|*.json", "risultati-plinti.json"); if (path != null) files.Write(path, CalculationFiles.Write(new { input = lastInput, report })); }), () => report != null);
        ReadMidasCommand = new(ReadMidas, e => { Invalidate(); Status = e.Message; });
    }
    public void SetProject(FootingRequest request)
    {
        foreach (var row in Parameters) row.PropertyChanged -= Edited;
        Invalidate(); Reactions = request.Reactions;
        Parameters = JsonSerializer.SerializeToElement(request.Parameters).EnumerateObject().Select(p => new ParameterRow { Key = p.Name, Label = Labels.GetValueOrDefault(p.Name, p.Name), Value = p.Value.ToString() }).ToArray();
        foreach (var row in Parameters) row.PropertyChanged += Edited;
        Changed(nameof(Parameters)); Changed(nameof(Reactions)); Status = "Progetto caricato. Pronto per il calcolo.";
    }
    private void Edited(object? sender, System.ComponentModel.PropertyChangedEventArgs e) { Invalidate(); Status = "Dati modificati: ricalcolare."; }
    private void Invalidate() { revision++; report = null; lastInput = null; Results = []; Changed(nameof(Results)); ExportCommand?.Refresh(); }
    private void Run(Action action) { try { action(); } catch (Exception e) { Invalidate(); Status = e.Message; } }
    public FootingRequest GetProject()
    {
        if (Parameters.Count == 0) throw new InvalidOperationException("Aprire un progetto o caricare un esempio.");
        var map = Parameters.ToDictionary(p => p.Key, p => p.Key == "limitState" ? (object)p.Value.Trim() : double.Parse(p.Value.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture));
        return new() { SchemaVersion = 1, Parameters = JsonSerializer.Deserialize<FootingParameters>(JsonSerializer.Serialize(map))!, Reactions = Reactions };
    }
    private void Calculate()
    {
        Invalidate(); var input = GetProject(); var computed = new FootingModule().Calculate(input); lastInput = input; report = computed;
        Results = report.Results.Select(r => (object)new { Nodo = r.Node, Caso = r.Case, r.N, r.QEff, r.QMin, r.QMax, r.Sliding, r.AsX, r.AsY, r.AsProvided, Terreno = r.SoilOK, CA = r.ConcreteOK, EsitoParziale = r.Ok, Note = string.Join("; ", r.Issues) }).ToArray();
        Changed(nameof(Results)); ExportCommand.Refresh(); Status = $"Calcolate {report.Results.Count} sestuple. {report.Scope}";
    }
    private async Task ReadMidas()
    {
        if (!AnalysisReady) throw new InvalidOperationException("Confermare risultati aggiornati e concomitanti.");
        Invalidate(); Reactions = []; Changed(nameof(Reactions)); var started = revision;
        var nodes = NodeIds.Split([' ', ',', ';', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
        var cases = Cases.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim()).ToArray();
        using var source = new MidasReactionSource(BaseUrl, ApiKey); var result = await source.ReadAsync(new(nodes, cases));
        if (started != revision) { Status = "Importazione ignorata: progetto modificato durante la richiesta."; return; }
        Invalidate(); Reactions = result.Rows; Changed(nameof(Reactions)); Status = $"Importate {Reactions.Count} sestuple kN/m. Confrontare con le tabelle di GEN NX.";
    }
}

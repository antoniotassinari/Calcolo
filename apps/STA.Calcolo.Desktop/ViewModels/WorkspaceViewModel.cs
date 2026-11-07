using STA.Calcolo.Application;
using STA.Calcolo.Desktop.Infrastructure;
using STA.Calcolo.Desktop.Services;
namespace STA.Calcolo.Desktop.ViewModels;

public sealed class WorkspaceViewModel : ObservableObject
{
    private readonly ICalculationWorkspace module; private readonly IFileService files;
    public string Title => module.Title; public string Description => module.Description; public string DataHint => module.DataHint; public bool HasData => DataHint.Length > 0;
    public IReadOnlyList<ModuleFieldViewModel> Fields { get; }
    private string data; public string Data { get => data; set { if (Set(ref data, value)) Invalidate(); } }
    private string status = "Modificare i dati e premere Calcola."; public string Status { get => status; private set => Set(ref status, value); }
    public bool HasPlot => module is IPlotWorkspace; public string PlotCaption => (module as IPlotWorkspace)?.PlotCaption ?? ""; public bool EqualAxes => (module as IPlotWorkspace)?.EqualAxes ?? false;
    public IReadOnlyList<PlotSeries> Series { get; private set; } = [];
    public IReadOnlyList<ResultRow> Results { get; private set; } = [];
    public RelayCommand CalculateCommand { get; }
    public RelayCommand OpenCommand { get; }
    public RelayCommand SaveCommand { get; }
    public RelayCommand ExportCommand { get; }
    public RelayCommand ImportDataCommand { get; }
    public WorkspaceViewModel(ICalculationWorkspace module, IFileService files)
    {
        this.module = module; this.files = files; data = module.ExampleData;
        Fields = module.Fields.Select(f => new ModuleFieldViewModel(f)).ToArray(); foreach (var f in Fields) f.PropertyChanged += (_, _) => Invalidate();
        CalculateCommand = new(() => Run(() => { Invalidate(); var computed = module.Calculate(Document()); if (computed.Any(r => !double.IsFinite(r.Valore))) throw new ArgumentException("Il calcolo ha prodotto valori non validi."); Series = module is IPlotWorkspace plot ? plot.Plot(Document(), computed) : []; Changed(nameof(Series)); Results = computed; Changed(nameof(Results)); ExportCommand!.Refresh(); Status = $"Calcolo completato: {Results.Count} risultati."; }));
        OpenCommand = new(() => Run(() => { var path = files.Open("Progetto modulo|*.json"); if (path != null) Load(ModuleFiles.Read<ModuleDocument>(files.Read(path))); }));
        SaveCommand = new(() => Run(() => { var doc = Document(); new ModuleInput(doc, module).Validate(); var path = files.Save("Progetto modulo|*.json", module.Id + ".json"); if (path != null) files.Write(path, ModuleFiles.Write(doc)); }));
        ExportCommand = new(() => Run(() => { var path = files.Save("Risultati JSON|*.json", module.Id + "-risultati.json"); if (path != null) files.Write(path, ModuleFiles.Write(new { Input = Document(), Results })); }), () => Results.Count > 0);
        ImportDataCommand = new(() => Run(() => { var path = files.Open("Dati delimitati da punto e virgola|*.csv;*.txt"); if (path != null) Data = files.Read(path); }), () => HasData);
    }
    public ModuleDocument Document() => new(1, module.Id, Fields.ToDictionary(f => f.Key, f => f.Value), Data);
    public void Load(ModuleDocument doc) { new ModuleInput(doc, module).Validate(); foreach (var f in Fields) f.Value = doc.Fields[f.Key]; Data = doc.Data; Invalidate(); Status = "Progetto caricato."; }
    private void Invalidate() { Series = []; Changed(nameof(Series)); Results = []; Changed(nameof(Results)); ExportCommand?.Refresh(); Status = "Dati modificati: ricalcolare."; }
    private void Run(Action action) { try { action(); } catch (Exception e) { Invalidate(); Status = e.Message; } }
}

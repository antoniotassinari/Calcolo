using System.Globalization;
using System.Text.Json;
using System.Windows;
using Microsoft.Win32;
using STA.Calcolo.Contracts;
using STA.Calcolo.IO;
using STA.Integrations.Midas;
using STA.Plinti.Core;
namespace STA.Calcolo.Desktop;
public partial class MainWindow : Window
{
    public sealed class ParameterRow { public string Key { get; init; } = ""; public string Label { get; init; } = ""; public string Value { get; set; } = ""; }
    private List<ParameterRow> parameters = [];
    private IReadOnlyList<NodeReaction> reactions = [];
    private FootingRequest? lastInput;
    private FootingReport? report;
    private static readonly Dictionary<string,string> Labels = new()
    {
        ["B"]="Base X [m]", ["L"]="Base Y [m]", ["h"]="Spessore [m]", ["cx"]="Pilastro X [m]", ["cy"]="Pilastro Y [m]",
        ["qRd"]="Pressione limite terreno [kPa]", ["fck"]="fck [MPa]", ["fyk"]="fyk [MPa]", ["gammaC"]="γc", ["gammaS"]="γs",
        ["gammaG"]="γ pesi aggiuntivi", ["beta"]="β punzonamento", ["cover"]="Copriferro [mm]", ["diameter"]="Diametro [mm]",
        ["spacing"]="Passo [mm]", ["density"]="Peso unitario cls [kN/m³]", ["mu"]="Attrito di progetto μ", ["extraWeight"]="Altri pesi [kN]",
        ["nodeHeight"]="Quota nodo sopra base [m]", ["alphaCC"]="αcc", ["limitState"]="Stato limite ULS / SLS"
    };
    public MainWindow() { InitializeComponent(); }
    private void FormatResultColumn(object sender, System.Windows.Controls.DataGridAutoGeneratingColumnEventArgs e)
    {
        if (e.PropertyType == typeof(double) && e.Column is System.Windows.Controls.DataGridTextColumn column)
            column.Binding = new System.Windows.Data.Binding(e.PropertyName) { StringFormat = "0.###", ConverterCulture = CultureInfo.GetCultureInfo("it-IT") };
    }
    private void Invalidate() { report = null; lastInput = null; ResultsGrid.ItemsSource = null; ExportButton.IsEnabled = false; }
    private void Edited(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e) { Invalidate(); Status.Text = "Dati modificati: ricalcolare."; }
    private void Run(Action action) { try { action(); } catch (Exception e) { Invalidate(); Status.Text = e.Message; } }
    private void SetProject(FootingRequest request)
    {
        Invalidate(); reactions = request.Reactions;
        parameters = JsonSerializer.SerializeToElement(request.Parameters).EnumerateObject().Select(p => new ParameterRow
        { Key = p.Name, Label = Labels.GetValueOrDefault(p.Name, p.Name), Value = p.Value.ToString() }).ToList();
        ParametersGrid.ItemsSource = parameters; ReactionsGrid.ItemsSource = reactions;
    }
    private FootingRequest GetProject()
    {
        ParametersGrid.CommitEdit(); ParametersGrid.CommitEdit();
        if (parameters.Count == 0) throw new InvalidOperationException("Aprire un progetto o un esempio");
        var map = parameters.ToDictionary(p => p.Key, p => p.Key == "limitState" ? (object)p.Value.Trim() : double.Parse(p.Value, NumberStyles.Float, CultureInfo.InvariantCulture));
        var p = JsonSerializer.Deserialize<FootingParameters>(JsonSerializer.Serialize(map))!;
        return new() { SchemaVersion = 1, Parameters = p, Reactions = reactions };
    }
    private void OpenProject(object sender, RoutedEventArgs e) => Run(() =>
    { var d = new OpenFileDialog { Filter = "Progetto JSON|*.json" }; if (d.ShowDialog() == true) { SetProject(CalculationFiles.ReadRequest(System.IO.File.ReadAllText(d.FileName))); Status.Text = "Progetto caricato."; } });
    private void SaveProject(object sender, RoutedEventArgs e) => Run(() =>
    { var input = GetProject(); _ = new FootingModule().Calculate(input); var d = new SaveFileDialog { Filter = "Progetto JSON|*.json", FileName = "plinti.json" }; if (d.ShowDialog() == true) System.IO.File.WriteAllText(d.FileName, CalculationFiles.Write(input)); });
    private void ImportCsv(object sender, RoutedEventArgs e) => Run(() =>
    { var d = new OpenFileDialog { Filter = "Reazioni CSV|*.csv" }; if (d.ShowDialog() == true) { reactions = CalculationFiles.ReadCsv(System.IO.File.ReadAllText(d.FileName)); Invalidate(); ReactionsGrid.ItemsSource = reactions; Status.Text = "CSV importato: verificare unità kN/m e segni."; } });
    private void LoadExample(object sender, RoutedEventArgs e) => Run(() =>
    { SetProject(CalculationFiles.ReadRequest(System.IO.File.ReadAllText(System.IO.Path.Combine(AppContext.BaseDirectory, "plinti-example.json")))); Status.Text = "Esempio sintetico: sostituire i dati del terreno e le reazioni."; });
    private void Calculate(object sender, RoutedEventArgs e) => Run(() =>
    {
        Invalidate(); lastInput = GetProject(); report = new FootingModule().Calculate(lastInput);
        ResultsGrid.ItemsSource = report.Results.Select(r => new { Nodo = r.Node, Caso = r.Case, r.N, r.QEff, r.QMin, r.QMax, r.Sliding, r.AsX, r.AsY, r.AsProvided,
            Terreno = r.SoilOK, CA = r.ConcreteOK, EsitoParziale = r.Ok, Note = string.Join("; ", r.Issues) }).ToArray();
        ExportButton.IsEnabled = true; Status.Text = $"Calcolate {report.Results.Count} sestuple. {report.Scope}";
    });
    private void Export(object sender, RoutedEventArgs e) => Run(() =>
    { if (report is null) return; var d = new SaveFileDialog { Filter = "Risultati JSON|*.json", FileName = "risultati-plinti.json" }; if (d.ShowDialog() == true) System.IO.File.WriteAllText(d.FileName, CalculationFiles.Write(new { input = lastInput, report })); });
    private async void ReadMidas(object sender, RoutedEventArgs e)
    {
        try
        {
            if (AnalysisReady.IsChecked != true) throw new InvalidOperationException("Confermare risultati aggiornati e concomitanti");
            Invalidate(); reactions = []; ReactionsGrid.ItemsSource = reactions; ReadButton.IsEnabled = false;
            var nodes = NodeIds.Text.Split([' ', ',', ';', '\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
            var cases = Cases.Text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Select(c => c.Trim()).ToArray();
            using var source = new MidasReactionSource(BaseUrl.Text, ApiKey.Password);
            var result = await source.ReadAsync(new(nodes, cases)); reactions = result.Rows; ReactionsGrid.ItemsSource = reactions;
            Invalidate(); Status.Text = $"Importate {reactions.Count} sestuple kN/m. Confrontare con le tabelle di GEN NX.";
        }
        catch (Exception ex) { Status.Text = ex.Message; }
        finally { ReadButton.IsEnabled = true; }
    }
}

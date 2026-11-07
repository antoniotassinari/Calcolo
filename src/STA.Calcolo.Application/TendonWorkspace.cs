using STA.Prefabbricato.Trefolatura;
namespace STA.Calcolo.Application;

public sealed class TendonWorkspace : ICalculationWorkspace, IPlotWorkspace
{
    public string PlotCaption => "Disposizione trefoli · X e Y [cm]";
    public bool EqualAxes => true;
    public IReadOnlyList<PlotSeries> Plot(ModuleDocument document, IReadOnlyList<ResultRow> results) => [new("Trefoli", Build(document).Trefoli.Select(p => new PlotPoint(p.X, p.Y)).ToArray(), Markers: true)];
    public string Id => "trefolatura";
    public string Title => "Trefolature";
    public string Description => "Area, baricentro e azioni iniziali dei trefoli; perdite di precompressione escluse.";
    public IReadOnlyList<ModuleField> Fields => [new("tiro", "Tensione iniziale [daN/cm²]", "14000")];
    public string DataHint => "Una riga per trefolo: X [cm]; Y [cm]; Area [cm²].";
    public string ExampleData => "-10;-20;1.39\n10;-20;1.39";
    public ClasseGestioneTrefolatura Build(ModuleDocument document)
    {
        var i = new ModuleInput(document, this); i.Validate(); var rows = i.Rows(3);
        if (rows.Any(r => r[2] <= 0)) throw new ArgumentException("L'area di ogni trefolo deve essere positiva.");
        var model = new ClasseGestioneTrefolatura() { TiroIniziale = i.Number("tiro", 0), Trefoli = rows.Select((r, n) => new ClasseGestioneTrefolo { ID = n + 1, X = r[0], Y = r[1], Area = r[2] }).ToList() };
        if (!new[] {model.AreaTotale,model.Baricentro.X,model.Baricentro.Y,model.Nfase0,model.MXfase0,model.MYfase0}.All(double.IsFinite)) throw new ArgumentException("Dati trefoli fuori intervallo numerico.");
        return model;
    }
    public IReadOnlyList<ResultRow> Calculate(ModuleDocument document)
    {
        var m = Build(document); return [new("Area totale", m.AreaTotale, "cm²"), new("Baricentro X", m.Baricentro.X, "cm"), new("Baricentro Y", m.Baricentro.Y, "cm"), new("N fase 0", m.Nfase0, "kN"), new("MX fase 0", m.MXfase0, "kN·m"), new("MY fase 0", m.MYfase0, "kN·m")];
    }
}

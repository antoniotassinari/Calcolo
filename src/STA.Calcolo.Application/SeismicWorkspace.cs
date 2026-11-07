using STA.Carichi.Sisma;
namespace STA.Calcolo.Application;

public sealed class SeismicWorkspace : ICalculationWorkspace, IPlotWorkspace
{
    public string PlotCaption => "Spettri · periodo [s], accelerazione [m/s²]";
    public bool EqualAxes => false;
    public IReadOnlyList<PlotSeries> Plot(ModuleDocument document, IReadOnlyList<ResultRow> results) => results.Where(r => r.Grandezza.Contains(" · T=")).GroupBy(r => r.Grandezza.Split(' ')[0]).Select(g => new PlotSeries(g.Key, g.Select(r => new PlotPoint(ModuleInput.Number(r.Grandezza.Split("T=")[1].Split(' ')[0]), r.Valore)).ToArray())).ToArray();
    public string Id => "sisma";
    public string Title => "Spettri sismici";
    public string Description => "Reticolo e formule del progetto storico. Coordinate in gradi decimali; spettri elastici o SLV di progetto con smorzamento 5%.";
    public IReadOnlyList<ModuleField> Fields => [new("lon", "Longitudine [°]", "12.19944"), new("lat", "Latitudine [°]", "44.41778"), new("vita", "Vita nominale [anni]", "50", ["10", "50", "100"]), new("uso", "Classe d'uso", "II", ["I", "II", "III", "IV"]), new("terreno", "Sottosuolo", "A", Enum.GetNames<TipiTerreni>()), new("q", "Fattore di struttura q", "1"), new("modo", "Spettri", "Elastici", ["Elastici", "Progetto SLV"]), new("direzione", "Direzione", "Orizzontale", ["Orizzontale", "Verticale"]), new("periodo", "Periodo massimo visualizzato [s]", "4")];
    public string DataHint => "";
    public string ExampleData => "";
    public IReadOnlyList<ResultRow> Calculate(ModuleDocument document)
    {
        var i = new ModuleInput(document, this); i.Validate();
        var m = new Sisma(i.Number("lon", 6, 19), i.Number("lat", 36, 48));
        m.TipoCostruzione = m.ElencoViteNominali.Single(v => v.Vn == i.Number("vita", 10, 100));
        m.ClasseUso = m.ElencoClassiUso.Single(v => v.ID == i.Text("uso"));
        m.CategoriaSottosuolo = m.ElencoCategorieSottosuolo.Single(v => v.Categoria == i.Choice<TipiTerreni>("terreno"));
        m.FattoreStruttura = i.Number("q", 1, 10); m.creaSpettri();
        var result = new List<ResultRow> { new("Periodo riferimento", m.PeriodoRiferimento, "anni") };
        var maxPeriod = i.Number("periodo", 0.1, 20);
        foreach (var s in i.Text("modo") == "Elastici" ? m.spettri : m.spettriProgetto)
        {
            result.Add(new($"{s.statoLimite} · ag", s.numAgMedia, "m/s²"));
            result.Add(new($"{s.statoLimite} · Tc", s.numTc, "s"));
            result.AddRange(s.valoriSpettrali.Where(v => v.periodo <= maxPeriod).Select(v => new ResultRow($"{s.statoLimite} · T={v.periodo.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture)} s", i.Text("direzione") == "Orizzontale" ? v.risposta : v.rispostaVert, "m/s²")));
        }
        if (result.Any(r => !double.IsFinite(r.Valore))) throw new ArgumentException("Coordinate o parametri non supportati dal reticolo storico.");
        return result;
    }
}

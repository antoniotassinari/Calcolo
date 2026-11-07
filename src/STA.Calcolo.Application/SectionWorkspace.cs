using STA.Geometria;
using STA.Geometria.Masse;
namespace STA.Calcolo.Application;

public sealed class SectionWorkspace : ICalculationWorkspace, IPlotWorkspace
{
    public string PlotCaption => "Sezione · X e Y [cm]";
    public bool EqualAxes => true;
    public IReadOnlyList<PlotSeries> Plot(ModuleDocument document, IReadOnlyList<ResultRow> results) => [new("Contorno", Build(document).Vertici.Select(p => new PlotPoint(p.X, p.Y)).ToArray(), true)];
    public string Id => "sezione";
    public string Title => "Sezioni geometriche";
    public string Description => "Area e baricentro di una sezione poligonale semplice, senza fori. Vertici ordinati lungo il contorno.";
    public IReadOnlyList<ModuleField> Fields => [new("nome", "Nome sezione", "Rettangolo 30 × 50")];
    public string DataHint => "Una riga per vertice: X [cm]; Y [cm]. Non ripetere il primo vertice.";
    public string ExampleData => "0;0\n30;0\n30;50\n0;50";
    public Sezione Build(ModuleDocument document)
    {
        var i = new ModuleInput(document, this); i.Validate(); var rows = i.Rows(2);
        if (rows.Length < 3) throw new ArgumentException("Servono almeno tre vertici.");
        var points = rows.Select((r, n) => new Punto2D { ID = (n + 1).ToString(), X = r[0], Y = r[1] }).ToList();
        // Reject repeated vertices and self intersections before passing to the historical geometry engine.
        if (points.Select(p => (p.X, p.Y)).Distinct().Count() != points.Count) throw new ArgumentException("Vertici duplicati.");
        double Cross(Punto2D a, Punto2D b, Punto2D c) => (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);
        for (int a = 0; a < points.Count; a++) for (int b = a + 1; b < points.Count; b++)
        {
            int an = (a + 1) % points.Count, bn = (b + 1) % points.Count;
            if (an == b || bn == a) continue;
            if (Math.Max(Math.Min(points[a].X, points[an].X), Math.Min(points[b].X, points[bn].X)) <= Math.Min(Math.Max(points[a].X, points[an].X), Math.Max(points[b].X, points[bn].X)) && Math.Max(Math.Min(points[a].Y, points[an].Y), Math.Min(points[b].Y, points[bn].Y)) <= Math.Min(Math.Max(points[a].Y, points[an].Y), Math.Max(points[b].Y, points[bn].Y)) && Cross(points[a], points[an], points[b]) * Cross(points[a], points[an], points[bn]) <= 0 && Cross(points[b], points[bn], points[a]) * Cross(points[b], points[bn], points[an]) <= 0)
                throw new ArgumentException("Contorno non semplice o lati sovrapposti.");
        }
        var model = new Sezione { Nome = i.Text("nome"), Vertici = points }; model.RicalcolaProprietà();
        if (!double.IsFinite(model.Area) || !double.IsFinite(model.Baricentro.X) || !double.IsFinite(model.Baricentro.Y)) throw new ArgumentException("Dimensioni della sezione fuori intervallo numerico.");
        return model;
    }
    public IReadOnlyList<ResultRow> Calculate(ModuleDocument document)
    {
        var m = Build(document); return [new("Area", Math.Abs(m.Area), "cm²"), new("Baricentro X", m.Baricentro.X, "cm"), new("Baricentro Y", m.Baricentro.Y, "cm")];
    }
}

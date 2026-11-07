using STA.Carichi.Neve;
namespace STA.Calcolo.Application;

public sealed class SnowWorkspace : ICalculationWorkspace
{
    public string Id => "neve";
    public string Title => "Carico neve";
    public string Description => "Carico al suolo e coefficiente di forma dalle formule storiche. Copertura a singola falda, Ct = 1; accumuli esclusi.";
    public IReadOnlyList<ModuleField> Fields { get; }
    public SnowWorkspace()
    {
        var model = new CaricoNeve();
        Fields = [new("zona", "Zona", model.ListaZone[0].Nome, model.ListaZone.Select(x => x.Nome).ToArray()), new("topografia", "Topografia", "Normale", model.ListaClassiTopografia.Select(x => x.Topografia).ToArray()), new("quota", "Quota [m]", "100"), new("falda", "Inclinazione falda [°]", "20")];
    }
    public string DataHint => "";
    public string ExampleData => "";
    public IReadOnlyList<ResultRow> Calculate(ModuleDocument document)
    {
        var i = new ModuleInput(document, this); i.Validate(); var m = new CaricoNeve();
        m.ZonaScelta = m.ListaZone.Single(x => x.Nome == i.Text("zona")); m.ClasseTopografiaScelta = m.ListaClassiTopografia.Single(x => x.Topografia == i.Text("topografia"));
        m.AltezzaTopografia = i.Number("quota", 0, 1500); m.InclinazioneFalda = i.Number("falda", 0, 90);
        return [new("qsk", m.Qsk, "kN/m²"), new("μ1", m.CoefficienteM1, ""), new("Ce", m.ClasseTopografiaScelta.Ce, ""), new("qs (Ct = 1)", m.Qsk * m.CoefficienteM1 * m.ClasseTopografiaScelta.Ce, "kN/m²")];
    }
}

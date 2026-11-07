namespace STA.Calcolo.Application;

public sealed class ConcreteWorkspace : ICalculationWorkspace
{
    public string Id => "calcestruzzo";
    public string Title => "Calcestruzzo";
    public string Description => "Proprietà del materiale secondo le formule del progetto storico (NTC 2008).";
    public IReadOnlyList<ModuleField> Fields => [new("classe", "Classe", "C25_30", Enum.GetNames<Calcestruzzo.ClassiNormale>())];
    public string DataHint => "";
    public string ExampleData => "";
    public IReadOnlyList<ResultRow> Calculate(ModuleDocument document)
    {
        var input = new ModuleInput(document, this); input.Validate(); var m = new Calcestruzzo.Calcestruzzo(input.Choice<Calcestruzzo.ClassiNormale>("classe"));
        return [new("Rck", m.Rck, "MPa"), new("fck", m.fck, "MPa"), new("fcd", m.fcd, "MPa"), new("fcm", m.fcm, "MPa"), new("fctm", m.fctm, "MPa"), new("fcfm", m.fcfm, "MPa"), new("Ecm", m.Ecm, "MPa")];
    }
}

namespace STA.Calcolo.Application;

public sealed class SteelWorkspace : ICalculationWorkspace
{
    public string Id => "acciaio";
    public string Title => "Acciaio per armature";
    public string Description => "Proprietà delle classi di acciaio presenti nel progetto originale.";
    public IReadOnlyList<ModuleField> Fields => [new("tipo", "Tipo", "B450C", Enum.GetNames<Acciai.TipoAcciai>())];
    public string DataHint => "";
    public string ExampleData => "";
    public IReadOnlyList<ResultRow> Calculate(ModuleDocument document)
    {
        var input = new ModuleInput(document, this); input.Validate(); var m = new Acciai.AcciaioArmatura(input.Choice<Acciai.TipoAcciai>("tipo"));
        return [new("fy", m.fy, "MPa"), new("ft", m.ft, "MPa"), new("fyd", m.fyd, "MPa")];
    }
}

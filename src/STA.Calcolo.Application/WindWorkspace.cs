namespace STA.Calcolo.Application;

public sealed class WindWorkspace : ICalculationWorkspace
{
    public string Id => "vento";
    public string Title => "Carico vento";
    public string Description => "Pressione di riferimento ed esposizione dalle formule storiche; coefficienti aerodinamici e dinamici della costruzione esclusi.";
    public IReadOnlyList<ModuleField> Fields => [new("zona", "Zona", "Zona2", Enum.GetNames<Vento.Vento.Zone>()), new("rugosita", "Rugosità", "C", Enum.GetNames<Vento.Vento.ClassiRugosità>()), new("distanza", "Distanza dalla costa", "TerraMenoDi10km", Enum.GetNames<Vento.Vento.DistanzeDaCosta>()), new("quota", "Quota sito [m]", "100"), new("altezza", "Altezza struttura [m]", "10")];
    public string DataHint => "";
    public string ExampleData => "";
    public IReadOnlyList<ResultRow> Calculate(ModuleDocument document)
    {
        var i = new ModuleInput(document, this); i.Validate();
        var m = new Vento.Vento(i.Choice<Vento.Vento.Zone>("zona")) { AltezzaGeografica = i.Number("quota", 0, 1500), AltezzaStruttura = i.Number("altezza", 0.01, 200) };
        m.RugositàTerreno = new(i.Choice<Vento.Vento.ClassiRugosità>("rugosita")); m.DistanzaDaCosta = i.Choice<Vento.Vento.DistanzeDaCosta>("distanza");
        if (m.ClasseEsposizione == null) throw new ArgumentException("La combinazione di zona, rugosità e distanza dalla costa non è prevista nelle tabelle del modulo.");
        return [new("Velocità riferimento", m.VelocitàRiferimento, "m/s"), new("Pressione riferimento", m.PressioneRiferimento, "N/m²"), new("Ce massimo", m.CeMax, ""), new("Pressione massima", m.PressioneMassima, "N/m²")];
    }
}

using STA_Dimensionamento_Plinti;
using STA_Dimensionamento_Plinti.pali;
using STA_Dimensionamento_Plinti.verifiche;
using STA.Calcolo.IO;
namespace STA.Calcolo.Application;

public sealed class HistoricalFootingWorkspace(bool project = false) : ICalculationWorkspace
{
    public string Id => project ? "progetto-plinti-storici" : "plinto-storico";
    public string Title => project ? "Progetto plinti storici" : "Plinto singolo storico";
    public string Description => "Confronto con il motore storico: Meyerhof o portata assiale dei pali. Verifiche parziali, esclusi armature, cedimenti e torsione. Ogni nodo usa la geometria impostata.";
    public IReadOnlyList<ModuleField> Fields => [new("tipo", "Tipo plinto", "Superficiale", ["Superficiale", "Pali rettangolare", "Pali triangolare"]), new("a", "Base X [cm]", "300"), new("b", "Base Y [cm]", "300"), new("h", "Spessore [cm]", "50"), new("imposta", "Profondità imposta [cm]", "100"), new("q", "Limite terreno [daN/cm²]", "2.5"), new("ex", "Eccentricità pilastro X [cm]", "0"), new("ey", "Eccentricità pilastro Y [cm]", "0"), new("gamma", "Peso terreno [daN/m³], 0 = escluso", "0"), new("diametro", "Diametro pali [cm]", "60"), new("portata", "Portata palo [kN]", "600"), new("ordini", "Ordini pali (2–20)", "2"), new("colonne", "Colonne pali (2–20)", "2")];
    public string DataHint => "CSV reazioni con intestazione node;case;FX;FY;FZ;MX;MY;MZ. Forze kN, momenti kN·m. FZ positiva in compressione.";
    public string ExampleData => project ? "node;case;FX;FY;FZ;MX;MY;MZ\n1;ULS01;0;0;900;0;0;0\n2;ULS01;0;0;600;20;10;0" : "node;case;FX;FY;FZ;MX;MY;MZ\n1;ULS01;0;0;900;0;0;0";
    public IReadOnlyList<ResultRow> Calculate(ModuleDocument document)
    {
        var i = new ModuleInput(document, this); i.Validate(); var rows = CalculationFiles.ReadCsv(document.Data);
        if (!project && rows.Select(r => r.Node).Distinct().Count() != 1) throw new ArgumentException("Il modulo singolo ammette un solo nodo. Usare Progetto plinti per più nodi.");
        if (rows.Any(r => r.FZ <= 0 || r.MZ != 0)) throw new ArgumentException("Il motore storico richiede compressione positiva e torsione nulla.");
        var result = new List<ResultRow>(); var h = i.Number("h", 1);
        if (i.Text("tipo") == "Superficiale")
        {
            var model = new Plinto(TipoPlinto.Superficiale, i.Number("a", 1), i.Number("b", 1), h, i.Number("imposta", h), i.Number("q", 0.001));
            model.assegnaPosizione(i.Number("ex", -100000), i.Number("ey", -100000));
            var gamma = i.Number("gamma", 0); if (gamma > 0) model.assegnaTerreno(gamma);
            foreach (var row in rows)
            {
                if (row.FX != 0 || row.FY != 0) throw new ArgumentException("Il confronto Meyerhof storico non verifica il taglio; usare Plinti · reazioni e MIDAS.");
                var check = new verificaMeyerhof(new(row.Case, row.FX, row.FY, row.FZ, row.MX, row.MY, row.MZ), model);
                if (model.A <= 2 * check.Ex || model.B <= 2 * check.Ey) throw new ArgumentException($"Nodo {row.Node}: risultante esterna alla base efficace.");
                string label = $"{row.Node} · {row.Case}";
                result.AddRange([new(label + " · pressione", check.Pressione, "daN/cm²"), new(label + " · limite", check.ValoreLimite, "daN/cm²"), new(label + " · utilizzo", check.Pressione / check.ValoreLimite, "")]);
            }
        }
        else
        {
            var model = new PlintoSuPali(); var pile = new Palo(i.Number("diametro", 1), i.Number("portata", 0.001));
            if (i.Text("tipo") == "Pali triangolare") model.generaPlintoTriangolare(pile, h);
            else { var orders = i.Number("ordini", 2, 20); var columns = i.Number("colonne", 2, 20); if (orders != Math.Truncate(orders) || columns != Math.Truncate(columns)) throw new ArgumentException("Ordini e colonne devono essere interi."); model.generaPlintoRettangolare(pile, (int)orders, (int)columns, h); }
            foreach (var row in rows)
            {
                var check = new verificaPlintoSuPali(new(row.Case, row.FX, row.FY, row.FZ, row.MX, row.MY, row.MZ), model);
                string label = $"{row.Node} · {row.Case}";
                result.AddRange([new(label + " · reazione massima", check.ReazioneMassima, "kN"), new(label + " · taglio per palo (non verificato)", check.TaglioSuPalo, "kN"), new(label + " · limite assiale", check.ValoreLimite, "kN"), new(label + " · utilizzo assiale", check.ReazioneMassima / check.ValoreLimite, "")]);
            }
        }
        return result;
    }
}

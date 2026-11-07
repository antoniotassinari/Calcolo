using System.Globalization;
using STA.Calcolo.Application;
using STA_Dimensionamento_Plinti;
using STA_Dimensionamento_Plinti.pali;
using STA_Dimensionamento_Plinti.verifiche;
internal static class ApplicationTests
{
    static ModuleDocument Example(ICalculationWorkspace module) => new(1, module.Id, module.Fields.ToDictionary(f => f.Key, f => f.Default), module.ExampleData);
    static void Assert(bool value) { if (!value) throw new Exception("Application assertion failed"); }
    static void Near(double a, double b) { if (!double.IsFinite(a) || Math.Abs(a - b) > 1e-7) throw new Exception($"{a} != {b}"); }
    static void Reject(Action action) { try { action(); } catch (ArgumentException) { return; } throw new Exception("Expected rejection"); }
    public static void Run(Action<string, Action> test)
    {
        foreach (var module in WorkspaceCatalog.Create()) test("modulo " + module.Id + " indipendente da lingua Windows", () =>
        {
            var saved = CultureInfo.CurrentCulture;
            try { CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US"); var first = module.Calculate(Example(module)); CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("it-IT"); var second = module.Calculate(Example(module)); Assert(first.Count > 0 && first.Count == second.Count); for (int i = 0; i < first.Count; i++) Near(first[i].Valore, second[i].Valore); }
            finally { CultureInfo.CurrentCulture = saved; }
        });
        test("materiali: tutte le classi producono valori finiti", () => { foreach (var module in new ICalculationWorkspace[] { new ConcreteWorkspace(), new SteelWorkspace() }) foreach (var value in module.Fields[0].Choices!) { var doc = Example(module); doc.Fields[module.Fields[0].Key] = value; Assert(module.Calculate(doc).All(r => double.IsFinite(r.Valore) && r.Valore > 0)); } });
        test("sezione: area, baricentro, inversione e contorni non validi", () => { var m = new SectionWorkspace(); var doc = Example(m); var result = m.Calculate(doc); Near(result[0].Valore, 1500); Near(result[1].Valore, 15); Near(result[2].Valore, 25); Near(m.Calculate(doc with { Data = string.Join('\n', doc.Data.Split('\n').Reverse()) })[0].Valore, 1500); Reject(() => m.Calculate(doc with { Data = "0;0\n30;50\n30;0\n0;50" })); Reject(() => m.Calculate(doc with { Data = "0;0\n1;0\n2;0" })); });
        test("trefolatura: equilibrio indipendente e area invalida", () => { var m = new TendonWorkspace(); var doc = Example(m); var r = m.Calculate(doc); Near(r[0].Valore, 2.78); Near(r[3].Valore, 389.2); Near(r[4].Valore, -77.84); Reject(() => m.Calculate(doc with { Data = "0;0;0" })); });
        test("neve: falda oltre 60 gradi", () => { var m = new SnowWorkspace(); var d = Example(m); d.Fields["falda"] = "65"; Near(m.Calculate(d)[3].Valore, 0); d.Fields["quota"] = "NaN"; Reject(() => m.Calculate(d)); });
        test("sisma: nodo esatto del reticolo", () => { var m = new STA.Carichi.Sisma.Sisma(12.19944, 44.41778); var p = m.elencoDistanze[0].puntoEst; var exact = new STA.Carichi.Sisma.Sisma(p.X, p.Y); Assert(exact.spettri.All(s => double.IsFinite(s.numAgMedia))); });
        test("plinti storici: equilibrio centrale e base efficace invalida", () => { var m = new HistoricalFootingWorkspace(); var d = Example(m); Near(m.Calculate(d)[0].Valore, 1.125); Reject(() => m.Calculate(d with { Data = "node;case;FX;FY;FZ;MX;MY;MZ\n1;ULS;0;0;900;2000;2000;0" })); d.Fields["tipo"] = "Pali triangolare"; Near(m.Calculate(d)[0].Valore, 300); d.Fields["tipo"] = "Pali rettangolare"; Near(m.Calculate(d)[0].Valore, 225); });
        test("pali triangolari: massimo su tutti i pali, non ultimo palo", () => { var m = new PlintoSuPali(); m.generaPlintoTriangolare(new Palo(60, 600), 50); var r = new Reazione("ULS", 0, 0, 900, 100, 0, 0); var check = new verificaPlintoSuPali(r, m); var expected = m.Pali.Max(p => 300 + 100 / m.Jx() * Math.Abs(p.Posizione.Y) / 100 * p.Area() / 10000); Near(check.ReazioneMassima, expected); Assert(check.ReazioneMassima > 300 + 100 / m.Jx() * Math.Abs(m.Pali.Last().Posizione.Y) / 100 * m.Pali.Last().Area() / 10000); });
        test("peso terreno sopra plinto esclude lo spessore del calcestruzzo",()=>{var m=new Plinto(TipoPlinto.Superficiale,300,300,50,100,2.5);m.assegnaTerreno(1800);Near(m.PesoTerrenoSopra,8100);});
        test("contratti moduli: versione e campi sconosciuti", () => { var m = new SteelWorkspace(); var d = Example(m); Reject(() => m.Calculate(d with { SchemaVersion = 2 })); Reject(() => m.Calculate(d with { Module = "neve" })); d.Fields["extra"] = "1"; Reject(() => m.Calculate(d)); });
        test("trave: scambio sezioni e trefolature, salvataggio e riapertura", () => { var service = new BeamProjectService(); var project = new BeamProject(1, "T1", [Example(new SectionWorkspace())], [Example(new TendonWorkspace())]); var copy = service.Read(service.Write(project)); var model = service.Build(copy); Near(Math.Abs(model.ElencoSezioni[0].Area), 1500); Near(model.MaschereTrefoli[0].Nfase0, 389.2); Assert(new STA.Prefabbricato.TraveIsostatica.TraveIsostatica().ElencoSezioni.Count == 0); Reject(() => service.ReadModule(ModuleFiles.Write(project.Sections[0]), "trefolatura")); });
    }
}

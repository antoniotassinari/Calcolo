using STA.Materiali;
using STA.Muratura.Core;
namespace STA.Calcolo.Reporting;

public static class MasonryDerivation
{
    public static IReadOnlyList<ReportPage> Build(MasonryProject p)
    {
        MasonryFiles.Validate(p);
        var a=MasonryCalculator.Total(p.Before);var b=MasonryCalculator.Total(p.After);
        var f=FrameCalculator.Calculate(p.Frame,a,b);var c=MasonryReportBuilder.Compare(p);
        string N(double x)=>MasonryReportBuilder.Number(x);
        List<ReportEquation> equations=[
            new("(1) Geometria e carico", "Aᵢ", "lᵢ · tᵢ", "", "A in cm²; l, t, h in cm. Nᵢ = qᵢ · lᵢ / 100 [kN] se si usa q [kN/m]; altrimenti Nᵢ è il carico assegnato."),
            new("(2) Compressione media", "σᵢ", "1000 · Nᵢ", "Aᵢ", "σ in N/cm²; compressione positiva. Il fattore 1000 converte kN in N."),
            new("(3) Resistenza muraria corretta", "τᵢ", "τ₀,ᵢ · Cᵣ,ᵢ", "FCᵢ", "τ in N/cm². Cᵣ è il prodotto dei sette coefficienti; FC = 1,35 (LC1), 1,20 (LC2), 1,00 (LC3). La stessa correzione si applica a fm."),
            new("(4) Moduli corretti", "Eᵢ ; Gᵢ", "E₀,ᵢ · Cₑ,ᵢ · χᵢ ; G₀,ᵢ · Cₑ,ᵢ · χᵢ", "", "Moduli in MPa. Cₑ comprende malta, giunti, nucleo, iniezioni, intonaco. χ = 0,5 se fessurato, altrimenti 1. FC non divide i moduli."),
            new("(5) Rigidezza del maschio", "Kᵢ", "100 · Gᵢ · Aᵢ", "1,2 · hᵢ · [1 + Gᵢ / (1,2 · Eᵢ) · (hᵢ / lᵢ)²]", "K in N/cm. 100 converte MPa in N/cm². L’altezza efficace coincide con h."),
            new("(6) Taglio per fessurazione diagonale", "Vₜ,ᵢ", "Aᵢ · 1,5 · τᵢ · √[1 + σᵢ / (1,5 · τᵢ)]", "1000 · bᵢ", "Vₜ in kN; bᵢ = max[1; min(1,5; hᵢ/lᵢ)]. Il divisore 1000 converte N in kN."),
            new("(7) Spostamento al raggiungimento del taglio", "dᵧ,ᵢ", "10000 · Vₜ,ᵢ", "Kᵢ", "dᵧ in mm. Il fattore 10000 combina kN → N e cm → mm."),
            new("(8) Spostamento ultimo del modello originale", "dᵤ,ᵢ", "0,004 · max[1; min(2; hᵢ/lᵢ − 0,5)] · hᵢ · 10", "", "dᵤ in mm; per lo stato si assume min(dᵤ,ᵢ). Interpolazione del foglio originale, non criterio generale degli Eurocodici.")
        ];
        if(f!=null) equations.AddRange([
            new("(9) Rigidezza della cerchiatura", "Kf", "nₚ · nc · α · Eₛ · 10 · Jₓ", "hf³", "K in N/cm; Eₛ in daN/cm², Jₓ in cm⁴, h in cm; nₚ profili per colonna, nc colonne; α = 12 portale chiuso, 3 aperto.", $"K telaio = {N(f.K)} N/cm"),
            new("(10) Resistenza massima della cerchiatura", "Vf", "2 · nc · fᵧₖ · 10 · (nₚ · Wₓ)", "γM0 · hf · 100", "V in kN; fᵧₖ in MPa, Wₓ in cm³. Modello delle colonne sull’asse forte del foglio originale.", $"V telaio = {N(f.V)} kN"),
            new("(11) Contributo effettivamente mobilitato", "Vf,eff", "min[Vf; Vf · dᵤ,dopo / dᵧ,f]", "", "dᵧ,f = 10000 · Vf / Kf [mm]; dᵤ,dopo è il minimo dei maschi dopo l’intervento.", $"min[{N(f.V)}; {N(f.V)} × {N(f.AvailableDu)} / {N(f.Dy)}] = {N(f.Veff)} kN")
        ]);
        equations.AddRange([
            new("(12) Somma delle rigidezze", "Kfinale", "Σ Kᵢ,dopo + Kf", "", "Kprima = Σ Kᵢ,prima; contributo telaio nullo se disattivato.", $"Prima: {string.Join(" + ",a.Rows.Select(r=>N(r.K)))} = {N(a.K)} N/cm\nFinale: {N(b.K)} + {N(f?.K??0)} = {N(c[0].Finale)} N/cm"),
            new("(13) Somma delle resistenze", "Vfinale", "Σ Vₜ,ᵢ,dopo + Vf,eff", "", "Vprima = Σ Vₜ,ᵢ,prima. Somma dei tagli, non curva a spostamento compatibile.", $"Prima: {string.Join(" + ",a.Rows.Select(r=>N(r.Vt)))} = {N(a.Vt)} kN\nFinale: {N(b.Vt)} + {N(f?.Veff??0)} = {N(c[1].Finale)} kN"),
            new("(14) Variazione della rigidezza", "ΔK [%]", "100 · (Kfinale − Kprima)", "Kprima", "Criterio di confronto adottato dal programma: −15% ≤ ΔK ≤ +15%.", $"100 × ({N(c[0].Finale)} − {N(a.K)}) / {N(a.K)} = {N(c[0].Variazione)} %"),
            new("(15) Variazione della resistenza", "ΔV [%]", "100 · (Vfinale − Vprima)", "Vprima", "Criterio di confronto adottato dal programma: ΔV ≥ 0%.", $"100 × ({N(c[1].Finale)} − {N(a.Vt)}) / {N(a.Vt)} = {N(c[1].Variazione)} %")
        ]);
        return equations.Chunk(3).Select((chunk,i)=>new ReportPage($"Percorso di calcolo · {i+1}",[],[],["Notazione ingegneristica: indice i = singolo maschio; 0 = dato di base; f = telaio. Equazioni del modello implementato, non attestazione di conformità agli Eurocodici. Calcoli a precisione completa; valori mostrati arrotondati."],Equations:chunk)).ToArray();
    }
}

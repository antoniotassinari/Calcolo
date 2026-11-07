using System.Globalization;
using STA.Materiali;
using STA.Muratura.Core;
namespace STA.Calcolo.Reporting;

public static class MasonryReportBuilder
{
    public static string Number(double value)=>value.ToString(Math.Abs(value)>=1e9?"0.###E+0":"N2",CultureInfo.GetCultureInfo("it-IT"));
    public static ComparisonRow[] Compare(MasonryProject p)
    {
        MasonryFiles.Validate(p);var a=MasonryCalculator.Total(p.Before);var b=MasonryCalculator.Total(p.After);var f=FrameCalculator.Calculate(p.Frame,a,b);
        double k=f?.K??0,v=f?.Veff??0;
        return [new("Rigidezza K",a.K,b.K,k,b.K+k,((b.K+k)/a.K-1)*100,"N/cm"),new("Resistenza ΣVt",a.Vt,b.Vt,v,b.Vt+v,((b.Vt+v)/a.Vt-1)*100,"kN")];
    }
    public static PierSummaryRow[] Piers(MasonryProject p)=>new[]{("Prima",p.Before),("Dopo",p.After)}.SelectMany(state=>state.Item2.Select((pier,i)=>{var r=MasonryCalculator.Calculate(pier);return new PierSummaryRow(state.Item1,i+1,r.N,r.Sigma,r.K,r.Vt,r.Dy,r.Du);})).ToArray();
    public static ReportDocument Build(MasonryProject p,DateTime created)
    {
        MasonryFiles.Validate(p);var comparison=Compare(p);var a=MasonryCalculator.Total(p.Before);var b=MasonryCalculator.Total(p.After);var frame=FrameCalculator.Calculate(p.Frame,a,b);
        List<ReportPage> pages=[];
        var totals=new ReportTable("Confronto prima e dopo",["Grandezza","Prima","Dopo muratura","Telaio","Finale","Δ %"],comparison.Select(r=>new[]{r.Grandezza+" ["+r.Unita+"]",Number(r.Prima),Number(r.Dopo),Number(r.Telaio),Number(r.Finale),Number(r.Variazione)}).ToArray());
        var summary=new ReportTable("Esito del confronto",["Criterio","Esito"],[new[]{"Rigidezza finale entro ±15% del prima",FrameCalculator.StiffnessAccepts(comparison[0].Variazione)?"Entro ±15%":"Fuori dall’intervallo ±15%"},new[]{"Resistenza finale non inferiore al prima",comparison[1].Variazione>=0?"Non diminuita":"Diminuita"},new[]{"Contributo della cerchiatura",frame is null?"Escluso dal confronto":"Incluso, limitato dallo spostamento disponibile"}]);
        pages.Add(new("Relazione di confronto dei maschi murari",[totals,summary],[],[
            p.Title,
            $"Il confronto comprende {p.Before.Length} maschi prima e {p.After.Length} maschi dopo l’intervento. I valori finali comprendono il telaio soltanto quando attivato.",
            "La resistenza confrontata è la somma dei contributi a taglio per fessurazione diagonale. Il risultato è un confronto preliminare del modello originale, non una verifica strutturale completa."]));
        pages.AddRange(MasonryDerivation.Build(p));
        foreach(var (name,piers) in new[]{("Prima dell’intervento",p.Before),("Dopo l’intervento",p.After)})
        {
            for(int start=0;start<piers.Length;start+=2)
                pages.Add(new(name+" · schede dei maschi",[],piers.Skip(start).Take(2).Select((pier,i)=>Card(pier,start+i+1)).ToArray(),["Dimensioni in cm. Resistenze murarie in N/cm²; moduli in MPa. Il carico N è positivo a compressione. Peso proprio e combinazioni non aggiunti automaticamente."],name.StartsWith("Dopo")?"135F6E":"192845"));
        }
        var perPier=Piers(p);
        pages.Add(new("Risultati per maschio",[new("Prima dell’intervento",["Maschio","N [kN]","σ [N/cm²]","K [N/cm]","Vt [kN]","dy [mm]","du [mm]"],Rows(perPier.Where(x=>x.Stato=="Prima"))),new("Dopo l’intervento",["Maschio","N [kN]","σ [N/cm²]","K [N/cm]","Vt [kN]","dy [mm]","du [mm]"],Rows(perPier.Where(x=>x.Stato=="Dopo")))],[],["K = rigidezza laterale; Vt = resistenza a taglio; dy = spostamento al raggiungimento di Vt; du = spostamento ultimo del modello Excel. σ è la tensione media di compressione."]));
        if(frame!=null)
        {
            var f=p.Frame;
            pages.Add(new("Cerchiatura in acciaio",[new("Geometria e profilo",["Parametro","Valore"],[new[]{"Profilo selezionato",f.Profile},new[]{"Altezza telaio",Number(f.H)+" cm"},new[]{"Colonne / profili per colonna",f.NColumns+" / "+f.NProfiles},new[]{"Portale",f.Portal=="closed"?"Chiuso":"Aperto"},new[]{"Jx / Wx del singolo profilo",Number(frame.Profile.Jx)+" cm⁴ / "+Number(frame.Profile.Wx)+" cm³"},new[]{"E acciaio / fyk / γM0",Number(f.E)+" daN/cm² / "+Number(f.Fy)+" MPa / "+Number(f.Gamma)}]),
                new("Contributo del telaio",["Grandezza","Valore"],[new[]{"Rigidezza K",Number(frame.K)+" N/cm"},new[]{"Resistenza massima V",Number(frame.V)+" kN"},new[]{"Spostamento dy telaio",Number(frame.Dy)+" mm"},new[]{"Spostamento disponibile du muratura",Number(frame.AvailableDu)+" mm"},new[]{"Resistenza effettiva mobilitata Veff",Number(frame.Veff)+" kN"},new[]{"Resistenza finale muratura + telaio",Number(frame.FinalV)+" kN"},new[]{"Rigidezza finale muratura + telaio",Number(frame.FinalK)+" N/cm"}])],[],["Il confronto finale usa Veff, non automaticamente la resistenza massima del telaio. Catalogo OPPO, copia del 06/11/2026; dati indicativi.","Modello semplificato delle sole colonne sull’asse forte. Non sono verificati traverso, nodi, ancoraggi, instabilità, interazione N-M, taglio e classe della sezione. Il portale modifica K, mentre la formula di V resta quella del foglio originale."]));
        }
        pages.Add(new("Metodo e limiti del confronto",[],[],[
            "L’altezza efficace coincide con l’altezza geometrica. Le resistenze di base sono divise una sola volta per il fattore di confidenza: LC1 = 1,35; LC2 = 1,20; LC3 = 1,00.",
            "I sette coefficienti correttivi moltiplicano le resistenze. Per E e G si applicano malta, giunti, nucleo, iniezioni e intonaco armato. La fessurazione dimezza i moduli una sola volta.",
            "A = l × t; σ = N × 1000 / A; b = max(1; min(1,5; h/l)). Vt = A × 1,5 × τcorr / b × √(1 + σ / (1,5 × τcorr)) / 1000.",
            "K = 100 × Gcorr × A / (1,2 × h) / [1 + Gcorr / (1,2 × Ecorr) × (h/l)²]. dy = Vt × 10000 / K. du conserva l’interpolazione del foglio originale tra 0,4% e 0,8% dell’altezza.",
            "La somma Vt non comprende scorrimento o pressoflessione e non rappresenta una curva di capacità a spostamento compatibile. Gli esiti numerici non costituiscono un’attestazione di conformità dell’intervento.",
            "I dati sono quelli del progetto al momento dell’esportazione. Le modifiche successive in Word non modificano il progetto né ricalcolano i risultati. TA-Calcolo genera Word e PDF dalla stessa rappresentazione dei dati."]));
        return new(p.Title,created,pages);
    }
    private static string[][] Rows(IEnumerable<PierSummaryRow> rows)=>rows.Select(r=>new[]{r.Maschio.ToString(),Number(r.N),Number(r.Sigma),Number(r.K),Number(r.Vt),Number(r.Dy),Number(r.Du)}).ToArray();
    private static ReportCard Card(MasonryPier p,int index)
    {
        var r=MasonryCalculator.Calculate(p);double fc=MasonryCatalog.Confidence(p.Lc),cr=p.C.Aggregate(1d,(x,y)=>x*y),ce=new[]{0,1,4,5,6}.Aggregate(1d,(x,i)=>x*p.C[i])*(p.Cracked?.5:1);
        ReportLine L(string label,double value,string unit="")=>new(label,Number(value)+(unit.Length>0?" "+unit:""));
        string[] names=["Malta buona","Giunti sottili","Ricorsi / listature","Connessione trasversale","Nucleo scadente","Iniezioni","Intonaco armato"];
        return new("Maschio "+index,[
            new("Geometria",[L("Altezza h",p.H,"cm"),L("Lunghezza l",p.L,"cm"),L("Spessore t",p.T,"cm"),L("Area A",r.A,"cm²")]),
            new("Materiale murario",[new("Tipologia",p.Type),new("Conoscenza / FC",p.Lc+" / "+Number(fc)),L("fm prima di FC",p.FmBase,"N/cm²"),L("τ prima di FC",p.TauBase,"N/cm²"),L("E / G non fessurati",p.E,"MPa / "+Number(p.G)+" MPa"),L("Peso specifico",p.Rho!.Value,"daN/m³"),new("Sezione fessurata",p.Cracked?"Sì · E e G × 0,5":"No")]),
            new("Coefficienti correttivi",names.Select((name,i)=>L(name,p.C[i])).ToArray()),
            new("Caratteristiche corrette",[L("fm corretta",p.FmBase/fc*cr,"N/cm²"),L("τ corretta",p.TauBase/fc*cr,"N/cm²"),L("E corretta",p.E*ce,"MPa"),L("G corretta",p.G*ce,"MPa")]),
            new("Carico sul maschio",[new("Modalità",p.LoadMode=="linear"?"Distribuito q":"Risultante N"),p.LoadMode=="linear"?L("q applicato",p.Q,"kN/m"):L("N applicato",p.N,"kN"),L("N risultante",r.N,"kN")]),
            new("Risultati del maschio",[L("Compressione σ",r.Sigma,"N/cm²"),L("Fattore b",r.B),L("Rigidezza K",r.K,"N/cm"),L("Resistenza Vt",r.Vt,"kN"),L("Spostamento dy",r.Dy,"mm"),L("Spostamento du",r.Du,"mm")])]);
    }
}

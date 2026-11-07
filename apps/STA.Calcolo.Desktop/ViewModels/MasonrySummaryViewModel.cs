using STA.Muratura.Core;
namespace STA.Calcolo.Desktop.ViewModels;
public sealed record MasonrySummaryViewModel(string StiffnessDelta,string ResistanceDelta,string StiffnessValues,string ResistanceValues,string StiffnessOutcome,string ResistanceOutcome)
{
    public static MasonrySummaryViewModel Empty {get;}=new("—","—","Completare i dati per il confronto.","Completare i dati per il confronto.","Dati non validi o incompleti","Dati non validi o incompleti");
    public static MasonrySummaryViewModel Create(MasonryProject p)
    {
        var a=MasonryCalculator.Total(p.Before);var b=MasonryCalculator.Total(p.After);
        var f=FrameCalculator.Calculate(p.Frame,a,b);double k=f?.FinalK??b.K,v=f?.FinalV??b.Vt,dk=(k/a.K-1)*100,dv=(v/a.Vt-1)*100;
        string end=f==null?"Dopo":"Dopo + cerchiatura";
        return new($"{dk:N2} %",$"{dv:N2} %",$"Prima {a.K:N2} N/cm\n{end} {k:N2} N/cm",$"Prima {a.Vt:N2} kN\n{end} {v:N2} kN",FrameCalculator.StiffnessAccepts(dk)?"Entro ±15%":"Oltre ±15%",dv>=0?"Non diminuita":"Diminuita");
    }
}

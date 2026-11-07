using STA.Materiali;
namespace STA.Muratura.Core;

public static class MasonryCalculator
{
    public static void Positive(params double[] values) { if (values.Any(x => !double.IsFinite(x) || x <= 0)) throw new ArgumentException("Geometria, materiali e coefficienti devono essere positivi e finiti."); }
    public static MasonryPier Preset(MasonryPier p, string id, string level)
    {
        MasonryCatalog.Confidence(level);
        var m = MasonryCatalog.All.Single(x => x.Id == id);
        double Pick(double[] a) => level == "LC1" ? a[0] : a.Average();
        return p with { MaterialId=id, Type=m.Name, MaterialVersion=2, Lc=level, FmBase=Pick(m.F)*100, TauBase=Pick(m.Tau)*100, E=m.E.Average(), G=m.G.Average(), Rho=m.Weight*100 };
    }
    public static PierResult Calculate(MasonryPier p)
    {
        if (p.MaterialVersion != 2) throw new ArgumentException("Materiale non migrato.");
        Positive(p.H,p.L,p.T,p.Rho ?? double.NaN,p.FmBase,p.TauBase,p.E,p.G);
        if (p.C is null || p.C.Length != 7) throw new ArgumentException("Sono richiesti sette coefficienti."); Positive(p.C);
        double n = p.LoadMode switch { "total" => p.N, "linear" => p.Q*p.L/100, _ => throw new ArgumentException("Carico non valido.") };
        if (!double.IsFinite(n) || n < 0) throw new ArgumentException("Carico di compressione non negativo richiesto.");
        double cr=p.C.Aggregate(1d,(a,b)=>a*b), ce=new[]{0,1,4,5,6}.Aggregate(1d,(a,i)=>a*p.C[i]);
        double tau=p.TauBase/MasonryCatalog.Confidence(p.Lc)*cr, e=p.E*ce*(p.Cracked?.5:1), g=p.G*ce*(p.Cracked?.5:1);
        double a=p.L*p.T, sigma=n*1000/a, b=Math.Clamp(p.H/p.L,1,1.5);
        double k=g*100*a/(1.2*p.H)/(1+g/e/1.2*Math.Pow(p.H/p.L,2)), vt=a*1.5*tau/b*Math.Sqrt(1+sigma/(1.5*tau))/1000;
        double du=.004*Math.Clamp(p.H/p.L-.5,1,2)*p.H*10, dy=vt*10000/k;
        Positive(a,b,k,vt,du,dy); if (!double.IsFinite(sigma)) throw new ArgumentException("Valori fuori intervallo.");
        return new(n,a,sigma,b,k,vt,du,dy);
    }
    public static StateResult Total(MasonryPier[] piers)
    {
        if (piers is null || piers.Length is <1 or >10 || piers.Any(p=>p is null)) throw new ArgumentException("Servono da 1 a 10 maschi per stato.");
        var r=piers.Select(Calculate).ToArray(); var result=new StateResult(r,r.Sum(x=>x.K),r.Sum(x=>x.Vt),r.Min(x=>x.Du),r.Sum(x=>x.N));
        Positive(result.K,result.Vt,result.Du); if (!double.IsFinite(result.N)) throw new ArgumentException("Carico totale fuori intervallo."); return result;
    }
}

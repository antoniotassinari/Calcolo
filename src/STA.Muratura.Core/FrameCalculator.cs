using STA.Profilati.Core;
namespace STA.Muratura.Core;
public static class FrameCalculator
{
    public static bool StiffnessAccepts(double delta) => double.IsFinite(delta) && Math.Abs(delta)<=15+8*2.220446049250313e-16*100;
    public static FrameResult? Calculate(FrameInput f, StateResult before, StateResult after)
    {
        if (!f.Enabled) return null;
        MasonryCalculator.Positive(f.Fy,f.Gamma,f.H,f.E,f.NProfiles,f.NColumns);
        if (f.CatalogVersion!=ProfileCatalog.Version || f.Portal is not ("closed" or "open")) throw new ArgumentException("Catalogo o tipo di portale non valido.");
        var p=ProfileCatalog.All.SingleOrDefault(p=>p.Id==f.Profile) ?? throw new ArgumentException("Profilo non disponibile.");
        double j=p.Jx*f.NProfiles,w=p.Wx*f.NProfiles,v=2d*f.NColumns*f.Fy*10*w/(f.Gamma*f.H)/100;
        double k=(double)f.NProfiles*f.NColumns*(f.Portal=="closed"?12:3)*f.E*10*p.Jx/Math.Pow(f.H,3),dy=v*10000/k;
        double veff=Math.Min(v*after.Du/dy,v),finalV=after.Vt+veff,finalK=after.K+k,dv=(finalV/before.Vt-1)*100,dk=(finalK/before.K-1)*100;
        double? ws=after.Vt>before.Vt?null:.5*Math.Abs(after.Vt-before.Vt)*1000*f.Gamma*f.H/(2*f.Fy*100);
        MasonryCalculator.Positive(j,w,v,k,dy,veff,finalV,finalK);
        if (!double.IsFinite(dv)||!double.IsFinite(dk)||ws is double s&&!double.IsFinite(s)) throw new ArgumentException("Risultati fuori intervallo.");
        return new(p,j,w,v,k,dy,veff,finalV,finalK,dv,dk,ws,after.Du);
    }
    public static ProfileProposal[] Design(FrameInput f, StateResult before, StateResult after)
    {
        if (!f.Enabled) throw new ArgumentException("Attivare la cerchiatura.");
        return new[]{"HEA","HEB","IPE"}.Select(family=>{
            var all=ProfileCatalog.All.Where(p=>p.Family==family).OrderBy(p=>p.Size).Select(p=>Calculate(f with {Profile=p.Id},before,after)!).ToArray();
            var valid=all.Where(r=>StiffnessAccepts(r.DK)&&r.FinalV>before.Vt).ToArray(); return new ProfileProposal(family,valid.FirstOrDefault(),valid.Length,all.Length);
        }).ToArray();
    }
}

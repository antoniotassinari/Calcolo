namespace STA.Plinti.Core;

/// <summary>Motore indipendente da interfaccia, rete e piattaforma, in kN/m/kPa.</summary>
public static class FootingCalculator
{
    /// <summary>Convalida dominio geometrico, materiali e coefficienti.</summary>
    public static void Validate(FootingParameters p)
    {
        foreach (var (name,value) in new (string,double)[] {("B",p.B),("L",p.L),("h",p.H),("cx",p.Cx),("cy",p.Cy),("qRd",p.QRd),("fck",p.Fck),("fyk",p.Fyk),("gammaC",p.GammaC),("gammaS",p.GammaS),("gammaG",p.GammaG),("beta",p.Beta)})
            Require(double.IsFinite(value)&&value>0,$"{name}: valore non valido");
        foreach (var (name,value) in new (string,double)[] {("cover",p.Cover),("diameter",p.Diameter),("spacing",p.Spacing),("density",p.Density),("mu",p.Mu),("extraWeight",p.ExtraWeight),("nodeHeight",p.NodeHeight)})
            Require(double.IsFinite(value)&&value>=0,$"{name}: valore non valido");
        Require(p.Diameter>0&&p.Spacing>0&&p.Beta>=1&&p.GammaC>=1&&p.GammaS>=1,"Diametri, passi o coefficienti non validi");
        Require(p.Cx<p.B&&p.Cy<p.L,"Il pilastro deve essere interno al plinto");
        Require(p.H-p.Cover/1000-1.5*p.Diameter/1000>0,"Altezza utile insufficiente");
        Require(p.LimitState is "ULS" or "SLS","Selezionare SLU o SLE");
        Require(double.IsFinite(p.AlphaCC)&&p.AlphaCC>0&&p.AlphaCC<=1,"alphaCC non valido");
        Require(p.Fck>=12&&p.Fck<=50&&p.Fyk>=400&&p.Fyk<=600,"Campo implementato: fck 12–50 MPa, fyk 400–600 MPa");
    }

    /// <summary>Esclude sestuple di estremi indipendenti e risultati non concomitanti.</summary>
    public static void ValidateCase(string name)
    {
        ReactionValidation.ValidateCase(name);
    }

    /// <summary>Controlla nodi, valori e unicità senza aggregare casi diversi.</summary>
    public static IReadOnlyList<FootingResult> Calculate(IReadOnlyList<NodeReaction> rows,FootingParameters p)
    {
        Validate(p);
        ReactionValidation.Validate(rows);
        return rows.Select(r=>Check(r,p)).ToArray();
    }

    /// <summary>Calcola l'equilibrio e i controlli parziali per una sestupla convalidata.</summary>
    private static FootingResult Check(NodeReaction r,FootingParameters p)
    {
        var weight=p.B*p.L*p.H*p.Density+p.ExtraWeight;
        var n=r.FZ+p.GammaG*weight;
        var mx=-r.MX+p.NodeHeight*r.FY;var my=-r.MY-p.NodeHeight*r.FX;
        var horizontal=Hypot(r.FX,r.FY);
        var ex=n>0?Math.Abs(my/n):double.PositiveInfinity;var ey=n>0?Math.Abs(mx/n):double.PositiveInfinity;
        var be=p.B-2*ex;var le=p.L-2*ey;var area=be>0&&le>0?be*le:0;
        var qEff=n>0&&area>0?n/area:double.PositiveInfinity;
        var mean=n/(p.B*p.L);var delta=6*Math.Abs(my)/(p.L*p.B*p.B)+6*Math.Abs(mx)/(p.B*p.L*p.L);
        var qMax=mean+delta;var qMin=mean-delta;
        var contact=n>0&&qMin>=-1e-8;
        var sliding=n>0?horizontal/(p.Mu*n==0?1e-100:p.Mu*n):double.PositiveInfinity;
        var issues=new List<string>();
        if(n<=0)issues.Add("Sollevamento: risultante verticale non compressiva");
        if(area==0)issues.Add("Risultante esterna alla base efficace");
        if(!contact)issues.Add("Contatto parziale: distribuzione elastica e verifiche CA non applicabili");
        if(Math.Abs(r.MZ)>1e-6)issues.Add("Torsione MZ: verifica non implementata");
        var soilOK=n>0&&area>0&&(p.LimitState=="SLS"?qMax:qEff)<=p.QRd&&contact&&sliding<=1&&Math.Abs(r.MZ)<=1e-6;
        var d=p.H-p.Cover/1000-1.5*p.Diameter/1000;
        var fyd=p.Fyk/p.GammaS;var fcd=p.AlphaCC*p.Fck/p.GammaC;
        var steel=1000/p.Spacing*Math.PI*p.Diameter*p.Diameter/4;
        var fctm=.3*Math.Pow(p.Fck,2d/3);var asMin=Math.Max(.26*fctm/p.Fyk,.0013)*1000*d*1000;
        var ax=(p.B-p.Cx)/2;var ay=(p.L-p.Cy)/2;
        var mX=Math.Max(0,qMax)*ax*ax/2;var mY=Math.Max(0,qMax)*ay*ay/2;
        var asX=Math.Max(asMin,mX*1e6/(.9*d*1000*fyd));var asY=Math.Max(asMin,mY*1e6/(.9*d*1000*fyd));
        var rho=Math.Min(steel/(1000*d*1000),.02);var k=Math.Min(1+Math.Sqrt(200/(d*1000)),2);
        var vRdc=Math.Max(.18/p.GammaC*k*Math.Cbrt(100*rho*p.Fck),.035*Math.Pow(k,1.5)*Math.Sqrt(p.Fck));
        var vEdX=Math.Max(0,qMax)*Math.Max(0,ax-d)/(d*1000);var vEdY=Math.Max(0,qMax)*Math.Max(0,ay-d)/(d*1000);
        var internalPerimeter=ax>=2*d&&ay>=2*d;
        var u1=2*(p.Cx+p.Cy)+4*Math.PI*d;
        var aInside=p.Cx*p.Cy+4*d*(p.Cx+p.Cy)+4*Math.PI*d*d;
        var vp=p.Beta*Math.Max(0,n-Math.Max(0,qMin)*aInside)/(u1*d*1000);
        var betaMin=n>0?1+1.8*Hypot(my/(n*p.Cx),mx/(n*p.Cy)):double.PositiveInfinity;
        var vFace=p.Beta*n/(2*(p.Cx+p.Cy)*d*1000);
        var vMax=.5*.6*(1-p.Fck/250)*fcd;
        var flexLimit=.167*fcd*1000*d*1000*d*1000/1e6;
        if(!internalPerimeter)issues.Add("Perimetro di punzonamento a 2d oltre il bordo: serve verifica specifica");
        if(p.Beta<betaMin)issues.Add("β assegnato inferiore alla stima per eccentricità biassiale del pilastro interno");
        if(Math.Max(mX,mY)>flexLimit)issues.Add("Flessione oltre il campo del braccio interno semplificato z=0,9d");
        var spacingOK=p.Spacing<=Math.Min(3*p.H*1000,400);
        if(!spacingOK)issues.Add("Passo armatura oltre il limite adottato min(3h, 400 mm)");
        var concreteOK=contact&&internalPerimeter&&p.Beta>=betaMin&&steel>=asX&&steel>=asY&&vEdX<=vRdc&&vEdY<=vRdc&&vp<=vRdc&&vFace<=vMax&&Math.Max(mX,mY)<=flexLimit&&spacingOK;
        if(p.LimitState=="SLS")issues.Add("SLE: verificati solo equilibrio e pressione; cedimenti e fessurazione non calcolati");
        return new FootingResult {Node=r.Node,Case=r.Case,B=p.B,L=p.L,H=p.H,N=n,Horizontal=horizontal,Mx=mx,My=my,Weight=weight,Ex=ex,Ey=ey,Be=be,Le=le,QEff=qEff,QMin=qMin,QMax=qMax,Sliding=sliding,D=d,AsProvided=steel,AsMin=asMin,AsX=asX,AsY=asY,MX=mX,MY=mY,VEdX=vEdX,VEdY=vEdY,VRdc=vRdc,Vp=vp,VFace=vFace,VMax=vMax,BetaMin=betaMin,SoilOK=soilOK,ConcreteOK=p.LimitState=="ULS"?concreteOK:null,Ok=soilOK&&(p.LimitState=="SLS"||concreteOK),Issues=issues};
    }

    /// <summary>Trova per ciascun nodo la prima base quadrata valida su una griglia limitata.</summary>
    public static IReadOnlyList<DesignResult> Design(IReadOnlyList<NodeReaction> rows,FootingParameters p,double maxB,double step)
    {
        _=Calculate(rows,p);
        Require(double.IsFinite(maxB)&&maxB>=p.B&&double.IsFinite(step)&&step>0,"Intervallo di ricerca non valido");
        Require(Math.Ceiling((maxB-p.B)/step)<=500,"Ricerca limitata a 500 incrementi");
        var output=new List<DesignResult>();
        foreach(var group in rows.GroupBy(r=>r.Node))
        {
            IReadOnlyList<FootingResult> last=[];var found=false;
            for(int i=0;p.B+i*step<=maxB+1e-9;i++)
            {
                var b=Math.Round(p.B+i*step,8,MidpointRounding.AwayFromZero);
                last=Calculate(group.ToArray(),p with {B=b,L=b});
                if(last.All(r=>r.Ok)){found=true;break;}
            }
            output.Add(new DesignResult(group.Key,found,last));
        }
        return output;
    }

    /// <summary>Rende espliciti gli errori di dominio per tutti gli host.</summary>
    private static void Require(bool condition,string message){if(!condition)throw new ArgumentException(message);}
    /// <summary>Norma euclidea delle due componenti, nello stesso sistema di unità.</summary>
    private static double Hypot(double x,double y)=>Math.Sqrt(x*x+y*y);
}

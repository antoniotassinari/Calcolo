using STA.Materiali;
namespace STA.Muratura.Core;

/// <summary>Adatta il materiale indipendente al contratto JSON piatto del sito originale.</summary>
public static class MasonryMaterialMapping
{
    public static MasonryMaterialState Read(MasonryPier p)=>new(){MaterialId=p.MaterialId,Type=p.Type,Lc=p.Lc,FmBase=p.FmBase,TauBase=p.TauBase,E=p.E,G=p.G,Rho=p.Rho,Cracked=p.Cracked,C=[..p.C]};
    public static void Apply(MasonryPier p,MasonryMaterialState material)
    {
        p.MaterialId=material.MaterialId;p.Type=material.Type;p.MaterialVersion=2;p.Lc=material.Lc;
        p.FmBase=material.FmBase;p.TauBase=material.TauBase;p.E=material.E;p.G=material.G;
        p.Rho=material.Rho;p.Cracked=material.Cracked;p.C=[..material.C];
    }
}

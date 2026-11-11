using System.Globalization;
using STA.Calcolo.Desktop.Infrastructure;
using STA.Materiali;
using STA.Muratura.Core;
namespace STA.Calcolo.Desktop.ViewModels;

public sealed class MasonryMaterialViewModel : ObservableObject
{
    private MasonryMaterialState model;
    public MasonryFieldsViewModel Editor {get;private set;}=null!;
    public IReadOnlyList<MasonryMaterial> Materials {get;}=[new("custom","Personalizzato",[],[],[],[],null,"Valori personalizzati: verificare resistenze, moduli e peso specifico."),..MasonryCatalog.All];
    public IReadOnlyList<ModuleFieldViewModel> CoefficientFields {get;}
    public string[] Levels {get;}=["LC1","LC2","LC3"];
    private string materialId,level;
    public string MaterialId {get=>materialId;set{if(value!=null&&Set(ref materialId,value))ApplyPreset();}}
    public string Level {get=>level;set{if(value!=null&&Set(ref level,value))ApplyPreset();}}
    public bool Cracked {get=>Editor.ByKey["Cracked"].Value=="Sì";set=>Editor.ByKey["Cracked"].Value=value?"Sì":"No";}
    private string error="";
    public string MaterialNote=>error.Length>0?error:Materials.FirstOrDefault(m=>m.Id==MaterialId)?.Note??"Materiale personalizzato.";
    public string ConfidenceText {get{try{return MasonryCatalog.Confidence(Level).ToString("N2");}catch{return "—";}}}
    public string FmDesignText => DesignValue(m => m.FmBase / MasonryCatalog.Confidence(m.Lc) * m.C.Aggregate(1d, (a, b) => a * b));
    public string TauDesignText => DesignValue(m => m.TauBase / MasonryCatalog.Confidence(m.Lc) * m.C.Aggregate(1d, (a, b) => a * b));
    public string EDesignText => DesignValue(m => m.E * ElasticFactor(m));
    public string GDesignText => DesignValue(m => m.G * ElasticFactor(m));
    private static double ElasticFactor(MasonryMaterialState m) => new[] { 0, 1, 4, 5, 6 }.Aggregate(1d, (a, i) => a * m.C[i]) * (m.Cracked ? .5 : 1);
    private string DesignValue(Func<MasonryMaterialState, double> calculate)
    {
        try
        {
            var m = Build();
            MasonryCalculator.Positive(m.FmBase, m.TauBase, m.E, m.G);
            MasonryCalculator.Positive(m.C);
            var value = calculate(m);
            return double.IsFinite(value) ? value.ToString("N2") : "—";
        }
        catch { return "—"; }
    }
    public MasonryMaterialViewModel(MasonryMaterialState model)
    {
        this.model=model;materialId=model.MaterialId;level=model.Lc;
        CoefficientFields=new[]{"Malta buona","Giunti sottili","Ricorsi o listature","Connessione trasversale","Nucleo scadente","Iniezioni","Intonaco armato"}.Select((name,i)=>new ModuleFieldViewModel(new(i.ToString(),name,model.C[i].ToString(CultureInfo.InvariantCulture)))).ToArray();
        foreach(var f in CoefficientFields)f.PropertyChanged+=(_,_)=>Refresh();
        ResetEditor();
    }
    private void ResetEditor()
    {
        Editor=new(model,[("FmBase","fm tabellare [N/cm²]",null),("TauBase","τ tabellare [N/cm²]",null),("E","E tabellare [MPa]",null),("G","G tabellare [MPa]",null),("Rho","Peso specifico [daN/m³]",null),("Cracked","Fessurata",["No","Sì"])]);
        foreach(var f in Editor.Fields.Where(f=>f.Key!="Cracked"))f.PropertyChanged+=(_,_)=>{materialId="custom";Changed(nameof(MaterialId));Changed(nameof(MaterialNote));};
        Editor.PropertyChanged+=(_,_)=>Refresh();Changed(nameof(Editor));Refresh();
    }
    public MasonryMaterialState Build()
    {
        var result=model with {MaterialId=materialId,Type=materialId=="custom"?"Personalizzato":model.Type,Lc=Level,C=CoefficientFields.Select(f=>double.Parse(f.Value.Replace(',','.'),NumberStyles.Float,CultureInfo.InvariantCulture)).ToArray()};
        Editor.Apply(result);return result;
    }
    private void ApplyPreset()
    {
        try
        {
            var source=model with {Cracked=Cracked};
            if(MaterialId=="custom"){model=source with {MaterialId="custom",Type="Personalizzato",Lc=Level};}
            else {var pier=new MasonryPier();MasonryMaterialMapping.Apply(pier,source);model=MasonryMaterialMapping.Read(MasonryCalculator.Preset(pier,MaterialId,Level));ResetEditor();}
            error="";Refresh();
        }
        catch(Exception e){error=e.Message;Changed(nameof(MaterialNote));}
    }
    public void PresetAll(string id,string lc){materialId=id;level=lc;ApplyPreset();Changed(nameof(MaterialId));Changed(nameof(Level));}
    private void Refresh(){Changed(nameof(Cracked));Changed(nameof(ConfidenceText));Changed(nameof(FmDesignText));Changed(nameof(TauDesignText));Changed(nameof(EDesignText));Changed(nameof(GDesignText));Changed(nameof(MaterialNote));Changed(nameof(Editor));}
}

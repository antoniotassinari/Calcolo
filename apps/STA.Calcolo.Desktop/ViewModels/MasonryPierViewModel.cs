using STA.Calcolo.Desktop.Infrastructure;
using STA.Materiali;
using STA.Muratura.Core;
using System.Globalization;
namespace STA.Calcolo.Desktop.ViewModels;

public sealed class MasonryPierViewModel : ObservableObject
{
    private MasonryPier model;
    public string Label {get;}
    public MasonryFieldsViewModel Editor {get;private set;}=null!;
    public IReadOnlyList<MasonryMaterial> Materials {get;}=[new("custom","Personalizzato",[],[],[],[],null,"Valori personalizzati: verificare resistenze, moduli e peso specifico."),..MasonryCatalog.All];
    private string materialId;public string MaterialId {get=>materialId;set=>Set(ref materialId,value);}
    private string level;public string Level {get=>level;set=>Set(ref level,value);}
    public string[] Levels {get;}=["LC1","LC2","LC3"];
    public string MaterialNote=>Materials.FirstOrDefault(x=>x.Id==MaterialId)?.Note??"Materiale personalizzato.";
    public RelayCommand ApplyMaterialCommand {get;}
    public RelayCommand RemoveCommand {get;}
    private string coefficients;public string Coefficients {get=>coefficients;set=>Set(ref coefficients,value);}
    public MasonryPierViewModel(MasonryPier p,string label,Action<MasonryPierViewModel> remove)
    {
        model=p;Label=label;materialId=p.MaterialId;level=p.Lc;coefficients=string.Join(";",p.C.Select(x=>x.ToString(CultureInfo.InvariantCulture)));ResetEditor();
        ApplyMaterialCommand=new(()=>{try{var copy=Build();model=MaterialId=="custom"?copy with {MaterialId="custom",Type="Personalizzato",Lc=Level}:MasonryCalculator.Preset(copy,MaterialId,Level);ResetEditor();Changed(nameof(MaterialNote));}catch(Exception e){Note=e.Message;Changed(nameof(Note));}});
        RemoveCommand=new(()=>remove(this));
    }
    public string Note {get;private set;}="I valori resistenti sono prima di FC. E/G sono non fessurati; la riduzione ×0,5 viene applicata dal calcolo.";
    private void ResetEditor()
    {
        Editor=new(model,[("H","Altezza [cm]",null),("L","Lunghezza [cm]",null),("T","Spessore [cm]",null),("LoadMode","Modalità del carico",["Risultante N","Distribuito q"]),("N","N compressione [kN]",null),("Q","q [kN/m]",null),("Lc","Livello conoscenza",["LC1","LC2","LC3"]),("FmBase","fm prima di FC [daN/cm²]",null),("TauBase","τ prima di FC [daN/cm²]",null),("E","E [MPa]",null),("G","G [MPa]",null),("Rho","Peso specifico [daN/m³]",null),("Cracked","Fessurata",["No","Sì"])]);
        Editor.PropertyChanged+=(_,_)=>Changed(nameof(Editor));Changed(nameof(Editor));
    }
    public MasonryPier Build()
    {
        var p=model with {C=Coefficients.Split(';').Select(x=>double.Parse(x.Replace(',','.'),NumberStyles.Float,CultureInfo.InvariantCulture)).ToArray()};Editor.Apply(p);
        if(p.FmBase!=model.FmBase||p.TauBase!=model.TauBase||p.E!=model.E||p.G!=model.G||p.Lc!=model.Lc||p.Rho!=model.Rho){p.MaterialId="custom";p.Type="Personalizzato";}
        return p;
    }
    public void PresetAll(string id,string lc){model=MasonryCalculator.Preset(Build(),id,lc);MaterialId=id;Level=lc;ResetEditor();Changed(nameof(MaterialNote));}
}

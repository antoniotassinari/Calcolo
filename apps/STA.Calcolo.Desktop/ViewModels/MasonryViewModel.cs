using System.Collections.ObjectModel;
using STA.Calcolo.Application;
using STA.Calcolo.Desktop.Infrastructure;
using STA.Calcolo.Desktop.Services;
using STA.Materiali;
using STA.Muratura.Core;
using STA.Profilati.Core;
namespace STA.Calcolo.Desktop.ViewModels;

public sealed class MasonryViewModel : ObservableObject
{
    private readonly IFileService files;
    public ObservableCollection<MasonryPierViewModel> Before {get;}=[];
    public ObservableCollection<MasonryPierViewModel> After {get;}=[];
    private string title="Confronto maschi murari";public string Title {get=>title;set{if(Set(ref title,value))Invalidate();}}
    public MasonryFieldsViewModel Frame {get;private set;}=null!;
    public IReadOnlyList<MasonryMaterial> Materials=>MasonryCatalog.All;
    public string MaterialId {get;set;}="mattoni-calce";
    public string Level {get;set;}="LC1";
    public string[] Levels {get;}=["LC1","LC2","LC3"];
    private string status="Inserire i dati e calcolare.";public string Status {get=>status;private set=>Set(ref status,value);}
    public IReadOnlyList<ResultRow> Results {get;private set;}=[];
    public IReadOnlyList<ProfileProposal> Proposals {get;private set;}=[];
    private ProfileProposal? selectedProposal;public ProfileProposal? SelectedProposal{get=>selectedProposal;set{Set(ref selectedProposal,value);UseProfileCommand.Refresh();}}
    public RelayCommand AddBeforeCommand{get;} public RelayCommand AddAfterCommand{get;} public RelayCommand CopyCommand{get;} public RelayCommand ExampleCommand{get;}
    public RelayCommand ApplyMaterialCommand{get;} public RelayCommand CalculateCommand{get;} public RelayCommand DesignCommand{get;} public RelayCommand UseProfileCommand{get;}
    public RelayCommand OpenCommand{get;} public RelayCommand SaveCommand{get;} public RelayCommand ExportCommand{get;} public RelayCommand ReportCommand{get;}
    public RelayCommand EqualBeforeCommand{get;} public RelayCommand EqualAfterCommand{get;}
    public MasonryViewModel(IFileService files)
    {
        this.files=files;
        AddBeforeCommand=new(()=>Run(()=>Add(Before,MasonryCalculator.Preset(new(),MaterialId,Level))));AddAfterCommand=new(()=>Run(()=>Add(After,MasonryCalculator.Preset(new(),MaterialId,Level))));
        CopyCommand=new(()=>Run(()=>{var data=Before.Select(x=>x.Build()).ToArray();After.Clear();foreach(var p in data)Add(After,p with {C=[..p.C]});}));
        ExampleCommand=new(()=>Run(()=>Load(MasonryFiles.Example())));
        ApplyMaterialCommand=new(()=>Run(()=>{foreach(var p in Before.Concat(After))p.PresetAll(MaterialId,Level);}));
        EqualBeforeCommand=new(()=>Run(()=>Equal(Before)));EqualAfterCommand=new(()=>Run(()=>Equal(After)));
        CalculateCommand=new(()=>Run(Calculate));DesignCommand=new(()=>Run(()=>{Calculate();var p=Build();Proposals=FrameCalculator.Design(p.Frame,MasonryCalculator.Total(p.Before),MasonryCalculator.Total(p.After));Changed(nameof(Proposals));Status="Ricerca completata: selezionare una proposta disponibile.";}));
        UseProfileCommand=new(()=>Run(()=>{Frame.Fields.Single(f=>f.Key=="Profile").Value=SelectedProposal!.Candidate!.Profile.Id;Calculate();}),()=>SelectedProposal?.Candidate!=null);
        OpenCommand=new(()=>Run(()=>{var path=files.Open("Progetto muratura (anche dal sito)|*.json");if(path!=null){var text=files.Read(path);Load(MasonryFiles.Read(text));Status="Progetto aperto. Altezza efficace = altezza geometrica; controllare eventuali altezze efficaci del progetto originale.";}}));
        SaveCommand=new(()=>Run(()=>{var text=MasonryFiles.Write(Build());var path=files.Save("Progetto muratura|*.json","confronto-maschi.json");if(path!=null){files.Write(path,text);Status="Progetto salvato, compatibile con il sito.";}}));
        ExportCommand=new(()=>Run(()=>{var path=files.Save("Risultati JSON|*.json","muratura-risultati.json");if(path!=null)files.Write(path,ModuleFiles.Write(new{Input=Build(),Results}));}),()=>Results.Count>0);
        ReportCommand=new(()=>Run(()=>{var path=files.Save("Relazione HTML stampabile|*.html","confronto-maschi.html");if(path!=null){files.Write(path,MasonryReport.Create(Build(),Results));Status="Relazione salvata: aprirla nel browser per stampare o salvare in PDF.";}}),()=>Results.Count>0);
        Load(new());
    }
    private void Equal(ObservableCollection<MasonryPierViewModel> rows){var h=rows[0].Editor.Fields.Single(f=>f.Key=="H").Value;foreach(var p in rows)p.Editor.Fields.Single(f=>f.Key=="H").Value=h;}
    private void Add(ObservableCollection<MasonryPierViewModel> list,MasonryPier p)
    {
        if(list.Count>=10)throw new ArgumentException("Massimo 10 maschi per stato.");
        var item=new MasonryPierViewModel(p,"Maschio",row=>{if(list.Count>1){list.Remove(row);Invalidate();}else Status="Mantenere almeno un maschio per stato.";});item.PropertyChanged+=(_,_)=>Invalidate();list.Add(item);Invalidate();
    }
    public void Load(MasonryProject p)
    {
        Title=p.Title;Before.Clear();After.Clear();foreach(var row in p.Before)Add(Before,row);foreach(var row in p.After)Add(After,row);
        Frame=new(p.Frame,[("Enabled","Includi cerchiatura",["No","Sì"]),("Profile","Profilo OPPO",ProfileCatalog.All.Select(x=>x.Id).ToArray()),("H","Altezza telaio [cm]",null),("NColumns","Numero colonne",null),("NProfiles","Profili per colonna",null),("E","E acciaio [daN/cm²]",null),("Fy","fyk [MPa]",null),("Gamma","γM0",null),("Portal","Schema del portale",["Chiuso","Aperto"])]);Frame.PropertyChanged+=(_,_)=>Invalidate();Changed(nameof(Frame));Invalidate();
    }
    public MasonryProject Build(){var frame=new FrameInput();Frame.Apply(frame);return new(){Title=Title,Before=Before.Select(p=>p.Build()).ToArray(),After=After.Select(p=>p.Build()).ToArray(),Frame=frame};}
    private void Calculate()
    {
        Invalidate();var p=Build();MasonryFiles.Validate(p);var a=MasonryCalculator.Total(p.Before);var b=MasonryCalculator.Total(p.After);
        List<ResultRow> rows=[];
        foreach(var (name,state) in new[]{("Prima",a),("Dopo muratura",b)})
        {
            rows.Add(new(name+" · K totale",state.K,"N/cm"));rows.Add(new(name+" · ΣVt",state.Vt,"kN"));rows.Add(new(name+" · du minimo",state.Du,"mm"));
            for(int i=0;i<state.Rows.Length;i++){var r=state.Rows[i];string label=$"{name} / {i+1}";rows.AddRange([new(label+" · N",r.N,"kN"),new(label+" · σ",r.Sigma,"N/cm²"),new(label+" · b",r.B,""),new(label+" · K",r.K,"N/cm"),new(label+" · Vt",r.Vt,"kN"),new(label+" · dy",r.Dy,"mm"),new(label+" · du",r.Du,"mm")]);}
        }
        var f=FrameCalculator.Calculate(p.Frame,a,b);double dk=((f?.FinalK??b.K)/a.K-1)*100,dv=((f?.FinalV??b.Vt)/a.Vt-1)*100;
        rows.AddRange([new("ΔK finale / prima",dk,"%"),new("ΔV finale / prima",dv,"%")]);
        if(f!=null){rows.AddRange([new("Telaio · J colonna",f.J,"cm⁴"),new("Telaio · W colonna",f.W,"cm³"),new("Telaio · V massimo",f.V,"kN"),new("Telaio · K",f.K,"N/cm"),new("Telaio · dy",f.Dy,"mm"),new("Telaio · Veff mobilitato",f.Veff,"kN"),new("Finale · V",f.FinalV,"kN"),new("Finale · K",f.FinalK,"N/cm")]);if(f.Wstar is double w)rows.Add(new("W* (ipotesi due colonne)",w,"cm³"));}
        Results=rows;Changed(nameof(Results));ExportCommand.Refresh();ReportCommand.Refresh();Status=$"Resistenza {(dv>=0?"non diminuita":"diminuita")}; rigidezza {(FrameCalculator.StiffnessAccepts(dk)?"entro":"oltre")} ±15%. Confronto preliminare, non verifica completa.";
    }
    private void Invalidate(){Results=[];Proposals=[];selectedProposal=null;Changed(nameof(Results));Changed(nameof(Proposals));Changed(nameof(SelectedProposal));ExportCommand?.Refresh();ReportCommand?.Refresh();UseProfileCommand?.Refresh();Status="Dati modificati: ricalcolare.";}
    private void Run(Action action){try{action();}catch(Exception e){Invalidate();Status=e.Message;}}
}

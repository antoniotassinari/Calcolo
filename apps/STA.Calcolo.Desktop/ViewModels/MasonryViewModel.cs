using System.Collections.ObjectModel;
using STA.Calcolo.Application;
using STA.Calcolo.Desktop.Infrastructure;
using STA.Calcolo.Desktop.Services;
using STA.Materiali;
using STA.Muratura.Core;
using STA.Profilati.Core;
using STA.Calcolo.Reporting;
namespace STA.Calcolo.Desktop.ViewModels;

public sealed class MasonryViewModel : ObservableObject
{
    private readonly IFileService files;
    public MasonrySummaryViewModel Summary {get;private set;}=MasonrySummaryViewModel.Empty;
    public bool FrameEnabled {get=>Frame?.ByKey["Enabled"].Value=="Sì";set{if(Frame!=null)Frame.ByKey["Enabled"].Value=value?"Sì":"No";}}
    public int[] Counts {get;}=Enumerable.Range(1,10).ToArray();
    public int BeforeCount {get=>Before.Count;set=>Resize(Before,value);}
    public int AfterCount {get=>After.Count;set=>Resize(After,value);}
    private bool equalBefore,equalAfter,syncing;
    public bool EqualBefore {get=>equalBefore;set{Set(ref equalBefore,value);SyncHeights(Before,value);}}
    public bool EqualAfter {get=>equalAfter;set{Set(ref equalAfter,value);SyncHeights(After,value);}}
    private void Resize(ObservableCollection<MasonryPierViewModel> rows,int count){if(count<1||count>10)return;Run(()=>{while(rows.Count<count)Add(rows,MasonryCalculator.Preset(new(),MaterialId,Level));while(rows.Count>count)rows.RemoveAt(rows.Count-1);RefreshRows(rows);Invalidate();});}
    private void RefreshRows(ObservableCollection<MasonryPierViewModel> rows){for(int i=0;i<rows.Count;i++)rows[i].Label=$"Maschio {i+1}";Changed(nameof(BeforeCount));Changed(nameof(AfterCount));SyncHeights(rows,ReferenceEquals(rows,Before)?EqualBefore:EqualAfter);}
    private void SyncHeights(ObservableCollection<MasonryPierViewModel> rows,bool enabled){if(syncing||rows.Count==0)return;syncing=true;try{for(int i=0;i<rows.Count;i++){rows[i].SetHeightEditable(!enabled||i==0);if(enabled&&i>0)rows[i].Editor.ByKey["H"].Value=rows[0].Editor.ByKey["H"].Value;}}finally{syncing=false;}}

    public ObservableCollection<MasonryPierViewModel> Before {get;}=[];
    public ObservableCollection<MasonryPierViewModel> After {get;}=[];
    private string title="Confronto maschi murari";public string Title {get=>title;set{if(Set(ref title,value))Invalidate();}}
    public FrameViewModel Frame {get;private set;}=null!;
    public IReadOnlyList<MasonryMaterial> Materials=>MasonryCatalog.All;
    public string MaterialId {get;set;}="mattoni-calce";
    public string Level {get;set;}="LC1";
    public string[] Levels {get;}=["LC1","LC2","LC3"];
    private string status="Inserire i dati e calcolare.";public string Status {get=>status;private set=>Set(ref status,value);}
    public IReadOnlyList<ResultRow> Results {get;private set;}=[];
    public IReadOnlyList<ReportEquation> Derivation {get;private set;}=[];
    public IReadOnlyList<ComparisonRow> Comparison {get;private set;}=[];
    public IReadOnlyList<PierSummaryRow> PierResults {get;private set;}=[];
    public IEnumerable<PierSummaryRow> BeforeResults=>PierResults.Where(r=>r.Stato=="Prima");
    public IEnumerable<PierSummaryRow> AfterResults=>PierResults.Where(r=>r.Stato=="Dopo");
    public IReadOnlyList<ProfileProposal> Proposals=>Frame?.Proposals??[];
    public ProfileProposal? SelectedProposal {get=>Frame?.SelectedProposal;set{if(Frame!=null)Frame.SelectedProposal=value;}}
    public RelayCommand AddBeforeCommand{get;} public RelayCommand AddAfterCommand{get;} public RelayCommand CopyCommand{get;} public RelayCommand ExampleCommand{get;}
    public RelayCommand ApplyMaterialCommand{get;} public RelayCommand CalculateCommand{get;} public RelayCommand DesignCommand=>Frame?.DesignCommand!; public RelayCommand UseProfileCommand=>Frame?.UseProfileCommand!;
    public RelayCommand OpenCommand{get;} public RelayCommand SaveCommand{get;} public RelayCommand ExportCommand{get;} public RelayCommand ReportCommand{get;} public RelayCommand WordReportCommand{get;}
    public RelayCommand EqualBeforeCommand{get;} public RelayCommand EqualAfterCommand{get;}
    public MasonryViewModel(IFileService files,IMasonryReportService? reportService=null)
    {
        this.files=files;var reports=reportService??new MasonryReportService();
        AddBeforeCommand=new(()=>Run(()=>Add(Before,MasonryCalculator.Preset(new(),MaterialId,Level))));AddAfterCommand=new(()=>Run(()=>Add(After,MasonryCalculator.Preset(new(),MaterialId,Level))));
        CopyCommand=new(()=>Run(()=>{var data=Before.Select(x=>x.Build()).ToArray();After.Clear();foreach(var p in data)Add(After,p with {C=[..p.C]});}));
        ExampleCommand=new(()=>Run(()=>Load(MasonryFiles.Example())));
        ApplyMaterialCommand=new(()=>Run(()=>{foreach(var p in Before.Concat(After))p.PresetAll(MaterialId,Level);}));
        EqualBeforeCommand=new(()=>Run(()=>Equal(Before)));EqualAfterCommand=new(()=>Run(()=>Equal(After)));
        CalculateCommand=new(()=>Run(Calculate));
        OpenCommand=new(()=>Run(()=>{var path=files.Open("Progetto muratura (anche dal sito)|*.json");if(path!=null){var text=files.Read(path);Load(MasonryFiles.Read(text));Status="Progetto aperto. Altezza efficace = altezza geometrica; controllare eventuali altezze efficaci del progetto originale.";}}));
        SaveCommand=new(()=>Run(()=>{var text=MasonryFiles.Write(Build());var path=files.Save("Progetto muratura|*.json","confronto-maschi.json");if(path!=null){files.Write(path,text);Status="Progetto salvato, compatibile con il sito.";}}));
        ExportCommand=new(()=>Run(()=>{var path=files.Save("Risultati JSON|*.json","muratura-risultati.json");if(path!=null)files.Write(path,ModuleFiles.Write(new{Input=Build(),Results}));}),()=>Results.Count>0);
        ReportCommand=new(()=>Run(()=>{var p=Build();var path=files.Save("Relazione PDF|*.pdf","relazione-maschi.pdf");if(path!=null){files.WriteBytes(path,reports.Pdf(p));Status="Relazione PDF salvata in formato A4 con margini di 15 mm.";}}),()=>Results.Count>0);
        WordReportCommand=new(()=>Run(()=>{var p=Build();var path=files.Save("Relazione Word|*.docx","relazione-maschi.docx");if(path!=null){files.WriteBytes(path,reports.Word(p));Status="Relazione Word modificabile salvata in formato A4 con margini di 15 mm.";}}),()=>Results.Count>0);
        Load(new(){Before=[new(),new(),new()],After=[new(),new(),new()],EqualHeights=new(){["before"]=true,["after"]=true}});
    }
    private void Equal(ObservableCollection<MasonryPierViewModel> rows){var h=rows[0].Editor.Fields.Single(f=>f.Key=="H").Value;foreach(var p in rows)p.Editor.Fields.Single(f=>f.Key=="H").Value=h;}
    private void Add(ObservableCollection<MasonryPierViewModel> list,MasonryPier p)
    {
        if(list.Count>=10)throw new ArgumentException("Massimo 10 maschi per stato.");
        var item=new MasonryPierViewModel(p,"Maschio",row=>{if(list.Count>1){list.Remove(row);RefreshRows(list);Invalidate();}else Status="Mantenere almeno un maschio per stato.";});item.PropertyChanged+=(_,e)=>{if(e.PropertyName=="Editor")SyncHeights(list,ReferenceEquals(list,Before)?EqualBefore:EqualAfter);Invalidate();};list.Add(item);RefreshRows(list);Invalidate();
    }
    public void Load(MasonryProject p)
    {
        equalBefore=false;equalAfter=false;
        Title=p.Title;Before.Clear();After.Clear();foreach(var row in p.Before)Add(Before,row);foreach(var row in p.After)Add(After,row);
        Frame=new(p.Frame,()=> (MasonryCalculator.Total(Before.Select(p=>p.Build()).ToArray()),MasonryCalculator.Total(After.Select(p=>p.Build()).ToArray())),()=>Run(Calculate),message=>Status=message);Frame.PropertyChanged+=(_,e)=>{if(e.PropertyName is "Fields" or "Enabled"){Changed(nameof(FrameEnabled));Invalidate();}else {Changed(nameof(Proposals));Changed(nameof(SelectedProposal));}};Changed(nameof(DesignCommand));Changed(nameof(UseProfileCommand));Changed(nameof(Frame));Changed(nameof(FrameEnabled));EqualBefore=p.EqualHeights?.GetValueOrDefault("before")??false;EqualAfter=p.EqualHeights?.GetValueOrDefault("after")??false;Invalidate();
    }
    public MasonryProject Build(){var frame=Frame.Build();return new(){Title=Title,Before=Before.Select(p=>p.Build()).ToArray(),After=After.Select(p=>p.Build()).ToArray(),Frame=frame,EqualHeights=new(){["before"]=EqualBefore,["after"]=EqualAfter}};}
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
        Results=rows;Derivation=MasonryDerivation.Build(p).SelectMany(page=>page.Equations??[]).ToArray();Changed(nameof(Derivation));Comparison=MasonryReportBuilder.Compare(p);PierResults=MasonryReportBuilder.Piers(p);Changed(nameof(Comparison));Changed(nameof(BeforeResults));Changed(nameof(AfterResults));Changed(nameof(Results));ExportCommand.Refresh();ReportCommand.Refresh();WordReportCommand.Refresh();Status=$"Resistenza {(dv>=0?"non diminuita":"diminuita")}; rigidezza {(FrameCalculator.StiffnessAccepts(dk)?"entro":"oltre")} ±15%. Confronto preliminare, non verifica completa.";
    }
    private void Invalidate(){try{Summary=Frame==null?MasonrySummaryViewModel.Empty:MasonrySummaryViewModel.Create(Build());}catch{Summary=MasonrySummaryViewModel.Empty;}Changed(nameof(Summary));Results=[];Derivation=[];Changed(nameof(Derivation));Comparison=[];PierResults=[];Changed(nameof(Comparison));Changed(nameof(BeforeResults));Changed(nameof(AfterResults));Frame?.ClearProposals();Changed(nameof(Results));Changed(nameof(Proposals));Changed(nameof(SelectedProposal));ExportCommand?.Refresh();ReportCommand?.Refresh();WordReportCommand?.Refresh();UseProfileCommand?.Refresh();Status="Dati modificati: ricalcolare.";}
    private void Run(Action action){try{action();}catch(Exception e){Invalidate();Status=e.Message;}}
}

using System.Collections.ObjectModel;
using STA.Calcolo.Application;
using STA.Calcolo.Desktop.Infrastructure;
using STA.Calcolo.Desktop.Services;
namespace STA.Calcolo.Desktop.ViewModels;

public sealed class BeamViewModel : ObservableObject
{
    private readonly IFileService files; private readonly BeamProjectService service = new();
    private string name = "Trave 1"; public string Name { get => name; set => Set(ref name, value); }
    public ObservableCollection<ModuleDocument> Sections { get; } = []; public ObservableCollection<ModuleDocument> Tendons { get; } = [];
    private ModuleDocument? section; public ModuleDocument? SelectedSection { get => section; set { Set(ref section, value); RemoveSectionCommand.Refresh(); } }
    private ModuleDocument? tendon; public ModuleDocument? SelectedTendon { get => tendon; set { Set(ref tendon, value); RemoveTendonCommand.Refresh(); } }
    private string status = "Importare i progetti salvati dalle finestre Sezioni e Trefolature."; public string Status { get => status; private set => Set(ref status, value); }
    public RelayCommand ImportSectionCommand { get; }
    public RelayCommand ImportTendonCommand { get; }
    public RelayCommand RemoveSectionCommand { get; }
    public RelayCommand RemoveTendonCommand { get; }
    public RelayCommand OpenCommand { get; }
    public RelayCommand SaveCommand { get; }
    public RelayCommand CheckCommand { get; }
    public RelayCommand OpenSectionCommand { get; }
    public RelayCommand OpenTendonCommand { get; }
    public BeamViewModel(IFileService files, IWindowService windows)
    {
        this.files = files;
        ImportSectionCommand = new(() => Run(() => Import("sezione", Sections))); ImportTendonCommand = new(() => Run(() => Import("trefolatura", Tendons)));
        RemoveSectionCommand = new(() => { Sections.Remove(SelectedSection!); SelectedSection = null; Status = "Sezione rimossa."; }, () => SelectedSection != null);
        RemoveTendonCommand = new(() => { Tendons.Remove(SelectedTendon!); SelectedTendon = null; Status = "Trefolatura rimossa."; }, () => SelectedTendon != null);
        OpenCommand = new(() => Run(() => { var path = files.Open("Progetto trave|*.json"); if (path == null) return; Load(service.Read(files.Read(path))); }));
        SaveCommand = new(() => Run(() => { var text = service.Write(Document()); var path = files.Save("Progetto trave|*.json", "trave.json"); if (path != null) files.Write(path, text); }));
        CheckCommand = new(() => Run(() => { var model = service.Build(Document()); Status = $"Progetto leggibile: {model.ElencoSezioni.Count} sezioni, {model.MaschereTrefoli.Count} trefolature. Nessuna verifica strutturale della trave eseguita."; }));
        OpenSectionCommand = new(() => windows.Open("sezione")); OpenTendonCommand = new(() => windows.Open("trefolatura"));
    }
    public BeamProject Document() => new(1, Name, Sections.ToList(), Tendons.ToList());
    public void Load(BeamProject project) { service.Build(project); Name = project.Name; Sections.Clear(); Tendons.Clear(); foreach (var s in project.Sections) Sections.Add(s); foreach (var t in project.Tendons) Tendons.Add(t); SelectedSection = null; SelectedTendon = null; Status = "Progetto trave caricato."; }
    private void Import(string module, ObservableCollection<ModuleDocument> target) { var path = files.Open("Progetto modulo|*.json"); if (path != null) { target.Add(service.ReadModule(files.Read(path), module)); Status = "Modulo importato nel progetto trave."; } }
    private void Run(Action action) { try { action(); } catch (Exception e) { Status = e.Message; } }
}
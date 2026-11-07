using STA.Prefabbricato.TraveIsostatica;
namespace STA.Calcolo.Application;

public sealed class BeamProjectService
{
    public TraveIsostatica Build(BeamProject project)
    {
        if (project.SchemaVersion != 1 || string.IsNullOrWhiteSpace(project.Name) || project.Sections == null || project.Tendons == null) throw new ArgumentException("Progetto trave non valido.");
        var model = new TraveIsostatica { NomeTrave = project.Name };
        foreach (var section in project.Sections) model.ElencoSezioni.Add(new SectionWorkspace().Build(section));
        foreach (var tendon in project.Tendons) model.MaschereTrefoli.AggiungiTrefolatura(new TendonWorkspace().Build(tendon));
        return model;
    }
    public BeamProject Read(string json) { var project = ModuleFiles.Read<BeamProject>(json); Build(project); return project; }
    public string Write(BeamProject project) { Build(project); return ModuleFiles.Write(project); }
    public ModuleDocument ReadModule(string json, string module)
    {
        var doc = ModuleFiles.Read<ModuleDocument>(json);
        ICalculationWorkspace workspace = module == "sezione" ? new SectionWorkspace() : new TendonWorkspace();
        workspace.Calculate(doc); return doc;
    }
}

namespace STA.Calcolo.Application;

public sealed record BeamProject(int SchemaVersion, string Name, List<ModuleDocument> Sections, List<ModuleDocument> Tendons);

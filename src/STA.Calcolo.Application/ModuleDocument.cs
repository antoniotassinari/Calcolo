namespace STA.Calcolo.Application;

public sealed record ModuleDocument(int SchemaVersion, string Module, Dictionary<string, string> Fields, string Data);

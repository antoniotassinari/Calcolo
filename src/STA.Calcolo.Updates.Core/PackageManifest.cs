namespace STA.Calcolo.Updates;
public sealed record PackageManifest(int SchemaVersion, string Component, string Version, List<PackageFile> Files);

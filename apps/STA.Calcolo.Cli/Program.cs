using STA.Calcolo.IO;
using STA.Calcolo.Application;
using STA.Plinti.Core;
try
{
    if (args.Length is < 2 or > 3 || args[0] is not ("plinti" or "modulo")) { Console.Error.WriteLine("Uso: STA.Calcolo.Cli plinti|modulo progetto.json [risultati.json]"); return 2; }
    if(args[0]=="modulo")
    {
        var doc=ModuleFiles.Read<ModuleDocument>(await File.ReadAllTextAsync(args[1]));
        var module=WorkspaceCatalog.Create().SingleOrDefault(m=>m.Id==doc.Module) ?? throw new ArgumentException("Modulo sconosciuto.");
        var results=module.Calculate(doc);var output=ModuleFiles.Write(new {Input=doc,Results=results});
        if(args.Length==3) await File.WriteAllTextAsync(args[2],output);else Console.WriteLine(output);
        return 0;
    }
    var request = CalculationFiles.ReadRequest(await File.ReadAllTextAsync(args[1]));
    var report = new FootingModule().Calculate(request);
    var json = CalculationFiles.Write(new { input = request, report });
    if (args.Length == 3) await File.WriteAllTextAsync(args[2], json); else Console.WriteLine(json);
    return 0;
}
catch (Exception e) when (e is ArgumentException or System.Text.Json.JsonException or IOException or FormatException or OverflowException)
{ Console.Error.WriteLine(e.Message); return 1; }

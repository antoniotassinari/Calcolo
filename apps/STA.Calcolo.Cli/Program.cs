using STA.Calcolo.IO;
using STA.Plinti.Core;
try
{
    if (args.Length is < 2 or > 3 || args[0] != "plinti") { Console.Error.WriteLine("Uso: STA.Calcolo.Cli plinti progetto.json [risultati.json]"); return 2; }
    var request = CalculationFiles.ReadRequest(await File.ReadAllTextAsync(args[1]));
    var report = new FootingModule().Calculate(request);
    var json = CalculationFiles.Write(new { input = request, report });
    if (args.Length == 3) await File.WriteAllTextAsync(args[2], json); else Console.WriteLine(json);
    return 0;
}
catch (Exception e) when (e is ArgumentException or System.Text.Json.JsonException or IOException or FormatException or OverflowException)
{ Console.Error.WriteLine(e.Message); return 1; }

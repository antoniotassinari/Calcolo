using System.Globalization;
namespace STA.Calcolo.Application;

public sealed class ModuleInput(ModuleDocument document, ICalculationWorkspace module)
{
    public void Validate()
    {
        if (document == null || document.SchemaVersion != 1 || document.Module != module.Id || document.Fields == null || document.Data == null || document.Fields.Values.Any(v=>v==null))
            throw new ArgumentException("Formato progetto o modulo non compatibile.");
        if (!document.Fields.Keys.Order().SequenceEqual(module.Fields.Select(f => f.Key).Order())) throw new ArgumentException("Parametri del progetto incompleti o sconosciuti.");
        foreach (var field in module.Fields) if (field.Choices != null && !field.Choices.Contains(Text(field.Key))) throw new ArgumentException($"{field.Label}: scelta non valida.");
    }
    public string Text(string key) => document.Fields.TryGetValue(key, out var value) ? value.Trim() : throw new ArgumentException($"Parametro mancante: {key}");
    public static double Number(string text)
    {
        if (!double.TryParse(text.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out var value) || !double.IsFinite(value)) throw new ArgumentException($"Valore numerico non valido: {text}");
        return value;
    }
    public double Number(string key, double min, double max = double.MaxValue)
    {
        var value = Number(Text(key));
        if (value < min || value > max) throw new ArgumentException($"{key}: valore fuori intervallo [{min}, {max}].");
        return value;
    }
    public T Choice<T>(string key) where T : struct, Enum => Enum.Parse<T>(Text(key));
    public double[][] Rows(int columns)
    {
        var lines = document.Data.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length == 0 || lines.Length > 10000) throw new ArgumentException("Inserire da 1 a 10000 righe.");
        return lines.Select((line, i) => { var cells = line.Split(';'); if (cells.Length != columns) throw new ArgumentException($"Riga {i + 1}: attese {columns} colonne separate da ;"); return cells.Select(Number).ToArray(); }).ToArray();
    }
}

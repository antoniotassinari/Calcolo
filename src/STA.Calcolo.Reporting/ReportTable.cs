namespace STA.Calcolo.Reporting;
public sealed record ReportTable(string Title,string[] Headers,IReadOnlyList<string[]> Rows);

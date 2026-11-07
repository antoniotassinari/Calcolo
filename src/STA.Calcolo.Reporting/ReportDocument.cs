namespace STA.Calcolo.Reporting;
public sealed record ReportDocument(string Title,DateTime Created,IReadOnlyList<ReportPage> Pages);

namespace STA.Calcolo.Reporting;
public sealed record ReportGroup(string Title,IReadOnlyList<ReportLine> Lines);

namespace STA.Calcolo.Reporting;
public sealed record ReportPage(string Title,IReadOnlyList<ReportTable> Tables,IReadOnlyList<ReportCard> Cards,IReadOnlyList<string> Notes,string Accent="192845",IReadOnlyList<ReportEquation>? Equations=null);

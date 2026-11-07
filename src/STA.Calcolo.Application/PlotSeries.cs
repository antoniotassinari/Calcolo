namespace STA.Calcolo.Application;

public sealed record PlotSeries(string Name, IReadOnlyList<PlotPoint> Points, bool Closed = false, bool Markers = false);

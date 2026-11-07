namespace STA.Calcolo.Application;

public interface IPlotWorkspace
{
    string PlotCaption { get; }
    bool EqualAxes { get; }
    IReadOnlyList<PlotSeries> Plot(ModuleDocument document, IReadOnlyList<ResultRow> results);
}

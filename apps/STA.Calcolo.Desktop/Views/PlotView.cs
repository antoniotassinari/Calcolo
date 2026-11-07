using System.Globalization;
using System.Windows;
using System.Windows.Media;
using STA.Calcolo.Application;
namespace STA.Calcolo.Desktop.Views;
// Rendering only. Coordinates and calculated series are supplied by the application service.
public sealed class PlotView : FrameworkElement
{
    public static readonly DependencyProperty SeriesProperty = DependencyProperty.Register(nameof(Series), typeof(IReadOnlyList<PlotSeries>), typeof(PlotView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
    public static readonly DependencyProperty EqualAxesProperty = DependencyProperty.Register(nameof(EqualAxes), typeof(bool), typeof(PlotView), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
    public IReadOnlyList<PlotSeries>? Series { get => (IReadOnlyList<PlotSeries>?)GetValue(SeriesProperty); set => SetValue(SeriesProperty, value); }
    public bool EqualAxes { get => (bool)GetValue(EqualAxesProperty); set => SetValue(EqualAxesProperty, value); }
    protected override void OnRender(DrawingContext dc)
    {
        base.OnRender(dc); dc.DrawRectangle(Brushes.White, null, new Rect(RenderSize));
        var points = Series?.SelectMany(s => s.Points).ToArray(); if (points == null || points.Length == 0 || ActualWidth < 150 || ActualHeight < 100) return;
        double left = 60, top = 30, width = ActualWidth - 85, height = ActualHeight - 65;
        double minX = points.Min(p => p.X), maxX = points.Max(p => p.X), minY = points.Min(p => p.Y), maxY = points.Max(p => p.Y);
        double rangeX = Math.Max(maxX - minX, 1e-6), rangeY = Math.Max(maxY - minY, 1e-6); minX -= rangeX * .08; minY -= rangeY * .08; rangeX *= 1.16; rangeY *= 1.16;
        double sx = width / rangeX, sy = height / rangeY; if (EqualAxes) { sx = sy = Math.Min(sx, sy); left += (width - rangeX * sx) / 2; top += (height - rangeY * sy) / 2; }
        Point Map(PlotPoint p) => new(left + (p.X - minX) * sx, top + (minY + rangeY - p.Y) * sy);
        void Text(string value, double x, double y, Brush color) => dc.DrawText(new FormattedText(value, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Segoe UI"), 11, color, VisualTreeHelper.GetDpi(this).PixelsPerDip), new(x, y));
        for (int n = 0; n <= 4; n++)
        {
            double x = minX + rangeX * n / 4, y = minY + rangeY * n / 4; var p = Map(new(x, minY)); var q = Map(new(minX, y));
            dc.DrawLine(new Pen(Brushes.Gainsboro, 1), Map(new(x, minY)), Map(new(x, minY + rangeY)));
            dc.DrawLine(new Pen(Brushes.Gainsboro, 1), Map(new(minX, y)), Map(new(minX + rangeX, y)));
            Text(x.ToString("0.##"), p.X - 10, p.Y + 5, Brushes.DimGray); Text(y.ToString("0.##"), q.X - 48, q.Y - 7, Brushes.DimGray);
        }
        Brush[] colors = [Brushes.SteelBlue, Brushes.DarkOrange, Brushes.SeaGreen, Brushes.MediumVioletRed]; int index = 0;
        foreach (var series in Series!)
        {
            var color = colors[index % colors.Length]; Text(series.Name, 10 + index * 110, 6, color); index++;
            if (!series.Markers && series.Points.Count > 1) { var geometry = new StreamGeometry(); using (var context = geometry.Open()) { context.BeginFigure(Map(series.Points[0]), false, series.Closed); context.PolyLineTo(series.Points.Skip(1).Select(Map).ToArray(), true, false); } dc.DrawGeometry(null, new Pen(color, 2), geometry); }
            if (series.Markers || series.Closed) foreach (var p in series.Points) dc.DrawEllipse(color, null, Map(p), 4, 4);
        }
    }
}

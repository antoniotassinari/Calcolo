using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using STA.Calcolo.Desktop;

internal static class Program
{
 [STAThread]
 static int Main()
 {
  var app = new Application(); var window = new MainWindow();
  void Invoke(string method) => typeof(MainWindow).GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(window, [window, new RoutedEventArgs()]);
  Invoke("LoadExample"); Invoke("Calculate");
  var status = (TextBlock)window.FindName("Status"); var grid = (DataGrid)window.FindName("ResultsGrid");
  if (grid.Items.Count != 1 || !status.Text.StartsWith("Calcolate 1")) throw new Exception(status.Text);
  if (!((Button)window.FindName("ExportButton")).IsEnabled) throw new Exception("Export disabled");
  var content=(FrameworkElement)window.Content;
  content.Measure(new Size(1240,850));content.Arrange(new Rect(0,0,1240,850));content.UpdateLayout();
  var bitmap=new RenderTargetBitmap(1240,850,96,96,PixelFormats.Pbgra32);bitmap.Render(content);
  System.IO.Directory.CreateDirectory("artifacts");using(var file=System.IO.File.Create("artifacts/desktop-smoke.png")){var encoder=new PngBitmapEncoder();encoder.Frames.Add(BitmapFrame.Create(bitmap));encoder.Save(file);}
  var parameters=(DataGrid)window.FindName("ParametersGrid");
  var row=parameters.Items.Cast<MainWindow.ParameterRow>().Single(p=>p.Key=="qRd");row.Value="non valido";Invoke("Calculate");
  if (grid.Items.Count != 0 || ((Button)window.FindName("ExportButton")).IsEnabled) throw new Exception("Stale result after invalid input");
  window.Close();app.Shutdown();Console.WriteLine("PASS WPF: esempio, calcolo, rendering, invalidazione risultati");return 0;
 }
}

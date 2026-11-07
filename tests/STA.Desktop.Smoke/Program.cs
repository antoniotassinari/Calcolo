using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using STA.Calcolo.Application;
using STA.Calcolo.Desktop;
using STA.Calcolo.Desktop.Views;
using STA.Calcolo.Desktop.ViewModels;
using STA.Calcolo.Desktop.Services;
internal static class Program
{
    [STAThread]
    static int Main()
    {
        var app = new System.Windows.Application { ShutdownMode = ShutdownMode.OnExplicitShutdown }; var files = new MemoryFileService(); var windows = new WindowService(files); var mainVm = new MainViewModel(windows); var main = new MainWindow { DataContext = mainVm }; app.MainWindow = main;
        foreach (var module in WorkspaceCatalog.Create())
        {
            var vm = new WorkspaceViewModel(module, files); vm.CalculateCommand.Execute(null);
            if (vm.Results.Count == 0) throw new Exception(module.Id + ": " + vm.Status);
            files.NextSave = module.Id + ".json"; vm.SaveCommand.Execute(null); var saved = files.Content[files.NextSave];
            files.NextSave = module.Id + "-results.json"; vm.ExportCommand.Execute(null); if (!files.Content[files.NextSave].Contains("results")) throw new Exception("Export failed");
            var original = vm.Fields[0].Value; vm.Fields[0].Value = "changed"; files.NextOpen = module.Id + ".json"; vm.OpenCommand.Execute(null); if (vm.Fields[0].Value != original || vm.Results.Count != 0) throw new Exception("Round trip failed");
            vm.CalculateCommand.Execute(null);
            var window = new WorkspaceWindow { DataContext = vm }; Render(window, "module-" + module.Id); window.Close();
            vm.Fields[0].Value += " invalid"; if (vm.Results.Count != 0 || vm.ExportCommand.CanExecute(null)) throw new Exception("Stale results: " + module.Id);
            Console.WriteLine("PASS WPF " + module.Id);
        }
        var footing = new FootingViewModel(files); footing.ExampleCommand.Execute(null); footing.CalculateCommand.Execute(null);
        if (footing.Results.Length != 1 || !footing.ExportCommand.CanExecute(null)) throw new Exception(footing.Status);
        files.NextSave = "footing.json"; footing.SaveCommand.Execute(null); files.NextSave = "footing-results.json"; footing.ExportCommand.Execute(null);
        if (files.Content["footing-results.json"].Contains("ApiKey")) throw new Exception("Credential persisted");
        files.NextOpen = "footing.json"; footing.OpenCommand.Execute(null); footing.CalculateCommand.Execute(null);
        var beam = new BeamViewModel(files, windows); files.NextOpen = "sezione.json"; beam.ImportSectionCommand.Execute(null); files.NextOpen = "trefolatura.json"; beam.ImportTendonCommand.Execute(null);
        if (beam.Sections.Count != 1 || beam.Tendons.Count != 1) throw new Exception(beam.Status);
        files.NextSave = "beam.json"; beam.SaveCommand.Execute(null); beam.SelectedSection = beam.Sections[0]; beam.RemoveSectionCommand.Execute(null); if (beam.Sections.Count != 0) throw new Exception("Remove failed");
        files.NextOpen = "beam.json"; beam.OpenCommand.Execute(null); beam.CheckCommand.Execute(null); if (beam.Sections.Count != 1) throw new Exception(beam.Status);
        var bw = new BeamWindow { DataContext = beam }; Render(bw, "beam"); bw.Close();
        var fw = new FootingWindow { DataContext = footing }; Render(fw, "plinti"); fw.Close();
        footing.Parameters.Single(p => p.Key == "qRd").Value = "invalid"; if (footing.Results.Length != 0 || footing.ExportCommand.CanExecute(null)) throw new Exception("Stale footing results");
        footing.CalculateCommand.Execute(null); if (footing.Results.Length != 0) throw new Exception("Invalid footing result");
        Render(main, "desktop-smoke"); main.Show();
        foreach (var card in mainVm.Modules) { card.OpenCommand.Execute(null); card.OpenCommand.Execute(null); }
        if (main.OwnedWindows.Count != mainVm.Modules.Count) throw new Exception("Navigation duplicated windows");
        var sample = main.OwnedWindows.Cast<Window>().First(w => w.DataContext is WorkspaceViewModel); var context = sample.DataContext; sample.Close(); mainVm.Modules[0].OpenCommand.Execute(null); if (!main.OwnedWindows.Cast<Window>().Any(w => ReferenceEquals(w.DataContext, context))) throw new Exception("Reopen lost project");
        foreach (Window child in main.OwnedWindows.Cast<Window>().ToArray()) child.Close();
        main.Close(); app.Shutdown(); Console.WriteLine("PASS MVVM: navigation, rendering, calculations, invalidation"); return 0;
    }
    static void Render(Window window, string name)
    {
        window.Show(); window.UpdateLayout();
        var content = (FrameworkElement)window.Content; content.Measure(new Size(1180, 820)); content.Arrange(new Rect(0, 0, 1180, 820)); content.UpdateLayout();
        var bitmap = new RenderTargetBitmap(1180, 820, 96, 96, PixelFormats.Pbgra32); bitmap.Render(content); System.IO.Directory.CreateDirectory("artifacts"); using var file = System.IO.File.Create("artifacts/" + name + ".png"); var encoder = new PngBitmapEncoder(); encoder.Frames.Add(BitmapFrame.Create(bitmap)); encoder.Save(file);
    }
}
using System.IO;
using STA.Calcolo.Reporting;
using STA.Muratura.Core;
namespace STA.Calcolo.Desktop.Services;
public sealed class MasonryReportService : IMasonryReportService
{
    private static byte[] Logo()
    {
        using var stream=System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/STA.Calcolo.Desktop;component/Assets/Brand/Logo.png"))!.Stream;
        using var memory=new MemoryStream();stream.CopyTo(memory);return memory.ToArray();
    }
    public byte[] Word(MasonryProject project)=>WordReportWriter.Write(MasonryReportBuilder.Build(project,DateTime.Now),Logo());
    public byte[] Pdf(MasonryProject project)
    {
        var fonts=Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
        return PdfReportWriter.Write(MasonryReportBuilder.Build(project,DateTime.Now),Logo(),File.ReadAllBytes(Path.Combine(fonts,"arial.ttf")),File.ReadAllBytes(Path.Combine(fonts,"arialbd.ttf")));
    }
}

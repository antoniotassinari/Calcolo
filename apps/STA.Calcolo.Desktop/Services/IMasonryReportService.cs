using STA.Muratura.Core;
namespace STA.Calcolo.Desktop.Services;
public interface IMasonryReportService
{
    byte[] Word(MasonryProject project);
    byte[] Pdf(MasonryProject project);
}

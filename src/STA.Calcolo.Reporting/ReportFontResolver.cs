using PdfSharp.Fonts;
namespace STA.Calcolo.Reporting;
public sealed class ReportFontResolver(byte[] regular,byte[] bold) : IFontResolver
{
    public byte[] GetFont(string faceName)=>faceName=="ReportBold"?bold:regular;
    public FontResolverInfo ResolveTypeface(string familyName,bool isBold,bool isItalic)=>new(isBold?"ReportBold":"ReportRegular");
}

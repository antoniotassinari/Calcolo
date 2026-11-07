using System.IO.Compression;
using System.Xml.Linq;
using PdfSharp.Pdf.IO;
using STA.Calcolo.Reporting;
using STA.Muratura.Core;
using STA.Materiali;

void Assert(bool condition,string message){if(!condition)throw new Exception(message);}
var logo=File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory,"Logo.png"));
string fontDirectory=OperatingSystem.IsWindows()?Environment.GetFolderPath(Environment.SpecialFolder.Fonts):"/usr/share/fonts/truetype/liberation2";
if(!Directory.Exists(fontDirectory))fontDirectory="/usr/share/fonts/truetype/liberation";
var regular=File.ReadAllBytes(Path.Combine(fontDirectory,OperatingSystem.IsWindows()?"arial.ttf":"LiberationSans-Regular.ttf"));
var bold=File.ReadAllBytes(Path.Combine(fontDirectory,OperatingSystem.IsWindows()?"arialbd.ttf":"LiberationSans-Bold.ttf"));
var example=MasonryFiles.Example();var many=new MasonryProject{Title="Confronto completo con dieci maschi prima e dieci dopo"};
many.Before=Enumerable.Range(0,10).Select(i=>{var p=MasonryCalculator.Preset(new(),MasonryCatalog.All[i%8].Id,i%2==0?"LC1":"LC2");p.Rho??=1400;p.Cracked=i%2==0;p.LoadMode="linear";p.Q=45+i;return p;}).ToArray();
many.After=many.Before.Select((p,i)=>p with {L=200+i*20,C=[1.1,1.2,1,1,.9,1,1]}).ToArray();many.Frame=example.Frame with {Profile="HEB 1000"};
foreach(var (name,p) in new[]{("esempio",example),("20-maschi",many),("senza-telaio",example with {Frame=example.Frame with {Enabled=false}})})
{
    var data=MasonryReportBuilder.Build(p,new(2026,11,7));var compare=MasonryReportBuilder.Compare(p);
    Assert(compare.Length==2&&MasonryReportBuilder.Piers(p).Length==p.Before.Length+p.After.Length,"Missing rows");
    Assert(compare.All(r=>Math.Abs(r.Finale-r.Dopo-r.Telaio)<1e-6),"Final equilibrium");
    Assert(data.Pages.Sum(page=>page.Cards.Count)==p.Before.Length+p.After.Length,"Missing pier cards");
    var docx=WordReportWriter.Write(data,logo);using var zip=new ZipArchive(new MemoryStream(docx));
    using var xml=zip.GetEntry("word/document.xml")!.Open();var doc=XDocument.Load(xml);XNamespace w="http://schemas.openxmlformats.org/wordprocessingml/2006/main";
    Assert(doc.Descendants(w+"pgSz").Single().Attribute(w+"w")!.Value=="11906","Not A4");
    Assert(doc.Descendants(w+"pgMar").Single().Attribute(w+"left")!.Value=="850","Wrong margin");
    Assert(doc.Descendants(w+"br").Count()==data.Pages.Count-1,"Wrong pagination");
    XNamespace m="http://schemas.openxmlformats.org/officeDocument/2006/math";
    Assert(doc.Descendants(m+"f").Count()>=6,"Missing editable engineering fractions");
    Assert(data.Pages[1].Equations?.Count>0,"Derivation must follow summary");
    Assert(data.Pages.SelectMany(page=>page.Equations??[]).Any(e=>e.Title.StartsWith("(15)")&&e.Substitution.Contains(MasonryReportBuilder.Number(compare[1].Variazione))),"Missing numerical comparison");
    Assert(!doc.ToString().Contains("fmBase"),"JSON dump in report");
    var pdf=PdfReportWriter.Write(data,logo,regular,bold);using var opened=PdfReader.Open(new MemoryStream(pdf),PdfDocumentOpenMode.Import);
    Assert(opened.PageCount==data.Pages.Count,"Wrong PDF page count");
    Assert(opened.Pages.Cast<PdfSharp.Pdf.PdfPage>().All(page=>Math.Abs(page.Width.Point-595)<1&&Math.Abs(page.Height.Point-842)<1),"PDF not A4");
    Directory.CreateDirectory("artifacts/reports");File.WriteAllBytes($"artifacts/reports/test-{name}.pdf",pdf);File.WriteAllBytes($"artifacts/reports/test-{name}.docx",docx);
    Console.WriteLine($"PASS relazione {name}: {data.Pages.Count} pagine A4, dati, margini, DOCX modificabile e PDF");
}

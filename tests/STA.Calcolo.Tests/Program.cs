using System.Net;
using System.Text.Json;
using STA.Calcolo.Contracts;
using STA.Calcolo.IO;
using STA.Integrations.Midas;
using STA.Plinti.Core;
using STA.Geometria;
using STA.Geometria.Masse;

var request = CalculationFiles.ReadRequest(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "plinti-example.json")));
var p = request.Parameters; var r = request.Reactions[0]; int count = 0;
void Test(string name, Action test) { test(); count++; Console.WriteLine("PASS " + name); }
void Assert(bool value) { if (!value) throw new Exception("Assertion failed"); }
void Near(double actual, double expected) { if (Math.Abs(actual - expected) > 1e-8) throw new Exception($"{actual} != {expected}"); }
void Reject(Action action) { bool rejected = false; try { action(); } catch (Exception e) when (e is ArgumentException or InvalidDataException or JsonException or FormatException) { rejected = true; } Assert(rejected); }
FootingResult Check(NodeReaction row, FootingParameters? parameters = null) => FootingCalculator.Calculate([row], parameters ?? p)[0];
Test("equilibrio e pressione indipendenti", () => { var c = Check(r); Near(c.Weight,112.5); Near(c.N,1046.25); Near(c.QEff,116.25); Near(c.QMin,116.25); Assert(c.Ok); });
Test("trasporto reazioni e momenti", () => { var c=Check(r with {FX=10,FY=20,MX=30,MY=40});Near(c.Mx,-20);Near(c.My,-45);Near(c.Ex,45/c.N); });
Test("eccentricita biassiale", () => { var c=Check(r with {MX=90,MY=120},p with {Density=0});Near(c.Be,3-240d/900);Near(c.Le,2.8);Near(c.QMin,100-720d/27-540d/27); });
Test("sollevamento e contatto parziale", () => { foreach(var row in new[]{r with {FZ=-1000},r with {MY=2000},r with {MY=700}})Assert(!Check(row).Ok); });
Test("torsione e scorrimento", () => { Assert(!Check(r with {MZ=1}).Ok);Assert(!Check(r with {FX=1000}).SoilOK); });
Test("SLE usa qmax", () => { var c=Check(r with {MY=100},p with {LimitState="SLS",QRd=120});Assert(!c.SoilOK && c.ConcreteOK is null); });
Test("validazione input", () => { foreach(var invalid in new[]{p with {QRd=double.NaN},p with {B=0},p with {H=.01},p with {Cx=5},p with {Fck=90},p with {Spacing=0}})Reject(()=>Check(r,invalid));Reject(()=>Check(r with {FX=double.PositiveInfinity})); });
Test("invilluppi e duplicati", () => { foreach(var name in new[]{"C1(CB:max)","C1(CB:min)","C1(CB:all)","EQ(RS)","MOV(MV)",""})Reject(()=>Check(r with {Case=name}));Reject(()=>FootingCalculator.Calculate([r,r],p)); });
Test("ricerca base quadrata", () => { var d=FootingCalculator.Design([r,r with {Node=2,FZ=600}],p,4,.1);Assert(d.Count==2 && d.All(v=>v.Found));Reject(()=>FootingCalculator.Design([r],p,100,.001)); });
Test("punzonamento parziale", () => {Assert(!Check(r with {MY=100},p with {Beta=1}).Ok);Assert(!Check(r,p with {H=1}).Ok);});
Test("JSON versionato e round trip", () => { var copy=CalculationFiles.ReadRequest(CalculationFiles.Write(request));Near(Check(copy.Reactions[0],copy.Parameters).N,1046.25);Reject(()=>CalculationFiles.ReadRequest("{}"));Reject(()=>new FootingModule().Calculate(request with {SchemaVersion=2})); });
Test("CSV italiano con separatore e punto decimale", () => {var rows=CalculationFiles.ReadCsv("node;case;FX;FY;FZ;MX;MY;MZ\n1;ULS01(CB);0;0;900.5;0;0;0");Near(rows[0].FZ,900.5);Reject(()=>CalculationFiles.ReadCsv("node,case,FX,FY,FZ,MX,MY,MZ\n1,X,0,0,NaN,0,0,0"));});
Test("output JSON non applicabile", () => {using var doc=JsonDocument.Parse(CalculationFiles.Write(Check(r with {FZ=-10000})));Assert(doc.RootElement.GetProperty("qEff").ValueKind==JsonValueKind.Null);});
Test("geometria e sezioni interoperabili", () => {var section=new Sezione {Vertici=[new Punto2D{X=0,Y=0},new Punto2D{X=2,Y=0},new Punto2D{X=2,Y=3},new Punto2D{X=0,Y=3}]};section.RicalcolaProprietà();Near(Math.Abs(section.Area),6);Near(section.Baricentro.X,1);Near(section.Baricentro.Y,1.5);section.Vertici.Reverse();section.RicalcolaProprietà();Near(section.Baricentro.X,1);Near(section.Baricentro.Y,1.5);});
Test("baricentro triangolo e sezione degenere",()=>{var f=STA.Geometria.Masse.Functions.Functions.calcolaBaricentro(new(){new(){X=1,Y=1},new(){X=4,Y=1},new(){X=1,Y=4}});Near(f.X,2);Near(f.Y,2);Reject(()=>STA.Geometria.Masse.Functions.Functions.calcolaBaricentro(new(){new(){X=0,Y=0},new(){X=1,Y=1},new(){X=2,Y=2}}));});
Test("risorsa sismica presente", () => {using var stream=typeof(STA.Carichi.Sisma.Sisma).Assembly.GetManifestResourceStream("STA.Carichi.Sisma.TabellaParametriSpettrali.csv");Assert(stream is not null && stream.Length>1000);});
const string table="""{"REACTIONG":{"HEAD":["Load Case","FZ [kN]","Node","FX","FY","MX","MY","MZ"],"DATA":[["ULS01(CB)",900,1,0,0,0,0,0]]}}""";
Test("parser MIDAS colonne riordinate",()=>{using var doc=JsonDocument.Parse(table);var rows=MidasReactionSource.ParseTable(doc.RootElement);Near(rows[0].FZ,900);Assert(rows[0].Node==1);});
Test("parser MIDAS rifiuta righe malformate",()=>{foreach(var bad in new[]{table.Replace("900,1","900,1.5"),table.Replace("\"MZ\"","\"MX\""),table.Replace("900,1,0,0,0,0,0","900,1,0,0,0,0")})Reject(()=>{using var doc=JsonDocument.Parse(bad);MidasReactionSource.ParseTable(doc.RootElement);});});
var handler=new FakeHandler(table);
using(var source=new MidasReactionSource("https://example.test/gen","test-only",handler))
{
 var batch=await source.ReadAsync(new([1],["ULS01(CB)"]));
 Test("MIDAS importa reazioni attraverso contratto comune",()=>{Near(new FootingModule().Calculate(request with {Reactions=batch.Rows}).Results[0].N,1046.25);Assert(handler.Calls==2);Assert(!CalculationFiles.Write(batch).Contains("test-only"));});
 bool rejected=false;try{await source.ReadAsync(new([1],["ULS01(CB)","MISSING(CB)"]));}catch(InvalidDataException){rejected=true;}Test("MIDAS rifiuta combinazioni mancanti",()=>Assert(rejected));
}
Test("MIDAS URL e chiave",()=>{Reject(()=>new MidasReactionSource("http://example.test","key"));Reject(()=>new MidasReactionSource("https://example.test",""));});
Console.WriteLine($"{count} test superati");

sealed class FakeHandler(string table) : HttpMessageHandler
{
 public int Calls {get;private set;}
 protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,CancellationToken ct)
 {
  Calls++;
  if(!request.Headers.Contains("MAPI-Key"))throw new Exception("Missing key");
  if(request.RequestUri!.AbsolutePath.EndsWith("/db/NODE"))
  {if(request.Method!=HttpMethod.Get)throw new Exception("Unexpected write");return new(HttpStatusCode.OK){Content=new StringContent("{\"NODE\":{\"1\":{}}}")};}
  if(request.Method!=HttpMethod.Post || !request.RequestUri.AbsolutePath.EndsWith("/post/TABLE"))throw new Exception("Unexpected endpoint");
  var body=await request.Content!.ReadAsStringAsync(ct);using var doc=JsonDocument.Parse(body);var arg=doc.RootElement.GetProperty("Argument");
  if(arg.GetProperty("TABLE_TYPE").GetString()!="REACTIONG" || arg.GetProperty("UNIT").GetProperty("FORCE").GetString()!="KN")throw new Exception("Wrong request contract");
  return new(HttpStatusCode.OK){Content=new StringContent(table)};
 }
}

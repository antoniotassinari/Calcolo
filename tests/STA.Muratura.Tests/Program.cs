using System.Text.Json;
using STA.Muratura.Core;
using STA.Materiali;
using STA.Profilati.Core;

int count=0;
void Near(double actual,double expected){if(!double.IsFinite(actual)||Math.Abs(actual-expected)>1e-10*Math.Max(1,Math.Abs(expected)))throw new Exception($"{actual} != {expected}");}
void Assert(bool b){if(!b)throw new Exception("Assertion failed");}
void Reject(Action f){try{f();}catch(Exception e)when(e is ArgumentException or JsonException or InvalidOperationException){return;}throw new Exception("Invalid input accepted");}
void CheckState(StateResult actual,JsonElement expected)
{
    Near(actual.K,expected.GetProperty("K").GetDouble());Near(actual.Vt,expected.GetProperty("Vt").GetDouble());Near(actual.Du,expected.GetProperty("du").GetDouble());Near(actual.N,expected.GetProperty("N").GetDouble());
    int i=0;foreach(var row in expected.GetProperty("rows").EnumerateArray()){var r=actual.Rows[i++];foreach(var (value,key) in new[]{(r.N,"N"),(r.A,"A"),(r.Sigma,"sigma"),(r.B,"b"),(r.K,"K"),(r.Vt,"Vt"),(r.Du,"du"),(r.Dy,"dy")})Near(value,row.GetProperty(key).GetDouble());}
}
using var fixtures=JsonDocument.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,"reference.json")));
foreach(var c in fixtures.RootElement.EnumerateArray())
{
    var p=MasonryFiles.Read(c.GetProperty("input").GetRawText());var a=MasonryCalculator.Total(p.Before);var b=MasonryCalculator.Total(p.After);CheckState(a,c.GetProperty("before"));CheckState(b,c.GetProperty("after"));
    var f=FrameCalculator.Calculate(p.Frame,a,b);
    if(f!=null)
    {
        var e=c.GetProperty("frame");foreach(var (value,key) in new[]{(f.J,"J"),(f.W,"W"),(f.V,"V"),(f.K,"K"),(f.Dy,"dy"),(f.Veff,"Veff"),(f.FinalV,"finalV"),(f.FinalK,"finalK"),(f.DV,"dV"),(f.DK,"dK")})Near(value,e.GetProperty(key).GetDouble());
        if(f.Wstar is double w)Near(w,e.GetProperty("Wstar").GetDouble());
        var design=FrameCalculator.Design(p.Frame,a,b);int i=0;foreach(var family in c.GetProperty("design").GetProperty("families").EnumerateArray()){var d=design[i++];Assert(d.ValidCount==family.GetProperty("validCount").GetInt32());Assert(d.CheckedCount==family.GetProperty("checkedCount").GetInt32());var candidate=family.GetProperty("candidate");Assert(d.Candidate?.Profile.Id==(candidate.ValueKind==JsonValueKind.Null?null:candidate.GetProperty("p").GetProperty("id").GetString()));}
    }
    var copy=MasonryFiles.Read(MasonryFiles.Write(p));Near(MasonryCalculator.Total(copy.Before).K,a.K);count++;
}
foreach(var m in MasonryCatalog.All)foreach(var lc in new[]{"LC1","LC2"}){var p=MasonryCalculator.Preset(new(),m.Id,lc);Near(p.FmBase,(lc=="LC1"?m.F[0]:m.F.Average())*100);Near(p.E,m.E.Average());Assert(p.Rho==m.Weight*100);}
var sample=MasonryFiles.Example();
Reject(()=>MasonryCalculator.Calculate(sample.Before[0] with {H=0}));Reject(()=>MasonryCalculator.Calculate(sample.Before[0] with {N=-1}));Reject(()=>MasonryCalculator.Calculate(sample.Before[0] with {Rho=null}));Reject(()=>MasonryCalculator.Calculate(sample.Before[0] with {C=[1]}));Reject(()=>MasonryCalculator.Total([]));Reject(()=>MasonryCalculator.Total(Enumerable.Repeat(new MasonryPier(),11).ToArray()));
Reject(()=>MasonryFiles.Read("{}"));Reject(()=>MasonryFiles.Read(MasonryFiles.Write(sample).Replace("\"h\": 340","\"omitted\": 340")));
Reject(()=>MasonryFiles.Read(MasonryFiles.Write(sample).Replace("\"version\": 2","\"version\": 99")));
var old=MasonryFiles.Write(sample).Replace("\"fmBase\": 260","\"fm\": 192.59259259259258").Replace("\"tauBase\": 5","\"tau\": 3.7037037037037033").Replace("\"materialVersion\": 2","\"materialVersion\": 1");
Near(MasonryCalculator.Total(MasonryFiles.Read(old).Before).K,MasonryCalculator.Total(sample.Before).K);
Assert(ProfileCatalog.All.Count==66);Assert(MasonryCatalog.All.Count==8);Assert(FrameCalculator.StiffnessAccepts(15));Assert(!FrameCalculator.StiffnessAccepts(15.00001));
Console.WriteLine($"PASS {count} confronti numerici JavaScript/C#, cataloghi condivisi, importazione legacy, round trip e dati non validi");

var beforeMulti=MasonryCalculator.Total(sample.Before);var afterMulti=MasonryCalculator.Total(sample.After);
var first=sample.Frame with {Enabled=true,Profile="HEA 180"};
var second=first with {Profile="IPE 200",H=240};
var one=FrameCalculator.Calculate(first,beforeMulti,afterMulti)!;
var two=FrameCalculator.Calculate(second,beforeMulti,afterMulti)!;
var group=FrameGroupCalculator.Calculate([first,second],beforeMulti,afterMulti);
Near(group.FinalK,afterMulti.K+one.K+two.K);Near(group.FinalV,afterMulti.Vt+one.Veff+two.Veff);
Near(FrameGroupCalculator.Calculate([first,second with {Enabled=false}],beforeMulti,afterMulti).FinalK,one.FinalK);
var multiple=sample with {Version=3,Frame=first,AdditionalFrames=[second]};
var roundtrip=MasonryFiles.Read(MasonryFiles.Write(multiple));
Assert(roundtrip.Version==3&&roundtrip.AdditionalFrames.Single().Profile=="IPE 200");
Near(FrameGroupCalculator.Calculate(roundtrip.AllFrames,beforeMulti,afterMulti).FinalV,group.FinalV);
Reject(()=>MasonryFiles.Validate(multiple with {AdditionalFrames=[second with {H=0}]}));
Reject(()=>MasonryFiles.Read(MasonryFiles.Write(multiple).Replace("\"additionalFrames\": [","\"additionalFrames\": [null,")));
// Search each family against an independent exhaustive sum of the other frame and candidate.
var proposals=FrameCalculator.Design(first,beforeMulti,FrameGroupCalculator.IncludeOthers([second],beforeMulti,afterMulti));
foreach(var proposal in proposals)
{
    var valid=ProfileCatalog.All.Where(p=>p.Family==proposal.Family).OrderBy(p=>p.Size).Where(p=>{
        var candidate=FrameCalculator.Calculate(first with {Profile=p.Id},beforeMulti,afterMulti)!;
        return FrameCalculator.StiffnessAccepts(((afterMulti.K+two.K+candidate.K)/beforeMulti.K-1)*100)&&afterMulti.Vt+two.Veff+candidate.Veff>beforeMulti.Vt;
    }).ToArray();
    Assert(proposal.ValidCount==valid.Length);Assert(proposal.Candidate?.Profile.Id==valid.FirstOrDefault()?.Id);
    if(proposal.Candidate is {} candidate){Near(candidate.FinalK,afterMulti.K+two.K+candidate.K);Near(candidate.FinalV,afterMulti.Vt+two.Veff+candidate.Veff);}
}
var huge=second with {Profile="HEB 1000",NProfiles=1000};
Assert(FrameCalculator.Design(first,beforeMulti,FrameGroupCalculator.IncludeOthers([huge],beforeMulti,afterMulti)).All(p=>p.Candidate==null));
Console.WriteLine("PASS cerchiature multiple: contributi, esclusione, JSON e ricerca con altri telai");

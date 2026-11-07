using System.Globalization;
using STA.Calcolo.Application;
using STA.Calcolo.Desktop.Infrastructure;
namespace STA.Calcolo.Desktop.ViewModels;

public sealed class MasonryFieldsViewModel : ObservableObject
{
    public IReadOnlyList<ModuleFieldViewModel> Fields {get;}
    public MasonryFieldsViewModel(object model, (string Key,string Label,string[]? Choices)[] definitions)
    {
        Fields=definitions.Select(d=>new ModuleFieldViewModel(new(d.Key,d.Label,Display(Convert.ToString(model.GetType().GetProperty(d.Key)!.GetValue(model),CultureInfo.InvariantCulture)??""),d.Choices))).ToArray();
        foreach(var f in Fields)f.PropertyChanged+=(_,_)=>Changed(nameof(Fields));
    }
    private static string Display(string value)=>value switch {"True"=>"Sì","False"=>"No","total"=>"Risultante N","linear"=>"Distribuito q","closed"=>"Chiuso","open"=>"Aperto",_=>value};
    public void Apply(object model)
    {
        foreach(var f in Fields)
        {
            var valueText=f.Value switch {"Sì"=>"True","No"=>"False","Risultante N"=>"total","Distribuito q"=>"linear","Chiuso"=>"closed","Aperto"=>"open",_=>f.Value};
            var p=model.GetType().GetProperty(f.Key)!;var type=Nullable.GetUnderlyingType(p.PropertyType)??p.PropertyType;
            if(Nullable.GetUnderlyingType(p.PropertyType)!=null && string.IsNullOrWhiteSpace(f.Value)){p.SetValue(model,null);continue;}
            object value=type==typeof(string)?valueText:type==typeof(bool)?bool.Parse(valueText):type==typeof(int)?(object)int.Parse(f.Value,CultureInfo.InvariantCulture):double.Parse(f.Value.Replace(',','.'),NumberStyles.Float,CultureInfo.InvariantCulture);
            p.SetValue(model,value);
        }
    }
}

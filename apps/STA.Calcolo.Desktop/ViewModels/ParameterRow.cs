using STA.Calcolo.Desktop.Infrastructure;
namespace STA.Calcolo.Desktop.ViewModels;

public sealed class ParameterRow : ObservableObject
{
    public string Key { get; init; } = "";
    public string Label { get; init; } = "";
    private string value = "";
    public string Value { get => value; set => Set(ref this.value, value); }
}

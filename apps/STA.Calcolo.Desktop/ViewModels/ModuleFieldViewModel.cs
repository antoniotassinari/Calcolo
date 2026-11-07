using STA.Calcolo.Application;
using STA.Calcolo.Desktop.Infrastructure;
namespace STA.Calcolo.Desktop.ViewModels;

public sealed class ModuleFieldViewModel(ModuleField definition) : ObservableObject
{
    public string Key => definition.Key; public string Label => definition.Label; public string[]? Choices => definition.Choices; public bool HasChoices => Choices != null;
    private string value = definition.Default;
    public string Value { get => value; set => Set(ref this.value, value); }
}

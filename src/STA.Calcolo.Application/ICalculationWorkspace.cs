namespace STA.Calcolo.Application;

public interface ICalculationWorkspace
{
    string Id { get; }
    string Title { get; }
    string Description { get; }
    IReadOnlyList<ModuleField> Fields { get; }
    string DataHint { get; }
    string ExampleData { get; }
    IReadOnlyList<ResultRow> Calculate(ModuleDocument document);
}

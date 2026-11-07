using System.Windows.Input;
namespace STA.Calcolo.Desktop.Infrastructure;

public sealed class AsyncCommand(Func<Task> action, Action<Exception> onError) : ICommand
{
    public bool IsRunning { get; private set; }
    public bool CanExecute(object? parameter) => !IsRunning;
    public event EventHandler? CanExecuteChanged;
    public async Task ExecuteAsync()
    {
        if (IsRunning) return;
        IsRunning = true; CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        try { await action(); }
        catch (Exception e) { onError(e); }
        finally { IsRunning = false; CanExecuteChanged?.Invoke(this, EventArgs.Empty); }
    }
    public async void Execute(object? parameter) => await ExecuteAsync();
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace STA_Dimensionamento_Plinti.Wpf
{
    public interface IStartupCheck
    {
        void VerifyInitialization();
    }

    public abstract class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void Raise([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        protected bool Set<T>(ref T field, T value, [CallerMemberName] string name = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            Raise(name);
            return true;
        }
    }

    public sealed class RelayCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Predicate<object> canExecute;
        public RelayCommand(Action execute, Func<bool> canExecute = null)
            : this(_ => execute(), canExecute == null ? null : new Predicate<object>(_ => canExecute()))
        {
            ArgumentNullException.ThrowIfNull(execute);
        }
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }
        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter) => canExecute?.Invoke(parameter) ?? true;
        public void Execute(object parameter)
        {
            if (CanExecute(parameter)) execute(parameter);
        }
        public void Refresh() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public sealed class InputException : Exception
    {
        public InputException(string message) : base(message) { }
    }

    public sealed class NumberField : ObservableObject
    {
        private string text;
        private string error = "";
        public string Key { get; }
        public string Label { get; }
        public string Text
        {
            get => text;
            set { if (Set(ref text, value ?? "")) Error = ""; }
        }
        public string Error { get => error; private set => Set(ref error, value); }
        public NumberField(string key, string label, string initial = "")
        {
            Key = key;
            Label = label;
            text = initial;
        }
        public double Read(bool positive = false, bool nonNegative = false)
        {
            if (!double.TryParse(Text, NumberStyles.Float, CultureInfo.CurrentCulture, out double value)
                || !double.IsFinite(value))
                return Fail("inserire un numero finito (separatore decimale: " + CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator + ").");
            if (positive && value <= 0) return Fail("il valore deve essere maggiore di zero.");
            if (nonNegative && value < 0) return Fail("il valore non può essere negativo.");
            Error = "";
            return value;
        }
        public int ReadCount()
        {
            double value = Read(positive: true);
            if (value != Math.Truncate(value) || value > 100)
                return (int)Fail("inserire un intero tra 1 e 100.");
            return (int)value;
        }
        private double Fail(string message)
        {
            Error = Label + ": " + message;
            throw new InputException(Error);
        }
        public void Load(double value) => Text = value.ToString("R", CultureInfo.CurrentCulture);
    }
}

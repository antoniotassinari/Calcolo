using System.Windows;
using System.Windows.Controls;
namespace STA.Calcolo.Desktop.Views;
// Presentation-only bridge: the key remains in memory and is never part of a project file.
public static class PasswordBinding
{
    public static readonly DependencyProperty ValueProperty = DependencyProperty.RegisterAttached("Value", typeof(string), typeof(PasswordBinding), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, Changed));
    public static string GetValue(DependencyObject obj) => (string)obj.GetValue(ValueProperty);
    public static void SetValue(DependencyObject obj, string value) => obj.SetValue(ValueProperty, value);
    private static void Changed(DependencyObject obj, DependencyPropertyChangedEventArgs e)
    {
        if (obj is not PasswordBox box) return;
        box.PasswordChanged -= Edited;
        if (box.Password != (string)e.NewValue) box.Password = (string)e.NewValue;
        box.PasswordChanged += Edited;
    }
    private static void Edited(object sender, RoutedEventArgs e) => ((PasswordBox)sender).SetCurrentValue(ValueProperty, ((PasswordBox)sender).Password);
}

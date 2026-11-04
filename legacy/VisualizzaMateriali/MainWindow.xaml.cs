using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CLS = Calcestruzzo;


namespace VisualizzaMateriali
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            gridProprietà.Children.Clear();
            gridProprietà.Children.Add(new CLS.ucPropCLS());
        }

        private void mnItemAcciaioArmatura_Click(object sender, RoutedEventArgs e)
        {
            gridProprietà.Children.Clear();
            gridProprietà.Children.Add(new Acciai.ucPropAcciaioArmatura());
        }
    }
}

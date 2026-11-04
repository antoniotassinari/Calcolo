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

namespace FinestraTest
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

        private void btVento_Click(object sender, RoutedEventArgs e)
        {
            controlloWPFVentoBase.ucVento propVento = new controlloWPFVentoBase.ucVento();
            Window win = new Window();
            win.Width = 500;
            win.Content = propVento;
            win.Show();

        }

        private void btMateriali_Click(object sender, RoutedEventArgs e)
        {
            VisualizzaMateriali.MainWindow win = new VisualizzaMateriali.MainWindow();

            win.ShowDialog();
        }

        private void btNeve_Click(object sender, RoutedEventArgs e)
        {
            STA.Carichi.Neve.ucCaricoNeve propNeve = new STA.Carichi.Neve.ucCaricoNeve();
            Window win = new Window();
            win.Content = propNeve;
            win.ShowDialog();
        }

        private void btSismica_Click(object sender, RoutedEventArgs e)
        {
            STA.Carichi.Sisma.ucSisma propSismca = new STA.Carichi.Sisma.ucSisma();
            Window win = new Window();
            win.Content = propSismca;
            win.ShowDialog();
        }

        private void btTravePref_Click(object sender, RoutedEventArgs e)
        {
            Window win = new Window();
            win.ShowDialog();
        }
    }
}

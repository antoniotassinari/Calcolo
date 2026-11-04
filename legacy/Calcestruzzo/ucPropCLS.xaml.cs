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


namespace Calcestruzzo
{
    /// <summary>
    /// Logica di interazione per ucPropCLS.xaml
    /// </summary>
    public partial class ucPropCLS : UserControl
    {
        /// <summary>
        /// Classe calcestruzzo abbinata al controllo
        /// </summary>
        public Calcestruzzo clsCorrente = new Calcestruzzo();

        public ClassiCLSToStringConverter convertCLS = new ClassiCLSToStringConverter();

        public ucPropCLS()
        {
            InitializeComponent();
            DataContext = clsCorrente;
        }

        private void cbClasse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            clsCorrente = new Calcestruzzo(clsCorrente.Classe);
            DataContext = clsCorrente;
        }





    }
}

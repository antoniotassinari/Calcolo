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
using Calcestruzzo;

namespace Acciai
{
    /// <summary>
    /// Logica di interazione per ucPropAcciaioArmatura.xaml
    /// </summary>
    public partial class ucPropAcciaioArmatura : UserControl
    {
        public AcciaioArmatura acciaioCorrente = new AcciaioArmatura();

        public ucPropAcciaioArmatura()
        {
            InitializeComponent();
            DataContext = acciaioCorrente;
            foreach (string nomeTipo in Enum.GetNames(typeof(TipoAcciai)))
            {
                cbClasse.Items.Add(nomeTipo);
            }
            cbClasse.SelectedItem = acciaioCorrente.Tipo.ToString();
        }

        private void cbClasse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            acciaioCorrente = new AcciaioArmatura((TipoAcciai)Enum.Parse(typeof(TipoAcciai), cbClasse.SelectedItem.ToString(), true));
            mainGrid.DataContext = acciaioCorrente;
            
        }
    }
}

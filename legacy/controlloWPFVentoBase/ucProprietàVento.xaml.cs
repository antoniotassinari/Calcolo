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

namespace controlloWPFVentoBase
{
    /// <summary>
    /// Logica di interazione per UserControl1.xaml
    /// </summary>
    public partial class ucVento : UserControl
    {
        /// <summary>
        /// Vento associato al controllo
        /// </summary>
        public Vento.Vento VentoAssociato = new Vento.Vento();

        /// <summary>
        /// Costruttore
        /// </summary>
        public ucVento()
        {
            InitializeComponent();
            zoneComboBox.ItemsSource = Enum.GetValues(typeof(Vento.Vento.Zone));
            this.DataContext = VentoAssociato;
        }

        private void zoneComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            VentoAssociato.ProprietàZona = new Vento.GestioneZonaGeografica((Vento.Vento.Zone)zoneComboBox.SelectedItem);
        }

        private void posizioneComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                VentoAssociato.DistanzaDaCosta = (Vento.Vento.DistanzeDaCosta)posizioneComboBox.SelectedItem;
            }
            catch(Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void RugositàComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                VentoAssociato.RugositàTerreno = new Vento.GestioneRugositàTerreno((Vento.Vento.ClassiRugosità)RugositàComboBox.SelectedItem);

            }
            catch { }
        }




       
    }
}

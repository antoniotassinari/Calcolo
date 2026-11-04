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
using STA_Dimensionamento_Plinti.pali;

namespace STA_Dimensionamento_Plinti.plinti
{
    /// <summary>
    /// Logica di interazione per ucPropPlintoRettangolare.xaml
    /// </summary>
    public partial class ucPropPlintoSuPaliRettangolare : UserControl, IUserControlPLinto
    {
        private PlintoSuPali _plinto = new PlintoSuPali();

        public PlintoSuPali plinto { get { return _plinto; } }

        public ucPropPlintoSuPaliRettangolare()
        {
            InitializeComponent();
        }

        public ucPropPlintoSuPaliRettangolare(PlintoSuPali plintoIn)
            : this()
        {
            _plinto = plintoIn;
            tbDiametro.Text = plinto.Pali[0].Diametro.ToString("F0");
            tbOrdini.Text = plinto.Ordini.ToString();
            tbColonne.Text = plinto.Colonne.ToString();
            tbPortataLimite.Text = plinto.ValoreLimite.ToString();
             tbSpessore.Text = plinto.H.ToString();
            tbAltezzaComplessiva.Text = plinto.A.ToString("F0");
             tbLarghezzaComplessiva.Text = plinto.B.ToString("F0");
             tbNumeroComplessivoPali.Text = plinto.Pali.Count.ToString("F0");

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

                //Rende il plinto associato a questo controllo proprietà un plinto rettangolare con ordini e colonne regolari
                _plinto.generaPlintoRettangolare(new Palo(double.Parse(tbDiametro.Text), double.Parse(tbPortataLimite.Text)), int.Parse(tbOrdini.Text), int.Parse(tbColonne.Text), double.Parse(tbSpessore.Text));

                OnPlintoValidato(new AggiuntoPlintoValido(plinto));

        }

        /// <summary>
        /// Funzione che attiva l'evento, per il passaggio del PLinto creato
        /// </summary>
        /// <param name="e">Argomenti di passaggio</param>
        protected virtual void OnPlintoValidato(AggiuntoPlintoValido e)
        {
            EventHandler<AggiuntoPlintoValido> handler = PlintoValidato;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #region Membri di IUserControlPLinto

        public event EventHandler<AggiuntoPlintoValido> PlintoValidato;

        #endregion
    }
}

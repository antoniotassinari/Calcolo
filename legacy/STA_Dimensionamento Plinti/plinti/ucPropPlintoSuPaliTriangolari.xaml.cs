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

namespace STA_Dimensionamento_Plinti.plinti
{
    /// <summary>
    /// Logica di interazione per ucPropPlintoSuPaliTriangolari.xaml
    /// </summary>
    public partial class ucPropPlintoSuPaliTriangolari : UserControl
    {
        private PlintoSuPali _plinto { get; set; }

        public PlintoSuPali plinto { get { return _plinto; } }

        public ucPropPlintoSuPaliTriangolari()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Costruttore a partire da un plinto su pali
        /// </summary>
        /// <param name="plintoIn">plinto su pali triangolare in ingresso</param>
        public ucPropPlintoSuPaliTriangolari(PlintoSuPali plintoIn) : this()
        {
            _plinto = plintoIn;
            tbDiametroPali.Text = plintoIn.Pali[0].Diametro.ToString();
            tbPortataLimite.Text = plintoIn.ValoreLimite.ToString();

            tbAreaPali.Text = plinto.AreaPali().ToString();
            tbJx.Text = plinto.Jx().ToString("F2");
            tbJy.Text = plinto.Jy().ToString("F2");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PlintoSuPali plintoprov = new PlintoSuPali();
            plintoprov.generaPlintoTriangolare(new pali.Palo(double.Parse(tbDiametroPali.Text), double.Parse(tbPortataLimite.Text)), 0);
            _plinto = plintoprov;
            OnPlintoValidato(new AggiuntoPlintoValido(plintoprov));


        }

        /// <summary>
        /// gestione dell'evento che accade quando accate che il plinto venga validato
        /// </summary>
        public event EventHandler<AggiuntoPlintoValido> PlintoValidato;


        protected virtual void OnPlintoValidato(AggiuntoPlintoValido e)
        {
            EventHandler<AggiuntoPlintoValido> handler = PlintoValidato;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}

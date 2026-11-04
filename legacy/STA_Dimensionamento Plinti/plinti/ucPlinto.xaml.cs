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
    /// Logica di interazione per ucPlintoSuPali.xaml
    /// </summary>
    public partial class ucPlinto : UserControl, IUserControlPLinto
    {

        IPlinto _plinto { get; set; }

        public ucPlinto()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Costruisce il controllo in funzione del tipo di plinto
        /// </summary>
        /// <param name="tipoIn"></param>
        public ucPlinto(TipoPlinto tipoIn) : this()
        {
            switch (tipoIn)
            {
                case TipoPlinto.Superficiale:
                    //Modifica l'interfaccia del controllo per la gestione di un plinto superficiale
                    comboBoxTipologia.Items.Clear();
                    comboBoxTipologia.Items.Add("Superficiale");
                    comboBoxTipologia.SelectedIndex = 0;
                    ucPropPlintoSuperficiale controlloProp = new ucPropPlintoSuperficiale();
                    controlloProp.PlintoValidato += PrendiPlintoValidato;
                    gridProp.Children.Add(controlloProp);
                    break;
                
               
                default:
                    
                    break;
            }
        }

        /// <summary>
        /// Costruttore a partire da un IPlinto
        /// </summary>
        /// <param name="plintoIn">IPlinto in ingresso</param>
        public ucPlinto(IPlinto plintoIn)
            : this()
        {
            switch (plintoIn.Tipo)
            {
                case TipoPlinto.Superficiale:
                    //Modifica l'interfaccia del controllo per la gestione di un plinto superficiale
                    Plinto plintoProv = (Plinto)plintoIn;
                    comboBoxTipologia.Items.Clear();
                    comboBoxTipologia.Items.Add("Superficiale");
                    comboBoxTipologia.SelectedIndex = 0;
                    ucPropPlintoSuperficiale controlloProp = new ucPropPlintoSuperficiale(plintoProv);
                    controlloProp.PlintoValidato += PrendiPlintoValidato;
                    gridProp.Children.Add(controlloProp);
                    break;

                     
                case TipoPlinto.SuPali:
                    PlintoSuPali plintoProvPali = (PlintoSuPali)plintoIn;
                    comboBoxTipologia.SelectedIndex = 1;
                    gridProp.Children.Clear();

                    switch (plintoProvPali.TipoSuPali)
                    {
                        case TipoligiePlintiPali.RettangolareRegolare:
                            ucPropPlintoSuPaliRettangolare controlloPropPali = new ucPropPlintoSuPaliRettangolare(plintoProvPali);
                            controlloPropPali.PlintoValidato += PrendiPlintoValidato;
                            gridProp.Children.Add(controlloPropPali);
                            break;
                            
                        case TipoligiePlintiPali.Triangolare:
                             ucPropPlintoSuPaliTriangolari controlloPropPaliTriang = new ucPropPlintoSuPaliTriangolari(plintoProvPali);
                                controlloPropPaliTriang.PlintoValidato += PrendiPlintoValidato;
                             gridProp.Children.Add(controlloPropPaliTriang);
                             break;

                        default :
                            break;

                    }


                    break;

                default:
                    break;
            }
        }


        private void PrendiPlintoValidato(object sender, AggiuntoPlintoValido e)
        {
            OnPlintoValidato(e);

        }


        protected virtual void OnPlintoValidato(AggiuntoPlintoValido e)
        {
            EventHandler<AggiuntoPlintoValido> handler = PlintoValidato;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// metodo per costruire l'interfaccia di definizione di un plinto rettangolare con
        /// </summary>
        private void mascheraPropRettangolare()
        {

            if (gridProp != null)
            {
                gridProp.Children.Clear();
                ucPropPlintoSuPaliRettangolare controlloPlintoRettProv = new ucPropPlintoSuPaliRettangolare();
                controlloPlintoRettProv.PlintoValidato += PrendiPlintoValidato;
                gridProp.Children.Add(controlloPlintoRettProv);

            }

        }

        private void mascheraPropTriangolare()
        {
            if (gridProp != null)
            {
                gridProp.Children.Clear();
                ucPropPlintoSuPaliTriangolari controlloPlintoTriangProv = new ucPropPlintoSuPaliTriangolari();
                controlloPlintoTriangProv.PlintoValidato += PrendiPlintoValidato;
                gridProp.Children.Add(controlloPlintoTriangProv);


            }


        }


        

        #region IUserControlPLinto Membri di

        public event EventHandler<AggiuntoPlintoValido> PlintoValidato;

        #endregion

        

        private void comboBoxTipologia_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (comboBoxTipologia.SelectedIndex)
            {
                case 1:
                    mascheraPropRettangolare();
                    break;

                case 2:
                    mascheraPropTriangolare();

                    break;


                default:

                    break;
            }
        }

    }
}

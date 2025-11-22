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
using System.Xml;
using STA_Dimensionamento_Plinti.plinti;

namespace STA_Dimensionamento_Plinti
{
    /// <summary>
    /// Logica di interazione per UserControl1.xaml
    /// </summary>
    public partial class ucPropPlintoSuperficiale : UserControl , IUserControlPLinto
    {
        public int progressivo = 0;

        /// <summary>
        /// istanza della classe Plinto per la "matematica" di funzionamento
        /// </summary>
        private Plinto _plinto { get; set; }

        /// <summary>
        /// classe plinto associata al controllo
        /// </summary>
        public Plinto plinto
        { get { return _plinto; } }

        /// <summary>
        /// Metodo per creare il controllo da zero
        /// </summary>
        public ucPropPlintoSuperficiale()
        {
            InitializeComponent();

        }

        /// <summary>
        /// Costruttore pubblico a partire da un plinto
        /// </summary>
        /// <param name="plintoIn">plinto in ingresso</param>
        public ucPropPlintoSuperficiale(Plinto plintoIn) : this()
        {
            _plinto = plintoIn;

            if (plinto.validità == PlintoValido.Valido)
            {
                tbA.Text = plintoIn.A.ToString("F0");

                tbB.Text = plintoIn.B.ToString("F0");

                tbSpessore.Text = plintoIn.H.ToString("F0");

                tbQuota.Text = plintoIn.Imposta.ToString("F0");
                tbPresLimite.Text = plintoIn.ValoreLimite.ToString("F2");

                tbEccX.Text = plinto.EccentricitaPilastro.X.ToString();
                tbEccY.Text = plinto.EccentricitaPilastro.Y.ToString();

                textBoxArea.Text = plinto.Area.ToString("F0");

                textBoxVolume.Text = plinto.Volume.ToString("F4");

                textBoxPeso.Text = plinto.Peso.ToString("F0");

                if (plinto.ConsideraTerrenoSopra)
                {
                    cbTerreno.IsChecked = true;
                    tbPesoTerreno.Text = plinto.PesoSpecTerreno.ToString("F0");
                    tbPesoTerrenoTotale.Text = plinto.PesoTerrenoSopra.ToString("F0");

                }   
                
                else
                {
                    cbTerreno.IsChecked = false;
                }
                
            }
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

        /// <summary>
        /// SAlva il nodo XML del controllo. In realtà lo recupera dalla classe plinto pubblica istanziata
        /// </summary>
        /// <returns>restituisce il nodo</returns>
        public XmlNode salvaXml()
        {
            return plinto.salvaXML();

        }

        private void attivaControlliTerreno()
        {
            
        }


        /// <summary>
        /// Metodo per la lettura del nodo XMl
        /// </summary>
        /// <param name="nodoIn">Nodo strutturato per la lettura</param>
        public void leggiXml(XmlNode nodoIn)
        {
            _plinto = new Plinto(nodoIn);

            if (plinto.validità == PlintoValido.Valido)
            {
                tbA.Text = plinto.A.ToString("F0");
                tbB.Text = plinto.B.ToString("F0");
                tbSpessore.Text = plinto.H.ToString("F0");
                tbQuota.Text = plinto.Imposta.ToString("F0");


                tbPresLimite.Text = plinto.ValoreLimite.ToString();
               

                cbTerreno.IsChecked = plinto.ConsideraTerrenoSopra;

                if (cbTerreno.IsChecked == true)
                {
                   

                    tbPesoTerreno.Text = _plinto.PesoSpecTerreno.ToString("F0");

                    tbPesoTerrenoTotale.Text = _plinto.PesoTerrenoSopra.ToString("F0");

                    
                }

                textBoxArea.Text = plinto.Area.ToString("F0");

                textBoxVolume.Text = plinto.Volume.ToString("F4");

                textBoxPeso.Text = plinto.Peso.ToString("F0");

                OnPlintoValidato(new AggiuntoPlintoValido(plinto));


                
            }


        }

        /// <summary>
        /// Gestione evento click del punsante
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btModifica_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                _plinto = new Plinto(TipoPlinto.Superficiale, double.Parse(tbA.Text), double.Parse(tbB.Text), double.Parse(tbSpessore.Text), double.Parse(tbQuota.Text), double.Parse(tbPresLimite.Text));
                _plinto.assegnaPosizione(double.Parse(tbEccX.Text), double.Parse(tbEccY.Text));
                if (plinto.validità == PlintoValido.Valido)
                {
                    

                    textBoxArea.Text = plinto.Area.ToString("F0");

                    textBoxVolume.Text = plinto.Volume.ToString("F4");

                    textBoxPeso.Text = plinto.Peso.ToString("F0");
                                        

                    if (cbTerreno.IsChecked == true)
                    {
                        _plinto.assegnaTerreno(double.Parse(tbPesoTerreno.Text));
                        
                        try
                        {
                            tbPesoTerrenoTotale.Text = _plinto.PesoTerrenoSopra.ToString("F0");
                        }
                        catch { }
                    }
                    else
                    {
                        _plinto.nonConsiderareTerreno();
                    }
                    OnPlintoValidato(new AggiuntoPlintoValido(plinto));
                }
                

            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }



        /// <summary>
        /// Quando il checkbox è selezionato, fa quello che viene inserito
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {

        }



    }
}

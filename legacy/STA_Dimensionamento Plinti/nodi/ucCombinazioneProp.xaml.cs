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

namespace STA_Dimensionamento_Plinti
{
    /// <summary>
    /// Logica di interazione per ucCombinazioneProp.xaml
    /// </summary>
    public partial class ucCombinazioneProp : UserControl
    {
        public enum TipoRiga
        {
            /// <summary>
            /// tipo Titolo
            /// </summary>
            Titolo,
            /// <summary>
            /// Riga di dati da modificare
            /// </summary>
            Dati
        }

        public TipoRiga tipo = TipoRiga.Dati;

        Reazione reazioneProv = new Reazione("1", 0, 0, 0, 0, 0, 0);

        private Verifica risultatiVerifica { get; set; }

        public event EventHandler<EventArgs> prontoPerEliminazione;

        protected virtual void OnProntoPerEliminazione(EventArgs e)
        {
            if (prontoPerEliminazione != null)
            {
                prontoPerEliminazione(this, e);
            }
        }

        public ucCombinazioneProp()
        {
            
            InitializeComponent();
            btModifica.IsEnabled = false;
            

        }

        public ucCombinazioneProp(TipoRiga tipoRiga, Nodo nodoIn)
            : this()
        {
            switch (tipoRiga)
            {
                case TipoRiga.Titolo:
                    generaIntestazione();
                    break;

                default:
                    generaRigaDati(nodoIn.Nome);
                    break;
                    
            }
        }

        /// <summary>
        /// Costruttore a partire dalla reazione e dal nome del nodo
        /// </summary>
        /// <param name="nomeNodo">nome del nodo</param>
        /// <param name="reaIn">reazione da visualizzare</param>
        public ucCombinazioneProp(TipoRiga tipoRiga, string nomeNodo, Reazione reaIn) : this()
        {
            switch (tipoRiga)
            {
                case TipoRiga.Titolo:
                    generaIntestazione();
                    if ((reaIn.ListaVerifiche != null) && (reaIn.ListaVerifiche.Count > 0))
                    {
                        int i = 0;
                        foreach (verifiche.IVerificaPlinto verificaCheck in reaIn.ListaVerifiche)
                        {

                            ColumnDefinition colonna = new ColumnDefinition();
                            colonna.Name = "ColResPlinto_" + i.ToString();
                            Risultati.ColumnDefinitions.Add(colonna);
                            switch (verificaCheck.PlintoDiverifica.Tipo)
                            {
                                case TipoPlinto.Superficiale:
                                    verifiche.ucVerificaMeyerhof ControlloTitolo = new verifiche.ucVerificaMeyerhof(verificaCheck.PlintoDiverifica.CaratteristichePlinto());
                                    Risultati.Children.Add(ControlloTitolo);
                                    Grid.SetColumn(ControlloTitolo, i);
                                    break;

                                case TipoPlinto.SuPali:
                                    verifiche.ucVerificaPlintoSuPali ControlloTitoloNEw = new verifiche.ucVerificaPlintoSuPali(verificaCheck.PlintoDiverifica.CaratteristichePlinto());
                                    Risultati.Children.Add(ControlloTitoloNEw);
                                    Grid.SetColumn(ControlloTitoloNEw, i);
                                    break;
                            }
   
                            i++;
                        }
                    }

                    break;

                default:

                    tbNodo.Text = nomeNodo;
                    reazioneProv = reaIn;
                    leggiPropCombinazione(reazioneProv);
                    btModifica.Visibility = System.Windows.Visibility.Hidden;
                    btModifica.IsEnabled = false;

                    //Qui legge le verifiche per visualizzarle
                    if ((reaIn.ListaVerifiche != null) && (reaIn.ListaVerifiche.Count > 0))
                    {
                        int i = 0;
                        foreach (verifiche.IVerificaPlinto verificaCheck in reaIn.ListaVerifiche)
                        {

                            ColumnDefinition colonna = new ColumnDefinition();
                            colonna.Name = "ColResPlinto_" + i.ToString();
                            Risultati.ColumnDefinitions.Add(colonna);
                            /*if (this.tipo == TipoRiga.Titolo)
                            {
                                verifiche.ucVerificaMeyerhof ControlloTitolo = new verifiche.ucVerificaMeyerhof(verificaCheck.plinto.CaratteristichePlinto());
                                Risultati.Children.Add(ControlloTitolo);
                                Grid.SetColumn(ControlloTitolo, i);
                            }
                            else
                            {*/
                            switch (verificaCheck.PlintoDiverifica.Tipo)
                            {
                                case TipoPlinto.Superficiale:
                                    verifiche.ucVerificaMeyerhof controlloRisultatiPlinti = new verifiche.ucVerificaMeyerhof((verifiche.verificaMeyerhof)verificaCheck);
                                    Risultati.Children.Add(controlloRisultatiPlinti);
                                    Grid.SetColumn(controlloRisultatiPlinti, i);
                                    break;

                                case TipoPlinto.SuPali:
                                    verifiche.ucVerificaPlintoSuPali controlloRisultatiPlintiPali = new verifiche.ucVerificaPlintoSuPali((verifiche.verificaPlintoSuPali)verificaCheck);
                                    Risultati.Children.Add(controlloRisultatiPlintiPali);
                                    Grid.SetColumn(controlloRisultatiPlintiPali, i);
                                    break;
                            }
                            //}

                            i++;
                        }
                    }
                    break;
            }
        }

        private void generaRigaDati(string nomeNodo)
        {
            tbNodo.Text = nomeNodo;
            leggiPropCombinazione(reazioneProv);

            
        }

        public void preparaPerEliminazione()
        {
            btModifica.Content = "Canc";
            btModifica.IsEnabled = true;
            
        }


        private void leggiPropCombinazione(Reazione reaIN)
        {
            tbComb.Text = reaIN.Combinazione;
            tbFZ.Text = reaIN.FZ.ToString("F0");

            tbMX.Text = reaIN.MX.ToString("F0");
            tbMY.Text = reaIN.MY.ToString("F0");

            
        }

        private void modificaReazioni(ref Reazione reaIn)
        {
            reaIn = new Reazione(tbComb.Text, 0, 0, double.Parse(tbFZ.Text), double.Parse(tbMX.Text), double.Parse(tbMY.Text), 0);
        }

        private void campiModificati()
        {
            btModifica.Content = "OK";
            btModifica.IsEnabled = true;
        }

        /// <summary>
        /// crea le colonne di instestazione
        /// </summary>
        private void generaIntestazione()
        {
            tipo = TipoRiga.Titolo;

            tbNodo.Text = "Nodo";
            tbNodo.FontWeight = FontWeights.Bold;
            tbNodo.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;

            tbComb.Text = "Comb";
            tbComb.FontWeight = FontWeights.Bold;


            tbFZ.Text = "FZ [kN]";
            tbFZ.FontWeight = FontWeights.Bold;


            tbMX.Text = "MX [kN*m]";
            tbMX.FontWeight = FontWeights.Bold;


            tbMY.Text = "MY [kN*m]";
            tbMY.FontWeight = FontWeights.Bold;


            btModifica.Visibility = System.Windows.Visibility.Hidden;

        }

        

        /// <summary>
        /// Gestione dell'evento della modifica di un testo qualunque
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbMY_TextChanged(object sender, TextChangedEventArgs e)
        {
            campiModificati();
        }


        /// <summary>
        /// Metodo per la modifica della reazione di calcolo.
        /// </summary>
        public void accettaModificheComb()
        {
            try
            {
                modificaReazioni(ref reazioneProv);
                btModifica.Content = "";
                btModifica.IsEnabled = false;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
            

        }

        /// <summary>
        /// Esegue i calcoli e la verifica di ogni plinto
        /// </summary>
        /// <param name="plintoVer">Plinto per la verifica</param>
        public void calcolaSingoloPlinto(Plinto plintoVer)
        {
            Risultati.Children.Clear();
            Risultati.Children.Add(new verifiche.ucVerificaMeyerhof(new verifiche.verificaMeyerhof(reazioneProv, plintoVer)));
        }

        /// <summary>
        /// Esegue i calcoli per tutte le reazioni presenti
        /// </summary>
        /// <param name="listaPlinti"></param>
        public void calcolaMultiPlinto(ListaPlinti listaPlinti)
        {
            Risultati.Children.Clear();
            int i = 0;
            foreach (Plinto plintoIn in listaPlinti)
            {
                
                ColumnDefinition colonna = new ColumnDefinition();
                colonna.Name = "ColResPlinto_" + i.ToString();
                Risultati.ColumnDefinitions.Add(colonna);
                if (this.tipo == TipoRiga.Titolo)
                {
                    verifiche.ucVerificaMeyerhof ControlloTitolo = new verifiche.ucVerificaMeyerhof(plintoIn.CaratteristichePlinto());
                    Risultati.Children.Add(ControlloTitolo);
                    Grid.SetColumn(ControlloTitolo, i);
                }
                else
                {
                    verifiche.ucVerificaMeyerhof controlloRisultatiPlinti = new verifiche.ucVerificaMeyerhof(new verifiche.verificaMeyerhof(reazioneProv, plintoIn));
                    Risultati.Children.Add(controlloRisultatiPlinti);
                    Grid.SetColumn(controlloRisultatiPlinti, i);
                }

                i++;
            }
        }

        private void btModifica_Click(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            switch (bt.Content.ToString())
            {
                case "OK":
                    accettaModificheComb();
                    break;
                case "Canc":
                    OnProntoPerEliminazione(new EventArgs());
                    break;
            }
            
            
        }



        /// <summary>
        /// Restituisce il nodo Xml
        /// </summary>
        /// <returns></returns>
        public XmlNode salvaXml()
        {

            return reazioneProv.salvaXml();

        }

        /// <summary>
        /// metodo per la lettura dati XML
        /// </summary>
        /// <param name="XmlNodoIn">Nodo in ingresso</param>
        public void apriXml(XmlNode XmlNodoIn)
        {
            reazioneProv = new Reazione(XmlNodoIn);
            leggiPropCombinazione(reazioneProv);
            accettaModificheComb();
        }
    }
}

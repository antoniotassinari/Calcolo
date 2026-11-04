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

namespace STA_Dimensionamento_Plinti.verifiche
{
    /// <summary>
    /// Logica di interazione per ucReportRisultati.xaml
    /// </summary>
    public partial class ucReportRisultati : UserControl
    {
        private double TaglioMassimo = 0;

        public ucReportRisultati()
        {
            InitializeComponent();
        }

        public event EventHandler<ArgomentoNodo> nodoScelto;

        protected virtual void OnNodoScelto(ArgomentoNodo e)
        {
            EventHandler<ArgomentoNodo> handler = nodoScelto;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Visualizza i risultati sul controllo
        /// </summary>
        /// <param name="Progetto">Classe progetto in ingresso</param>
        public void leggiRisultati(progetto.ProgettoPlinti Progetto)
        {
            spNodiNonVerificati.Items.Clear();
            spNodiVerificati.Items.Clear();
            spVisualizzaPerPlinti.Children.Clear();
            ListaNodi nodiVerificati = new ListaNodi();
            foreach (IPlinto plintoIN in Progetto.Plinti)
            {
                Expander expPlintIn = new Expander();
                expPlintIn.Header = plintoIN.CaratteristichePlinto();

                ListView spPlintoIn = new ListView();

                expPlintIn.Content = spPlintoIn;

                foreach (Nodo nodoIn in Progetto.Nodi)
                {
                    if (nodiVerificati.Contains(nodoIn) == false)
                    {

                        bool NodoVerificato = true;

                        double reaMax = 0;
                        string reazioneMaggiore = "";

                        foreach (Reazione rea in nodoIn.Reazioni)
                        {
                            foreach (IVerificaPlinto verifica in rea.ListaVerifiche)
                            {
                                if (verifica.PlintoDiverifica.CaratteristichePlinto() == plintoIN.CaratteristichePlinto())
                                {
                                    if (reaMax < verifica.ReazioneMassima)
                                    {
                                        reaMax = verifica.ReazioneMassima;
                                        reazioneMaggiore = rea.Combinazione + "|" + reaMax.ToString("F0");
                                    }

                                    if (!verifica.Verificato())
                                    {
                                        NodoVerificato = false;
                                        break;
                                    }
                                    
                                    //prova a verificare se è presente un taglio su palo e poi effetta un controllo per determinare se è superiorire a quello memorizzato
                                    try
                                    {
                                        verificaPlintoSuPali verificaCast = (verificaPlintoSuPali)verifica;
                                        if (verificaCast.TaglioSuPalo > TaglioMassimo)
                                        {
                                            TaglioMassimo = verificaCast.TaglioSuPalo;
                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                            }
                        }

                        if (NodoVerificato)
                        {
                            nodiVerificati.Add(nodoIn);
                            spPlintoIn.Items.Add(nodoIn.Nome + " | " + reazioneMaggiore);
                            spNodiVerificati.Items.Add(nodoIn);
                        }
                    }
                }
                spVisualizzaPerPlinti.Children.Add(expPlintIn);
            }


            foreach (Nodo nodo in Progetto.Nodi)
            {
                if (!nodiVerificati.Contains(nodo))
                {
                    spNodiNonVerificati.Items.Add(nodo);
                }
            }

            labeltaglio.Content += TaglioMassimo.ToString("F2");
        }

        private void spNodiVerificati_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OnNodoScelto(new ArgomentoNodo((Nodo)spNodiVerificati.SelectedItem));
        }

        private void spNodiNonVerificati_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OnNodoScelto(new ArgomentoNodo((Nodo)spNodiNonVerificati.SelectedItem));
        }



    }
}

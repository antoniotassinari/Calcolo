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
using System.IO;
using System.Threading;


namespace STA_Dimensionamento_Plinti.progetto
{
    /// <summary>
    /// Logica di interazione per ucProgettoPlintoNew.xaml
    /// </summary>
    public partial class ucProgettoPlintoNew : UserControl
    {
        #region proprietà private

        /// <summary>
        /// Istanza della classe ProgettoPlinti che fa tutto il lavoro dietro all'interfaccia
        /// </summary>
        private ProgettoPlinti sProgetto = new ProgettoPlinti();

        #endregion

        #region proprietà pubbliche
        
        /// <summary>
        /// Classe associata al controllo
        /// </summary>
        public ProgettoPlinti Progetto { get { return sProgetto; } }

        public List<Nodo> Verificati = new List<Nodo>();
        public List<Nodo> NonVerificati = new List<Nodo>();

        #endregion

        public ucProgettoPlintoNew()
        {
            
            InitializeComponent();
            elencoPlinti.aggiuntoUnPlintoValido += PrendiPrintoValidati;
            controlloReport.nodoScelto += EvidenziaNodiScelti;
        }

        private void EvidenziaNodiScelti(object sender, ArgomentoNodo e)
        {
            tabItemCalcolo.Focus();
            controlloNodi.listViewNodi.SelectedItem = e.nodo;
        }

        private void PrendiPrintoValidati(object sender, AggiuntoPlintoValido e)
        {
            Progetto.assumiPlintiPresenti(elencoPlinti.Plinti);
            elencoPlinti.visualizzaPlinti(Progetto.Plinti);
            controlloProgettoManuale.aggiornaPlinti(Progetto.Plinti);
            controlloNodi.visualizzaNodi(Progetto.Nodi);
        }

        /// <summary>
        /// metodo per lettura del file. gestisce tutto la classe progettoPlinti
        /// </summary>
        /// <param name="FilePath">string con il percorso del file</param>
        public void apriFileRelazione(string FilePath)
        {
            controlloViewTxt.viewTXT(FilePath);
            labelFile.Content = FilePath;
            Progetto.leggiFile(FilePath);
            labelCheck.Content = "Numero Nodi Presenti: " + Progetto.Nodi.Count.ToString() + " | ThreadID: " + Thread.CurrentThread.ManagedThreadId.ToString();
            controlloNodi.visualizzaNodi(Progetto.Nodi);
            controlloProgettoManuale.aggiornaNodi(Progetto.Nodi);

        }

        /// <summary>
        /// succede questo quando clicchi il pulsante apri relazione
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btApriRelazione_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "File di testo (*.txt)|*.txt";
            ofd.FilterIndex = 1;
            ofd.ShowDialog();
            apriFileRelazione(ofd.FileName);
        }

        private void btSalva_Click(object sender, RoutedEventArgs e)
        {
            Progetto.salvaXML();
        }

        /// <summary>
        /// Effettua il calcolo dei Risultati
        /// </summary>
        private void effettuaCalcoli()
        {

                Progetto.calcola();
                elencoPlinti.visualizzaPlinti(Progetto.Plinti);
                controlloNodi.visualizzaNodi(Progetto.Nodi);
                controlloReport.leggiRisultati(Progetto);
                


        }

        /// <summary>
        /// metodo per aprire il file XML
        /// </summary>
        private void apriXML()
        {
            Progetto.apriXML();
            elencoPlinti.visualizzaPlinti(Progetto.Plinti);
            controlloNodi.visualizzaNodi(Progetto.Nodi);
            controlloProgettoManuale.aggiornaPlinti(Progetto.Plinti);
            controlloProgettoManuale.aggiornaNodi(Progetto.Nodi);
        }

        private void btCalcola_Click(object sender, RoutedEventArgs e)
        {
            effettuaCalcoli();
        }

        private void btApri_Click(object sender, RoutedEventArgs e)
        {
            apriXML();
        }

        private void btScriviRelazione_Click(object sender, RoutedEventArgs e)
        {
            Progetto.scrivirelazioneWord();
        }
    }
}

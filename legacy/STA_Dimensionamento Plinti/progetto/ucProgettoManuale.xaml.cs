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
using STA_Dimensionamento_Plinti.plinti;

namespace STA_Dimensionamento_Plinti.progetto
{
    /// <summary>
    /// Logica di interazione per ucProgettoManuale.xaml
    /// </summary>
    public partial class ucProgettoManuale : UserControl
    {
        //istanza privata della classe lista plinti
        ListaPlinti Plinti { get; set; }

        public ucProgettoManuale()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Metodo per visualizzare i plinti, mediante lo stackPanel ed i controlli appositi
        /// </summary>
        /// <param name="plintiIngresso"></param>
        public void aggiornaPlinti(ListaPlinti plintiIngresso)
        {
            //prima acquisisce la lista dei plinti
            Plinti = plintiIngresso;

            //pulisce lo stackpanel dai controlli precedenti
            stackPanelPlinti.Children.Clear();

            //Aggiorna i controlli per visualizzare i plinti dentro al progetto
            foreach (IPlinto plintoIn in Plinti)
            {
                //Istanza un nuovo controllo per il progetto dei nodi
                ucVisualizzaNodiSuiPlinti controlloPerPlinto = new ucVisualizzaNodiSuiPlinti(plintoIn);

                //associa all'evento RichiestaAggiungereNodi definita nel controllo ucVisualizzaNodiSuiPlinti la funzione InviaNodi definita sotto
                controlloPerPlinto.RichiestaAggiungereNodi += InviaNodi;

                //aggiunge il controllo allo stackPanel
                stackPanelPlinti.Children.Add(controlloPerPlinto);
            }
        }

        /// <summary>
        /// Aggiorna i nodi Nodi
        /// </summary>
        /// <param name="nodiIngresso">Lista di Nodi in ingresso</param>
        public void aggiornaNodi(ListaNodi nodiIngresso)
        {
            //prima pulisce il listview dei nodi
            listaNodi.Items.Clear();

            //Poi aggiunge in nodi direttamente dentro al controllo, visto che la classe nodo ha una metodo ToString() che permette di visualizzarli dentro al controllo
            foreach (Nodo nodoIn in nodiIngresso)
            {
                listaNodi.Items.Add(nodoIn);
            }
        }

        /// <summary>
        /// metodo per il passaggio dei nodi
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InviaNodi(object sender, EventArgs e)
        {
            //impone il cast sull'oggett che chiama il calcolo
            ucVisualizzaNodiSuiPlinti controlloCalcolo = (ucVisualizzaNodiSuiPlinti)sender;
            
            //effettua il cast a ListaNodi sugli elementi selezionati dell'elenco nodi. Deve dare esito positivo perchè io inserisco direttamente delle istanza della classe Nodo
            //ListaNodi nodiDaInviare = (ListaNodi)listaNodi.SelectedItems;
            ListaNodi nodiDaInviare = new ListaNodi();

            foreach (object obj in listaNodi.SelectedItems)
            {
                Nodo nodoIn = (Nodo)obj;
                nodiDaInviare.Add(nodoIn);
            }


            //richiama il metodo per la ricezione dei da parte del controllo che ha generato l'evento
            controlloCalcolo.RiceviNodi(nodiDaInviare);

            //qui rimuove i nodi dall'elenco sulla destra
            foreach (Nodo nodoDaPassare in nodiDaInviare)
            {
                listaNodi.Items.Remove(nodoDaPassare);
            }
        }
    }
}

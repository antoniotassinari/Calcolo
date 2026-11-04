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
    /// Logica di interazione per ucVisualizzaNodiSuiPlinti.xaml
    /// </summary>
    public partial class ucVisualizzaNodiSuiPlinti : UserControl
    {
        public IPlinto plintoAssociato { get; set; }

        public ListaNodi nodiAssociati { get; set; }

        public ucVisualizzaNodiSuiPlinti(IPlinto plintoIn)
        {
            InitializeComponent();
            plintoAssociato = plintoIn;
            expanderPrincipale.DataContext = plintoAssociato;
            tblkPlinto.Text = plintoAssociato.CaratteristichePlinto();
        }

        /// <summary>
        /// Evento generato quanto di richiede di aggiungere Nodi da verificare al controllo
        /// </summary>
        public event EventHandler RichiestaAggiungereNodi;

        /// <summary>
        /// metodo per sollevare l'evento
        /// </summary>
        protected virtual void SuRichiestaAggiungereNodi()
        {
            EventHandler handler = RichiestaAggiungereNodi;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SuRichiestaAggiungereNodi();
        }

        /// <summary>
        /// metodo per riceve nodi
        /// </summary>
        /// <param name="nodiIn">listaNodi in ingresso</param>
        public void RiceviNodi(ListaNodi nodiIn)
        {
            nodiAssociati = new ListaNodi();
            


            ListaPlinti plintoIn = new ListaPlinti();
            plintoIn.Add(plintoAssociato);
            int i = 0;

            foreach (Nodo nodoIn in nodiIn)
            {
                nodoIn.calcolaNodo(plintoIn);
                nodiAssociati.Add(nodoIn);

                ListViewItem nodoVista = new ListViewItem();
                nodoVista.Content = nodoIn.Nome;
                

                bool esitoVerifica = true;

                foreach (Reazione reaCheck in nodoIn.Reazioni)
                {
                    
                    if (reaCheck.ListaVerifiche[0].Verificato() == false)
                    {
                       esitoVerifica = false;
                       nodoVista.Background = new SolidColorBrush(Color.FromArgb(50, 255, 0, 0));
                       
                    }

                }


                listaNodi.Items.Add(nodoVista);

                

                
                
                

                
            }




        }



    }
}

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
    /// Logica di interazione per ucElencoPlinti.xaml
    /// </summary>
    public partial class ucElencoPlinti : UserControl
    {

        private generali.TipoControllo tipoDiControllo { get; set; }

        /// <summary>
        /// lista privata IPlinti
        /// </summary>
        private ListaPlinti sPlinti = new ListaPlinti();

        public event EventHandler<AggiuntoPlintoValido> aggiuntoUnPlintoValido;

        protected virtual void OnAggiuntoPlintoValido(AggiuntoPlintoValido e)
        {
            if (aggiuntoUnPlintoValido != null)
            {
                aggiuntoUnPlintoValido(this, e);
            }
        }

        //Mi serve solo per modificare il plinto: memorizza quello vecchio che sarà da sostituire
        IPlinto plintoDaModificare { get; set; }

        /// <summary>
        /// Plinti presenti
        /// </summary>
        public ListaPlinti Plinti { get { return sPlinti; } }

        /// <summary>
        /// Costruttore
        /// </summary>
        public ucElencoPlinti()
        {
            InitializeComponent();

        }

        
        private void PrendiPlintoValidato(object sender, AggiuntoPlintoValido e)
        {
            switch (tipoDiControllo)
            {
                case generali.TipoControllo.Aggiunta:
                                    sPlinti.Add(e.plinto);


                    break;

                case generali.TipoControllo.Modifica:
                    modificaPlintoSelezionato(e.plinto);

                    break;
            }
            OnAggiuntoPlintoValido(e);
            gridProp.Children.Remove((ucPlinto)sender);


           }
        
        private void btAggiungiPlinto_Click(object sender, RoutedEventArgs e)
        {
            tipoDiControllo = generali.TipoControllo.Aggiunta;
            TipoPlinto sceltaPlinto = (TipoPlinto)combBoxTipo.SelectedIndex;
            aggiungiControlloPlinto(sceltaPlinto);
        }

        /// <summary>
        /// Aggiunge il controllo del Plinto
        /// </summary>
        /// <param name="tipoPlintoIn">Tipo di plinto in ingresso</param>
        private void aggiungiControlloPlinto(TipoPlinto tipoPlintoIn)
        {
            gridProp.Children.Clear();
            switch (tipoPlintoIn)
            {
                case TipoPlinto.Superficiale:
                    ucPlinto controlloPlinto = new ucPlinto(tipoPlintoIn);
                    controlloPlinto.PlintoValidato += PrendiPlintoValidato;
                    gridProp.Children.Add(controlloPlinto);
                break; 

                case TipoPlinto.SuPali:
                ucPlinto controlloPlintoPorv = new ucPlinto();
                controlloPlintoPorv.PlintoValidato += PrendiPlintoValidato;
                gridProp.Children.Add(controlloPlintoPorv);
                break;
            }
        }

     

        /// <summary>
        /// Metodo per visualizzare la lista di plinti
        /// </summary>
        /// <param name="listaPlinti"></param>
        public void visualizzaPlinti(ListaPlinti listaPlinti)
        {
                sPlinti = listaPlinti;
                lvSuperficiali.Items.Clear();
                lvSuPali.Items.Clear();
                foreach (IPlinto plintoIn in listaPlinti)
                {
                    if (plintoIn.Tipo == TipoPlinto.Superficiale)
                    {
                        lvSuperficiali.Items.Add(plintoIn);
                    }
                    if (plintoIn.Tipo == TipoPlinto.SuPali)
                    {
                        lvSuPali.Items.Add(plintoIn);
                    }
                }
            
        }

        /// <summary>
        /// Metodo per modificare il plinto che è selezionato
        /// </summary>
        /// <param name="plintoIn">nuovo plinto che deve sostituire quello presente</param>
        private void modificaPlintoSelezionato(IPlinto plintoIn)
        {
            sPlinti.Remove(plintoDaModificare);
            sPlinti.Add(plintoIn);

        }

        private void visualizzaPlintoSelezionato(IPlinto plintoSelezionato)
        {
            tipoDiControllo = generali.TipoControllo.Modifica;
            plintoDaModificare = plintoSelezionato;
            gridProp.Children.Clear();
            try
            {
                gridProp.Children.Clear();
                ucPlinto controlloModifica = new ucPlinto(plintoSelezionato);
                        controlloModifica.PlintoValidato += PrendiPlintoValidato;
                        gridProp.Children.Add(controlloModifica);
            }
            catch(Exception exc)    
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void lvSuperficiali_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvSuperficiali.Items.Count != 0)
            {
                IPlinto plintoSelezionato = (IPlinto)lvSuperficiali.SelectedItem;
                visualizzaPlintoSelezionato(plintoSelezionato);
                
            }
        }

        private void lvSuPali_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvSuPali.Items.Count != 0)
            {
                IPlinto plintoSelezionato = (IPlinto)lvSuPali.SelectedItem;
                visualizzaPlintoSelezionato(plintoSelezionato);
                
            }
        }

        private void btEliminaTipo_Click(object sender, RoutedEventArgs e)
        {

                sPlinti.Remove(plintoDaModificare);
                OnAggiuntoPlintoValido(new AggiuntoPlintoValido(plintoDaModificare));
                gridProp.Children.Clear();
        }

    }
}

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
using System.IO;
using System.Xml;


namespace STA_Dimensionamento_Plinti
{


    /// <summary>
    /// Logica di interazione per ucCombEsterne.xaml
    /// </summary>
    public partial class ucCombEsterne : UserControl
    {
        #region proprietà private

        #endregion


        #region proprietà pubbliche

        #endregion

        public ucCombEsterne()
        {
            InitializeComponent();
        }

        #region metodi
        /// <summary>
        /// Effettua il calcolo dei risutlati di tutte le combinazioni presenti per i plinti presenti
        /// </summary>
        /// <param name="listaPlintiIn">Lista plinti in ingresso</param>
        public void calcolaRisultati(ListaPlinti listaPlintiIn)
        {
            foreach (ucCombPropExt contrRea in spNodi.Children)
            {
                contrRea.calcolaMultiPlinto(listaPlintiIn);
            }

        }

        /// <summary>
        /// metodo per visualizzare la lista di nodi
        /// </summary>
        /// <param name="nodiDaVisualizzare">lista nodi in ingresso</param>
        public void visualizzaNodi(ListaNodi nodiDaVisualizzare)
        {
            spNodi.Children.Clear();
            listViewNodi.Items.Clear();
            foreach (Nodo nodoIn in nodiDaVisualizzare)
            {
                listViewNodi.Items.Add(nodoIn);
                //spNodi.Children.Add(new ucCombPropExt(nodoIn));
            }
        }

        private void listViewNodi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            spNodi.Children.Clear();
            foreach (object nodoIn in listViewNodi.SelectedItems)
            {
                Nodo nodoProv = (Nodo)nodoIn;

                spNodi.Children.Add(new ucCombPropExt(nodoProv));

            }
        }

        
/*
        /// <summary>
        /// metodo per la lettura dei dati XMl
        /// </summary>
        ///<param name="nodoIn">Nodo in ingresso</param>
        public void apriXML(XmlNode nodoIn)
        {
            Nodi.Clear();
            if (nodoIn.Name == "Nodi")
            {
                foreach (XmlNode nodoNodo in nodoIn.ChildNodes)
                {
                    Nodi.Add(new Nodo(nodoNodo));
                }
            }

            foreach (Nodo nodo in Nodi)
            {
                spNodi.Children.Add(new ucCombPropExt(nodo));
            }

        }
 */
            #endregion

    }
}

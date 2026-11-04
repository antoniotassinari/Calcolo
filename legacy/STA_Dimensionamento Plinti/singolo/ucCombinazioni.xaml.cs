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
    /// Logica di interazione per ucCombinazioni.xaml
    /// </summary>
    public partial class ucCombinazioni : UserControl
    {

        List<Reazione> reazioniCalcolo = new List<Reazione>();

        ucCombinazioneProp rigaTitolo = new ucCombinazioneProp( ucCombinazioneProp.TipoRiga.Titolo, null);

        public Plinto plintoCalc { get; set; }
        
        public ucCombinazioni()
        {
            InitializeComponent();

        }

        private void EliminaControlloPronto(object sender, EventArgs e)
        {

        }

        private void btAggiungi_Click(object sender, RoutedEventArgs e)
        {

            reazioniCalcolo.Add(new Reazione("COMB" + (reazioniCalcolo.Count + 1).ToString(), 0, 0, 0, 0, 0, 0));

            lvComb.ItemsSource = null;
            lvComb.ItemsSource = reazioniCalcolo;
            
        }

        private void btElimina_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btCalcola_Click(object sender, RoutedEventArgs e)
        {
            if (plintoCalc == null)
            {
                System.Windows.MessageBox.Show("Prima di calcolare, creare e validare il plinto.");
                return;
            }

            foreach (Reazione reaCheck in reazioniCalcolo)
            {
                reaCheck.ListaVerifiche.Clear();
                reaCheck.ListaVerifiche.Add(new verifiche.verificaMeyerhof(reaCheck, plintoCalc));
                
            }
            lvComb.ItemsSource = null;
            lvComb.ItemsSource = reazioniCalcolo;

        }

        public XmlNode salvaXml()
        {
            XmlNode xmlNodo = generali.Statici.xmldoc.CreateNode(XmlNodeType.Element, "Combinazioni", "");
            //foreach (ucCombinazioneProp combPropProvv in spConbinazioni.Children)
            //{
            //    if (combPropProvv.tipo == ucCombinazioneProp.TipoRiga.Dati)
            //    {
            //        xmlNodo.AppendChild(combPropProvv.salvaXml());
            //    }

            //}

            foreach (Reazione reaSalv in reazioniCalcolo)
            {
                xmlNodo.AppendChild(reaSalv.salvaXml());
            }

            return xmlNodo;
        }

        public void apriXml(XmlNode XmlNodoIn)
        {
            //foreach (XmlNode XmlNodoFiglio in XmlNodoIn.ChildNodes)
            //{
            //    ucCombinazioneProp controlloDaAggiungere = new ucCombinazioneProp(ucCombinazioneProp.TipoRiga.Dati, nodoUnico);
            //    controlloDaAggiungere.apriXml(XmlNodoFiglio);
            //    spConbinazioni.Children.Add(controlloDaAggiungere);
            //}

            //foreach (ucCombinazioneProp combPro in spConbinazioni.Children)
            //{

            //}
            reazioniCalcolo.Clear();
            foreach (XmlNode XmlNodoFiglio in XmlNodoIn)
            {
                
                if (XmlNodoFiglio.Name == "Combinazione")
                {
                    reazioniCalcolo.Add(new Reazione(XmlNodoFiglio));
                }
            }
            lvComb.ItemsSource = null;
            lvComb.ItemsSource = reazioniCalcolo;
        }
    }
}

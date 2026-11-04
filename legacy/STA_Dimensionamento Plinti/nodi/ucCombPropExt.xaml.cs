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
    /// Logica di interazione per ucCombPropExt.xaml
    /// </summary>
    public partial class ucCombPropExt : UserControl
    {
        /// <summary>
        /// Nodo associato al controllo
        /// </summary>
        public Nodo nodo { get; set; }

        public ucCombPropExt()
        {
            InitializeComponent();
        }

        //public event EventHandler<EventArgs> letturaNodoInCorso;


        /// <summary>
        /// Costruttore a partire dal nodo
        /// </summary>
        /// <param name="nodoIn"></param>
        public ucCombPropExt(Nodo nodoIn) : this()
        {
            nodo = nodoIn;

            expNodo.Header = nodo.Nome;

            ucCombinazioneProp rigaTitolo = new ucCombinazioneProp(ucCombinazioneProp.TipoRiga.Titolo, nodo.Nome, nodo.Reazioni[0]);
            rigaTitolo.Name = "combRow" + spCombos.Children.Count.ToString();
            spCombos.Children.Add(rigaTitolo);

            foreach (Reazione rea in nodo.Reazioni)
            {
                ucCombinazioneProp rigaComb = new ucCombinazioneProp(ucCombinazioneProp.TipoRiga.Dati, nodo.Nome, rea);
                    
                spCombos.Children.Add(rigaComb);
            }
        }


        public XmlNode salvaXml()
        {
            XmlNode xmlNodo = generali.Statici.xmldoc.CreateNode(XmlNodeType.Element, "Nodo", "");

            XmlAttribute nodoNome = generali.Statici.xmldoc.CreateAttribute("Nome");
            nodoNome.Value = nodo.Nome;
            xmlNodo.Attributes.Append(nodoNome);

            foreach (Reazione rea in nodo.Reazioni)
            {
                xmlNodo.AppendChild(rea.salvaXml());
            }

            return xmlNodo;
        }


        /// <summary>
        /// metodo per il calcolo dei plinti
        /// </summary>
        /// <param name="PlintiIn">Lista dei plinti in ingresso</param>
        public void calcolaMultiPlinto(ListaPlinti PlintiIn)
        {
            foreach (ucCombinazioneProp ControlloCombExt in spCombos.Children)
            {
                ControlloCombExt.calcolaMultiPlinto(PlintiIn);
            }
        }
    }
}

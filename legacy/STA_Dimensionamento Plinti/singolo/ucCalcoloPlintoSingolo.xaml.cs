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
    /// Logica di interazione per ucCalcoloPlintoSingolo.xaml
    /// </summary>
    public partial class ucCalcoloPlintoSingolo : UserControl
    {
        private Plinto _plinto { get; set; }

        public Plinto plinto { get { return _plinto; } }

        private void PrendiPlintoValidato(object sender, AggiuntoPlintoValido e)
        {
            IUserControlPLinto controlloValidato = (IUserControlPLinto)sender;

            _plinto = (Plinto)e.plinto;
            controlloCombinazioni.plintoCalc = _plinto;
        }

        public ucCalcoloPlintoSingolo()
        {
            
            InitializeComponent();
            controlloPlinto.PlintoValidato += PrendiPlintoValidato;
        }

        public XmlNode salvaXml()
        {
            XmlNode nodoContenuto = generali.Statici.xmldoc.CreateNode(XmlNodeType.Element, "Calcolo_Plinto", "");

            nodoContenuto.AppendChild(controlloPlinto.salvaXml());

            nodoContenuto.AppendChild(controlloCombinazioni.salvaXml());

            return nodoContenuto;
        }

        public void apriXml(XmlNode nodoIn)
        {
            controlloPlinto.leggiXml(nodoIn.SelectSingleNode("Plinto"));
            controlloCombinazioni.apriXml(nodoIn.SelectSingleNode("Combinazioni"));

        }
    }


}

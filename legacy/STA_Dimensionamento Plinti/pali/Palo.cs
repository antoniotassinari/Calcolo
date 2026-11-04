using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using STA.Geometria;
using System.Xml;

namespace STA_Dimensionamento_Plinti.pali
{
    /// <summary>
    /// Classe astratta del Palo
    /// </summary>
    public class Palo : IPalo
    {
        #region proprietà private
        private double sDiametro { get; set; }
        private Punto2D sPosizione = new Punto2D();
        private double sPortataLimite { get; set; }

        #endregion


        #region proprietà pubbliche

        /// <summary>
        /// Diametro del palo [kN]
        /// </summary>
        public double Diametro { get { return sDiametro; } }
        /// <summary>
        /// Posizione del Palo 
        /// </summary>
        public Punto2D Posizione { get { return sPosizione; } }
        /// <summary>
        /// Portata Limite del Palo [kN]
        /// </summary>
        public double PortataLimite { get { return sPortataLimite; } }

        #endregion

        #region Costruttore
                /// <summary>
        /// Costruttore di Base, richiede di fornire quantomeno il diametro
        /// </summary>
        /// <param name="diametro"></param>
        public Palo(double diametro, double portataLimite)
        {
            sDiametro = diametro;
            sPortataLimite = portataLimite;
        }

        /// <summary>
        /// Costruttore a partire dal diametro, dalla portat limite e dalla posizione
        /// </summary>
        /// <param name="diametro">Diametro [cm]</param>
        /// <param name="portataLimite">Portata Limite del palo [kN]</param>
        /// <param name="posizione">posizione [cm]</param>
        public Palo(double diametro, double portataLimite, Punto2D posizione) : this (diametro, portataLimite)
        {
            sPosizione = posizione;
        }

        /// <summary>
        /// Costruttore con nodo XML
        /// </summary>
        /// <param name="nodoIn">nodo XML in ingresso</param>
        public Palo(XmlNode nodoIn)
        {
            sDiametro = double.Parse(nodoIn.Attributes["prop_Diametro"].Value);
            sPortataLimite = double.Parse(nodoIn.Attributes["prop_Portata"].Value);
            Punto2D posizione = new Punto2D();
            posizione.X = double.Parse(nodoIn.Attributes["prop_X"].Value);
            posizione.Y = double.Parse(nodoIn.Attributes["prop_Y"].Value);
            AssegnaPosizione(posizione);
        }

        #endregion

        #region metodi pubblici

        /// <summary>
        /// Area della sezione del palo in cmq
        /// </summary>
        /// <returns>restituisce un double</returns>
        public double Area()
        {
            return Math.Pow(Diametro / 2, 2) * Math.PI;
        }

        public void AssegnaPosizione(Punto2D puntoPosizione)
        {
            sPosizione = puntoPosizione;
        }



        /// <summary>
        /// Metodo per il salvataggio del palo trivellato
        /// </summary>
        /// <returns>è ancora da implementare</returns>
        public XmlNode salvaXML()
        {
            XmlNode nodoProvv = generali.Statici.xmldoc.CreateNode(XmlNodeType.Element, "Palo", "");

            XmlAttribute nodoTipo = generali.Statici.xmldoc.CreateAttribute("prop_Tipo");
            nodoTipo.Value = "Trivellato";
            nodoProvv.Attributes.Append(nodoTipo);

            XmlAttribute nodoDiametro = generali.Statici.xmldoc.CreateAttribute("prop_Diametro");
            nodoDiametro.Value = Diametro.ToString();
            nodoProvv.Attributes.Append(nodoDiametro);

            XmlAttribute nodoX = generali.Statici.xmldoc.CreateAttribute("prop_X");
            nodoX.Value = Posizione.X.ToString();
            nodoProvv.Attributes.Append(nodoX);

            XmlAttribute nodoY = generali.Statici.xmldoc.CreateAttribute("prop_Y");
            nodoY.Value = Posizione.Y.ToString();
            nodoProvv.Attributes.Append(nodoY);

            XmlAttribute nodoPortata = generali.Statici.xmldoc.CreateAttribute("prop_Portata");
            nodoPortata.Value = PortataLimite.ToString();
            nodoProvv.Attributes.Append(nodoPortata);

            return nodoProvv;
        }

        #endregion

    }


}

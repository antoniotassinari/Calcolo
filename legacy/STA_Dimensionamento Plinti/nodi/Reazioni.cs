using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Xml;

namespace STA_Dimensionamento_Plinti
{
    partial class Reazioni : List<Reazione>
    {

    }

    public class Verifica
    {

        private double sEx {get; set;}
        private double sEy {get; set;}
        private double sAreaEff { get; set; }
        private double sPressione { get; set; }

        public double Ex { get { return sEx; } }
        public double Ey { get { return sEy; } }
        public double AreaEff { get { return sAreaEff; } }
        public double Pressione { get { return sPressione; } }

        public Verifica(Reazione rea, Plinto pli)
        {
            sEx = rea.MX*100*100/ (pli.PesoTotale() + rea.FZ*100);
            sEy = rea.MY * 100 * 100 / (pli.PesoTotale() + rea.FZ*100);

            sAreaEff = (pli.A - 2 * Ex) * (pli.B - 2 * Ey);

            sPressione = (pli.PesoTotale() + rea.FZ * 100) / AreaEff;

        }
    }

    public class Reazione
    {
        //vorrei avere sempre le reazioni in kN. Per adesso lo assumo in kN. Voglio sviluppare un sistema per la conversione automatica.
        //Idea al momento: introddure nel metodo per la costruzione le unità.

        #region proprietà private

        //dichiarazione variabili protette
        private string sCombinazione = "";
        private double sFX = new double();
        private double sFY = new double();
        private double sFZ = new double();
        private double sMX = new double();
        private double sMY = new double();
        private double sMZ = new double();

        private List<verifiche.IVerificaPlinto> sListaVerifiche = new List<verifiche.IVerificaPlinto>();
        //dichiarazione metodi pubblici per definizione variabili

        #endregion


        #region proprietà pubbliche
        /// <summary>
        /// Resistuisce il nome della combinazione
        /// </summary>
        public string Combinazione
        {
            get
            {
                return sCombinazione;
            }
            set { sCombinazione = value; }
        }

        /// <summary>
        /// Resistuisce la reazione F in direzione X
        /// </summary>
        public double FX
        {
            get
            {
                return sFX;
            }
        }

        /// <summary>
        /// Resistuisce la reazione F in direzione Y
        /// </summary>
        public double FY
        {
            
            get
            {
                return sFY;
            }
        }

        /// <summary>
        /// Resistuisce la reazione F in direzione Z
        /// </summary>
        public double FZ
        {

            get
            {
                return sFZ;
            }
            set { sFZ = value; }
        }

        /// <summary>
        /// Resistuisce la reazione M in direzione X
        /// </summary>
        public double MX
        {

            get
            {
                return sMX;
            }
            set { sMX = value; }
        }

        /// <summary>
        /// Resistuisce la reazione M in direzione Y
        /// </summary>
        public double MY
        {

            get
            {
                return sMY;
            }
            set { sMY = value; }
        }

        /// <summary>
        /// Resistuisce la reazione M in direzione Z
        /// </summary>
        public double MZ
        {

            get
            {
                return sMZ;
            }
        }
        /// <summary>
        /// Lista delle verifiche. Sono in numero pari ai plinti. uso una lista di object perchè prevedo di implementare diversi tipi di plinti.
        /// </summary>
        public List<verifiche.IVerificaPlinto> ListaVerifiche { get { return sListaVerifiche; } }

        #endregion


        #region Costruttori
        /// <summary>
        /// Costruzione della classe Reazione. E' necessario introdurre il nome della combinazione ed in valori delle Forze e Momenti
        /// </summary>
        /// <param name="inCombinazione">Nome della combinazione</param>
        /// <param name="inFX">Forza in direzione X [kN]</param>
        /// <param name="inFY">Forza in direzione Y [kN]</param>
        /// <param name="inFZ">Forza in direzione Z [kN]</param>
        /// <param name="inMX">Momento con versore X [kN*m]</param>
        /// <param name="inMY">Momento con versore Y [kN*m]</param>
        /// <param name="inMZ">Momento con versore Z [kN*m]</param>
        public Reazione(string inCombinazione, double inFX, double inFY, double inFZ, double inMX, double inMY, double inMZ)
        {
            sCombinazione = inCombinazione;
            sFX = inFX;
            sFY = inFY;
            sFZ = inFZ;
            sMX = inMX;
            sMY = inMY;
            sMZ = inMZ;
            
        }


        /// <summary>
        /// Costruttore a partire da nodo XMl
        /// </summary>
        /// <param name="XmlNodoIn">Nodo Xml in ingresso</param>
        public Reazione(XmlNode XmlNodoIn)
        {
            sCombinazione = XmlNodoIn.Attributes["prop_Nome"].Value;
            sFX = double.Parse(XmlNodoIn.Attributes["prop_FX"].Value);
            sFY = double.Parse(XmlNodoIn.Attributes["prop_FY"].Value);
            sFZ = double.Parse(XmlNodoIn.Attributes["prop_FZ"].Value);
            sMX = double.Parse(XmlNodoIn.Attributes["prop_MX"].Value);
            sMY = double.Parse(XmlNodoIn.Attributes["prop_MY"].Value);
            sMZ = double.Parse(XmlNodoIn.Attributes["prop_MZ"].Value);
        }
        #endregion

        #region metodi

        /// <summary>
        /// metodo per la verifica dei plinti
        /// </summary>
        /// <param name="plintiVerifica">lista dei plinti per la verifica</param>
        public void verificaPlinti(ListaPlinti plintiVerifica)
        {
            sListaVerifiche.Clear();
            foreach (IPlinto plintoCheck in plintiVerifica)
            {
                switch (plintoCheck.Tipo)
                {
                    case TipoPlinto.Superficiale:
                        Plinto plintoCheckSuperficiale = (Plinto)plintoCheck;
                        sListaVerifiche.Add(new verifiche.verificaMeyerhof(this, plintoCheckSuperficiale));
                        break;

                    case TipoPlinto.SuPali:
                        PlintoSuPali plintoCheckSuPali = (PlintoSuPali)plintoCheck;
                        sListaVerifiche.Add(new verifiche.verificaPlintoSuPali(this, plintoCheckSuPali));
                        break;

                    default:
                        break;
                }
                
            }
        }

        
        /// <summary>
        /// Restituisce un TreeNode già formato con i figli.
        /// </summary>
        public TreeNode Nodo
        {
            get
            {
                TreeNode tnCombinazione = new TreeNode(Combinazione);
                TreeNode[] nodiAzioni = new TreeNode[6];
                nodiAzioni[0] = new TreeNode("FX: " + FX.ToString());
                nodiAzioni[1]= new TreeNode("FY: " + FY.ToString());
                nodiAzioni[2]= new TreeNode("FZ: " + FZ.ToString());
                nodiAzioni[3]= new TreeNode("MX: " + MX.ToString());
                nodiAzioni[4]= new TreeNode("MY: " + MY.ToString());
                nodiAzioni[5]= new TreeNode("MZ: " +MZ.ToString());
                tnCombinazione.Nodes.AddRange(nodiAzioni);
                return tnCombinazione;

            }
        }

       

        public XmlNode salvaXml()
        {
            XmlNode xmlNodo = generali.Statici.xmldoc.CreateNode(XmlNodeType.Element, "Combinazione", "");

            XmlAttribute nodoCombinazione = generali.Statici.xmldoc.CreateAttribute("prop_Nome");
            nodoCombinazione.Value = Combinazione;
            xmlNodo.Attributes.Append(nodoCombinazione);


            XmlAttribute nodoFX = generali.Statici.xmldoc.CreateAttribute("prop_FX");
            nodoFX.Value = FX.ToString();
            xmlNodo.Attributes.Append(nodoFX);

            XmlAttribute nodoFY = generali.Statici.xmldoc.CreateAttribute("prop_FY");
            nodoFY.Value = FY.ToString();
            xmlNodo.Attributes.Append(nodoFY);

            XmlAttribute nodoFZ = generali.Statici.xmldoc.CreateAttribute("prop_FZ");
            nodoFZ.Value = FZ.ToString();
            xmlNodo.Attributes.Append(nodoFZ);

            XmlAttribute nodoMX = generali.Statici.xmldoc.CreateAttribute("prop_MX");
            nodoMX.Value = MX.ToString();
            xmlNodo.Attributes.Append(nodoMX);

            XmlAttribute nodoMY = generali.Statici.xmldoc.CreateAttribute("prop_MY");
            nodoMY.Value = MY.ToString();
            xmlNodo.Attributes.Append(nodoMY);

            XmlAttribute nodoMZ = generali.Statici.xmldoc.CreateAttribute("prop_MZ");
            nodoMZ.Value = MZ.ToString();
            xmlNodo.Attributes.Append(nodoMZ);

            return xmlNodo;

        }
        #endregion


    }
}

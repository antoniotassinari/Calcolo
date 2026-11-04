using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using STA_Dimensionamento_Plinti.pali;
using System.Xml;

using Word = Microsoft.Office.Interop.Word;
using STA.Geometria;

namespace STA_Dimensionamento_Plinti
{
    /// <summary>
    /// Enumerazione delle tipologie di plinto previste
    /// </summary>
    public enum TipoPlinto
    {
        /// <summary>
        /// Fondazione superficiale
        /// </summary>
        Superficiale,
        /// <summary>
        /// Fondazione su Pali
        /// </summary>
        SuPali

    }

    
    public enum PlintoValido
    {
        Valido,
        NonValido
    }

    /// <summary>
    /// Classe per la gestione della lista di plinti
    /// </summary>
    public partial class ListaPlinti : List<IPlinto>
    {


        /// <summary>
        /// Lista dei plinti Superficiali
        /// </summary>
        private List<Plinto> Superficiali = new List<Plinto>();

        /// <summary>
        /// Lista dei plinti Su Pali
        /// </summary>
        private List<PlintoSuPali> SuPali = new List<PlintoSuPali>();

        /// <summary>
        /// Metodo per salvare in XML
        /// </summary>
        /// <returns>Nodo XML strutturato</returns>
        public XmlNode salvaXML()
        {
            XmlNode xmlNodo = generali.Statici.xmldoc.CreateNode(XmlNodeType.Element, "Plinti", "");

            foreach (IPlinto plintoIn in this)
            {
                xmlNodo.AppendChild(plintoIn.salvaXML());
            }

            return xmlNodo;
        }

        /// <summary>
        /// metodo per riempire la lista da nodo XML
        /// </summary>
        /// <param name="nodoIN"></param>
        public void apriXML(XmlNode nodoXML)
        {
            this.Clear();
            foreach (XmlNode nodoIn in nodoXML.ChildNodes)
            {
                if (nodoIn.Name == "Plinto")
                {
                    if (nodoIn.Attributes["prop_Tipo"].Value == "Superficiale")
                    {
                        this.Add(new Plinto(nodoIn));
                    }
                    if (nodoIn.Attributes["prop_Tipo"].Value == "SuPali")
                    {
                        this.Add(new PlintoSuPali(nodoIn));

                    }

                }

            }
            
        }

        public void aggiungiTabellePlinti(ref Word.Document docWord)
        {
            object oMissing = System.Reflection.Missing.Value;
            separa();
            object endOfDoc = "\\endofdoc";
            //Word.Paragraph paraPlinti;
            Word.Range rng = docWord.Bookmarks.get_Item(ref endOfDoc).Range;
                      


            //Prima le tabelle dei plinti su pali

            Word.Table tabellaPlintiSuperficiali;
            //Word.Table tabellaPlintiSuPali;

            if (Superficiali.Count != 0)
            {
                tabellaPlintiSuperficiali = docWord.Tables.Add(rng, Superficiali.Count+1, 5, ref oMissing, ref oMissing);
                int i = 1;
                tabellaPlintiSuperficiali.Cell(i,1).Range.Text = "Numero Tipologia";
                tabellaPlintiSuperficiali.Cell(i,2).Range.Text = "A";
                tabellaPlintiSuperficiali.Cell(i,3).Range.Text="B";
                tabellaPlintiSuperficiali.Cell(i,4).Range.Text = "H";
                tabellaPlintiSuperficiali.Cell(i,5).Range.Text= "Q";
                foreach (Plinto plintoIn in Superficiali)
                {
                    i++;
                    tabellaPlintiSuperficiali.Cell(i, 1).Range.Text = i.ToString("F0");
                    tabellaPlintiSuperficiali.Cell(i, 2).Range.Text = plintoIn.A.ToString("F0");
                    tabellaPlintiSuperficiali.Cell(i, 3).Range.Text = plintoIn.B.ToString("F0");
                    tabellaPlintiSuperficiali.Cell(i, 4).Range.Text = plintoIn.H.ToString("F0");
                    tabellaPlintiSuperficiali.Cell(i, 5).Range.Text = plintoIn.Imposta.ToString("F0");
                }
                tabellaPlintiSuperficiali.Rows[1].Range.Font.Bold = 1;
                
            }


        }

        /// <summary>
        /// Metodo per separare i plinti superficiali da quelli su pali
        /// </summary>
        private void separa()
        {
            Superficiali.Clear();
            SuPali.Clear();
            foreach (IPlinto iplinto in this)
            {
                switch (iplinto.Tipo)
                {
                    case TipoPlinto.Superficiale:
                        Superficiali.Add((Plinto)iplinto);
                        break;

                    case TipoPlinto.SuPali:
                        SuPali.Add((PlintoSuPali)iplinto);
                        break;
                }
            }


        }
    }

    

    /// <summary>
    /// Classe per la gestione del plinto
    /// </summary>
    public class Plinto : IPlinto, IComparable
    {

        #region proprietà private

        private double sA { get; set; }
        private double sB { get; set; }
        private double sH { get; set; }
        /// <summary>
        /// tipo di plinto
        /// </summary>
        private TipoPlinto sTipo { get; set; }
        private double sImposta { get; set; }
        private double sPressioneLimite { get; set; }

        //proprietà di base del plinto
        private double sArea { get; set; } //in cm
        private double sVolume { get; set; } //in mc
        private double sPeso { get; set; }  //in daN

        //proprietà terreno
        private double sPesoSpecTerreno {get; set;} //in daN per mc
        private double sPesoTerrenoSopra {get; set;} //in daN

        //posizione eccentrica
        private Punto2D sEccentricitaPilastro = new Punto2D();
        
        #endregion

        #region proprietà pubbliche

        /// <summary>
        /// Dimensione sezione lungo X globale
        /// </summary>
        public double A { get { return sA; } }
        /// <summary>
        /// Dimensione sezione lungo Y globale
        /// </summary>
        public double B { get { return sB; } }
        /// <summary>
        /// Spessore della sezione
        /// </summary>
        public double H {get {return sH;}}
        /// <summary>
        /// Tipologia di plinto
        /// </summary>
        public TipoPlinto Tipo { get { return sTipo; } }
        /// <summary>
        /// Quota di imposta della fondazione
        /// </summary>
        public double Imposta { get { return sImposta; } }
        /// <summary>
        /// Posizione del pilastro rispetto al baricentro del plinto
        /// </summary>
        public Punto2D EccentricitaPilastro { get { return sEccentricitaPilastro; } }

        /// <summary>
        /// Valore limite della pressione sul terreno
        /// </summary>
        public double ValoreLimite { get { return sPressioneLimite; } }


        /// <summary>
        /// Area di base cm
        /// </summary>
        public double Area { get { return sArea; } }
        /// <summary>
        /// volume di cls del plinto in mc
        /// </summary>
        public double Volume { get { return sVolume; } }
        /// <summary>
        /// Peso del plinto in daN
        /// </summary>
        public double Peso { get { return sPeso; } }
        /// <summary>
        /// Peso Specifico Terreno in daN su mc
        /// </summary>
        public double PesoSpecTerreno { get { return sPesoSpecTerreno; } }

        public bool ConsideraTerrenoSopra = false;
        public PlintoValido validità = PlintoValido.NonValido;

        public double PesoTerrenoSopra { get { return sPesoTerrenoSopra; } }
        #endregion

        #region Costruttori

        /// <summary>
        /// Costruttore della classe di base
        /// </summary>
        /// <param name="tipoIN">Tipologia del plinto</param>
        /// <param name="dimensioneX">dimensione in direzione X globale in cm</param>
        /// <param name="dimensioneY">dimensione in direzione Y globale in cm</param>
        /// <param name="spessore">spessore della sezione in cm</param>
        /// <param name="imposta">quota di imposta della sezione in cm, deve essere negativo</param>
        public Plinto(TipoPlinto tipoIN, double dimensioneX, double dimensioneY, double spessore, double imposta, double portataLimite)
        {
            sTipo = tipoIN;
            sA = dimensioneX;
            sB = dimensioneY;
            sH = spessore;
            sImposta = imposta;
            sPressioneLimite = portataLimite;
            sEccentricitaPilastro.X = 0;
            sEccentricitaPilastro.Y = 0;
            calcolaProprietàBase();
        }

        /// <summary>
        /// Costruttore a partitre dal nodo XML strutturato
        /// </summary>
        /// <param name="nodoIn">nodo XML strutturato</param>
        public Plinto(XmlNode nodoIn)
        {
            sA = double.Parse(nodoIn.Attributes["prop_A"].Value);
            sB = double.Parse(nodoIn.Attributes["prop_B"].Value);
            sH = double.Parse(nodoIn.Attributes["prop_H"].Value);
            sImposta = double.Parse(nodoIn.Attributes["prop_Q"].Value);
            sPressioneLimite = double.Parse(nodoIn.Attributes["prop_PresLim"].Value);
            try
            {
                assegnaPosizione(double.Parse(nodoIn.Attributes["prop_EccX"].Value), double.Parse(nodoIn.Attributes["prop_EccY"].Value));
            }
            catch { }
            ConsideraTerrenoSopra = bool.Parse(nodoIn.Attributes["prop_ConsideraTerreno"].Value);

            if (ConsideraTerrenoSopra == true)
            {
                sPesoSpecTerreno = double.Parse(nodoIn.Attributes["prop_GammaTerreno"].Value);
            }

            calcolaProprietàBase();
        }

        #endregion

        #region metodi pubblici
        /// <summary>
        /// Metodo interno che calcola le proprietà di base del plinto
        /// </summary>
        private void calcolaProprietàBase()
        {
            sArea = A * B;  //calcola l'area
            sVolume = (A / 100) * (B / 100) * (H / 100); //calcola il volume
            sPeso = Volume * 2500;  //calcolo il peso

            validità = PlintoValido.Valido;
        }

        /// <summary>
        /// Metodo che restituisce il nodo XML strutturato
        /// </summary>
        /// <returns>Nodo XML</returns>
        public XmlNode salvaXML()
        {
            XmlNode nodoProvv = generali.Statici.xmldoc.CreateNode(XmlNodeType.Element, "Plinto", "");

            XmlAttribute nodoTipo = generali.Statici.xmldoc.CreateAttribute("prop_Tipo");
            nodoTipo.Value = Tipo.ToString();
            nodoProvv.Attributes.Append(nodoTipo);

            XmlAttribute nodoA = generali.Statici.xmldoc.CreateAttribute("prop_A");
            nodoA.Value = A.ToString();
            nodoProvv.Attributes.Append(nodoA);

            XmlAttribute nodoB = generali.Statici.xmldoc.CreateAttribute("prop_B");
            nodoB.Value = B.ToString();
            nodoProvv.Attributes.Append(nodoB);

            XmlAttribute nodoH = generali.Statici.xmldoc.CreateAttribute("prop_H");
            nodoH.Value = H.ToString();
            nodoProvv.Attributes.Append(nodoH);

            XmlAttribute nodoImposta = generali.Statici.xmldoc.CreateAttribute("prop_Q");
            nodoImposta.Value = Imposta.ToString();
            nodoProvv.Attributes.Append(nodoImposta);

            XmlAttribute nodoPressioneLimite = generali.Statici.xmldoc.CreateAttribute("prop_PresLim");
            nodoPressioneLimite.Value = ValoreLimite.ToString();
            nodoProvv.Attributes.Append(nodoPressioneLimite);

            XmlAttribute nodoEccX = generali.Statici.xmldoc.CreateAttribute("prop_EccX");
            nodoEccX.Value = EccentricitaPilastro.X.ToString();
            nodoProvv.Attributes.Append(nodoEccX);

            XmlAttribute nodoEccY = generali.Statici.xmldoc.CreateAttribute("prop_EccY");
            nodoEccY.Value = EccentricitaPilastro.Y.ToString();
            nodoProvv.Attributes.Append(nodoEccY);

            XmlAttribute nodoConsideraTerreno = generali.Statici.xmldoc.CreateAttribute("prop_ConsideraTerreno");
            nodoConsideraTerreno.Value = ConsideraTerrenoSopra.ToString();
            nodoProvv.Attributes.Append(nodoConsideraTerreno);

            if (ConsideraTerrenoSopra == true)
            {
                XmlAttribute nodoGammaTerreno = generali.Statici.xmldoc.CreateAttribute("prop_GammaTerreno");
                nodoGammaTerreno.Value = PesoSpecTerreno.ToString();
                nodoProvv.Attributes.Append(nodoGammaTerreno);
            }

            return nodoProvv;
        }


        /// <summary>
        /// Metodo Carico totale superiore, considera se il terreno deve essere considerato.
        /// </summary>
        /// <returns>restituisce il peso complessivo in kN</returns>
        public double PesoTotale()
        {
            if (ConsideraTerrenoSopra == true)
            {
                return Peso + PesoTerrenoSopra;
            }
            else
            {
                return Peso;
            }

        }

        /// <summary>
        /// metodo per assegnare e considerare il terreno
        /// </summary>
        /// <param name="pesoTerreno">peso specifico del terreno in daN/mc</param>
        public void assegnaTerreno(double pesoTerreno)
        {
            ConsideraTerrenoSopra = true;

            sPesoSpecTerreno = pesoTerreno;

            sPesoTerrenoSopra = Area / 10000 * Math.Abs(Imposta) / 100 * pesoTerreno;

        }

        /// <summary>
        /// Metodo per non considerare il terreno nel plinto
        /// </summary>
        public void nonConsiderareTerreno()
        {
            ConsideraTerrenoSopra = false;
        }

        /// <summary>
        /// Volume del terreno portato
        /// </summary>
        /// <returns>Volume del terreno in mc</returns>
        public double VolumeTerrenoSup()
        {
            return (Imposta/100 - H/100) * A/100 * B/100;
        }

        /// <summary>
        /// assegna l'eccentricità del pilastro rispetto al baricentro del plin
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void assegnaPosizione(double x, double y)
        {
            sEccentricitaPilastro.X = x;
            sEccentricitaPilastro.Y = y;
        }

        /// <summary>
        /// Restituisce una stringa con le caratteristiche del Plinto
        /// </summary>
        /// <returns>Stringa formata</returns>
        public string CaratteristichePlinto()
        {
            string uscita = "";

            uscita += Tipo.ToString();
            uscita += " | " + A.ToString("F0");
            uscita += " x " + B.ToString("F0");
            uscita += " x " + H.ToString("F0");
            uscita += " | Q = " + Imposta.ToString("F0");
            uscita += " | pLim = " + ValoreLimite.ToString("F2");
            uscita += " | Ecc: (" + EccentricitaPilastro.Y.ToString() + "," + EccentricitaPilastro.Y.ToString() + ")";
            if (ConsideraTerrenoSopra == true)
            {
                uscita += " | Con Terreno Sopra (gamma = " + PesoSpecTerreno.ToString("F0") + ")";
            }

            return uscita;
        }

        public override string ToString()
        {
            return CaratteristichePlinto();
        }

        #endregion



        #region IComparable Membri di
        /// <summary>
        /// Metodo per il confronto della classe
        /// </summary>
        /// <param name="other">Plinto di confronto</param>
        /// <returns>Restituisce il confronto</returns>
        public int CompareTo(object o)
        {
            IPlinto other = (IPlinto)o;
            if (other.Tipo == TipoPlinto.Superficiale)
            {
                Plinto otherSuperficiale = (Plinto)other;
                if (otherSuperficiale.Volume.CompareTo(this.Volume) == 0)
                {
                    return this.Area.CompareTo(otherSuperficiale.Area);
                }
                else
                {
                    return this.Volume.CompareTo(otherSuperficiale.Volume);
                }
            }
            else
            {
                return -1;
            }
        }

        #endregion


    }

    public enum TipoligiePlintiPali
    {
        RettangolareRegolare,
        Triangolare,
    }


    /// <summary>
    /// Classe per la gestione dei plinti su pali
    /// </summary>
    public class PlintoSuPali : IPlinto , IComparable
    {
        #region proprietà private

        private List<Palo> sPali = new List<Palo>();

        private double sA { get; set; }

        private double sB { get; set; }

        private double sH { get; set; }

        private int sOrdini { get; set; }

        private int sColonne { get; set; }

        //private Figura sSagoma { get; set; }

        private double sDistanzaPali { get; set; }

        private TipoligiePlintiPali sTipoSuPali {get; set;}

        #endregion




        #region proprietà pubbliche

        /// <summary>
        /// Tipo di plinto.
        /// </summary>
        public TipoPlinto Tipo { get { return TipoPlinto.SuPali; } }

        /// <summary>
        /// Altezza totale se rettangolare
        /// </summary>
        public double A { get { return sA; } }

        /// <summary>
        /// Larghezza totale
        /// </summary>
        public double B { get { return sB; } }

        /// <summary>
        /// Spessore
        /// </summary>
        public double H { get { return sH; } }

        /// <summary>
        /// Ordini di pali
        /// </summary>
        public int Ordini { get { return sOrdini; } }

        /// <summary>
        /// Colonne di pali
        /// </summary>
        public int Colonne { get { return sColonne; } }


        /// <summary>
        /// Elenco dei pali presenti nel plinto
        /// </summary>
        public List<Palo> Pali { get { return sPali; } }

        /// <summary>
        /// distanza tra un palo e l'altro
        /// </summary>
        public double DistanzaPali { get { return sDistanzaPali; } }

        /// <summary>
        /// Valore limite di portata del Plinto
        /// </summary>
        public double ValoreLimite
        {
            get
            {
                if (Pali[0] != null)
                {
                    return Pali[0].PortataLimite;
                }
                else { return 0; }
            }
        }

        /// <summary>
        /// Restituisce la tipologia di plinto su pali
        /// </summary>
        public TipoligiePlintiPali TipoSuPali { get { return sTipoSuPali; } }

        #endregion

        #region Costruttori

        /// <summary>
        /// Costruttore generico, per poi far "funzionare" la classe è necessario invocare un metodo di creazione. Al momento sono previsto solo pali rettangolari e con ordini e colonne regolari di pali
        /// </summary>
        public PlintoSuPali()
        {

        }

        /// <summary>
        /// Costruttore a partire dal nodo XML strutturato
        /// </summary>
        /// <param name="nodoIn">nodo XML in ingresso</param>
        public PlintoSuPali(XmlNode nodoIn)
        {

                if (nodoIn.Attributes["prop_TipoSuPali"].Value == "RettangolareRegolare")
                {

                    sOrdini = int.Parse(nodoIn.Attributes["prop_Ordini"].Value);
                    sColonne = int.Parse(nodoIn.Attributes["prop_Colonne"].Value);
                    sH = double.Parse(nodoIn.Attributes["prop_Spessore"].Value);
                    foreach (XmlNode nodoPalo in nodoIn.ChildNodes)
                    {
                        if (nodoPalo.Name == "Palo")
                        {

                            sPali.Add(new Palo(nodoPalo));
                        }
                    }
                    sA = Pali[0].Diametro * ((3 * (Ordini - 1) + 2));
                    sB = Pali[0].Diametro * ((3 * (Colonne - 1) + 2));
                }

                if (nodoIn.Attributes["prop_TipoSuPali"].Value == "Triangolare")
                {
                    foreach (XmlNode nodoPalo in nodoIn.ChildNodes)
                    {
                        if (nodoPalo.Name == "Palo")
                        {

                            sPali.Add(new Palo(nodoPalo));
                        }
                    }
                    generaPlintoTriangolare(Pali[0], double.Parse(nodoIn.Attributes["prop_Spessore"].Value));

                }
            
        }

        #endregion


        #region Metodi Pubblici
        /// <summary>
        /// metodo per generare il plinto rettangolare e con ordini e colonne regolari di pali. 
        /// </summary>
        /// <param name="altezzaSezione">altezza della sezione rettangolare su questa si dispongono gli ordini</param>
        /// <param name="larghezzaSezione">larghezza della sezione su questa si dispongono le colonne</param>
        /// <param name="Spessore">spessore del plinto</param>
        /// <param name="diametroPali">diametro dei pali</param>
        /// <param name="ordini">ordini di pali</param>
        /// <param name="colonne">colonne di pali</param>
        public void generaPlintoRettangolare(Palo palo, int ordini, int colonne, double spessore)
        {
            sTipoSuPali = TipoligiePlintiPali.RettangolareRegolare;
            sPali.Clear();
            sColonne = colonne;
            sOrdini = ordini;
            sH = spessore;
            sA = palo.Diametro * ((3 * (ordini - 1) + 2));
            sB = palo.Diametro * ((3 * (colonne - 1) + 2));
            int i = 0;
            for (i = 0; i < ordini; i++)
            {
                int j = 0;
                for (j = 0; j < colonne; j++)
                {
                    Punto2D posizionePalo = new Punto2D();
                    posizionePalo.X = 3 * palo.Diametro * (colonne - 1) / 2 - j * 3 * palo.Diametro;
                    posizionePalo.Y = 3 * palo.Diametro * (ordini - 1) / 2 - i * 3 * palo.Diametro;
                    palo.AssegnaPosizione(posizionePalo);

                    sPali.Add(new Palo(palo.Diametro, palo.PortataLimite, posizionePalo));

                }

            }



        }

        public void generaPlintoTriangolare(Palo palo, double spessore)
        {
            sTipoSuPali = TipoligiePlintiPali.Triangolare;
            sPali.Clear();
            sColonne = 1;
            sOrdini = 1;
            sH = spessore;
            sA = palo.Diametro * 5;
            sB = palo.Diametro * 5;

            //lunghezza del cateto del triangolo rettangolo che forma metà della figura
            double cateto = palo.Diametro * 3/2 * Math.Sqrt(3);

            //Aggiunta dei pali

            //primo palo in alto
            Punto2D posizionePalo1 = new Punto2D();
            posizionePalo1.X = 0;
            posizionePalo1.Y = cateto * 2 / 3;
            sPali.Add(new Palo(palo.Diametro, palo.PortataLimite, posizionePalo1));

            //secondo palo in basso a destra
            Punto2D posizionePalo2 = new Punto2D();
            posizionePalo2.X = 1.5 * palo.Diametro;
            posizionePalo2.Y = -1/3 * cateto;
            sPali.Add(new Palo(palo.Diametro, palo.PortataLimite, posizionePalo2));

            //terzo palo in basso a sinistra
            Punto2D posizionePalo3 = new Punto2D();
            posizionePalo3.X = -1.5 * palo.Diametro;
            posizionePalo3.Y = -1/3* palo.Diametro;
            sPali.Add(new Palo(palo.Diametro, palo.PortataLimite, posizionePalo3));




        }
        

        /// <summary>
        /// Area della sezione in pianta [mq]
        /// </summary>
        /// <returns></returns>
        public double Area()
        {
            return sA * sB;
        }

        /// <summary>
        /// Volume del corpo del plinto [mc]
        /// </summary>
        /// <returns></returns>
        public double Volume()
        {
            return Area() * sH;
        }

        /// <summary>
        /// Area totale dei Pali in mq
        /// </summary>
        /// <returns>area dei pali</returns>
        public double AreaPali()
        {
            double AreaProv = 0;
            foreach (IPalo palo in sPali)
            {
                AreaProv += palo.Area();
            }
            return AreaProv;

        }

        /// <summary>
        /// Metodo che restituisce l'inerzia attorno all'asse x
        /// </summary>
        /// <returns></returns>
        public double Jx()
        {
            double prov = 0;
            foreach (IPalo palo in Pali)
            {
                prov += palo.Area()/10000 * Math.Pow(palo.Posizione.Y/100, 2);
            }
            return prov;

        }

        /// <summary>
        /// Metodo che restituisce l'inerzia attorno all'asse y
        /// </summary>
        /// <returns></returns>
        public double Jy()
        {
            double prov = 0;
            foreach (IPalo palo in Pali)
            {
                prov += palo.Area()/10000 * Math.Pow(palo.Posizione.X/100, 2);
            }
            return prov;
        }

        #endregion

        #region IPlinto Membri di

        public XmlNode salvaXML()
        {
            XmlNode nodoProvv = generali.Statici.xmldoc.CreateNode(XmlNodeType.Element, "Plinto", "");

            XmlAttribute nodoTipo = generali.Statici.xmldoc.CreateAttribute("prop_Tipo");
            nodoTipo.Value = Tipo.ToString();
            nodoProvv.Attributes.Append(nodoTipo);

            XmlAttribute nodoTipoSuPali = generali.Statici.xmldoc.CreateAttribute("prop_TipoSuPali");
            nodoTipoSuPali.Value = TipoSuPali.ToString();
            nodoProvv.Attributes.Append(nodoTipoSuPali);

            XmlAttribute nodoOrdini = generali.Statici.xmldoc.CreateAttribute("prop_Ordini");
            nodoOrdini.Value = sOrdini.ToString();
            nodoProvv.Attributes.Append(nodoOrdini);

            XmlAttribute nodoColonne = generali.Statici.xmldoc.CreateAttribute("prop_Colonne");
            nodoColonne.Value = sColonne.ToString();
            nodoProvv.Attributes.Append(nodoColonne);

            XmlAttribute nodoSpessore = generali.Statici.xmldoc.CreateAttribute("prop_Spessore");
            nodoSpessore.Value = H.ToString();
            nodoProvv.Attributes.Append(nodoSpessore);

            foreach (IPalo paloIn in Pali)
            {
                nodoProvv.AppendChild(paloIn.salvaXML());
            }

            return nodoProvv;
        }

        /// <summary>
        /// Restituisce una stringa di riepilogo caratteritstiche
        /// </summary>
        /// <returns></returns>
        public string CaratteristichePlinto()
        {
            string uscita = "";

            uscita += Tipo.ToString();
            uscita += " | " +  TipoSuPali.ToString();
            uscita += " | Ø" +  Pali[0].Diametro.ToString("F0"); //prende il diametro del primo dei pali, poichè sono comunque tutti uguali

            switch (TipoSuPali)
            {
                case TipoligiePlintiPali.RettangolareRegolare:
                    uscita += " | " + sOrdini.ToString("F0");
                    uscita += " x " + sColonne.ToString("F0");
                    break;

                case TipoligiePlintiPali.Triangolare:
                    break;
            }
            
            
            uscita += " | H = " + H.ToString("F0");
            uscita += " | P.Lim = " + ValoreLimite.ToString("F0");
            return uscita;
        }

        #endregion

        #region IComparable Membri di

        /// <summary>
        /// Metodo per confrontare la classe
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            IPlinto other = (IPlinto)obj;
            //effettua prima il confronto con altri plinti su pali
            if (other.Tipo == TipoPlinto.SuPali)
            {
                PlintoSuPali otherSuPali = (PlintoSuPali)other;
                if (otherSuPali.Pali[0].Diametro.CompareTo(this.Pali[0].Diametro) == 0)
                {
                    return this.Pali.Count.CompareTo(otherSuPali.Pali.Count);
                }
                else
                {
                    return this.Pali[0].Diametro.CompareTo(otherSuPali.Pali[0].Diametro);
                }
            }
                //in confronto ai plinti superficiali è comunque "superiore"
            else
            {
                return 1;
            }
        }

        public override string ToString()
        {
            return CaratteristichePlinto();
        }

        #endregion
    }
}

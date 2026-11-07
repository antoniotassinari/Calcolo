using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using STA_Dimensionamento_Plinti.pali;
using System.Xml;


using STA.Geometria;

namespace STA_Dimensionamento_Plinti;

public class PlintoSuPali : IPlinto, IComparable
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

    private TipoligiePlintiPali sTipoSuPali { get; set; }

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
        double cateto = palo.Diametro * 3 / 2 * Math.Sqrt(3);

        //Aggiunta dei pali

        //primo palo in alto
        Punto2D posizionePalo1 = new Punto2D();
        posizionePalo1.X = 0;
        posizionePalo1.Y = cateto * 2 / 3;
        sPali.Add(new Palo(palo.Diametro, palo.PortataLimite, posizionePalo1));

        //secondo palo in basso a destra
        Punto2D posizionePalo2 = new Punto2D();
        posizionePalo2.X = 1.5 * palo.Diametro;
        posizionePalo2.Y = -1 / 3 * cateto;
        sPali.Add(new Palo(palo.Diametro, palo.PortataLimite, posizionePalo2));

        //terzo palo in basso a sinistra
        Punto2D posizionePalo3 = new Punto2D();
        posizionePalo3.X = -1.5 * palo.Diametro;
        posizionePalo3.Y = -1 / 3 * palo.Diametro;
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
            prov += palo.Area() / 10000 * Math.Pow(palo.Posizione.Y / 100, 2);
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
            prov += palo.Area() / 10000 * Math.Pow(palo.Posizione.X / 100, 2);
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
        uscita += " | " + TipoSuPali.ToString();
        uscita += " | Ø" + Pali[0].Diametro.ToString("F0"); //prende il diametro del primo dei pali, poichè sono comunque tutti uguali

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

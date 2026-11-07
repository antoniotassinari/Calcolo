using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using STA_Dimensionamento_Plinti.pali;
using System.Xml;


using STA.Geometria;

namespace STA_Dimensionamento_Plinti;

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
    private double sPesoSpecTerreno { get; set; } //in daN per mc
    private double sPesoTerrenoSopra { get; set; } //in daN

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
    public double H { get { return sH; } }
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

        sPesoTerrenoSopra = Math.Max(0, VolumeTerrenoSup()) * pesoTerreno;

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
        return (Imposta / 100 - H / 100) * A / 100 * B / 100;
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

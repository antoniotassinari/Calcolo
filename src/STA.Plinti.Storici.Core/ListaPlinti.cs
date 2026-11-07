using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using STA_Dimensionamento_Plinti.pali;
using System.Xml;


using STA.Geometria;

namespace STA_Dimensionamento_Plinti;

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

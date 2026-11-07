using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using STA_Dimensionamento_Plinti.pali;
using System.Xml;


using STA.Geometria;

namespace STA_Dimensionamento_Plinti;

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

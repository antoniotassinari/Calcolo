using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace STA_Dimensionamento_Plinti
{
    /// <summary>
    /// Interfaccia per la gestione comune del plinto
    /// </summary>
    public interface IPlinto
    {
        /// <summary>
        /// Metodo per il salvataggio della struttura in XML
        /// </summary>
        /// <returns>Restituisce il metodo strutturato</returns>
        XmlNode salvaXML();

        /// <summary>
        /// Tipologia di plinto
        /// </summary>
        TipoPlinto Tipo { get; }

        /// <summary>
        /// Metodo per avere le caratteristiche del Plinto
        /// </summary>
        /// <returns>Stringa con le caratteristiche</returns>
        string CaratteristichePlinto();
        
        /// <summary>
        /// Valore limite di resistenza
        /// </summary>
        double ValoreLimite { get; }

    }
}

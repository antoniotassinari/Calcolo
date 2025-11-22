using System;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Windows;
using System.Xml.Serialization;
using STA.Geometria;
using System.ComponentModel;

namespace STA.Geometria.Masse
{
    /// <summary>
    /// Classe per la gestione della sezione
    /// </summary>
    [XmlRoot]
    public class Sezione
    {
        #region Properties

        /// <summary>
        /// Identificativo della sezione
        /// </summary>
        public string Nome { get; set; }

        /// <summary>
        /// Lista dei vertici della sezione
        /// </summary>
        public List<Punto2D> Vertici { get; set; }

        /// <summary>
        /// Area della sezione
        /// </summary>
        public double Area { get; set; }

        /// <summary>
        /// Baricentro della sezione
        /// </summary>
        public Punto2D Baricentro { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Costruttore di default
        /// </summary>
        public Sezione()
        {
            Vertici = new List<Punto2D>();
            Baricentro = new Punto2D();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Ricalcola le proprietà geometriche della sezione
        /// </summary>
        public void RicalcolaProprietà()
        {
            if (Vertici.Count >= 3)
            {
                Area = Functions.Functions.Area2D(Vertici);
                Baricentro = Functions.Functions.calcolaBaricentro(Vertici);
            }
        }

        #endregion
    }
}

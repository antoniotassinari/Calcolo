using System;
using System.ComponentModel;

namespace STA.Geometria
{
    /// <summary>
    /// Classe per la gestione dei punti bidimensionali
    /// </summary>
    public class Punto2D
    {
        /// <summary>
        /// Identificativo univoco del punto
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Coordinata X in cm
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Coordinata Y in cm
        /// </summary>
        public double Y { get; set; }
    }
}

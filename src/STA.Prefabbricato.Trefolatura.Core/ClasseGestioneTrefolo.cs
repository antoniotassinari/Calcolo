using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Collections.ObjectModel;


namespace STA.Prefabbricato.Trefolatura
{
    /// <summary>
    /// Classe per la gestione del trefolo
    /// </summary>
    [Serializable]
    public class ClasseGestioneTrefolo
    {
        #region prop

        #region prop interne

        private int _ID { get; set; }
        private double _Area { get; set; }
        private double _X { get; set; }
        private double _Y { get; set; }

        #endregion

        #region prop esterne

        /// <summary>
        /// Identificativo del trefolo
        /// </summary>
        public int ID
        {
            get { return _ID; }
            set
            {
                _ID = value;
            }
        }

        /// <summary>
        /// Area in cmq
        /// </summary>
        public double Area
        {
            get { return _Area; }
            set
            {
                _Area = value;
            }
        }

        /// <summary>
        /// Coordinata X rispetto al riferimento della trefolatura
        /// </summary>
        public double X
        {
            get { return _X; }
            set
            {
                _X = value;
            }
        }

        /// <summary>
        /// Coordinata Y rispetto al riferimento della trefolatura
        /// </summary>
        public double Y
        {
            get { return _Y; }
            set
            {
                _Y = value;
            }
        }

        #endregion

        #endregion

        #region Costruttore

        

        #endregion
    }


}

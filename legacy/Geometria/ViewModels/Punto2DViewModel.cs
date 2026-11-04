using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using STA.Common;

namespace STA.Geometria.ViewModels
{
    public class Punto2DViewModel : ViewModelBase
    {
        private Punto2D _Punto {get; set;}

        /// <summary>
        /// Punto Associato
        /// </summary>
        public Punto2D Punto
        {
            get { return _Punto; }
            set 
            {
                _Punto = value;
                RaisePropertyChangedEvent("Punto");
            }
        }

        /// <summary>
        /// Coordinata X
        /// </summary>
        public double X
        {
            get { return Punto.X; }
            set
            {
                Punto.X = value;
                RaisePropertyChangedEvent("X");
            }

        }

        public double Y
        {
            get { return Punto.Y; }
            set
            {
                Punto.Y = value;
                RaisePropertyChangedEvent("Y");
            }
        }

        public string ID
        {
            get { return Punto.ID; }
            set
            {
                Punto.ID = value;
                RaisePropertyChangedEvent("ID");
            }
        }

        #region Costruttore

        /// <summary>
        /// Costruttore con parametro
        /// </summary>
        /// <param name="puntoIn">parametro Punto2D in ingresso</param>
        public Punto2DViewModel(Punto2D puntoIn)
        {
            _Punto = puntoIn;
            RaisePropertyChangedEvent("Punto");
        }

        /// <summary>
        /// Costruttore senza parametro, istanzia un nuovo punto...
        /// </summary>
        public Punto2DViewModel()
            : this(new Punto2D())
        { }

        #endregion
    }
}

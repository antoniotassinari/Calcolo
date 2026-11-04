using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using STA.Common;

namespace STA.Prefabbricato.Trefolatura.ViewModels
{
    public class ClasseGestioneTrefoloViewModel : ViewModelBase
    {
        private ClasseGestioneTrefolo _Trefolo;
        private bool _IsLocked;

        public bool IsLocked
        {
            get {return _IsLocked;}
            set
            {
                _IsLocked = value;
                RaisePropertyChangedEvent("IsLocked");
            }
            
        }

        public ClasseGestioneTrefolo Trefolo
        {
            get { return _Trefolo; }
            set
            {
                _Trefolo = value;
                RaisePropertyChangedEvent("Trefolo");
            }
        }

        /// <summary>
        /// Identificativo
        /// </summary>
        public int ID
        {
            get { return Trefolo.ID; }
        }

        /// <summary>
        /// Area in cmq
        /// </summary>
        public double Area
        {
            get { return Trefolo.Area; }
            set
            {
                Trefolo.Area = value;
                RaisePropertyChangedEvent("Area");
            }
        }

        /// <summary>
        /// Ascissa in cm
        /// </summary>
        public double X
        {
            get { return Trefolo.X; }
            set
            {
                Trefolo.X = value;
                RaisePropertyChangedEvent("X");
            }

        }

        public double Y
        {
            get { return Trefolo.Y; }
            set
            {
                Trefolo.Y = value;
                RaisePropertyChangedEvent("Y");
            }
        }

        public ClasseGestioneTrefoloViewModel(ClasseGestioneTrefolo trefoloIn)
        {
            Trefolo = trefoloIn;
        }
    }
}

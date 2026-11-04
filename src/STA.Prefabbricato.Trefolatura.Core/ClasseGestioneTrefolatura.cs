using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace STA.Prefabbricato.Trefolatura
{
    public class ClasseGestioneTrefolatura
    {
        #region prop

        #region prop interne

        private int _ID { get; set; }
        private List<ClasseGestioneTrefolo> _Trefoli { get; set; }
        private double _TiroIniziale { get; set; }

        #endregion

        #region prop esterne

        /// <summary>
        /// Identificativo della trefolatura
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
        /// Elenco dei trefoli
        /// </summary>
        public List<ClasseGestioneTrefolo> Trefoli
        {
            get { return _Trefoli; }
            set { _Trefoli = value; }
        }

        /// <summary>
        /// Numero dei trefoli presenti
        /// </summary>
        public int NumeroTrefoliPresenti
        {
            get { return Trefoli.Count; }
        }

        /// <summary>
        /// Tiro iniziale sui trefoli [daN/cmq]
        /// </summary>
        public double TiroIniziale
        {
            get { return _TiroIniziale; }
            set
            {
                _TiroIniziale = value;
            }
        }

        /// <summary>
        /// Area totale della trefolatura
        /// </summary>
        public double AreaTotale
        {
            get
            {
                double area = 0;
                foreach (ClasseGestioneTrefolo tref in Trefoli)
                {
                    area += tref.Area;
                }
                return area;
            }
        }

        /// <summary>
        /// Tiro iniziale [kN]
        /// </summary>
        public double Nfase0
        {
            get
            {
                double N = 0;
                foreach (ClasseGestioneTrefolo trefolo in Trefoli)
                {
                    N += trefolo.Area * TiroIniziale / 100;
                }
                return N;
            }
        }

        /// <summary>
        /// Momento con versore in direzione X in fase 0
        /// </summary>
        public double MXfase0
        {
            get
            {
                return Nfase0 * Baricentro.Y / 100;
            }
        }

        /// <summary>
        /// Momento con versore in direzione Y in fase 0
        /// </summary>
        public double MYfase0
        {
            get
            {
                return Nfase0 * Baricentro.X / 100;
            }
        }

        /// <summary>
        /// Punto che individua il baricentro
        /// </summary>
        public STA.Geometria.Punto2D Baricentro
        {
            get
            {
                double xg = 0;
                double yg = 0;

                foreach (ClasseGestioneTrefolo tref in Trefoli)
                {
                    xg += (tref.Area * tref.X);
                    yg += (tref.Area * tref.Y);
                }

                return new Geometria.Punto2D { X = xg / AreaTotale, Y = yg / AreaTotale };

            }
        }

        /// <summary>
        /// Controlla se la trefolatura è aperta per modifiche
        /// </summary>
        public bool HasEditOpen
        {
            get;
            set;

        }

        #endregion


        #endregion



        #region costruttori

        /// <summary>
        /// Costruttore principale
        /// </summary>
        public ClasseGestioneTrefolatura()
        {
            ID = 1;
            TiroIniziale = 14000;
            _Trefoli = new List<ClasseGestioneTrefolo>();
        }

        #endregion
    }

    public class ClasseGestioneElencoTrefolature : ObservableCollection<ClasseGestioneTrefolatura>
    {
        /// <summary>
        /// Metodo per inserire la nuova trefolatura
        /// </summary>
        /// <param name="trefolaturaIn"></param>
        public void AggiungiTrefolatura(ClasseGestioneTrefolatura trefolaturaIn)
        {
            if (trefolaturaIn != null)
            {
                trefolaturaIn.ID = this.Count + 1;
                this.Add(trefolaturaIn);
            }
            else { return; }
        }

        /// <summary>
        /// Metodo per rinumerare le trefolature in ordine crescente
        /// </summary>
        public void RinumeraTrefolature()
        {
            int i = 1;
            foreach (ClasseGestioneTrefolatura trefProv in this)
            {
                trefProv.ID = i;
                i++;
            }
        }

        /// <summary>
        /// Eliminare la trefolatura in base all'istanza
        /// </summary>
        /// <param name="trefolaturaIn">Trefolatura in ingresso</param>
        public void EliminaTrefolatura(ClasseGestioneTrefolatura trefolaturaIn)
        {
            if (trefolaturaIn == null)
            {
                return;
            }

            this.Remove(trefolaturaIn);

        }

        /// <summary>
        /// Elimina la trefolatura in base all'Identificativo
        /// </summary>
        /// <param name="IDin"></param>
        public void EliminaTrefolatura(int IDin)
        {
            if (IDin <= 0)
            {
                return;
            }
            foreach (ClasseGestioneTrefolatura trefIn in this)
            {
                if (trefIn.ID == IDin)
                {
                    EliminaTrefolatura(trefIn);
                }
            }
        }


        /// <summary>
        /// Restituisce la trefolatura in base all'ID
        /// </summary>
        /// <param name="IDin">parametro int in ingresso</param>
        /// <returns></returns>
        public ClasseGestioneTrefolatura ScegliTrefolatura(int IDin)
        {
            foreach (ClasseGestioneTrefolatura trefolatura in this)
            {
                if (trefolatura.ID == IDin)
                {
                    return trefolatura;
                }
                
            }
            return null;
        }
    }
}

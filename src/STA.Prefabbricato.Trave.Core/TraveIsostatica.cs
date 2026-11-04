using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

using STA.Prefabbricato.Trefolatura;

using STA.Geometria.Masse;

namespace STA.Prefabbricato.TraveIsostatica
{
    /// <summary>
    /// Classe per il calcolo di una trave prefabbricata isostatica
    /// </summary>
    public class TraveIsostatica
    {
        #region Membri

        /// <summary>
        /// interna: nome della trave
        /// </summary>
        private string _NomeTrave { get; set; }


        /// <summary>
        /// interna: elenco delle maschere trefoli
        /// </summary>
        private ClasseGestioneElencoTrefolature _MaschereTrefoli { get; set; }

        /// <summary>
        /// interna: elenco delle sezioni disponibili
        /// </summary>
        private List<Sezione> _ElencoSezioni { get; set; }

        #endregion


        #region membri pubblici

        /// <summary>
        /// Esterna: elenco delle maschere trefoli
        /// </summary>
        public ClasseGestioneElencoTrefolature MaschereTrefoli
        {
            get { return _MaschereTrefoli; }
            set { _MaschereTrefoli = value; }
        }

        /// <summary>
        /// Esterna: elenco delle sezioni disponibili
        /// </summary>
        public List<Sezione> ElencoSezioni
        {
            get { return _ElencoSezioni; }
            set { _ElencoSezioni = value; }
        }

        /// <summary>
        /// esterna: Nome della trave
        /// </summary>
        public string NomeTrave
        {
            get { return _NomeTrave; }
            set { _NomeTrave = value; }
        }

        #endregion

        #region Costruttore

        public TraveIsostatica()
        {
            MaschereTrefoli = new ClasseGestioneElencoTrefolature();
        }


        #endregion
    }
}

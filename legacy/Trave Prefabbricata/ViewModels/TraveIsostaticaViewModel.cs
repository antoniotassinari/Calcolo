using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using STA.Common;

using STA.Prefabbricato.Trefolatura.ViewModels;


namespace STA.Prefabbricato.TraveIsostatica.ViewModels
{
    public class TraveIsostaticaViewModel : ViewModelBase
    {
        #region private

        private TraveIsostatica _traveIsostatica { get; set; }
        private ClasseGestioneElencoTrefolatureViewModel _TrefolatureVM { get; set; }

        #endregion

        #region proprietà

        public TraveIsostatica traveIsostatica
        {
            get { return _traveIsostatica; }
            set
            {
                _traveIsostatica = value;
                RaisePropertyChangedEvent("traveIsostatica");
            }
        }



        public string NomeTrave
        {
            get { return traveIsostatica.NomeTrave; }
            set
            {
                traveIsostatica.NomeTrave = value;
                RaisePropertyChangedEvent("NomeTrave");
            }
        }

        public ClasseGestioneElencoTrefolatureViewModel TrefolatureVM
        {
            get { return _TrefolatureVM; }
        }

        public ICommand AggiungiTrefolatura
        {
            get { return TrefolatureVM.AggiungiTrefolaturaComando; }
        }

        #endregion

        #region Costruttore

        /// <summary>
        /// Costruttore con parametro
        /// </summary>
        /// <param name="traveIn"></param>
        public TraveIsostaticaViewModel(TraveIsostatica traveIn)
        {
            _traveIsostatica = traveIn;
            _TrefolatureVM = new ClasseGestioneElencoTrefolatureViewModel(this.traveIsostatica.MaschereTrefoli);
        }

        /// <summary>
        /// Costruttore senza parametro
        /// </summary>
        public TraveIsostaticaViewModel()
            : this(new TraveIsostatica())
        {
        }

        #endregion

    }
}

using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using STA.Prefabbricato.Trefolatura.Commands;


using STA.Common;

namespace STA.Prefabbricato.Trefolatura.ViewModels
{
    public class ClasseGestioneElencoTrefolatureViewModel : ViewModelBase
    {
        #region Membri

        private ClasseGestioneElencoTrefolature _Trefolature;

        #endregion

        #region prop

        public ObservableCollection<ClasseGestioneTrefolaturaViewModel> Trefolature;
        public ICommand AggiungiTrefolaturaComando;



        #endregion

        #region Costruttori

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="trefIn"></param>
        public ClasseGestioneElencoTrefolatureViewModel(ClasseGestioneElencoTrefolature trefIn)
        {
            _Trefolature = trefIn;
            this.AggiungiTrefolaturaComando = new AggiungiTrefolaturaCommand(this);
            if (Trefolature == null)
            {
                Trefolature = new ObservableCollection<ClasseGestioneTrefolaturaViewModel>(); ;
            }

            AggiornaVisualizzazione();
            
        }

        #endregion

        #region Metodi per i comandi

        public void AggiungiTrefolatura()
        {
            _Trefolature.AggiungiTrefolatura(new ClasseGestioneTrefolatura());
            AggiornaVisualizzazione();
        }

        public ClasseGestioneTrefolaturaViewModel ScegliTrefolatura(int ID)
        {
            return new ClasseGestioneTrefolaturaViewModel(_Trefolature.ScegliTrefolatura(ID));
        }

        public void EliminaTrefolatura(ClasseGestioneTrefolatura tref)
        {
            _Trefolature.EliminaTrefolatura(tref);
        }

        public void AggiornaVisualizzazione()
        {
            Trefolature.Clear();
            if (_Trefolature != null)
            {
                foreach (ClasseGestioneTrefolatura trefolatura in _Trefolature)
                {
                    Trefolature.Add(new ClasseGestioneTrefolaturaViewModel(trefolatura));
                }
            }
            
        }

        #endregion
    }
}

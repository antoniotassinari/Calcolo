using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using STA.Common;
using System.Collections.ObjectModel;

using STA.Prefabbricato.Trefolatura.Commands;

namespace STA.Prefabbricato.Trefolatura.ViewModels
{
    /// <summary>
    /// Classe di visualizzazione e binding della classe gestione trefolatura
    /// </summary>
    public class ClasseGestioneTrefolaturaViewModel : ViewModelBase
    {
        private ClasseGestioneTrefolatura _Trefolatura;

        #region Costruttore
        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="tref"></param>
        public ClasseGestioneTrefolaturaViewModel(ClasseGestioneTrefolatura tref)
        {
            _Trefolatura = tref;
            AggiungiTrefoloComando = new AggiungiTrefoloCommand(this);
            EliminaTrefoloComando = new EliminaTrefoloCommand(this);
            TrefoliVM = new ObservableCollection<ClasseGestioneTrefoloViewModel>();
            AggiornaProprietà();
        }

        /// <summary>
        /// Costruttore di base
        /// </summary>
        public ClasseGestioneTrefolaturaViewModel()
            : this(new ClasseGestioneTrefolatura())
        {
        }
        #endregion

        #region Membri

        /// <summary>
        /// Comando per aggiungere il trefolo
        /// </summary>
        public ICommand AggiungiTrefoloComando { get; set; }

        /// <summary>
        /// Comando per eliminare il trefolo
        /// </summary>
        public ICommand EliminaTrefoloComando { get; set; }

        public ClasseGestioneTrefolatura Trefolatura
        {
            get {return _Trefolatura;}
            set
            {
                _Trefolatura = value;
                AggiornaProprietà();
            }
        }

        /// <summary>
        /// E' la proprietà che indica se è già aperto un controllo per la modifica
        /// </summary>
        public bool HasEditControlOpen
        {
            get { return Trefolatura.HasEditOpen; }
            set
            {
                Trefolatura.HasEditOpen = value;
                RaisePropertyChangedEvent("HasEditControlOpen");
            }
        }

        /// <summary>
        /// Identificativo
        /// </summary>
        public int ID
        {
            get { return Trefolatura.ID; }
        }

        public ObservableCollection<ClasseGestioneTrefoloViewModel> TrefoliVM
        {
            get;
            set;
        }
    

        /// <summary>
        /// Numero dei trefoli presenti
        /// </summary>
        public int NumeroTrefoliPresenti
        {
            get { return Trefolatura.Trefoli.Count; }
        }

        public double TiroIniziale
        {
            get { return Trefolatura.TiroIniziale; }
            set
            {
                Trefolatura.TiroIniziale = value;
                AggiornaProprietà();
            }
        }

        /// <summary>
        /// Area Totale in cmq
        /// </summary>
        public double AreaTotale
        {
            get { return Trefolatura.AreaTotale; }
            
        }

        public double MXfase0
        {
            get
            {
                return Trefolatura.MXfase0;
            }
        }

        public double MYfase0
        {
            get
            {
                return Trefolatura.MYfase0;
            }
        }

        public double Nfase0
        {
            get
            {
                return Trefolatura.Nfase0;
            }

        }

        public Geometria.Punto2D Baricentro
        {
            get
            {
                return Trefolatura.Baricentro;
            }
        }



        //public 

        #endregion

        #region Metodi

        private void AggiornaProprietà()
        {
            if (Trefolatura.Trefoli != null)
            {
                TrefoliVM.Clear();
                foreach (ClasseGestioneTrefolo trefoloIn in Trefolatura.Trefoli)
                {
                    ClasseGestioneTrefoloViewModel nuovoTrefoloVM = new ClasseGestioneTrefoloViewModel(trefoloIn);

                    TrefoliVM.Add(nuovoTrefoloVM);

                    nuovoTrefoloVM.PropertyChanged += OnModificaPropTrefoli;
                }
            }
            RaisePropertyChangedEvent("TiroIniziale");
            RaisePropertyChangedEvent("NumeroTrefoliPresenti");
            RaisePropertyChangedEvent("TrefoliVM");
            OnModificaPropTrefoli(this, null);
        }

        private void OnModificaPropTrefoli(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChangedEvent("AreaTotale");
            RaisePropertyChangedEvent("Nfase0");
            RaisePropertyChangedEvent("MXfase0");
            RaisePropertyChangedEvent("MYfase0");
            RaisePropertyChangedEvent("Baricentro");
        }

        public void AggiungiTrefolo()
        {
            Trefolatura.Trefoli.Add(new ClasseGestioneTrefolo());
            AggiornaProprietà();
        }

        public void EliminaTrefolo(ClasseGestioneTrefoloViewModel trefoloVMIn)
        {
            Trefolatura.Trefoli.Remove(trefoloVMIn.Trefolo);
            AggiornaProprietà();
        }

        #endregion
    }
}

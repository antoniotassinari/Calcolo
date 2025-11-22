using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.ObjectModel;

using STA.Common;
using STA.Geometria.Masse.Commands;
using STA.Geometria.ViewModels;

namespace STA.Geometria.Masse.ViewModels
{
    /// <summary>
    /// View model per la modifica di una sezione
    /// </summary>
    public class SezioneEditViewModel : ViewModelBase
    {
        private Sezione _sezione;

        #region Properties

        /// <summary>
        /// Classe modello associato al viewmodel
        /// </summary>
        public Sezione Sezione
        {
            get { return _sezione; }
            set 
            { 
                _sezione = value;
                RaisePropertyChangedEvent("Sezione");
            }
        }

        /// <summary>
        /// Nome della sezione
        /// </summary>
        public string NomeSezione
        {
            get { return _sezione != null ? _sezione.Nome : string.Empty; }
            set
            {
                if (_sezione != null)
                {
                    _sezione.Nome = value;
                    RaisePropertyChangedEvent("NomeSezione");
                }
            }
        }

        /// <summary>
        /// Comando per aggiungere un vertice
        /// </summary>
        public ICommand AggiungiVerticeComando { get; set; }

        /// <summary>
        /// Collezione di vertici view model
        /// </summary>
        public ObservableCollection<Punto2DViewModel> Vertici { get; set; }

        /// <summary>
        /// Area della sezione
        /// </summary>
        public double Area
        {
            get { return _sezione != null ? _sezione.Area : 0.0; }
        }

        /// <summary>
        /// View model del baricentro
        /// </summary>
        public Punto2DViewModel BaricentroVM
        {
            get 
            { 
                return _sezione != null && _sezione.Baricentro != null 
                    ? new Punto2DViewModel(_sezione.Baricentro) 
                    : null; 
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Costruttore con parametro
        /// </summary>
        /// <param name="sezIn">Sezione in ingresso</param>
        public SezioneEditViewModel(Sezione sezIn)
        {
            if (sezIn == null)
                throw new ArgumentNullException(nameof(sezIn));
                
            _sezione = sezIn;
            AggiungiVerticeComando = new AggiungiVerticeCommand(this);
        }

        /// <summary>
        /// Costruttore senza parametro
        /// </summary>
        public SezioneEditViewModel() : this(new Sezione())
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Aggiunge un nuovo vertice alla sezione
        /// </summary>
        public void AggiungiVertice()
        {
            if (_sezione == null) 
                return;

            var puntoNuovo = new Punto2D { ID = (_sezione.Vertici.Count + 1).ToString() };
            _sezione.Vertici.Add(puntoNuovo);
            AggiornaProprietà();
        }

        /// <summary>
        /// Aggiorna tutte le proprietà del viewmodel
        /// </summary>
        public void AggiornaProprietà()
        {
            if (_sezione == null) 
                return;

            _sezione.RicalcolaProprietà();
            
            if (Vertici == null)
                Vertici = new ObservableCollection<Punto2DViewModel>();
                
            Vertici.Clear();
            
            foreach (var puntoIn in _sezione.Vertici)
            {
                var puntoVM = new Punto2DViewModel(puntoIn);
                puntoVM.PropertyChanged += OnEventoModifica;
                Vertici.Add(puntoVM);
            }

            RaisePropertyChangedEvent("Vertici");
            RaisePropertyChangedEvent("Area");
            RaisePropertyChangedEvent("BaricentroVM");
        }

        /// <summary>
        /// Gestisce l'evento di modifica delle proprietà
        /// </summary>
        private void OnEventoModifica(object sender, PropertyChangedEventArgs e)
        {
            AggiornaProprietà();
        }

        #endregion
    }
}

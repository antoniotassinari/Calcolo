using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Prism;

using STA.Interfaccia.Common.BaseClasses;
using STA.Interfaccia.Common.Events;

using STA.Prefabbricato.Interfaccia.Moduli.GestioneTrefolatura.Commands;
using STA.Prefabbricato.Interfaccia.Moduli.GestioneTraveIsostatica;

using STA.Prefabbricato.Interfaccia;
using STA.Prefabbricato.Trefolatura;
using STA.Prefabbricato.Trefolatura.ViewModels;
using STA.Prefabbricato.Trefolatura.Commands;



namespace STA.Prefabbricato.Interfaccia.Moduli.GestioneTrefolatura.ViewModels
{
    public class ModuloGestioneTrefolaturaSelectionViewModel : ViewModelBase, INavigationAware
    {
        
        
        #region Membri

        private ClasseGestioneElencoTrefolatureViewModel _ViewModel { get; set; }
        private ClasseGestioneTrefolaturaViewModel _TrefolaturaScelta { get; set; }

        #endregion


        #region prop


        public ICommand AggiungiTrefolaturaComando
        {
            get { return _ViewModel.AggiungiTrefolaturaComando; }
        }

        public ICommand MostraModificaTrefolaturaComando
        {
            get;
            set;
        }

        public ObservableCollection<ClasseGestioneTrefolaturaViewModel> Trefolature
        {
            get { return _ViewModel.Trefolature; }
            set
            {
                _ViewModel.Trefolature = value;
                
            }
        }

        public ClasseGestioneTrefolaturaViewModel TrefolaturaScelta
        {
            get { return _TrefolaturaScelta; }
            set
            {
                _TrefolaturaScelta = value;
                RaisePropertyChangedEvent("TrefolaturaScelta");
            }
        }

        #endregion

        #region INavigationAware Membri di

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        #endregion

        public ModuloGestioneTrefolaturaSelectionViewModel(ClasseGestioneElencoTrefolatureViewModel trefolatureViewModel)
        {
            _ViewModel = trefolatureViewModel;
            MostraModificaTrefolaturaComando = new ShowModuloGestioneTrefolaturaCommand(this);

        }






    }
}

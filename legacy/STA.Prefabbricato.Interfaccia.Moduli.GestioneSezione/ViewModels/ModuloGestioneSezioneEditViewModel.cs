using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;

using STA.Common;

using STA.Prefabbricato.Interfaccia.Moduli.GestioneSezione.Commands;

namespace STA.Prefabbricato.Interfaccia.Moduli.GestioneSezione.ViewModels
{
    public class ModuloGestioneSezioneEditViewModel : ViewModelBase, INavigationAware
    {
        

        public ModuloGestioneSezioneEditViewModel()
        {
            
        }

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
    }
}

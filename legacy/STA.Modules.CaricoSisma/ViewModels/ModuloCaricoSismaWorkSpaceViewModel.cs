using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using STA.Interfaccia.Common.BaseClasses;
using STA.Carichi.Sisma;

namespace STA.Modules.CaricoSisma.ViewModels
{
    class ModuloCaricoSismaWorkSpaceViewModel : ViewModelBase, INavigationAware
    {
        public Sisma sismaAssociato = ModuloCaricoSisma.SismaAssociato;



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

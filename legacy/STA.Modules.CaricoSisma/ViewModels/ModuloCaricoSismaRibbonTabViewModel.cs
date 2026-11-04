using System.Windows.Input;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using STA.Interfaccia.Common.BaseClasses;
using STA.Interfaccia.Common.Events;
using STA.Modules.CaricoSisma.Commands;
using STA.Carichi.Sisma;

namespace STA.Modules.CaricoSisma.ViewModels
{
    public class ModuloCaricoSismaRibbonTabViewModel : ViewModelBase, INavigationAware
    {
        private Sisma _sismaViewModel;

        

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            throw new System.NotImplementedException();
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            throw new System.NotImplementedException();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            throw new System.NotImplementedException();
        }
    }
}

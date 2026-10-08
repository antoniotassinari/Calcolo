using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Prism.Ioc;
using Vento;

namespace STA.Modules.CaricoVento
{
    public class ModuloCaricoVento : IModule
    {
        public static Vento.Vento VentoAssociato = new Vento.Vento();

        // Register types with the container
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register view for DI by name so legacy code can resolve by key
            containerRegistry.Register<object, STA.Modules.CaricoVento.Views.ModuloCaricoVentoWorkspace>("ModuloCaricoVentoWorkspace");
        }

        // Called after the container is configured
        public void OnInitialized(IContainerProvider containerProvider)
        {
            // Register view with region
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("TaskButtonRegion", typeof(Views.ModuloCaricoVentoPulsante));
        }
    }
}

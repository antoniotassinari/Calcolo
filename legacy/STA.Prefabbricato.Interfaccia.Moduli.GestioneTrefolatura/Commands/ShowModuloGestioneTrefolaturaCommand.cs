using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Prism.Events;
using Prism.Regions;
using Prism.Ioc;

using STA.Interfaccia.Common.Events;

using STA.Prefabbricato.Interfaccia.Moduli.GestioneTraveIsostatica;

using STA.Prefabbricato.Interfaccia.Moduli.GestioneTrefolatura.ViewModels;
using STA.Prefabbricato.Interfaccia.Moduli.GestioneTrefolatura.Views;

namespace STA.Prefabbricato.Interfaccia.Moduli.GestioneTrefolatura.Commands
{
    
    public class ShowModuloGestioneTrefolaturaCommand : ICommand
    {
        private ModuloGestioneTrefolaturaSelectionViewModel _viewModel;

        public ShowModuloGestioneTrefolaturaCommand(ModuloGestioneTrefolaturaSelectionViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            if (_viewModel.TrefolaturaScelta != null && !_viewModel.TrefolaturaScelta.HasEditControlOpen)
            {
                //MessageBox.Show("Scelta la trefolatura: " + ModuloGestioneTraveIsostatica.traveIsostaticaViewModel.TrefolatureVM.Trefolature.Count.ToString());

                // Resolve region manager from the container via ServiceLocator replacement
                var regionManager = Prism.Ioc.ContainerLocator.Container.Resolve<IRegionManager>();

                

                ModuloGestioneTrefolaturaWorkSpaceView TrefolaturaView = new ModuloGestioneTrefolaturaWorkSpaceView(_viewModel.TrefolaturaScelta);

                regionManager.AddToRegion("WorkspaceRegion", TrefolaturaView);
                _viewModel.TrefolaturaScelta.HasEditControlOpen = true;
            }

            //if (_viewModel.TrefolaturaSelezionata != null)
            //{
            //    var regionManager = ServiceLocator.Current.GetInstance<IRegionManager>();
            //    UriQuery query = new UriQuery();
            //    query.Add("ID", _viewModel.TrefolaturaSelezionata.ID.ToString());
            //    var moduloGestioneTrefolaturaWorkspace = new Uri("ModuloGestioneTrefolaturaWorkSpaceView"+query.ToString(), UriKind.Relative);


            //    //foreach (var view in regionManager.Regions["WorkspaceRegion"].Views)
            //    //{
                    
            //    //    if (view.GetType() == typeof(ModuloGestioneTrefolaturaWorkSpaceView))
            //    //    {
            //    //        regionManager.Regions["WorkspaceRegion"].Remove("ModuloGestioneTrefolaturaWorkSpaceView");
            //    //    }

            //    //}
            //    regionManager.AddToRegion("WorkspaceRegion", new ModuloGestioneTrefolaturaWorkSpaceView(new ModuloGestioneTrefolaturaWorkspaceViewModel(_viewModel.TrefolaturaSelezionata.ID)));
            //    //regionManager.RequestNavigate("WorkspaceRegion", moduloGestioneTrefolaturaWorkspace);
                
            //    //MessageBox.Show("Numero di viste presenti: " + regionManager.Regions["WorkspaceRegion"].Views.ToList().Count);

            //}
        }
                    /// <summary>
        /// Callback method invoked when navigation has completed.
        /// </summary>
        /// <param name="result">Provides information about the result of the navigation.</param>
        private void NavigationCompleted(NavigationResult result)
        {
            // Exit if navigation was not successful
            if (result.Result != true) return;

            // Publish ViewRequestedEvent
            var eventAggregator = Prism.Ioc.ContainerLocator.Container.Resolve<IEventAggregator>();
            var navigationCompletedEvent = eventAggregator.GetEvent<NavigationCompletedEvent>();
            navigationCompletedEvent.Publish("ModuloGestioneTrefolatura");

        }

    }
}

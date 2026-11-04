using System;
using System.Windows.Input;
using STA.Interfaccia.Common.BaseClasses;
using STA.Modules.CaricoSisma.Commands;

namespace STA.Modules.CaricoSisma.ViewModels
{
    /// <summary>
    /// ViewModel for Seismic Load Button
    /// </summary>
    public class ModuloCaricoSismaPulsanteViewModel : ViewModelBase
    {
        private bool? p_IsChecked;

        public ICommand ShowModuloCaricoSismaView { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ModuloCaricoSismaPulsanteViewModel()
        {
            this.Initialize();
        }

        /// <summary>
        /// Whether the button is checked (selected).
        /// </summary>
        public bool? IsChecked
        {
            get { return p_IsChecked; }
            set
            {
                base.RaisePropertyChangingEvent("IsChecked");
                p_IsChecked = value;
                base.RaisePropertyChangedEvent("IsChecked");
            }
        }

        /// <summary>
        /// Handles navigation completion
        /// </summary>
        private void OnNavigationCompleted(string publisher)
        {
            // Exit if this module published the event
            if (publisher == "ModuloCaricoSisma") 
                return;

            // Otherwise, uncheck this button
            this.IsChecked = false;
        }

        /// <summary>
        /// Initializes the view model
        /// </summary>
        private void Initialize()
        {
            this.ShowModuloCaricoSismaView = new ShowModuloCaricoSismaViewCommand(this);
            this.IsChecked = false;
        }
    }
}

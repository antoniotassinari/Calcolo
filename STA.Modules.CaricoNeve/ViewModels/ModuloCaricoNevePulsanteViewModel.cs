using System;
using System.Windows.Input;
using STA.Interfaccia.Common.BaseClasses;
using STA.Modules.CaricoNeve.Commands;

namespace STA.Modules.CaricoNeve.ViewModels
{
    /// <summary>
    /// ViewModel for Snow Load Button
    /// </summary>
    public class ModuloCaricoNevePulsanteViewModel : ViewModelBase
    {
        private bool? p_IsChecked;

        public ICommand ShowModuloCaricoNeve { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ModuloCaricoNevePulsanteViewModel()
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
            if (publisher == "ModuloCaricoNeve") 
                return;

            // Otherwise, uncheck this button
            this.IsChecked = false;
        }

        /// <summary>
        /// Initializes the view model
        /// </summary>
        private void Initialize()
        {
            this.ShowModuloCaricoNeve = new ShowModuloCaricoNeveViewCommand(this);
            this.IsChecked = false;
        }
    }
}

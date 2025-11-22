using System;
using System.Windows.Input;
using STA.Modules.CaricoVento.ViewModels;

namespace STA.Modules.CaricoVento.Commands
{
    /// <summary>
    /// Command to show the Wind Load module view
    /// </summary>
    public class ShowModuloCaricoVentoViewCommand : ICommand
    {
        private ModuloCaricoVentoPulsanteViewModel m_ViewModel;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ShowModuloCaricoVentoViewCommand(ModuloCaricoVentoPulsanteViewModel viewModel)
        {
            m_ViewModel = viewModel;
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
            // Execute command logic
            // Prism navigation removed - implement your navigation logic here
        }
    }
}

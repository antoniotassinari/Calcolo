using System;
using System.Windows.Input;
using STA.Modules.CaricoNeve.ViewModels;

namespace STA.Modules.CaricoNeve.Commands
{
    /// <summary>
    /// Command to show the Snow Load module view
    /// </summary>
    public class ShowModuloCaricoNeveViewCommand : ICommand
    {
        private ModuloCaricoNevePulsanteViewModel m_ViewModel;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ShowModuloCaricoNeveViewCommand(ModuloCaricoNevePulsanteViewModel viewModel)
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

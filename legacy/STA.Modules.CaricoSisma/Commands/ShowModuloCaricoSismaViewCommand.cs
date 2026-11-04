using System;
using System.Windows.Input;
using STA.Modules.CaricoSisma.ViewModels;

namespace STA.Modules.CaricoSisma.Commands
{
    /// <summary>
    /// Command to show the Seismic Load module view
    /// </summary>
    public class ShowModuloCaricoSismaViewCommand : ICommand
    {
        private ModuloCaricoSismaPulsanteViewModel m_ViewModel;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ShowModuloCaricoSismaViewCommand(ModuloCaricoSismaPulsanteViewModel viewModel)
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

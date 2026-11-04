using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

using STA.Geometria.Masse.ViewModels;

namespace STA.Geometria.Masse.Commands
{
    public class AggiungiVerticeCommand : ICommand
    {
        SezioneEditViewModel viewModel;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (viewModel != null)
            {
                viewModel.AggiungiVertice();
            }
        }

        public AggiungiVerticeCommand(SezioneEditViewModel vmIn)
        {
            viewModel = vmIn;
        }
    }
}

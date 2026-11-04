using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using STA.Prefabbricato.Trefolatura.ViewModels;

namespace STA.Prefabbricato.Trefolatura.Commands
{
    public class AggiungiTrefolaturaCommand : ICommand
    {
        ClasseGestioneElencoTrefolatureViewModel viewModel;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            this.viewModel.AggiungiTrefolatura();
        }

        public AggiungiTrefolaturaCommand(ClasseGestioneElencoTrefolatureViewModel vm)
        {
            this.viewModel = vm;
        }
    }
}

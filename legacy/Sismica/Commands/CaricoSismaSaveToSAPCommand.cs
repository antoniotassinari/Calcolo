using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using STA.Carichi.Sisma.ViewModels;

namespace STA.Carichi.Sisma.Commands
{
    public class CaricoSismaSaveToSAPCommand : ICommand
    {
        #region membri

        private CaricoSismaRibbonTabViewModel _viewModel;



        #endregion

        #region costr

        public CaricoSismaSaveToSAPCommand(CaricoSismaRibbonTabViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        #endregion


        #region ICommand Membri di

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
            _viewModel.SalvaPerEtabs();
        }

        #endregion
    }
}

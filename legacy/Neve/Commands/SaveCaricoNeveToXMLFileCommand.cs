using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using STA.Carichi.Neve.ViewModels;

using System.Xml.Serialization;
using System.IO;

namespace STA.Carichi.Neve.Commands
{
    public class SaveCaricoNeveToXMLFileCommand : ICommand
    {
        private CaricoNeveWorkspaceViewModel _viewModel;

        public SaveCaricoNeveToXMLFileCommand(CaricoNeveWorkspaceViewModel viewModel)
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
            _viewModel.SerilizeToXML();
        }
    }
}

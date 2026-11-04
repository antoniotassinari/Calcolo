using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STA.Common.Commands
{
    /// <summary>
    /// Command for saving data to XML file
    /// </summary>
    public class SaveToXMLFileCommand : ICommand
    {
        public event System.EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            // Implementation for saving to XML
        }
    }
}

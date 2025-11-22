using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using STA.Modules.CaricoSisma.ViewModels;

namespace STA.Modules.CaricoSisma.Views
{
    /// <summary>
    /// Logica di interazione per ModuloCaricoSismaWorkSpace.xaml
    /// </summary>
    public partial class ModuloCaricoSismaWorkSpace : UserControl
    {
        

        public ModuloCaricoSismaWorkSpace()
        {
            
            InitializeComponent();
            ControlloSisma.SismaAssociato = ModuloCaricoSisma.SismaAssociato;
        }
    }
}

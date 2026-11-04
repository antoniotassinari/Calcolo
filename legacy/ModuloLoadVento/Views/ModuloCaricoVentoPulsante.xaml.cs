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
using STA.Modules.CaricoVento.ViewModels;
using Microsoft.Practices.Prism.Regions;

namespace STA.Modules.CaricoVento.Views
{
    /// <summary>
    /// Logica di interazione per ModuloCaricoVentoPulsante.xaml
    /// </summary
    public partial class ModuloCaricoVentoPulsante : UserControl
    {
        public ModuloCaricoVentoPulsante(ModuloCaricoVentoPulsanteViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}

using System.Windows.Controls;
using Microsoft.Practices.Prism.Regions;
using STA.Modules.CaricoSisma.ViewModels;

namespace STA.Modules.CaricoSisma.Views
{
    /// <summary>
    /// Logica di interazione per ModuloSismaPulsante.xaml
    /// </summary>
    [ViewSortHint("03")]
    public partial class ModuloSismaPulsante : UserControl
    {
        public ModuloSismaPulsante(ModuloCaricoSismaPulsanteViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}

using System.Windows.Controls;
using STA.Modules.CaricoNeve.ViewModels;

namespace STA.Modules.CaricoNeve.Views
{
    /// <summary>
    /// Logica di interazione per ModuloCaricoNevePulsante.xaml
    /// </summary>
    public partial class ModuloCaricoNevePulsante : UserControl
    {
        public ModuloCaricoNevePulsante(ModuloCaricoNevePulsanteViewModel viewmodel)
        {
            InitializeComponent();
            this.DataContext = viewmodel;
        }
    }
}

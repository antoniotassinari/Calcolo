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

using Microsoft.Practices.Prism.Regions;

using STA.Prefabbricato.Interfaccia.Moduli.GestioneTrefolatura.ViewModels;

using STA.Prefabbricato.Interfaccia.Moduli.GestioneTraveIsostatica;

using STA.Prefabbricato.Trefolatura.ViewModels;

namespace STA.Prefabbricato.Interfaccia.Moduli.GestioneTrefolatura.Views
{
    /// <summary>
    /// Logica di interazione per ModuloGestioneTrefolaturaSelectionView.xaml
    /// </summary>
    [ViewSortHint("02")]
    public partial class ModuloGestioneTrefolaturaSelectionView : UserControl
    {
        ModuloGestioneTrefolaturaSelectionViewModel viewModel = new ModuloGestioneTrefolaturaSelectionViewModel(ModuloGestioneTraveIsostatica.traveIsostaticaViewModel.TrefolatureVM);

        public ModuloGestioneTrefolaturaSelectionView()
        {
            InitializeComponent();

            this.DataContext = viewModel;

        }
    }
}

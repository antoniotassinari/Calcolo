using Microsoft.Practices.Prism.Regions;
using Microsoft.Windows.Controls.Ribbon;
using STA.Carichi.Sisma.ViewModels;

namespace STA.Modules.CaricoSisma.Views
{
    /// <summary>
    /// Logica di interazione per ModuloCaricoSismaRibbonTab.xaml
    /// </summary>
    public partial class ModuloCaricoSismaRibbonTab : RibbonTab, IRegionMemberLifetime
    {
        CaricoSismaRibbonTabViewModel _viewModel;

        /// <summary>
        /// Costr
        /// </summary>
        public ModuloCaricoSismaRibbonTab()
        {
            InitializeComponent();
            _viewModel = new CaricoSismaRibbonTabViewModel(ModuloCaricoSisma.SismaAssociato);
            this.DataContext = _viewModel;
        }

        public bool KeepAlive
        {
            get { return false; }
        }
    }
}

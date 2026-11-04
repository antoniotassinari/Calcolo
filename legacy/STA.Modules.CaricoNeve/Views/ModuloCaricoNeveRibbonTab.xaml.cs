using Microsoft.Practices.Prism.Regions;
using Microsoft.Windows.Controls.Ribbon;
using STA.Carichi.Neve.ViewModels;

namespace STA.Modules.CaricoNeve.Views
{
    /// <summary>
    /// Logica di interazione per ModuloCaricoNeveRibbonTab.xaml
    /// </summary>
    public partial class ModuloCaricoNeveRibbonTab : RibbonTab, IRegionMemberLifetime
    {
        CaricoNeveWorkspaceViewModel _viewModel;

        public ModuloCaricoNeveRibbonTab()
        {
            InitializeComponent();
            rbg.DataContext = ModuloCaricoNeve._modelView;

        }



        bool IRegionMemberLifetime.KeepAlive
        {
            get { return false; }
        }
    }
}

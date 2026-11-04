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

namespace STA.Modules.CaricoVento.Views
{
    /// <summary>
    /// Logica di interazione per LoadVentoView.xaml
    /// </summary>
    public partial class ModuloCaricoVentoWorkspace : UserControl, IRegionMemberLifetime
    {


        public ModuloCaricoVentoWorkspace()
        {
            InitializeComponent();
            ucVento1.VentoAssociato = ModuloCaricoVento.VentoAssociato;
            ucVento1.DataContext = ModuloCaricoVento.VentoAssociato;
        }

        #region IRegionMemberLifetime Members

        public bool KeepAlive
        {
            get { return true; }
        }

        #endregion
    }
}

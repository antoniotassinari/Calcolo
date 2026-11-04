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
using System.Collections.ObjectModel;

using Microsoft.Practices.Prism.Regions;
using STA.Prefabbricato.Trefolatura.ViewModels;

using STA.Prefabbricato.Interfaccia.Moduli.GestioneTrefolatura.ViewModels;

namespace STA.Prefabbricato.Interfaccia.Moduli.GestioneTrefolatura.Views
{
    /// <summary>
    /// Logica di interazione per ModuloGestioneTrefolaturaWorkSpaceView.xaml
    /// </summary>
    public partial class ModuloGestioneTrefolaturaWorkSpaceView : UserControl, IRegionMemberLifetime
    {


        public ModuloGestioneTrefolaturaWorkSpaceView(ClasseGestioneTrefolaturaViewModel viewModelIn)
        {

            InitializeComponent();
            this.DataContext = viewModelIn;

        }

        public bool KeepAlive
        {
            get { return true; }
        }
    }
}

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

using Microsoft.Windows.Controls.Ribbon;

namespace STA.Prefabbricato.Interfaccia.Moduli.GestioneTraveIsostatica.Views
{
    /// <summary>
    /// Logica di interazione per ModuloGestioneTraveIsostaticaMainRibbonTab.xaml
    /// </summary>
    public partial class ModuloGestioneTraveIsostaticaMainRibbonTab : RibbonTab
    {
        public ModuloGestioneTraveIsostaticaMainRibbonTab()
        {
            InitializeComponent();
            this.DataContext = ModuloGestioneTraveIsostatica.traveIsostaticaViewModel;
        }
    }
}

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

using STA.Prefabbricato.Interfaccia.Moduli.GestioneSezione.ViewModels;


namespace STA.Prefabbricato.Interfaccia.Moduli.GestioneSezione.Views
{
    /// <summary>
    /// Logica di interazione per ModuloGestioneSezioneSelectionView.xaml
    /// </summary>
    public partial class ModuloGestioneSezioneSelectionView : UserControl
    {

        public ModuloGestioneSezioneSelectionView(ModuloGestioneSezioneSelectionViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}

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

using STA.Geometria.Masse.ViewModels;

namespace STA.Geometria.Masse.Views
{
    /// <summary>
    /// Logica di interazione per SezioneEditView.xaml
    /// </summary>
    public partial class SezioneEditView : UserControl
    {
        SezioneEditViewModel _viewModel;

        public SezioneEditView()
        {
            _viewModel = new SezioneEditViewModel();
            InitializeComponent();
            this.DataContext = _viewModel;
        }
    }
}

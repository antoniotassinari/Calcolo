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
using STA.Carichi.Neve.ViewModels;

namespace STA.Carichi.Neve
{
    /// <summary>
    /// Logica di interazione per ucCaricoNeve.xaml
    /// </summary>
    public partial class ucCaricoNeve : UserControl
    {
        CaricoNeveWorkspaceViewModel _viewModel;

        /// <summary>
        /// Costruttore con ingresso
        /// </summary>
        /// <param name="viewModel">view model in ingresso</param>
        public ucCaricoNeve(CaricoNeveWorkspaceViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            this.DataContext = _viewModel;

            //comboBoxZone.ItemsSource = NeveAssociata.ListaZone;
        }

        /// <summary>
        /// Costruttore base
        /// </summary>
        public ucCaricoNeve()
            : this(new CaricoNeveWorkspaceViewModel())
        {
        }

    }
}

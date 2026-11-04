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


namespace STA.Modules.CaricoNeve.Views
{
    /// <summary>
    /// Logica di interazione per ModuloCaricoNeveWorkSpace.xaml
    /// </summary>
    public partial class ModuloCaricoNeveWorkSpace : UserControl
    {
        public ModuloCaricoNeveWorkSpace()
        {
            InitializeComponent();
            ControlloCaricoNeve.DataContext = ModuloCaricoNeve._modelView;

        }
    }
}

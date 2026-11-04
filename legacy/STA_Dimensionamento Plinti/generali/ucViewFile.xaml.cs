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

namespace STA_Dimensionamento_Plinti.generali
{
    /// <summary>
    /// Logica di interazione per ucViewFile.xaml
    /// </summary>
    public partial class ucViewFile : UserControl
    {
        public ucViewFile()
        {
            InitializeComponent();
        }

        public void viewTXT(string filePath)
        {
            Paragraph paragraph = new Paragraph();

            paragraph.FontFamily = new FontFamily("Courier New");

            paragraph.FontSize = 10;

            paragraph.Inlines.Add(System.IO.File.ReadAllText(filePath));

            FlowDocument document = new FlowDocument(paragraph);

            FlowDocReader.Document = document;

            laFilePath.Content = filePath;
        }
    }
}

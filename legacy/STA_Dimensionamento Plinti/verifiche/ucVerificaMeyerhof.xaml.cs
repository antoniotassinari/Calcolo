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

namespace STA_Dimensionamento_Plinti.verifiche
{
    /// <summary>
    /// Logica di interazione per ucVerificaMeyerhof.xaml
    /// </summary>
    public partial class ucVerificaMeyerhof : UserControl
    {

        #region proprietà
        /// <summary>
        /// 
        /// </summary>
        private verificaMeyerhof VerificaMeyerhof { get; set; }
        #endregion


        #region costruttori
        /// <summary>
        /// Costruttore per la riga di dati
        /// </summary>
        /// <param name="verifica"></param>
        public ucVerificaMeyerhof(verificaMeyerhof verifica)
        {
            InitializeComponent();
            VerificaMeyerhof = verifica;
            tbEx.Text = verifica.Ex.ToString("F2");
            tbEy.Text = verifica.Ey.ToString("F2");
            tbAreaPress.Text = verifica.AreaEff.ToString("F2");
            tbPress.Text = verifica.Pressione.ToString("F3");

            if (verifica.Pressione >= 0)
            {
                switch (verifica.Verificato())
                {
                    case false:
                        mainGrid.Background = new SolidColorBrush(Color.FromArgb(50, 255, 0, 0));
                        break;

                    case true:
                        mainGrid.Background = new SolidColorBrush(Color.FromArgb(50, 0, 255, 0));
                        break;
                }
            }
            else
            {
                mainGrid.Background = new SolidColorBrush(Color.FromArgb(50, 255, 0, 0));
            }
        }

        /// <summary>
        /// Costruttore generico, genera la riga del titolo
        /// </summary>
        /// <param name="plintoIn">Plinto di calcolo</param>
        public ucVerificaMeyerhof(string nomePlinto)
        {
            InitializeComponent();
            mainGrid.ShowGridLines = false;
            //Crea la riga che individua i titoli
            mainGrid.RowDefinitions.Add(new RowDefinition());
            mainGrid.RowDefinitions.Add(new RowDefinition());

            Label etichettaPlinto = new Label();

            etichettaPlinto.Content = nomePlinto;
            etichettaPlinto.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;

            mainGrid.Children.Add(etichettaPlinto);

            Grid.SetRow(etichettaPlinto, 0);
            Grid.SetColumnSpan(etichettaPlinto, mainGrid.ColumnDefinitions.Count);


            //Modifica le caratteristiche delle etichette per visualizzare i titoli
            tbEx.FontWeight = FontWeights.Bold;
            Grid.SetRow(tbEx, 1);
            
            tbEy.FontWeight = FontWeights.Bold;
            Grid.SetRow(tbEy, 1);

            tbAreaPress.FontWeight = FontWeights.Bold;
            Grid.SetRow(tbAreaPress, 1);

            tbPress.FontWeight = FontWeights.Bold;
            Grid.SetRow(tbPress, 1);

            tbEx.Text = "Ex [cm]";
            tbEy.Text = "Ey [cm]";
            tbAreaPress.Text = "Area Eff. [cmq]";
            tbPress.Text = "press [daN/cmq]";

        }
        #endregion
    }
}

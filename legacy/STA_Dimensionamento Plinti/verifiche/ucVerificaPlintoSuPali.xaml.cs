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
    /// Logica di interazione per ucVerificaPlintoSuPali.xaml
    /// </summary>
    public partial class ucVerificaPlintoSuPali : UserControl
    {
        public ucVerificaPlintoSuPali()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Metodo per la creazione dell'intestazione
        /// </summary>
        /// <param name="nomePlinto"></param>
        public ucVerificaPlintoSuPali(string nomePlinto) :
            this()
        {
            // crea la riga del titolo
            mainGrid.RowDefinitions.Add(new RowDefinition());
            mainGrid.RowDefinitions.Add(new RowDefinition());

            // scrive il nome del Plinto
            TextBlock tblkPlinto = new TextBlock();
            tblkPlinto.Text = nomePlinto;
            tblkPlinto.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            TextBlock tblkTitoloPortata = new TextBlock();
            tblkTitoloPortata.Text = "P. Lim. [kN]";
            tblkTitoloPortata.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            TextBlock tblkTitoloReazioneMax = new TextBlock();
            tblkTitoloReazioneMax.Text = "Rea. Max. [kN]";
            tblkTitoloReazioneMax.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            TextBlock tblkTitoloTaglio = new TextBlock();
            tblkTitoloTaglio.Text = "Taglio [kN]";
            tblkTitoloTaglio.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;

            mainGrid.Children.Add(tblkPlinto);
            mainGrid.Children.Add(tblkTitoloPortata);
            mainGrid.Children.Add(tblkTitoloReazioneMax);
            
            Grid.SetRow(tblkPlinto, 0);
            Grid.SetColumnSpan(tblkPlinto, mainGrid.ColumnDefinitions.Count);

            Grid.SetRow(tblkTitoloPortata, 1);
            Grid.SetColumn(tblkTitoloPortata, 0);

            Grid.SetRow(tblkTitoloReazioneMax, 1);
            Grid.SetColumn(tblkTitoloReazioneMax, 1);

            Grid.SetRow(tblkTitoloTaglio, 1);
            Grid.SetColumn(tblkTitoloTaglio, 2);
        }

        public ucVerificaPlintoSuPali(verificaPlintoSuPali verifica) :
            this()
        {
            TextBlock tblkPortataLimite = new TextBlock();
            tblkPortataLimite.Style = (Style)(this.Resources["TextBlockStyleCombinazioni"]);
            tblkPortataLimite.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            tblkPortataLimite.Text = verifica.ValoreLimite.ToString("F0");
            mainGrid.Children.Add(tblkPortataLimite);

            TextBlock tblkReazioneMassima = new TextBlock();
            tblkReazioneMassima.Style = (Style)(this.Resources["TextBlockStyleCombinazioni"]);
            switch (verifica.Verificato())
            {
                case false:
                    mainGrid.Background = new SolidColorBrush(Color.FromArgb(50, 255, 0, 0));
                    break;

                case true:
                    mainGrid.Background = new SolidColorBrush(Color.FromArgb(50, 0, 255, 0));
                    break;
            }

            tblkReazioneMassima.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            tblkReazioneMassima.Text = verifica.ReazioneMassima.ToString("F2");

            mainGrid.Children.Add(tblkReazioneMassima);
            Grid.SetColumn(tblkReazioneMassima, 1);

            TextBlock tblkTaglio = new TextBlock();
            tblkPortataLimite.Style = (Style)(this.Resources["TextBlockStyleCombinazioni"]);
            tblkTaglio.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            tblkTaglio.Text = verifica.TaglioSuPalo.ToString("F2");

            mainGrid.Children.Add(tblkTaglio);
            Grid.SetColumn(tblkTaglio, 2);

        }
    }
}

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
using System.ComponentModel;
using STA.Grafica;

namespace STA.Carichi.Sisma
{
    /// <summary>
    /// Logica di interazione per ucSisma.xaml
    /// </summary>
    public partial class ucSisma : UserControl
    {
        public Sisma SismaAssociato = new Sisma(12.19944,﻿44.41778);



        public ucSisma()
        {
            InitializeComponent();
            SismaAssociato.PropertyChanged += aggiornaBrowser;
            this.DataContext = SismaAssociato;
            comboBoxTipoCostruzione.ItemsSource = SismaAssociato.ElencoViteNominali;
            comboBoxClasseUso.ItemsSource = SismaAssociato.ElencoClassiUso;
            comboBoxCategoriaSuolo.ItemsSource = SismaAssociato.ElencoCategorieSottosuolo;
            aggiornaBrowser(this, null);
            
           
        }


        private void webBrowser1_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (webBrowser1.Source != null && webBrowser1.Source != SismaAssociato.IndirizzoURLVisualizzazione)
            {
                            tbIndirizzo.Text = webBrowser1.Source.ToString();
            }

        }

        private void aggiornaBrowser(object sender, PropertyChangedEventArgs e)
        {
            if (webBrowser1.Source != SismaAssociato.IndirizzoURLVisualizzazione)
            {
                webBrowser1.Source = SismaAssociato.IndirizzoURLVisualizzazione;
            }

        }

        private void btAggiornaPosizione_Click(object sender, RoutedEventArgs e)
        {
            SismaAssociato.leggiCoorDaUrl(webBrowser1.Source);
        }

        private void listViewSpettri_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listViewSpettri.SelectedItem != null)
            {
                listViewValoriSpettrali.ItemsSource = ((spettro)listViewSpettri.SelectedItem).valoriSpettrali;

                chartCanvas.Width = gridGrafico.ActualWidth;
                chartCanvas.Height = gridGrafico.ActualHeight;
                chartCanvas.Children.Clear();
                textCanvas.Children.RemoveRange(1, textCanvas.Children.Count - 1);
                AggiungiGrafico((spettro)listViewSpettri.SelectedItem);

            }

        }

        private void AggiungiGrafico(spettro spettroIn)
        {
            textCanvas.Width = gridGrafico.ActualWidth;
            textCanvas.Height = gridGrafico.ActualHeight;

            ChartStyleGridlines cs;
            DataCollection dc;
            SerieDati ds;

            cs = new ChartStyleGridlines();
            dc = new DataCollection();
            ds = new SerieDati();

            cs.ChartCanvas = chartCanvas;
            cs.TextCanvas = textCanvas;
            cs.Title = spettroIn.statoLimite.ToString();

            cs.Xmin = 0;
            cs.Xmax = 5;
            cs.Ymin = 0;
            cs.Ymax = 10;

            cs.YTick = 2;

            cs.GridlinePattern = ChartStyleGridlines.GridlinePatternEnum.Dot;
            cs.GridlineColor = Brushes.Black;
            cs.AddChartStyle(tbTitle, tbXLabel, tbYLabel);



            spettro spettroAttuale = (spettro)listViewSpettri.SelectedItem;

            if (spettroAttuale.statoLimite == Sisma.statiLimite.SLV && SismaAssociato.FattoreStruttura != 1)
            {
                ds = new SerieDati();
                ds.LineColor = Brushes.Red;
                ds.LinePattern = SerieDati.LinePatternEnum.DashDot;
                ds.LineThickness = 2;

                foreach (valoreSpettrale valoreIn in ((spettro)listViewSpettri.SelectedItem).valoriSpettrali)
                {
                    ds.LineSeries.Points.Add(new Point(valoreIn.periodo, valoreIn.risposta));
                }
                dc.DataList.Add(ds);



                ds = new SerieDati();
                ds.LineColor = Brushes.Blue;
                ds.LineThickness = 2;

                foreach (valoreSpettrale valoreIn in SismaAssociato.spettriProgetto[0].valoriSpettrali)
                {
                    ds.LineSeries.Points.Add(new Point(valoreIn.periodo, valoreIn.risposta));
                }
                dc.DataList.Add(ds);

            }
            else
            {
                ds = new SerieDati();
                ds.LineColor = Brushes.Blue;
                ds.LineThickness = 2;

                foreach (valoreSpettrale valoreIn in ((spettro)listViewSpettri.SelectedItem).valoriSpettrali)
                {
                    ds.LineSeries.Points.Add(new Point(valoreIn.periodo, valoreIn.risposta));
                }
                dc.DataList.Add(ds);
            }




            dc.AddLines(cs);
        }

        private void gridGrafico_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if ((spettro)listViewSpettri.SelectedItem != null)
            {
                chartCanvas.Width = gridGrafico.ActualWidth;
                chartCanvas.Height = gridGrafico.ActualHeight;
                chartCanvas.Children.Clear();
                textCanvas.Children.RemoveRange(1, textCanvas.Children.Count - 1);
                AggiungiGrafico((spettro)listViewSpettri.SelectedItem);
            }
        }


    }
}

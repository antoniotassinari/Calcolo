using System;
using System.Collections.Generic;

namespace STA.Grafica
{
    /// <summary>
    /// Collezione di serie di dati per il grafico
    /// </summary>
    public class DataCollection
    {
        /// <summary>
        /// Lista delle serie di dati
        /// </summary>
        public List<SerieDati> DataList { get; set; }

        /// <summary>
        /// Costruttore di default
        /// </summary>
        public DataCollection()
        {
            DataList = new List<SerieDati>();
        }

        /// <summary>
        /// Aggiunge le linee al grafico
        /// </summary>
        /// <param name="cs">Stile del grafico</param>
        public void AddLines(ChartStyle cs)
        {
            if (cs == null) 
                throw new ArgumentNullException(nameof(cs));

            int j = 0;
            foreach (var ds in DataList)
            {
                if (ds.SeriesName == "Default Name")
                {
                    ds.SeriesName = string.Format("DataSeries{0}", j);
                }

                ds.AddLinePattern();

                for (int i = 0; i < ds.LineSeries.Points.Count; i++)
                {
                    ds.LineSeries.Points[i] = cs.NormalizePoint(ds.LineSeries.Points[i]);
                    ds.Simboli.AddSymbol(cs.ChartCanvas, ds.LineSeries.Points[i]);
                }

                cs.ChartCanvas.Children.Add(ds.LineSeries);
                j++;
            }
        }
    }
}

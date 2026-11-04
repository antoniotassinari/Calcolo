using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STA_Dimensionamento_Plinti.verifiche
{
    public class verificaPlintoSuPali : IVerificaPlinto
    {
        private double sPortataLimite { get; set; }

        private double sReazioneMassima { get; set; }

        private double sTaglioSuPalo { get; set; }

        private Reazione sReazioneDiCalcolo { get; set; }

        private PlintoSuPali sPlinto { get; set; }

        /// <summary>
        /// Reazione Massima del palo
        /// </summary>
        public double ReazioneMassima { get { return sReazioneMassima; } }

        /// <summary>
        /// Reazione di Calcolo associata
        /// </summary>
        public Reazione ReazioneDiCalcolo { get { return sReazioneDiCalcolo; } }

        /// <summary>
        /// Azione tagliante su singolo palo
        /// </summary>
        public double TaglioSuPalo { get { return sTaglioSuPalo; } }

        /// <summary>
        /// Costruttore unico, vuole una reazione ed un Plinto su pali
        /// </summary>
        /// <param name="rea">Reazione di calcolo</param>
        /// <param name="pli">Plinto per la verifica</param>
        public verificaPlintoSuPali(Reazione rea, PlintoSuPali pli)
        {
            sPlinto = pli;
            sPortataLimite = pli.Pali[0].PortataLimite;
            sReazioneMassima = 0;
            foreach (pali.IPalo paloCheck in pli.Pali)
            {
                double reazionePalo = rea.FZ / pli.Pali.Count;

                reazionePalo += Math.Abs(rea.MX) / pli.Jx() * Math.Abs(paloCheck.Posizione.Y)/100 * paloCheck.Area()/10000;

                reazionePalo += Math.Abs(rea.MY) / pli.Jy() * Math.Abs(paloCheck.Posizione.X) / 100 * paloCheck.Area() / 10000;

                sTaglioSuPalo = Math.Sqrt(Math.Pow(rea.FX / pli.Pali.Count, 2) + Math.Pow(rea.FY / pli.Pali.Count, 2));

                sReazioneMassima = reazionePalo;

            }

            sReazioneDiCalcolo = rea;

        }


        #region IVerificaPlinto Membri di

        public double ValoreLimite
        {
            get { return sPortataLimite; }
        }

        /// <summary>
        /// Metodo che controlla se la verifica è stat superata
        /// </summary>
        /// <returns></returns>
        public bool Verificato()
        {
            return (ValoreLimite > ReazioneMassima);
        }

        #endregion

        #region IVerificaPlinto Membri di


        public IPlinto PlintoDiverifica
        {
            get { return sPlinto; }
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STA_Dimensionamento_Plinti.verifiche
{
    public class verificaMeyerhof : IVerificaPlinto
    {

        #region proprietà Interne

        private double sEx { get; set; }
        private double sEy { get; set; }
        private double sAreaEff { get; set; }
        private double sPressione { get; set; }
        private Plinto _plinto { get; set; }
        private Reazione sReazioneDiCalcolo { get; set; }
        private double sValoreLimite { get; set; }

        #endregion

        #region proprietà Esterne

        /// <summary>
        /// Eccentricità complessiva in direzione X [cm]
        /// </summary>
        public double Ex { get { return sEx; } }
        /// <summary>
        /// Eccentricità complessiva in direzione y [cm]
        /// </summary>
        public double Ey { get { return sEy; } }
        /// <summary>
        /// Area efficace [cmq]
        /// </summary>
        public double AreaEff { get { return sAreaEff; } }
        /// <summary>
        /// Pressione [daN/cmq]
        /// </summary>
        public double Pressione { get { return sPressione; } }
        /// <summary>
        /// plinto associato alla verifica. Serve per alcune visualizzazioni
        /// </summary>
        public IPlinto PlintoDiverifica { get { return _plinto; } }

        public double ReazioneMassima { get { return Pressione; } }

        /// <summary>
        /// Reazione di calcolo associata alla verifica
        /// </summary>
        public Reazione ReazioneDiCalcolo { get { return sReazioneDiCalcolo; } }

        #endregion

        #region Costruttore

        /// <summary>
        /// Costruttore principale
        /// </summary>
        /// <param name="rea">Istanza di classe Reazione per fornire i valori delle azioni</param>
        /// <param name="plinto">Istanza di classe Plinto per fornire le caratteristiche</param>
        public verificaMeyerhof(Reazione rea, Plinto pli)
        {
            sValoreLimite = pli.ValoreLimite;
            sEy = Math.Abs(rea.MX + rea.FZ * pli.EccentricitaPilastro.Y / 100)  * 100 * 100 / (pli.PesoTotale() + rea.FZ * 100);
            sEx = Math.Abs(rea.MY + rea.FZ * pli.EccentricitaPilastro.X / 100) * 100 * 100 / (pli.PesoTotale() + rea.FZ * 100);
            sAreaEff = (pli.A - 2 * Ex) * (pli.B - 2 * Ey);
            sPressione = (pli.PesoTotale() + rea.FZ * 100) / AreaEff;
            sReazioneDiCalcolo = rea;
            _plinto = pli;
        }

        #endregion


        #region  Membri di IVerificaPlinto

        public double ValoreLimite
        {
            get { return sValoreLimite; }
        }

        /// <summary>
        /// Controlla la soddisfazione della verifica
        /// </summary>
        /// <returns></returns>
        public bool Verificato()
        {
            if (Pressione < ValoreLimite)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}

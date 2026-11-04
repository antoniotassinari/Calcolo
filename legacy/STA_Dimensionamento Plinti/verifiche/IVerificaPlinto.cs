using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STA_Dimensionamento_Plinti.verifiche
{
    /// <summary>
    /// Interfaccia per gestire le caratteristiche comuni della classi di verifica che mano mano verranno implementate nel programma
    /// </summary>
    public interface IVerificaPlinto
    {

        double ValoreLimite { get;}

        double ReazioneMassima { get; }

        /// <summary>
        /// Metodo di interfaccia ceh deve resistuire vero se la verifica neri confronti del valore limite è superata
        /// </summary>
        /// <returns>Vero se verificato, falso se altrimenti</returns>
        bool Verificato();

        /// <summary>
        /// Memorizza la reazione di calcolo
        /// </summary>
        Reazione ReazioneDiCalcolo { get; }

        /// <summary>
        /// Memorizza plinto di verifica
        /// </summary>
        IPlinto PlintoDiverifica { get; }
    }
}

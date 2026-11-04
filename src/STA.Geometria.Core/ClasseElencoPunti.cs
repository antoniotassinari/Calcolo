using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.ComponentModel;

namespace STA.Geometria
{
    /// <summary>
    /// Classe per la gestione dell'insieme di punti
    /// </summary>
    public partial class ClasseElencoPunti : List<Punto2D>, INotifyCollectionChanged
    {

        #region Metodi

        /// <summary>
        /// Metodo per aggiungere punti alla sezione
        /// </summary>
        /// <param name="puntoIngresso">punto da aggiungere</param>
        public void AggiungiPunto(Punto2D puntoIngresso)
        {
            ///Se il punto non è assegnato, gli assegna coordinate 0,0
            if (puntoIngresso == null)
            {
                puntoIngresso = new Punto2D { X = 0, Y = 0 };
            }

            ///Aggiunge il punto alla lista
            this.Add(puntoIngresso);

            ///Solleva l'evento di modifica dell'elenco
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, puntoIngresso));
            }

        }

        /// <summary>
        /// Aggiungi un punto con prop. non assegnato alla sezione
        /// </summary>
        public void AggiungiPunto()
        {
            AggiungiPunto(null);
        }


        #endregion


        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
